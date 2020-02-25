using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class Experiment : MonoBehaviour
{
    INIFile iniFileCtl;
    string config_file = "config.ini";
    System.Random rnd;

    private static Experiment instanceRef;


    [System.Serializable]
    public struct Result
    {
        public int index;
        public int sessionId;
        public int VRconditionId;
        public int actionId;
        public int intervalId;
        public int time_before_action;
        public int actual_interval;
        public float reported_interval;
        public int actionBeforeFake;
        public float agency;
        public float fake_latency;
    }

   

    [System.Serializable]
    public struct Status
    {
        public bool Training;
        public bool Practice;
        public bool ActiveVR;
        public bool ActiveNoVR;
        public Status(bool p1, bool p2, bool p3, bool p4)
        {
            Training   = p1;
            Practice   = p2;
            ActiveVR   = p3;
            ActiveNoVR = p4;
        }
    }
    public Status status = new Status(false, false, false,false );

    public enum ExpSession
    {
        TRAINING=0, TONE, PRACTICE, FIRST, SECOND
    }

    public static int num_sessions = 5;

    //parameters
    public int SubId = 1;
    public int day = 1;

    public int[] intervals = { 200, 400, 1200 };

    public int numTraining = 10;
    public int numTone = 20;
    public int numPractice = 5;
    public int numTrialsActive  = 80;
    public int numTrialsPassive = 20;

    public float training_rest_secs = 1.0f;
    public float task_rest_secs = 1.0f;
    public float task_answer_rest_secs = 0.5f;

    public int PassiveLatencyMin = 1500;
    public int PassiveLatencyMax = 2500;

    public int breakSeconds = 30;
    public int brakAfterTrials = 30;

    //

    public List<Result>[] ResultLists = new List<Result>[num_sessions];

    public int SessionId = 0;
   

    public int debug_mode_num_trials = 3;
    public bool debugMode = true;

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

        LoadVariablesFromInitFile();

        for (int i = 0; i < num_sessions; i++)
        {
            ResultLists[i] = new List<Result>();
        }


    }

    // Use this for initialization
    void Start()
    {
        rnd = new System.Random(SubId);

        for (int i = 0; i < num_sessions; i++)
        {
            for (int j = 0; j < intervals.Length; j++)
            {
                switch (i)
                {
                    case (int)ExpSession.FIRST:
                    case (int)ExpSession.SECOND:
                        for (int k = 0; k < numTrialsPassive; k++)
                        {
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            param.actionId = 0;

                            if ( i == (int)ExpSession.FIRST ) 
                            {
                                if( (SubId % 2 == 0) ^ (day ==1))
                                {
                                    param.VRconditionId = 0;
                                }
                                else
                                {
                                    param.VRconditionId = 1;
                                }
                            }
                            else
                            {
                                if ((SubId % 2 == 0) ^ (day == 1))
                                {
                                    param.VRconditionId = 1;
                                }
                                else
                                {
                                    param.VRconditionId = 0;
                                }
                            }
                            ResultLists[i].Add(param);
                        }
                        for (int k = 0; k < numTrialsActive; k++)
                        {
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            
                            param.actionId = 1;

                            if (i == (int)ExpSession.FIRST)
                            {
                                if ((SubId % 2 == 0) ^ (day == 1))
                                {
                                    param.VRconditionId = 0;
                                }
                                else
                                {
                                    param.VRconditionId = 1;
                                }
                            }
                            else
                            {
                                if ((SubId % 2 == 0) ^ (day == 1))
                                {
                                    param.VRconditionId = 1;
                                }
                                else
                                {
                                    param.VRconditionId = 0;
                                }
                            }

                            ResultLists[i].Add(param);
                        }
                        break;

                    case (int)ExpSession.PRACTICE:
                        for (int k = 0; k < numPractice; k++)
                        {

                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            param.actionId = 1;
                            param.VRconditionId = 1;

                            ResultLists[i].Add(param);
                        }
                        break;

                    case (int)ExpSession.TRAINING:
                        for (int k = 0; k < numTraining; k++)
                        {
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            int variance = UnityEngine.Random.Range(0, 200) - 100;
                            param.actual_interval = intervals[j] + variance;
                            //Debug.Log(variance + " " + param.actual_interval);
                            param.actionId = 0;
                            param.VRconditionId = 1;

                            ResultLists[i].Add(param);
                        }
                        break;

                    case (int)ExpSession.TONE:
                        for (int k = 0; k < numTone; k++)
                        {
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            param.actual_interval = intervals[j];
                            //Debug.Log(variance + " " + param.actual_interval);
                            param.actionId = 0;
                            param.VRconditionId = 1;

                            ResultLists[i].Add(param);
                        }
                        break;
                }   

                }
            }

        for (int i = 0; i < num_sessions; i++)
        {
            Shuffle(ResultLists[i]);
        }
        Debug.Log("Training :" + ResultLists[0].Count);
        Debug.Log("Practice :" + ResultLists[1].Count);
        Debug.Log("ActiveNoVR : " + ResultLists[2].Count);
        Debug.Log("ActiveOnly : " + ResultLists[3].Count);
     
    }


    void LoadVariablesFromInitFile()
    {
        string initfile = Application.dataPath + "/../" + config_file;
        iniFileCtl = new INIFile(initfile);

        LoadIntFromFile(ref SubId, "Experiment", "SubId");
        LoadIntFromFile(ref numTraining, "Experiment", "numTraining");
        LoadIntFromFile(ref numTone, "Experiment", "numTone");
        
        LoadIntFromFile(ref numPractice, "Experiment", "numPractice");
        LoadIntFromFile(ref numTrialsActive, "Experiment", "numTrialsActive");
        LoadIntFromFile(ref numTrialsPassive, "Experiment", "numTrialsPassive");

        LoadIntFromFile(ref intervals[0], "Experiment", "interval1");
        LoadIntFromFile(ref intervals[1], "Experiment", "interval2");
        LoadIntFromFile(ref intervals[2], "Experiment", "interval3");

        LoadFloatFromFile(ref task_rest_secs, "Experiment", "task_rest_secs");
        LoadFloatFromFile(ref task_answer_rest_secs, "Experiment", "task_answer_rest_secs");
        LoadIntFromFile(ref debug_mode_num_trials, "Experiment", "debug_mode_num_trials");
        LoadBooleanFromFile(ref debugMode, "Experiment", "debugMode");

        LoadIntFromFile(ref PassiveLatencyMin, "Experiment", "PassiveLatencyMin");
        LoadIntFromFile(ref PassiveLatencyMax, "Experiment", "PassiveLatencyMax");
        LoadIntFromFile(ref brakAfterTrials, "Experiment", "brakAfterTrials");
        LoadIntFromFile(ref breakSeconds, "Experiment", "breakSeconds");
        LoadIntFromFile(ref day, "Experiment", "day");
        
}


    void LoadIntFromFile(ref int var, string group, string var_name)
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

    void LoadFloatFromFile(ref float var, string group, string var_name)
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

    void LoadBooleanFromFile(ref bool var, string group, string var_name)
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

    void LoadStringFromFile(ref string var, string group, string var_name)
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
    void Update()
    {

    }


    void Swap<T>(List<T> list, int index1, int index2)
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
