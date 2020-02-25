using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.UI;

namespace DurationJudgement
{

    public class AbstractMenuManager : MonoBehaviour
    {

        public string result_dir = "Results/";

        string blocklog_dir = "Results/";
        string blocklog_filename = "/blocklog.txt";

        private static DurationJudgement.AbstractMenuManager instanceRef;
        protected AbstractSessionManager sessionCtl;
        public DataManager dataCtl = new DataManager();

        protected INIFile iniFileCtl;
        public System.Random rnd;


        public int HandModel = 0;

        protected Text InstructionText;

        public bool isBlockRunning = false;

        public enum ExpSession
        {
            PRACTICE = 0, TRAINING, MAIN
        }
        public ExpSession SessionId = ExpSession.PRACTICE;

        public int BlockId = 0;

        public bool isMultiGroupMenu = false;

        //parameters

        public int SubId = 1;

        public float task_rest_secs    = 1.0f;
        public float task_message_secs = 1.5f;
        public float task_target_secs  = 1.5f;
        public float task_target_image_secs = 1.5f;
        public float task_cross_secs  = 2.0f;
        public float task_action_secs  = 2.0f;
        public float task_wait_secs    = 0.5f;

        public float HeadOriginX = 0.05f;
        public float HeadOriginY = 1f;
        public float HeadOriginZ = 0.2f;

        public float VRHandOffSet = 0.2f;
        
        public int breakSeconds = 30;
        public int brakAfterTrials = 30;

        public Color inactiveColor;
        public Color defaultColor;
        public Color noActionColor;
        public Color selectedColor;
        public Color noActionSelectedColor;

        protected CycleHandPairs LeapHandCtl;

        protected int curBlockId = 0;

        public int numPreButtons = 2;
        public int numMainButtons = 24;
        public int numButtons = 26;

        public int numButtonsOnScreen = 8;
        public int numMultipleGroupMenu = 3;
        
        protected List<bool> buttonActive = new List<bool>();

        protected List<bool> buttonNoAction = new List<bool>();


        protected List<Button> buttons = new List<Button>();

        protected int selectedButton = 0;

        //

        public int debug_mode_num_trials = 3;
        public bool debugMode = true;

        public bool recordTrackingMain = false;
        public bool recordTrackingPractice = true;


        float last_time_stick = 0f;
        public float stick_interval = 0.1f;


        public int ButtonMinIndex  = 0;
        public int ButtonMaxIndex  = 30;


        List<GameObject> menuGroup;

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

            menuGroup = new List<GameObject>();
            for (int j = 0; j < numMultipleGroupMenu; j++)
            {
                menuGroup.Add(GameObject.Find("MenuGroup" + (j+1).ToString()));

                Debug.Log("Menu Group " + (j + 1).ToString() + " " + menuGroup[j]);
            }

            LeapHandCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();
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
            //Debug.Log("Button deactivate : " + curBlockId);
            int menuId = curBlockId + 2;

            SaveBlockCompleteLog(menuId);
            IncButton();

            Button b = buttons[menuId];
            b.gameObject.GetComponent<EventTrigger>().triggers.Clear();
            buttonActive[menuId] = false;

            UpdateButtons();

        }

        public void IncButton()
        {
            selectedButton++;
            if (selectedButton > ButtonMaxIndex - 1 )
            {
                selectedButton = 0;
            }
            UpdateButtons();
            
        }

        public void DecButton()
        {
            selectedButton--;

            Debug.Log(selectedButton + " " + ButtonMinIndex);
            if (selectedButton < ButtonMinIndex)
            {
                selectedButton = ButtonMaxIndex - 1;
            }
            UpdateButtons();
        }


