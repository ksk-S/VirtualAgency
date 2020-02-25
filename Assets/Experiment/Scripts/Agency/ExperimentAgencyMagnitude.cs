using UnityEngine;

using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.EventSystems;

public class ExperimentAgencyMagnitude : ExperimentAbstract
{

    public enum MenuLabel
    {
        TRAINING = 0, TONE, PRACTICE, FIRST, SECOND
    }

    protected override void DoAwake()
    {
        config_file = "configMagnitude.ini";
        LoadVariablesFromInitFile();


        //menues
        EventTrigger.Entry entryTraining = new EventTrigger.Entry();
        entryTraining.eventID = EventTriggerType.PointerUp;
        entryTraining.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.TRAINING); });
        GameObject.Find("StartTrainingButton").GetComponent<EventTrigger>().triggers.Add(entryTraining);

        /*
        if (expCtl.day == 2)
        {
            GameObject.Find("StartToneSession").GetComponent<Button>().interactable = false;
        }
        */
        //        else {
        EventTrigger.Entry entryTone = new EventTrigger.Entry();
        entryTone.eventID = EventTriggerType.PointerUp;
        entryTone.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.TONE); });
        GameObject.Find("StartToneSession").GetComponent<EventTrigger>().triggers.Add(entryTone);
        //      }

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


        Text menuText = GameObject.Find("MenuInstructionText").GetComponent<Text>();
        menuText.text = "Your Subject ID is " + SubId.ToString() + Environment.NewLine;
        menuText.text += "Click a button to start the experiments";



    }

    // Use this for initialization
    protected override void DoStart()
    {
        rnd = new System.Random(SubId);

        for (int i = 0; i < num_sessions; i++)
        {
            for (int j = 0; j < intervals.Length; j++)
            {
                switch (i)
                {
                    case (int)ExpSession.VR_FAKE:
                    case (int)ExpSession.NOVR_FAKE:
                        for (int k = 0; k < numFakePassive; k++)
                        {
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            param.actionId = 0;
                            param.VRconditionId = (i == (int)ExpSession.VR_FAKE) ? 1 : 0;

                            ResultLists[i][0].Add(param);
                        }
                        for (int k = 0; k < numFakeActive; k++)
                        {
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            param.actionId = 1;
                            param.VRconditionId = (i == (int)ExpSession.VR_FAKE) ? 1 : 0;

                            ResultLists[i][0].Add(param);
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
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            int variance = UnityEngine.Random.Range(0, 200) - 100;
                            param.actual_interval = intervals[j] + variance;
                            //Debug.Log(variance + " " + param.actual_interval);
                            param.actionId = 0;
                            param.VRconditionId = 1;

                            ResultLists[i][0].Add(param);
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

                            ResultLists[i][0].Add(param);
                        }
                        break;
                }   

                }
            }

        for (int i = 0; i < num_sessions; i++)
        {
            Shuffle(ResultLists[i][0]);

            Debug.Log(ResultLists[i][0].Count);
        }
     
    }

    public void ExperimentStart(MenuLabel menuId)
    {
        switch (menuId)
        {
            case MenuLabel.TRAINING:
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction1_Traininig").text;
                SessionId = ExpSession.TRAINING;
                break;
            case MenuLabel.TONE:
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction2_Tone").text;
                SessionId = ExpSession.TONE;
                break;
            case MenuLabel.PRACTICE:
                SessionId = ExpSession.PRACTICE;
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction3_Practice").text;
                break;
            case MenuLabel.FIRST:
                InstructionText.text = "4. First Main Session\n\n";
                if ((SubId % 2 == 0) ^ (day == 1))
                {
                    SessionId = ExpSession.VR_FAKE;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction4_VRSession").text;
                }
                else
                {
                    SessionId = ExpSession.NOVR_FAKE;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction5_NOVRSession").text;
                }

                break;
            case MenuLabel.SECOND:
                InstructionText.text = "5. Second Main Session\n\n";
                if ((SubId % 2 == 0) ^ (day == 1))
                {
                    SessionId = ExpSession.NOVR_FAKE;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction5_NOVRSession").text;
                }
                else
                {
                    SessionId = ExpSession.VR_FAKE;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction4_VRSession").text;
                }

                break;
        }
        Debug.Log(SessionId);

        if (SessionId == ExpSession.TRAINING || SessionId == ExpSession.TONE)
        {
            StartCoroutine(sessionCtl.CoTrainingStart(SessionId));
        }
        else {
            StartCoroutine(sessionCtl.CoExperimentStart(SessionId, 0));
        }
    }

    protected override void LoadVariablesFromInitFile()
    {
        string initfile = Application.dataPath + "/../" + config_file;
        iniFileCtl = new INIFile(initfile);

        LoadIntFromFile(ref SubId, "Experiment", "SubId");
        LoadIntFromFile(ref numTraining, "Experiment", "numTraining");
        LoadIntFromFile(ref numTone, "Experiment", "numTone");
        
        LoadIntFromFile(ref numPractice, "Experiment", "numPractice");
        LoadIntFromFile(ref numFakeActive, "Experiment", "numTrialsActive");
        LoadIntFromFile(ref numFakePassive, "Experiment", "numTrialsPassive");

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



    // Update is called once per frame
    protected override void DoUpdate()
    {

    }


}
