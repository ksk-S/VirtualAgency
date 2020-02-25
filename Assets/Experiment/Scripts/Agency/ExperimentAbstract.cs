using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;

public class ExperimentAbstract : MonoBehaviour
{
    protected INIFile iniFileCtl;
    protected string config_file = "config.ini";
    public System.Random rnd;

    public int HandModel = 0;

    public bool isRobot = false;

    private static ExperimentAbstract instanceRef;

    protected SessionManager sessionCtl;
    protected Text InstructionText;

    public bool isBlockRunning = false;


    [System.Serializable]
    public struct Result
    {
        public int index;
        public int sessionId;
        public int blockId;
        public int trialId;
        public int VRconditionId;
        public int actionId;
        public int intervalId;
        public int time_before_action;
        public int planned_interval;
        public int actual_interval;
        public float reported_interval;
        public int actionBeforeFake;
        public int question;
        public int askAgency;
        public float agency;
        public float fake_latency;
    }


    public enum ExpSession
    {
        TRAINING=0, TONE, PRACTICE, VR_FAKE, NOVR_FAKE, ACTIVE, PASSIVE, ACTIVE_ROBOT, PASSIVE_HAND
    }

    public ExpSession SessionId = ExpSession.TRAINING;

    public int BlockId = 0;

    public static int num_sessions = 9;
    public static int num_block = 4;

    //parameters
    public int SubId = 1;
    public int day = 1;
    public bool odd_subject;

    public int[] intervals = { 200, 400, 1200 };

    public int numTraining = 10;
    public int numTone = 20;
    public int numPractice = 5;
    public int numFakeActive = 80;
    public int numFakePassive = 20;

    public int numFakeRecording = 10;

    public int numActive = 20;
    public int numPassive = 20;
    public int numPassiveHand = 20;

    public float training_rest_secs = 1.0f;
    public float task_rest_secs = 1.0f;
    public float task_answer_rest_secs = 0.5f;

    public int PassiveLatencyMin = 1500;
    public int PassiveLatencyMax = 2500;

    public int breakSeconds = 30;
    public int brakAfterTrials = 30;

    //
    public List<Result>[][] ResultLists = new List<Result>[num_sessions][];

    public int debug_mode_num_trials = 3;
    public bool debugMode = true;

    public bool recordTracking = true;

    protected virtual void DoAwake() { }
    protected virtual void DoStart() { }
    protected virtual void DoUpdate() { }

    protected virtual void LoadVariablesFromInitFile() { }


    public virtual void DeactivateCurrentMenu() { }

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

        DoAwake();
        for (int i = 0; i < num_sessions; i++)
        {
        //    Debug.Log("initialize results:" + i);
            if (i == (int)ExpSession.VR_FAKE || i == (int)ExpSession.NOVR_FAKE)
            {
                ResultLists[i] = new List<Result>[num_block];

                for(int j =0; j< num_block; j++) {
                    ResultLists[i][j] = new List<Result>();
                }

            }
            else {
                ResultLists[i] = new List<Result>[1];
                ResultLists[i][0] = new List<Result>();
            }
        }

        InstructionText = GameObject.Find("InstructionText").GetComponent<Text>();

        sessionCtl = gameObject.GetComponent<SessionManager>(); 
    }


    // Use this for initialization
    void Start()
    {
        DoStart();
     
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