        public void UpdateButtons()
        {
            int deactive_count = 0;
            for (int i = 0; i < numButtons; i++)
            {
                ColorBlock cb = buttons[i].colors;
                if (buttonActive[i])
                {
                    if (buttonNoAction[i])
                    {
                        cb.normalColor = noActionColor;

                    }
                    else { 
                        cb.normalColor = defaultColor;
                    }
                }
                else
                {
                    cb.normalColor = inactiveColor;
                }
                buttons[i].colors = cb;

                if (!buttonActive[i]) deactive_count++;

            }
            //Debug.Log(selectedButton + " " + buttons.Count);
            ColorBlock cb2 = buttons[selectedButton].colors;
            if (buttonNoAction[selectedButton])
            {
                cb2.normalColor = noActionSelectedColor;
            }
            else
            {
                cb2.normalColor = selectedColor;
            }
            buttons[selectedButton].colors = cb2;

            // update the block;
            if (isMultiGroupMenu)
            {
                UpdateMultiGroupMenu();
            }

            if (deactive_count == numButtons)
            {
                return;
            }
            else
            {
                //Debug.Log(" Select Button " + selectedButton);
                if (!buttonActive[selectedButton])
                {
                    IncButton();
                    //Debug.Log(" Select Button Increased" + selectedButton);
                }
            }
        }

        public void UpdateMultiGroupMenu()
        {
            List<bool> deactive_group_count = new List<bool>();
            for(int i=0; i< numMultipleGroupMenu-1; i++)
            {
                deactive_group_count.Add(false);
            }

            for (int i = numPreButtons; i < numButtons; i++)
            {
                for (int j = 0; j < numMultipleGroupMenu -1; j++)
                {
                    int min = numPreButtons + numButtonsOnScreen * j;
                    int max = (numPreButtons + numButtonsOnScreen) + numButtonsOnScreen * j;

                    if( i >= min && i < max)
                    {
                        if (buttonActive[i]) { deactive_group_count[j] = true; }
                    }
                    /*
                    if (i >= 2 && i <= 9)

                        if (i >= 2 && i < 10)
                        {
                            if (buttonActive[i]) { deactive_group_count[0] = true; }
                        }
                    if (i >= 10 && i < 18)
                    {
                        if (buttonActive[i]) { deactive_group_count[1] = true; }
                    }
                    */
                }
            }

            // Debug.Log(" deactivate buttons " + deactive_group_count[0] + " " + deactive_group_count[1]);
            for (int j = 0; j < numMultipleGroupMenu - 1; j++)
            {
                menuGroup[j].SetActive(deactive_group_count[j]);
            }


            for (int i = 0; i < numMultipleGroupMenu - 1; i++)
            {
                if (deactive_group_count[i])
                {
                    if (i == 0)
                    {
                        ButtonMinIndex = 0;
                        ButtonMaxIndex = numButtonsOnScreen + numPreButtons;
                    }
                    else
                    {
                        ButtonMinIndex = numPreButtons + numButtonsOnScreen * i;
                        ButtonMaxIndex = numPreButtons + numButtonsOnScreen * (i+1);
                    }
                }
                else
                {
                    ButtonMinIndex = numPreButtons + numButtonsOnScreen * (numMultipleGroupMenu-1);
                    ButtonMaxIndex = numPreButtons + numButtonsOnScreen * numMultipleGroupMenu;


                }
            }
            /*
                if (deactive_group_count[0]) { 
                ButtonMinIndex = 0;
                ButtonMaxIndex = 10;  
            }
            else if(deactive_group_count[1])
            {
                ButtonMinIndex = 10;
                ButtonMaxIndex = 18;
            }
            else
            {
                ButtonMinIndex = 18;
                ButtonMaxIndex = 26;
            }
            */
        }


