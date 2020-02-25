using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;

public class ExperimentAgencyFakeBinary : ExperimentAbstract
{
    //List<int> order = new List<int>(){ 0, 1, 2 };

    CycleHandPairs LeapCtl;
    public enum MenuLabel
    {
        TRAINING=0, PRACTICE, FIRST, SECOND, THIRD, FORTH
    }

    protected override void DoAwake()
    {
        intervals = new int[5];
        LoadVariablesFromInitFile();

        rnd = new System.Random(SubId);

        EventTrigger.Entry entryTraining = new EventTrigger.Entry();
        entryTraining.eventID = EventTriggerType.PointerUp;
        entryTraining.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.TRAINING); });
        GameObject.Find("StartTrainingButton").GetComponent<EventTrigger>().triggers.Add(entryTraining);

        EventTrigger.Entry entryPractice = new EventTrigger.Entry();
        entryPractice.eventID = EventTriggerType.PointerUp;
        entryPractice.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.PRACTICE); });
        GameObject.Find("StartPracticeButton").GetComponent<EventTrigger>().triggers.Add(entryPractice);

        EventTrigger.Entry entryFirst = new EventTrigger.Entry();
        entryFirst.eventID = EventTriggerType.PointerUp;
        entryFirst.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.FIRST); });
        GameObject.Find("FirstSessionButton").GetComponent<EventTrigger>().triggers.Add(entryFirst);

        EventTrigger.Entry entrySecond = new EventTrigger.Entry();
        entrySecond.eventID = EventTriggerType.PointerUp;
        entrySecond.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.SECOND); });
        GameObject.Find("SecondSessionButton").GetComponent<EventTrigger>().triggers.Add(entrySecond);

        EventTrigger.Entry entryThird = new EventTrigger.Entry();
        entryThird.eventID = EventTriggerType.PointerUp;
        entryThird.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.THIRD); });
        GameObject.Find("ThirdSessionButton").GetComponent<EventTrigger>().triggers.Add(entryThird);

        EventTrigger.Entry entryForth = new EventTrigger.Entry();
        entryForth.eventID = EventTriggerType.PointerUp;
        entryForth.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.FORTH); });
        GameObject.Find("ForthSessionButton").GetComponent<EventTrigger>().triggers.Add(entryForth);

        
        odd_subject = (SubId % 2 == 0) ? false : true;
        Debug.Log("subid:" + SubId + " odd:" + (SubId % 2 == 0) + " " + odd_subject);


        Text menuText = GameObject.Find("MenuInstructionText").GetComponent<Text>();
        menuText.text = "SubID : " + SubId.ToString() +" " ;
        menuText.text += "Day : " + day.ToString() + " ";
        menuText.text += "ExMode : " + (odd_subject ? "With Hand" : "No Hand" );
        menuText.text += Environment.NewLine + Environment.NewLine;
        menuText.text += "Click a button to start the experiment";

        LeapCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();


    }

    // Use this for initialization
    protected override void DoStart()
    {

        for (int i = 0; i < num_sessions; i++)
        {
            for (int j = 0; j < intervals.Length; j++)
            {
                switch (i)
                {
                    case (int)ExpSession.VR_FAKE:
                    case (int)ExpSession.NOVR_FAKE:

                        for(int l=0; l< num_block; l++) {
                            for (int k = 0; k < numFakePassive; k++)
                            {
                                Result param = new Result();
                                param.sessionId = i;
                                param.intervalId = j;
                                param.actionId = 0;
                                param.VRconditionId = (i == (int)ExpSession.VR_FAKE) ? 1 : 0;

                                ResultLists[i][l].Add(param);
                            }
                            for (int k = 0; k < numFakeActive; k++)
                            {
                                Result param = new Result();
                                param.sessionId = i;
                                param.intervalId = j;
                                param.actionId = 1;
                                param.VRconditionId = (i == (int)ExpSession.VR_FAKE) ? 1 : 0;

                                ResultLists[i][l].Add(param);
                            }
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

                            ResultLists[i][0].Add(param);
                        }
                        break;

                    case (int)ExpSession.TRAINING:
                        for (int k = 0; k < numTraining; k++)
                        {
                            if (j == 0 || j == 4)
                            {
                                Result param = new Result();
                                param.sessionId = i;
                                param.intervalId = j;
                                // int variance = UnityEngine.Random.Range(0, 200) - 100;
                                param.actual_interval = intervals[j] ;
                                param.actionId = 1;
                                param.VRconditionId = 1;

                                //Debug.Log("error here" + i);
                                ResultLists[i][0].Add(param);
                            }
                        }
                        break;
                    }   

                }
            }

        for (int i = 0; i < num_sessions; i++)
        {
            if (i == (int)ExpSession.VR_FAKE || i == (int)ExpSession.NOVR_FAKE)
            {
                for (int j = 0; j < num_block; j++)
                {
                    Shuffle(ResultLists[i][j]);
                    Debug.Log("session:" + i + "." + j + " " + ResultLists[i][j].Count);
                }
            }
            else {

                Shuffle(ResultLists[i][0]);
                Debug.Log("session:" + i + " " + ResultLists[i][0].Count);
            }
        }

        StartCoroutine(ChangeHand());

    }

    IEnumerator ChangeHand()
    {
        yield return new WaitForSeconds(1.0f);
   
        LeapCtl.CurrentGroup = HandModel;
      
    }
    public void ExperimentStart(MenuLabel menuId)
    {
      
        LeapCtl.CurrentGroup = HandModel;
        
        
        isRobot = false;

        int blockId = 0;

        switch (menuId)
        {
            case MenuLabel.FIRST:
                InstructionText.text = "1st Block\n\n";
                blockId = 0;
                break;
            case MenuLabel.SECOND:
                InstructionText.text = "2nd Block\n\n";
                blockId = 1;
                break;
            case MenuLabel.THIRD:
                InstructionText.text = "3rd Block\n\n";
                blockId = 2;
                break;
            case MenuLabel.FORTH:
                InstructionText.text = "4th Block\n\n";
                blockId = 3;
                break;
        }

        switch (menuId)
        {
            case MenuLabel.TRAINING:
                SessionId = ExpSession.TRAINING;
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction1_Traininig").text;
                blockId = 0;
                break;
            case MenuLabel.PRACTICE:
                SessionId = ExpSession.PRACTICE;
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction3_PracticeActive").text;
                blockId = 0;
                break;
            case MenuLabel.FIRST:
            case MenuLabel.SECOND:
            case MenuLabel.THIRD:
            case MenuLabel.FORTH:
                if (odd_subject)
                {
                    Debug.Log("odd subject : VR mode");
                    SessionId = ExpSession.VR_FAKE;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction4_VRSession").text;
                }
                else { 

                    Debug.Log("even subject : NO VR mode");
                    SessionId = ExpSession.NOVR_FAKE;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction5_NOVRSession").text;
                }
                break;
        }

        Debug.Log(SessionId);

        if (SessionId == ExpSession.TONE)
        {
            StartCoroutine(sessionCtl.CoTrainingStart(SessionId));
        }
        else {
            StartCoroutine(sessionCtl.CoExperimentStart(SessionId, blockId));
        }
    }

    protected override void LoadVariablesFromInitFile()
    {
        string initfile = Application.dataPath + "/../" + config_file;
        iniFileCtl = new INIFile(initfile);

        LoadIntFromFile(ref SubId, "Experiment", "SubId");

        LoadIntFromFile(ref day, "Experiment", "Day");
        LoadIntFromFile(ref numTraining, "Experiment", "numTraining");
        LoadIntFromFile(ref numTone, "Experiment", "numTone");
        
        LoadIntFromFile(ref numPractice, "Experiment", "numPractice");
        LoadIntFromFile(ref numActive, "Experiment", "numActive");
        LoadIntFromFile(ref numPassive, "Experiment", "numPassive");


        LoadIntFromFile(ref numFakeActive, "Experiment", "numFakeActive");
        LoadIntFromFile(ref numFakePassive, "Experiment", "numFakePassive");

        LoadIntFromFile(ref intervals[0], "Experiment", "interval1");
        LoadIntFromFile(ref intervals[1], "Experiment", "interval2");
        LoadIntFromFile(ref intervals[2], "Experiment", "interval3");
        LoadIntFromFile(ref intervals[3], "Experiment", "interval4");
        LoadIntFromFile(ref intervals[4], "Experiment", "interval5");

        LoadFloatFromFile(ref task_rest_secs, "Experiment", "task_rest_secs");
        LoadFloatFromFile(ref task_answer_rest_secs, "Experiment", "task_answer_rest_secs");
        LoadIntFromFile(ref debug_mode_num_trials, "Experiment", "debug_mode_num_trials");
        LoadBooleanFromFile(ref debugMode, "Experiment", "debugMode");

        LoadIntFromFile(ref PassiveLatencyMin, "Experiment", "PassiveLatencyMin");
        LoadIntFromFile(ref PassiveLatencyMax, "Experiment", "PassiveLatencyMax");
        LoadIntFromFile(ref brakAfterTrials, "Experiment", "brakAfterTrials");
        LoadIntFromFile(ref breakSeconds, "Experiment", "breakSeconds");

        LoadIntFromFile(ref HandModel, "Experiment", "HandModel");

        Debug.Log("hand model " + HandModel);

    }



    // Update is called once per frame
    protected override void DoUpdate()
    {

    }


}
