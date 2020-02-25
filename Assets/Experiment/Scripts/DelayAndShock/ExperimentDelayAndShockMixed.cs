using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;

class ExperimentDelayAndShockMixed : AbstractExperimentDelayAndShock
{


    protected override void DoAwake()
    {
        LoadVariablesFromInitFile("configDelayShock.ini");

        sessionCtl = gameObject.GetComponent<DelayAndShockSessionManager>();

        numButtons = 13;

        //Menus
        for (int i = 0; i < numButtons; i++)
        {
            EventTrigger.Entry entryPoint = new EventTrigger.Entry();
            entryPoint.eventID = EventTriggerType.PointerDown;
            int iLocal = i;
            entryPoint.callback.AddListener((eventData) => { ExperimentStart(iLocal); });
            GameObject.Find("MenuButton" + i).GetComponent<EventTrigger>().triggers.Add(entryPoint);

            buttons.Add(GameObject.Find("MenuButton" + i).GetComponent<Button>());
        }


        for (int i = 0; i < numButtons; i++)
        {
            buttonActive.Add(true);
        }

        SelectButton();
    }


    // Use this for initialization
    protected override void DoStart()
    {
        GameObject headposition = GameObject.Find("HandSwither");
        headposition.transform.localPosition = new Vector3(HeadOriginX, HeadOriginY, HeadOriginZ);

        Debug.Log("numTrial:" + numTrial);

        for (int i = 0; i < num_sessions; i++)
        {
            if( (ExpSession)i == ExpSession.MAIN) {
                ResultLists[i] = new List<List<Result>>();;
                for(int repeatId=0; repeatId < num_block_repeat; repeatId++) {
                    for(int stimulusId=0; stimulusId < 3; stimulusId++) {
                        for (int delayId = 0; delayId < 2; delayId++)
                        {
                            int blockId = repeatId * 6 + stimulusId * 2 + delayId;
                            ResultLists[i].Add(new List<Result>());
                            for (int trialId = 0; trialId < numTrial; trialId++)
                            {
                                Result param = new Result();
                                param.sessionId = i;
                                param.blockId = blockId;
                                param.trialId = trialId;
                                param.repeatId = repeatId;

                                param.stimulusType = (StimulusType)stimulusId;
                                param.VRcondition = delayId == 0 ? (VRCondition)0 : (VRCondition)2;
                                param.agency = -1;

                                param.StimuliList = new List<ResultStimuli>();
                                ResultLists[i][blockId].Add(param);
                            }
                        }
                    }
                }
            }
        }

        Shuffle(ResultLists[(int)ExpSession.MAIN]);

        for (int i = 0; i < ResultLists[(int)ExpSession.MAIN].Count(); i++)
        {
            Shuffle(ResultLists[(int)ExpSession.MAIN][i]);
            List<Result> result = ResultLists[(int)ExpSession.MAIN][i];
            Debug.Log("Id:" + i + " blockId:" + result[0].blockId + " " + result[0].VRcondition + " " + result[0].stimulusType + " # of trials:" + result.Count);
        }

        LoadBlockCompleteLog();

        StartCoroutine(ChangeHand());

        GameObject.Find("VisualStimulus").GetComponent<CollisionDetector>().timeout =  task_stimulus_time_to_hide;

        sessionCtl.SetFilenameGlobal("all", SubId);
        sessionCtl.SaveHeaderToGlobalFile();

        //Debug.Log("head write");
    }

    IEnumerator ChangeHand()
    {
        yield return new WaitForSeconds(1.0f);
   
        LeapCtl.CurrentGroup = HandModel;
       
    }
    public void ExperimentStart(int menuId)
    {
        if(isBlockRunning)
        {
            return;
        }
        Debug.Log("Experiment Start MenuId:" + menuId);

        LeapCtl.CurrentGroup = HandModel;

        SessionId = ExpSession.MAIN;

        curBlockId = menuId - 1;
        if (menuId == 0)
        {
            InstructionText.text =  "Training Block\n";
            InstructionText.text += Resources.Load<TextAsset>("Texts/DelayEEG/instruction").text;

            isBlockRunning = true;
            StartCoroutine(sessionCtl.CoPracticeStart());
        }
        else
        {
            if (menuId == 1)
            {
                InstructionText.text = menuId + "st Main Block\n";
            }
            else if (menuId == 2)
            {
                InstructionText.text = menuId + "nd Main Block\n";
            }
            else if (menuId == 3)
            {
                InstructionText.text = menuId + "rd Main Block\n";
            }
            else
            {
                InstructionText.text = menuId + "th Main Block\n";
            }

            InstructionText.text += Resources.Load<TextAsset>("Texts/DelayEEG/instruction").text;

            isBlockRunning = true;
            StartCoroutine(sessionCtl.CoExperimentStart(SessionId, curBlockId));
        }
    }


    // Update is called once per frame
    protected override void DoUpdate()
    {
        // Power Mate Control
        if (!isBlockRunning)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (buttonActive[selectedButton])
                {
                    ExperimentStart(selectedButton);
                }
            }
        }
    }

}