        public void LoadBlockCompleteLog()
        {

            //            string filename = blocklog_dir + "Main/" + SubId + blocklog_filename;
            string filename = blocklog_dir + SubId + blocklog_filename;

            if (File.Exists(filename))
            {
                string[] readText = File.ReadAllLines(filename);
                foreach (string s in readText)
                {
                    string[] splittext = s.Split(',');
                    int disabledBlock = Int32.Parse(splittext[0]);
                    //Debug.Log("disable button:" + disabledBlock);
                    buttonActive[disabledBlock] = false;

                    //Debug.Log("Load Log : Disable " + disabledBlock);
                }
            }
            else
            {
                Debug.Log("Block Log File deos not exists");
            }
            UpdateButtons();

        }
        public void SaveBlockCompleteLog(int menuId)
        {
            // string filename = blocklog_dir + "Main/" + SubId + blocklog_filename;
            string filename = blocklog_dir +  SubId + blocklog_filename;

            DateTime dt = DateTime.Now;

            if (menuId == 0)
            {
                string text = menuId + ", Practice, " + dt.ToString() + Environment.NewLine;
                File.AppendAllText(filename, text);

            } else if (menuId == 1) {

                string text = menuId + ", Training, " + dt.ToString() + Environment.NewLine;
                File.AppendAllText(filename, text);
            }
            else
            {
                string text = menuId + ", Main, " + curBlockId + ", " + dt.ToString() + Environment.NewLine;
                File.AppendAllText(filename, text);

            }

        }


        protected void LoadVariablesFromInitFile(string filename = "configDelayShock.ini")
        {


            string initfile = Application.dataPath + "/../" + filename;
            Debug.Log("Loading config file: " + filename);
            iniFileCtl = new INIFile(initfile);

            LoadIntFromFile(ref SubId, "Experiment", "SubId");

            LoadIntFromFile(ref dataCtl.numPractice,     "Experiment", "numPractice");
            LoadIntFromFile(ref dataCtl.numTraining,     "Experiment", "numTraining");
            LoadIntFromFile(ref dataCtl.numTrial,        "Experiment", "numTrial");
            LoadIntFromFile(ref dataCtl.numTargetAction, "Experiment", "numTargetAction");


            LoadIntFromFile(ref brakAfterTrials,         "Experiment", "breakAfterTrials");
            LoadIntFromFile(ref breakSeconds,            "Experiment", "breakSeconds");

            LoadIntFromFile(ref dataCtl.num_blocks,      "Experiment", "num_blocks");

            LoadIntFromFile(ref debug_mode_num_trials, "Experiment", "debug_mode_num_trials");
            LoadBooleanFromFile(ref debugMode, "Experiment", "debugMode");

            LoadBooleanFromFile(ref recordTrackingMain, "Experiment", "recordTrackingMain");
            LoadBooleanFromFile(ref recordTrackingPractice, "Experiment", "recordTrackingPractice");
            LoadIntFromFile(ref HandModel, "Experiment", "HandModel");

            LoadFloatFromFile(ref task_rest_secs,        "Task", "task_rest_secs");
            LoadFloatFromFile(ref task_message_secs,     "Task", "task_message_secs");
            LoadFloatFromFile(ref task_target_secs,      "Task", "task_target_secs");
            LoadFloatFromFile(ref task_target_image_secs, "Task", "task_target_image_secs");
            LoadFloatFromFile(ref task_cross_secs,        "Task", "task_cross_secs");
            LoadFloatFromFile(ref task_action_secs,      "Task", "task_action_secs");
            LoadFloatFromFile(ref task_wait_secs,        "Task", "task_wait_secs");


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


        public IEnumerator ChangeHand(float wait)
        {
            yield return new WaitForSeconds(wait);

            LeapHandCtl.CurrentGroup = HandModel;

        }

        // Update is called once per frame
        protected void Update()
        {
            //Debug.Log(Input.GetAxis("HorizontalRStick"));
            
            if (!isBlockRunning)
            {
                //float d = Input.GetAxis("Mouse ScrollWheel");

                float d = Input.GetAxis("HorizontalRStick");

                if (Time.time - last_time_stick > stick_interval && d != 0f)
                {

                    if (d > 0.9)
                    {
                        IncButton();
                    }
                    else if (d < -0.9)
                    {
                        DecButton();
                    }
                    last_time_stick = Time.time;
                }

            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                UnityEngine.XR.InputTracking.Recenter();    // Unity 5.x系
            }

            DoUpdate();
        }


    }
}