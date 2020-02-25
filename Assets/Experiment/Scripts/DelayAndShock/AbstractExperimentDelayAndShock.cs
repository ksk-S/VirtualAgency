using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;

public class AbstractExperimentDelayAndShock : MonoBehaviour
{
    
    protected INIFile iniFileCtl;
    public System.Random rnd;

    public int HandModel = 0;

    private static AbstractExperimentDelayAndShock instanceRef;

    protected AbstractDelayAndShockSessionManager sessionCtl;
    protected Text InstructionText;

    public bool isBlockRunning = false;


    public enum ExpSession
    {
        PRACTICE = 0, MAIN
    }

    public enum StimulusType
    {
         TACTILE, VISUAL,  VISUALTACTILE
    }

    public enum VRCondition
    {
        SYNC, DELAY_S, DELAY_L
    }


    [System.Serializable]
    public struct ResultStimuli
    {
        public int index;
        public float stimulusVisualTime;
        public float stimulusTriggerTime;
        public int trigerLabel;
    }

    [System.Serializable]
    public struct Result
    {
        public int sessionId;
        public int blockId;
        public int trialId;
        public int repeatId;
        public int index;
        public VRCondition VRcondition;
        public StimulusType stimulusType;
        public float inductionStartTime;
        public float inductionEndTime;
        public float stimulusStartTime;
        public float stimulusEndTime;
        public float averageDelay;
        public float agency;
        public int numActualStimulus;
        public List<ResultStimuli> StimuliList;
                
    }



    public ExpSession SessionId = ExpSession.PRACTICE;

    public int BlockId = 0;

    public static int num_sessions = 2;

    //parameters

    public int SubId = 1;

    public int numPractice = 10;

    public int num_blocks = 8;
    public int num_block_repeat = 2;
    public int numTrial = 20;
    public int numStimulus = 5;

    public int delayFrameShort = 10;
    public int delayFrameLong = 50;


    public float task_rest_secs = 1.0f;
    public float task_induction_secs = 1.0f;
    public float task_after_task_rest_secs = 2.0f;
    public float task_stimulus_rest_secs = 0.5f;
    public float task_answer_rest_secs = 0.5f;
    public float task_stimulus_time_to_hide = 0.5f;

    public float task_message_secs = 3.0f;

    public float task_stimulus_jitter = 0.25f;


    public float HeadOriginX = 0.05f;
    public float HeadOriginY = 1f;
    public float HeadOriginZ = 0.2f;

    public float VRHandOffSet = 0.2f;

    public float stimulus_offset_x = 0f;
    public float stimulus_offset_y = 0f;
    public float stimulus_height = 0.2f;

    public int breakSeconds = 30;
    public int brakAfterTrials = 30;

    public Color inactiveColor;
    public Color defaultColor;
    public Color selectedColor;

    protected CycleHandPairs LeapCtl;

    protected int curBlockId = 0;

    protected int numButtons = 13;

    protected List<bool> buttonActive = new List<bool>();

    protected List<Button> buttons = new List<Button>();

    protected int selectedButton = 0;

    string blocklog_dir = "Results/";
    string blocklog_filename = "/blocklog.txt";
    //
    public List<List<Result>>[] ResultLists = new List<List<Result>>[num_sessions];

    public int debug_mode_num_trials = 3;
    public bool debugMode = true;

    public bool recordTracking = true;

    protected virtual void DoAwake() { }
    protected virtual void DoStart() { }
    protected virtual void DoUpdate() { }

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        //Result List
        for (int i = 0; i < num_sessions; i++)
        {
            ResultLists[i] = new List<List<Result>>();
        }


        LeapCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();
        rnd = new System.Random(SubId);


        DoAwake();
        Text menuText = GameObject.Find("MenuInstructionText").GetComponent<Text>();
        menuText.text = "Your Subject ID is " + SubId.ToString() + Environment.NewLine;
        menuText.text += "Click a button to start the experiments";

