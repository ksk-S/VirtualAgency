using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;


class ExperimentAgencyARBlocked : ExperimentAbstract
{
    GameObject EyeCentre;
    GameObject LeapPosition;
    GameObject ButtonGadget;

    List<int> order = new List<int>(){ 0, 1 };
    List<int> orderCB = new List<int>(4);

    public Color activeColor = Color.red;
    public Color passiveColor = Color.grey;

    public Color activeHighlightedColor = Color.red;
    public Color passiveHighlightedColor = Color.grey;

    float HeadOriginX = 0.05f;
    float HeadOriginY = 1f;
    float HeadOriginZ = 0.2f;


    float LeapHandX = 0.01f;
    float LeapHandY = -0.01f;
    float LeapHandZ = -0.1f;


    CycleHandPairs LeapCtl;
    public enum MenuLabel
    {
       TRAINING=0, PRACTICE, FIRST, SECOND, THIRD, FOURTH
    }

    MenuLabel currentMenu = MenuLabel.PRACTICE;

    Button[] buttons = new Button[6];

    protected override void DoAwake()
    {
        EyeCentre = GameObject.Find("HandSwither");
        ButtonGadget = GameObject.Find("ButtonGadget");
        LeapPosition = GameObject.Find("LeapHandController");

        rnd = new System.Random(SubId);

        intervals = new int[3];
        LoadVariablesFromInitFile();


        bool even_subject = (SubId % 2 == 0) ? true : false;

        if (even_subject)
        {
            order.Reverse();
        }

        orderCB.AddRange(order);
        List<int> reversed = new List<int>(order);
        reversed.Reverse();

        orderCB.AddRange(reversed);


        Debug.Log("# of Sessions " + orderCB.Count);
        string orderString="";
        for (int i = 0; i < 4; i++)
        {
            orderString += orderCB[i] + " ";
        }
        Debug.Log(orderString);

        EventTrigger.Entry entryTraining = new EventTrigger.Entry();
        entryTraining.eventID = EventTriggerType.PointerDown;
        entryTraining.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.TRAINING); });
        GameObject.Find("StartTrainingButton").GetComponent<EventTrigger>().triggers.Add(entryTraining);

        EventTrigger.Entry entryPractice = new EventTrigger.Entry();
        entryPractice.eventID = EventTriggerType.PointerDown;
        entryPractice.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.PRACTICE); });
        GameObject.Find("StartPracticeButton").GetComponent<EventTrigger>().triggers.Add(entryPractice);

        EventTrigger.Entry entryFirst = new EventTrigger.Entry();
        entryFirst.eventID = EventTriggerType.PointerDown;
        entryFirst.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.FIRST); });
        GameObject.Find("FirstSessionButton").GetComponent<EventTrigger>().triggers.Add(entryFirst);

        EventTrigger.Entry entrySecond = new EventTrigger.Entry();
        entrySecond.eventID = EventTriggerType.PointerDown;
        entrySecond.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.SECOND); });
        GameObject.Find("SecondSessionButton").GetComponent<EventTrigger>().triggers.Add(entrySecond);

        EventTrigger.Entry entryThird = new EventTrigger.Entry();
        entryThird.eventID = EventTriggerType.PointerDown;
        entryThird.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.THIRD); });
        GameObject.Find("ThirdSessionButton").GetComponent<EventTrigger>().triggers.Add(entryThird);

        EventTrigger.Entry entryFourth = new EventTrigger.Entry();
        entryFourth.eventID = EventTriggerType.PointerDown;
        entryFourth.callback.AddListener((eventData) => { ExperimentStart(MenuLabel.FOURTH); });
        GameObject.Find("FourthSessionButton").GetComponent<EventTrigger>().triggers.Add(entryFourth);

        Text menuText = GameObject.Find("MenuInstructionText").GetComponent<Text>();
        menuText.text = "Your Subject ID is " + SubId.ToString() + Environment.NewLine;
        menuText.text += "Click a button to start the experiments";

        buttons[0] = GameObject.Find("StartTrainingButton").GetComponent<Button>();
        buttons[1] = GameObject.Find("StartPracticeButton").GetComponent<Button>();
        buttons[2] = GameObject.Find("FirstSessionButton").GetComponent<Button>();
        buttons[3] = GameObject.Find("SecondSessionButton").GetComponent<Button>();
        buttons[4] = GameObject.Find("ThirdSessionButton").GetComponent<Button>();
        buttons[5] = GameObject.Find("FourthSessionButton").GetComponent<Button>();

        for (int i = 0; i < 2; i++)
        {
            ColorBlock cb_practice = buttons[i].colors;
            cb_practice.normalColor = new Color(0.9f, 0.9f, 0.9f);
            cb_practice.highlightedColor = Color.white;
            buttons[i].colors = cb_practice;
        }

        for (int i=2; i<6; i++)
        {
            Debug.Log(buttons[i].name + " " + orderCB[i - 2]);
            switch (orderCB[i-2])
            {
                
                case 0:
                    ColorBlock cb = buttons[i].colors;
                    cb.normalColor = activeColor;
                    cb.highlightedColor = activeHighlightedColor;
                    Debug.Log(activeColor.ToString());
                    buttons[i].colors = cb;
                    break;
                case 1:
                    ColorBlock cb2 = buttons[i].colors;
                    cb2.normalColor = passiveColor;
                    cb2.highlightedColor = passiveHighlightedColor;
                    buttons[i].colors = cb2;
                    break;
            }
        }


        LeapCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();
    }

    // Use this for initialization
    protected override void DoStart()
    {
        GameObject headposition = GameObject.Find("HandSwither");
        headposition.transform.localPosition = new Vector3(HeadOriginX, HeadOriginY, HeadOriginZ);


        LeapPosition.transform.localPosition = new Vector3(LeapHandX, LeapHandY, LeapHandZ);

        for (int i = 0; i < num_sessions; i++)
        {
            ResultLists[i] = new List<Result>[2];
            for (int blockId = 0; blockId < 2; blockId++)
            {
                ResultLists[i][blockId] = new List<Result>();
                for (int j = 0; j < intervals.Length; j++)
                {
                    switch (i)
                    {
                        case (int)ExpSession.ACTIVE:
                            for (int k = 0; k < numActive; k++)
                            {
                                Result param = new Result();
                                param.sessionId = i;
                                param.blockId = blockId;
                                param.intervalId = j;
                                param.VRconditionId = 0;
                                param.actionId = 1;
                                param.planned_interval = intervals[j];
                                ResultLists[i][blockId].Add(param);
                            }
                            break;
                        case (int)ExpSession.PASSIVE_HAND:

                            for (int k = 0; k < numPassiveHand; k++)
                            {
                                Result param = new Result();
                                param.sessionId = i;
                                param.blockId = blockId;
                                param.intervalId = j;
                                param.VRconditionId = 1;
                                param.actionId = 0;
                                param.planned_interval = intervals[j];
                                ResultLists[i][blockId].Add(param);
                            }
                            break;

                        case (int)ExpSession.PASSIVE:
                            for (int k = 0; k < numPassive; k++)
                            {
                                Result param = new Result();
                                param.sessionId = i;
                                param.blockId = blockId;
                                param.intervalId = j;
                                param.VRconditionId = 3; //vibration
                                param.actionId = 0;
                                param.planned_interval = intervals[j];
                                ResultLists[i][blockId].Add(param);
                            }
                            break;

                        case (int)ExpSession.PRACTICE:
                            if (blockId == 0)
                            {
                                for (int k = 0; k < numPractice; k++)
                                {
                                    Result param = new Result();
                                    param.sessionId = i;
                                    param.blockId = 0;
                                    param.intervalId = j;
                                    param.actionId = 1;
                                    param.VRconditionId = 0;
                                    param.planned_interval = intervals[j];

                                    ResultLists[i][blockId].Add(param);
                                }
                            }
                            break;

                        case (int)ExpSession.TRAINING:
                            if (blockId == 0)
                            {
                                for (int k = 0; k < numTraining; k++)
                                {
                                    Result param = new Result();
                                    param.sessionId = i;
                                    param.intervalId = j;
                                    int variance = UnityEngine.Random.Range(0, 200);
                                    param.planned_interval = intervals[j] + variance;
                                    //Debug.Log(variance + " " + param.actual_interval);
                                    param.actionId = 0;
                                    param.VRconditionId = 2; // tone

                                    ResultLists[i][blockId].Add(param);
                                }
                            }
                            break;
                    }
                }
             }
         }

        for (int i = 0; i < num_sessions; i++)
        {
            if (i == (int)ExpSession.ACTIVE || i == (int)ExpSession.PASSIVE_HAND || i == (int)ExpSession.PASSIVE)
            {
                for (int blockId = 0; blockId < 2; blockId++)
                {
                    Shuffle(ResultLists[i][blockId]);
                    Debug.Log("Sesssion:" + (ExpSession)i  +" # of trials:" + ResultLists[i][blockId].Count);
                }
            }
            else
            {
                Shuffle(ResultLists[i][0]);
                Debug.Log("Sesssion Id:" + (ExpSession)i + " # of trials:" + ResultLists[i][0].Count);
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
        if(isBlockRunning)
        {
            return;
        }

        isRobot = false;
        LeapCtl.CurrentGroup = HandModel;

        Debug.Log("Experiment Start MenuId:" + menuId);
        switch (menuId)
        {
            case MenuLabel.TRAINING:
                SessionId = ExpSession.TRAINING;
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction9_TrainingReproduction").text;
                break;
            case MenuLabel.PRACTICE:
                SessionId = ExpSession.PRACTICE;
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction9_PracticeReproduction").text;
                break;
            case MenuLabel.FIRST:
                InstructionText.text = "1st Main Block - ";
                break;
            case MenuLabel.SECOND:
                InstructionText.text = "2nd Main Block - ";
                break;
            case MenuLabel.THIRD:
                InstructionText.text = "3rd Main Block - ";
                break;
            case MenuLabel.FOURTH:
                InstructionText.text = "4th Main Block - ";
                break;
        }

        BlockId = 0;
        if(menuId == MenuLabel.FIRST || menuId == MenuLabel.SECOND || menuId == MenuLabel.THIRD || menuId == MenuLabel.FOURTH )
        {
            int id = (int)menuId - 2;
            Debug.Log("Sessions " + id + " " + orderCB[id]);
            switch ( orderCB[id] )
            {
                case 0:
                    SessionId = ExpSession.ACTIVE;
                    if(id > 1) { BlockId = 1; } else { BlockId = 0; }
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction10_AcitveReproduction").text;

                    break;
                case 1:
                    SessionId = ExpSession.PASSIVE;
                    if (id > 1) { BlockId = 1; } else { BlockId = 0; }
                    InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction12_PassiveReproduction").text;

                    break;
            }

        }

        currentMenu = menuId;

        Debug.Log(SessionId);

        isBlockRunning = true;
        StartCoroutine(sessionCtl.CoExperimentStart(SessionId, BlockId));
    }

    public override void DeactivateCurrentMenu()
    {
        Debug.Log("Button deactivate : " + (int)currentMenu);
        Button b = buttons[(int)currentMenu];

        ColorBlock cb = b.colors;
        cb.normalColor = new Color(0.1f, 0.1f, 0.1f);
        cb.highlightedColor = new Color(0.2f, 0.2f, 0.2f);
        b.colors = cb;

        b.gameObject.GetComponent<EventTrigger>().triggers.Clear();

    }

    protected override void LoadVariablesFromInitFile()
    {
        string initfile = Application.dataPath + "/../" + config_file;
        iniFileCtl = new INIFile(initfile);

        LoadIntFromFile(ref SubId, "Experiment", "SubId");

        LoadIntFromFile(ref numTraining, "Experiment", "numTraining");
        LoadIntFromFile(ref numPractice, "Experiment", "numPractice");
        LoadIntFromFile(ref numActive, "Experiment", "numActive");
        LoadIntFromFile(ref numPassive, "Experiment", "numPassive");
        LoadIntFromFile(ref numPassiveHand, "Experiment", "numPassiveHand");

        LoadIntFromFile(ref intervals[0], "Experiment", "interval1");
        LoadIntFromFile(ref intervals[1], "Experiment", "interval2");
        LoadIntFromFile(ref intervals[2], "Experiment", "interval3");

        LoadFloatFromFile(ref task_rest_secs, "Experiment", "task_rest_secs");
        LoadFloatFromFile(ref task_answer_rest_secs, "Experiment", "task_answer_rest_secs");
        LoadIntFromFile(ref debug_mode_num_trials, "Experiment", "debug_mode_num_trials");
        LoadBooleanFromFile(ref debugMode, "Experiment", "debugMode");
        LoadBooleanFromFile(ref recordTracking, "Experiment", "recordTracking");

        LoadIntFromFile(ref PassiveLatencyMin, "Experiment", "PassiveLatencyMin");
        LoadIntFromFile(ref PassiveLatencyMax, "Experiment", "PassiveLatencyMax");
        LoadIntFromFile(ref brakAfterTrials, "Experiment", "brakAfterTrials");
        LoadIntFromFile(ref breakSeconds, "Experiment", "breakSeconds");
        LoadIntFromFile(ref day, "Experiment", "day");

        LoadIntFromFile(ref HandModel, "Experiment", "FemaleHand");

        LoadIntFromFile(ref numFakeRecording, "Experiment", "numFakeRecording");

        LoadFloatFromFile(ref HeadOriginX, "VR", "HeadOriginX");
        LoadFloatFromFile(ref HeadOriginY, "VR", "HeadOriginY");
        LoadFloatFromFile(ref HeadOriginZ, "VR", "HeadOriginZ");


        LoadFloatFromFile(ref LeapHandX, "VR", "LeapHandX");
        LoadFloatFromFile(ref LeapHandY, "VR", "LeapHandY");
        LoadFloatFromFile(ref LeapHandZ, "VR", "LeapHandZ");

        Debug.Log("female " + HandModel);
    }



    // Update is called once per frame
    protected override void DoUpdate()
    {
        if (Input.GetKey(KeyCode.Keypad8))
        {
            LeapHandY += 0.001f;
            LeapPosition.transform.localPosition = new Vector3(LeapHandX, LeapHandY, LeapHandZ);

        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
             LeapHandY -= 0.001f;
            LeapPosition.transform.localPosition = new Vector3(LeapHandX, LeapHandY, LeapHandZ);

        }
        if (Input.GetKey(KeyCode.Keypad6))
        {
            LeapHandX += 0.001f;
            LeapPosition.transform.localPosition = new Vector3(LeapHandX, LeapHandY, LeapHandZ);

        }
        if (Input.GetKey(KeyCode.Keypad4))
        {
            LeapHandX -= 0.001f;
            LeapPosition.transform.localPosition = new Vector3(LeapHandX, LeapHandY, LeapHandZ);

        }
        if (Input.GetKey(KeyCode.Keypad9))
        {
            LeapHandZ += 0.001f;
            LeapPosition.transform.localPosition = new Vector3(LeapHandX, LeapHandY, LeapHandZ);

        }
        if (Input.GetKey(KeyCode.Keypad3))
        {
            LeapHandZ -= 0.001f;
            LeapPosition.transform.localPosition = new Vector3(LeapHandX, LeapHandY, LeapHandZ);

        }

        //up/down buttons
        if (Input.GetKey(KeyCode.Home))
        {
            ButtonGadget.transform.Translate(0, 0.005f, 0);

        }
        if (Input.GetKey(KeyCode.End))
        {
            ButtonGadget.transform.Translate(0, -0.005f, 0);
        }

        //rotatin by AD torotate HandSwither
        if (Input.GetKey(KeyCode.A))
        {
            EyeCentre.transform.Rotate(0, 0.05f, 0);

        }
        if (Input.GetKey(KeyCode.D))
        {
            EyeCentre.transform.Rotate(0f, -0.05f, 0f);
        }


        //move by arrow keys XZ HandSwither
        if (Input.GetKey(KeyCode.RightArrow))
        {

            EyeCentre.transform.Translate(0f, 0f, -0.01f);

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {

            EyeCentre.transform.Translate(0f, 0f, 0.01f);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            EyeCentre.transform.Translate(0.01f, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            EyeCentre.transform.Translate(-0.01f, 0f, 0f);
        }

        //move by page up/down keys Y HandSwither
        if (Input.GetKey(KeyCode.PageUp))
        {
            EyeCentre.transform.Translate(0, -0.01f, 0f);
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            EyeCentre.transform.Translate(0f, 0.01f, 0f);
        }



    }


}
