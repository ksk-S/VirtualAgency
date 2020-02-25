using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;


class ExperimentAgencyFakeReproductionBlockedAgencyOnly : ExperimentAbstract
{

    GameObject EyeCentre;
    // GameObject LeapPosition;
    GameObject ButtonGadget;

    List<int> order = new List<int>(){ 0, 1};
    List<int> orderCB = new List<int>(4);


    public Color activeColor = Color.red;
    public Color fakeColor = Color.blue;

    public Color activeHighlightedColor = Color.white;
    public Color fakeHighlightedColor = Color.white;

    float HeadOriginX = 1f;
    float HeadOriginY = 1f;
    float HeadOriginZ = -0.35f;

    bool double_even_subject;

    CycleHandPairs LeapCtl;
    public enum MenuLabel
    {
        PRACTICE = 0, FIRST, SECOND, THIRD, FOURTH
    }

    MenuLabel currentMenu = MenuLabel.PRACTICE;

    Button[] buttons = new Button[8];

    protected override void DoAwake()
    {

        EyeCentre = GameObject.Find("HandSwither");
        ButtonGadget = GameObject.Find("ButtonGadget");
       // LeapPosition = GameObject.Find("LeapHandController");

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


        double_even_subject = ( (SubId / 2) % 2 == 0) ? true : false;

        Debug.Log("sub " + SubId + " double even " + double_even_subject + " Block0:" + (double_even_subject ^ BlockId==0) + " Block1" + (double_even_subject ^ BlockId == 1));


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

        buttons[0] = GameObject.Find("StartPracticeButton").GetComponent<Button>();
        buttons[1] = GameObject.Find("FirstSessionButton").GetComponent<Button>();
        buttons[2] = GameObject.Find("SecondSessionButton").GetComponent<Button>();
        buttons[3] = GameObject.Find("ThirdSessionButton").GetComponent<Button>();
        buttons[4] = GameObject.Find("FourthSessionButton").GetComponent<Button>();

        for (int i = 0; i < 1; i++)
        {
            ColorBlock cb_practice = buttons[i].colors;
            cb_practice.normalColor = new Color(0.9f, 0.9f, 0.9f);
            cb_practice.highlightedColor = Color.white;
            buttons[i].colors = cb_practice;
        }

        for (int i=1; i<5; i++)
        {
            switch (orderCB[i-1])
            {
                case 0:
                    ColorBlock cb = buttons[i].colors;
                    cb.normalColor = activeColor;
                    cb.highlightedColor = activeHighlightedColor;
                    buttons[i].colors = cb;
                    break;
                case 1:
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
        GameObject headposition = GameObject.Find("HandSwither");
        headposition.transform.localPosition = new Vector3(HeadOriginX, HeadOriginY, HeadOriginZ);

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
                                param.question = double_even_subject ^ blockId == 0 ? 1 : 0;
                                param.askAgency = 1;
                                Debug.Log(param.question);
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
                                param.question = double_even_subject ^ blockId == 0 ? 1 : 0;
                                param.askAgency = 1;
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

        UnityEngine.XR.InputTracking.Recenter();
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

        Debug.Log("Experiment Start MenuId:" + menuId);
        switch (menuId)
        {
            case MenuLabel.PRACTICE:
                SessionId = ExpSession.PRACTICE;
                InstructionText.text = Resources.Load<TextAsset>("Texts/Agency/Instruction17_AgencyOnlyPractice").text;
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
            int id = (int)menuId - 1;
            Debug.Log("Sessions " + id + " " + orderCB[id]);
            switch ( orderCB[id] )
            {
                case 0:
                    SessionId = ExpSession.ACTIVE;
                    if(id > 1) { BlockId = 1; } else { BlockId = 0; }

                    if (ResultLists[(int)SessionId][BlockId][0].question == 0)
                    {
                        InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction13_Active_Agency").text;
                    }
                    else {
                        InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction14_Active_Causation").text;
                    };


                    //InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction10_AcitveReproduction").text;
                    isRobot = false;
                    LeapCtl.CurrentGroup = HandModel;
                    
                    break;
                case 1:
                    SessionId = ExpSession.PASSIVE_HAND;
                    if (id > 1) { BlockId = 1; } else { BlockId = 0; }

                    if (ResultLists[(int)SessionId][BlockId][0].question == 0)
                    {
                        InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction15_Fake_Agency").text;
                    }
                    else {
                        InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction16_Fake_Causation").text;
                    };
                    //InstructionText.text += Resources.Load<TextAsset>("Texts/Agency/Instruction11_PassiveHandReproduction").text;

                    isRobot = false;
                    LeapCtl.CurrentGroup = HandModel;
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

        Debug.Log("female " + HandModel);
    }



    // Update is called once per frame
    protected override void DoUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            UnityEngine.XR.InputTracking.Recenter();
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
