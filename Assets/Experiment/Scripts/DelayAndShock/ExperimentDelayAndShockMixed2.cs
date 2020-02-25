using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;

class ExperimentDelayAndShockMixed2 : AbstractExperimentDelayAndShock
{
 

    protected override void DoAwake()
    {

        LoadVariablesFromInitFile("configDelayShock2018.ini");
        sessionCtl = gameObject.GetComponent<AbstractDelayAndShockSessionManager>();
        sessionCtl.DisableAR();

        numButtons = 11;

        //Menus
        for (int i = 0; i < numButtons; i++)
        {
            EventTrigger.Entry entryPoint = new EventTrigger.Entry();
            entryPoint.eventID = EventTriggerType.PointerDown;
            int iLocal = i;
            entryPoint.callback.AddListener((eventData) => { ExperimentStart(iLocal); });
            GameObject.Find("MenuButton" + i).GetComponent<EventTrigger>().triggers.Add(entryPoint);

            buttons.Add(GameObject.Find("MenuButton" + i).GetComponent<Button>());

            buttonActive.Add(true);
        }

        selectedButton = 0;

        SelectButton();


    }


    // Use this for initialization
    protected override void DoStart()
    {


        GameObject headposition = GameObject.Find("HandSwither");
        headposition.transform.localPosition = new Vector3(HeadOriginX, HeadOriginY, HeadOriginZ);

        Debug.Log("numTrial:" + numTrial);


        ResultLists[(int)ExpSession.MAIN] = new List<List<Result>>();

        List<Result> TempList = new List<Result>();
        for (int stimulusId = 0; stimulusId < 2; stimulusId++)
        {
            for (int delayId = 0; delayId < 2; delayId++)
            {
                for (int trialId = 0; trialId < numTrial; trialId++)
                {
                    Result param = new Result();
                    param.sessionId = (int)ExpSession.MAIN;
                    param.blockId = 0;
                    param.trialId = trialId;
                    param.repeatId = 0;

                    param.stimulusType = stimulusId == 0 ? StimulusType.TACTILE : StimulusType.VISUALTACTILE;
                    param.VRcondition = delayId == 0 ? VRCondition.SYNC: VRCondition.DELAY_L;
                    param.agency = -1;

                    param.StimuliList = new List<ResultStimuli>();
                    TempList.Add(param);
                }

            }
        }

        Shuffle(TempList);
        

        //for (int i = 0; i < TempList.Count(); i++)
        //{
            //Result param = TempList[i];
            //Debug.Log(param.trialId + " " + param.stimulusType + " "+ param.VRcondition);
        //}

        int num_all = TempList.Count();
        int num_trial_per_block = num_all / num_blocks;
        Debug.Log("all num list : " + TempList.Count()+ " num_block:" + num_blocks + " num_trials:" + num_trial_per_block);

        ResultLists[(int)ExpSession.MAIN] = new List<List<Result>>();

        ResultLists[(int)ExpSession.MAIN].Add(new List<Result>());
        for (int blockId = 1; blockId < num_blocks+1; blockId++)
        {
            ResultLists[(int)ExpSession.MAIN].Add(new List<Result>());
            for (int trialId = 0; trialId < num_trial_per_block; trialId++)
            {
                int index = trialId + (blockId-1) * num_trial_per_block;
                //  Debug.Log(index + " " + blockId);
                

                Result param = TempList[index];
                param.index = index;
                param.trialId = trialId;
                param.blockId = blockId;
                ResultLists[(int)ExpSession.MAIN][blockId].Add(param);

                //Debug.Log("Id:" + index + " blockId:" + blockId + " " + param.VRcondition + " " + param.stimulusType);
            }
        }

        //bare hand
        for(int bareblock=0; bareblock <2; bareblock++)
        {
            ResultLists[(int)ExpSession.MAIN].Add(new List<Result>());
            for (int trialId = 0; trialId < numTrial/2; trialId++)
            {
                Result param = new Result();
                param.sessionId = (int)ExpSession.MAIN;
                param.blockId = bareblock == 0 ? 0 : num_blocks +1;
                param.trialId = trialId;
                param.repeatId = 0;
                param.stimulusType = StimulusType.TACTILE;
                param.VRcondition = (VRCondition)0;
                param.agency = -1;

                param.StimuliList = new List<ResultStimuli>();
                ResultLists[(int)ExpSession.MAIN][bareblock == 0 ? 0 : num_blocks + 1].Add(param);
            }
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
            InstructionText.text += Resources.Load<TextAsset>("Texts/DelayEEG/practice").text;

            isBlockRunning = true;
            StartCoroutine(sessionCtl.CoPracticeStart());
        }
        else if (menuId == 1 || menuId == 10)
        {
            InstructionText.text = "Bare Hand Block\n";
            InstructionText.text += Resources.Load<TextAsset>("Texts/DelayEEG/barehand").text;

            isBlockRunning = true;
            UnityEngine.Debug.Log("bare " + curBlockId);
            StartCoroutine(sessionCtl.CoBarehandStart(curBlockId));

        }
        else
        {

            UnityEngine.Debug.Log("main " + curBlockId);
            if (curBlockId == 1)
            {
                InstructionText.text = menuId + "st Main Block\n";
            }
            else if (curBlockId == 2)
            {
                InstructionText.text = menuId + "nd Main Block\n";
            }
            else if (curBlockId == 3)
            {
                InstructionText.text = menuId + "rd Main Block\n";
            }
            else
            {
                InstructionText.text = menuId + "th Main Block\n";
            }



            InstructionText.text += Resources.Load<TextAsset>("Texts/DelayEEG/instruction2").text;

            isBlockRunning = true;
            StartCoroutine(sessionCtl.CoExperimentStart(SessionId, curBlockId));
        }
    }




    // Update is called once per frame
    protected override void DoUpdate()
    {
        if (!isBlockRunning)
        {
            // Power Mate Control
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
