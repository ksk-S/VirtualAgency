using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;

public class ExperimentAgencyFakeBlocked : ExperimentAbstract
{
    List<int> order = new List<int>(){ 0, 1, 2 };
    public Color activeColor;
    public Color passiveColor;
    public Color fakeColor;

    public Color activeHighlightedColor;
    public Color passiveHighlightedColor;
    public Color fakeHighlightedColor;

    CycleHandPairs LeapCtl;
    public enum MenuLabel
    {
       PRACTICE=0, FIRST, SECOND, THIRD
    }

    protected override void DoAwake()
    {
        intervals = new int[5];
        LoadVariablesFromInitFile();

        rnd = new System.Random(SubId);
        Shuffle(order);

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


        Text menuText = GameObject.Find("MenuInstructionText").GetComponent<Text>();
        menuText.text = "Your Subject ID is " + SubId.ToString() + Environment.NewLine;
        menuText.text += "Click a button to start the experiments";

        Debug.Log(order[0] + " " + order[1] + " " + order[2]);


        Button[] buttons = new Button[3];
        buttons[0] = GameObject.Find("FirstSessionButton").GetComponent<Button>();
        buttons[1] = GameObject.Find("SecondSessionButton").GetComponent<Button>();
        buttons[2] = GameObject.Find("ThirdSessionButton").GetComponent<Button>();

        for (int i=0; i<3; i++)
        {
            switch (order[i])
            {
                case 0:
                    ColorBlock cb = buttons[i].colors;
                    cb.normalColor = activeColor;
                    cb.highlightedColor = activeHighlightedColor;
                    buttons[i].colors = cb;
                    break;
                case 1:
                    ColorBlock cb2 = buttons[i].colors;
                    cb2.normalColor = passiveColor;
                    cb2.highlightedColor = passiveHighlightedColor;
                    buttons[i].colors = cb2;
                    break;
                case 2:
                    ColorBlock cb3 = buttons[i].colors;
                    cb3.normalColor = fakeColor;
                    cb3.highlightedColor = fakeHighlightedColor;
                    buttons[i].colors = cb3;
                    break;
            }
        }


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
                    case (int)ExpSession.ACTIVE:
                        for (int k = 0; k < numActive; k++)
                        {
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            param.VRconditionId = 0;
                            param.actionId = 1;
                            ResultLists[i][0].Add(param);
                        }
                        break;
                    case (int)ExpSession.PASSIVE_HAND:

                        for (int k = 0; k < numActive; k++)
                        {
                            Result param = new Result();
                            param.sessionId = i;
                            param.intervalId = j;
                            param.VRconditionId = 1;
                            param.actionId = 0;
                            ResultLists[i][0].Add(param);
                        }
                        break;

                    case (int)ExpSession.PASSIVE:
                        for (int k = 0; k < numPassive; k++)
                        {
                                Result param = new Result();
                                param.sessionId = i;
                                param.intervalId = j;
                                param.VRconditionId = 0;
                                param.actionId = 0;
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
                    }   

                }
            }

        for (int i = 0; i < num_sessions; i++)
        {
            Shuffle(ResultLists[i][0]);
            Debug.Log( ResultLists[i][0].Count);
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
        switch (menuId)
        {
            case MenuLabel.PRACTICE:
                SessionId = ExpSession.PRACTICE;
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction3_PracticeActive").text;
                break;
            case MenuLabel.FIRST:
                InstructionText.text = "2. First Main Session\n\n";
                break;
            case MenuLabel.SECOND:
                InstructionText.text = "3. Second Main Session\n\n";
                break;
            case MenuLabel.THIRD:
                InstructionText.text = "4. Third Main Session\n\n";
                break;

        }

        if(menuId == MenuLabel.FIRST || menuId == MenuLabel.SECOND || menuId == MenuLabel.THIRD)
        {
            int id = (int)menuId - 1;
            Debug.Log("Sessions " + id + " " + order[id]);
            switch ( order[id] )
            {
                case 0:
                    SessionId = ExpSession.ACTIVE;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction6_Acitve").text;
                    isRobot = false;
                    LeapCtl.CurrentGroup = HandModel;
                    
                    break;
                case 1:
                    SessionId = ExpSession.PASSIVE;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction7_Passive").text;

                    isRobot = true;
                    LeapCtl.CurrentGroup = HandModel;
                  
                    break;
                case 2:
                    SessionId = ExpSession.PASSIVE_HAND;
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction8_PassiveHand").text;

                    isRobot = false;
                    LeapCtl.CurrentGroup = HandModel;
                    break;
            }

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
        LoadIntFromFile(ref numActive, "Experiment", "numActive");
        LoadIntFromFile(ref numPassive, "Experiment", "numPassive");

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
        LoadIntFromFile(ref day, "Experiment", "day");

        LoadIntFromFile(ref HandModel, "Experiment", "FemaleHand");

        Debug.Log("female " + HandModel);
    }



    // Update is called once per frame
    protected override void DoUpdate()
    {

    }


}