        InstructionText = GameObject.Find("InstructionText").GetComponent<Text>();

    }


    // Use this for initialization
    void Start()
    {
        Transform tr = GameObject.Find("LeapHandController").GetComponent<Transform>();
        tr.localPosition = new Vector3(VRHandOffSet, 0f, 0f);

        DoStart();
     
    }


    public void DeactivateCurrentMenu()
    {
        Debug.Log("Button deactivate : " + curBlockId);

      
            SaveBlockCompleteLog(curBlockId);
            IncButton();

            Button b = buttons[curBlockId + 1];
            b.gameObject.GetComponent<EventTrigger>().triggers.Clear();
            buttonActive[curBlockId + 1] = false;

            SelectButton();
       
    }

    public void IncButton()
    {
        selectedButton++;
        if (selectedButton > numButtons - 1)
        {
            selectedButton = 0;
            ColorBlock cb = buttons[selectedButton].colors;
            cb.normalColor = inactiveColor;
            buttons[selectedButton].colors = cb;
        }
        else
        {
            SelectButton();
        }
    }

    public void DecButton()
    {
        selectedButton--;
        if (selectedButton < 0)
        {
            selectedButton = numButtons - 1;
        }
        SelectButton();
    }


    public void SelectButton()
    {
        Debug.Log(" Select Button " + selectedButton);
        if (!buttonActive[selectedButton])
        {
            IncButton();
            Debug.Log(" Select Button Increased" + selectedButton);
        }

        for (int i = 0; i < numButtons; i++)
        {
            ColorBlock cb = buttons[i].colors;
            if (buttonActive[i])
            {
                cb.normalColor = defaultColor;
            }
            else
            {
                cb.normalColor = inactiveColor;
            }
            buttons[i].colors = cb;
        }

        ColorBlock cb2 = buttons[selectedButton].colors;
        cb2.normalColor = selectedColor;
        buttons[selectedButton].colors = cb2;

    }

    public void LoadBlockCompleteLog()
    {

        string filename = blocklog_dir + "Main/" + SubId + blocklog_filename;

        if (File.Exists(filename))
        {
            string[] readText = File.ReadAllLines(filename);
            foreach (string s in readText)
            {
                string[] splittext = s.Split(',');
                int disabledBlock = Int32.Parse(splittext[0]);
                Debug.Log("disable button:" + disabledBlock);
                buttonActive[disabledBlock] = false;
            }
        }
        else
        {
            Debug.Log("Block Log File deos not exists");
        }
        SelectButton();

    }
    public void SaveBlockCompleteLog(int curBlockId)
    {
        string filename = blocklog_dir + "Main/" + SubId + blocklog_filename;

        DateTime dt = DateTime.Now;

        if (curBlockId == -1)
        {
            string text = (curBlockId + 1) + ", Practice, Practice, " + dt.ToString() + Environment.NewLine;
            File.AppendAllText(filename, text);

        }
        else {
            List<Result> result = ResultLists[(int)ExpSession.MAIN][curBlockId];
            string text = (curBlockId + 1) + ", " + result[0].VRcondition + ", " + result[0].stimulusType + ", " + dt.ToString() + Environment.NewLine;
            File.AppendAllText(filename, text);

        }

    }


    protected void LoadVariablesFromInitFile(string filename = "configDelayShock.ini") {
       

        string initfile = Application.dataPath + "/../" + filename;
        Debug.Log("Loading config file: " + filename);
        iniFileCtl = new INIFile(initfile);

        LoadIntFromFile(ref SubId, "Experiment", "SubId");


        LoadIntFromFile(ref num_blocks, "Experiment", "num_blocks");
        LoadIntFromFile(ref num_block_repeat, "Experiment", "num_block_repeat");
        LoadIntFromFile(ref numTrial, "Experiment", "numTrial");
        LoadIntFromFile(ref numStimulus, "Experiment", "numStimulus");

        LoadIntFromFile(ref delayFrameShort, "Experiment", "delayFrameShort");
        LoadIntFromFile(ref delayFrameLong, "Experiment", "delayFrameLong");

        LoadFloatFromFile(ref task_rest_secs, "Experiment", "task_rest_secs");
        LoadFloatFromFile(ref task_induction_secs, "Experiment", "task_induction_secs");
        LoadFloatFromFile(ref task_after_task_rest_secs, "Experiment", "task_after_task_rest_secs");

        LoadFloatFromFile(ref task_stimulus_rest_secs, "Experiment", "task_stimulus_rest_secs");
        LoadFloatFromFile(ref task_answer_rest_secs, "Experiment", "task_answer_rest_secs");
        LoadFloatFromFile(ref task_stimulus_jitter, "Experiment", "task_stimulus_jitter");

        LoadFloatFromFile(ref task_stimulus_time_to_hide, "Experiment", "task_stimulus_time_to_hide");

        LoadFloatFromFile(ref task_message_secs, "Experiment", "task_message_secs");


        LoadIntFromFile(ref debug_mode_num_trials, "Experiment", "debug_mode_num_trials");
        LoadBooleanFromFile(ref debugMode, "Experiment", "debugMode");

        LoadBooleanFromFile(ref recordTracking, "Experiment", "recordTracking");
        LoadIntFromFile(ref brakAfterTrials, "Experiment", "brakAfterTrials");
        LoadIntFromFile(ref breakSeconds, "Experiment", "breakSeconds");
        LoadIntFromFile(ref HandModel, "Experiment", "HandModel");

        LoadFloatFromFile(ref stimulus_offset_x, "Experiment", "stimulus_offset_x");
        LoadFloatFromFile(ref stimulus_offset_y, "Experiment", "stimulus_offset_y");
        LoadFloatFromFile(ref stimulus_height, "Experiment", "stimulus_height");


        LoadFloatFromFile(ref HeadOriginX, "VR", "HeadOriginX");
        LoadFloatFromFile(ref HeadOriginY, "VR", "HeadOriginY");
        LoadFloatFromFile(ref HeadOriginZ, "VR", "HeadOriginZ");

        LoadFloatFromFile(ref VRHandOffSet, "VR", "VRHandOffSet");
        

        Debug.Log("Hand Model " + HandModel);
    }

    protected void LoadIntFromFile(ref int var, string group, string var_name)
    {
        try
        {
            var = int.Parse(iniFileCtl[group, var_name]);
        }
        catch
        {
            Debug.Log(var_name + " is not numerical in the Init file ");
        }
    }

    protected void LoadFloatFromFile(ref float var, string group, string var_name)
    {
        try
        {
            var = float.Parse(iniFileCtl[group, var_name]);
            // Debug.Log(var);
        }
        catch
        {
            Debug.Log(var_name + " is not numerical in the Init file ");
        }
    }

    protected void LoadBooleanFromFile(ref bool var, string group, string var_name)
    {
        try
        {
            var = iniFileCtl[group, var_name] == "true" ? true : false;
        }
        catch
        {
            Debug.Log(var_name + " is not boolean in the Init file ");
        }

    }

    protected void LoadStringFromFile(ref string var, string group, string var_name)
    {
        try
        {
            var = iniFileCtl[group, var_name].Trim();
        }
        catch
        {
            Debug.Log(var_name + " is not boolean in the Init file ");
        }

    }

    // Update is called once per frame
    protected void Update()
    {
        /*
        if (!isBlockRunning)
        {
            float d = Input.GetAxis("Mouse ScrollWheel");

            if (d > 0.1)
            {
                IncButton();
            }
            else if (d < -0.1)
            {
                DecButton();
            }
        
        }
        */
        DoUpdate();
    }


    protected void Swap<T>(List<T> list, int index1, int index2)
    {
        var a = list[index1];
        list[index1] = list[index2];
        list[index2] = a;
    }

    public void Shuffle<T>(List<T> list)
    {
        //var rnd = new System.Random(Guid.NewGuid().GetHashCode());
        Enumerable.Range(1, list.Count).Reverse().ToList().ForEach(i => Swap(list, rnd.Next(i), i - 1));
    }



}
