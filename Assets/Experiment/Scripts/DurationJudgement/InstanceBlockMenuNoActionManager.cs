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
    class InstanceBlockMenuNoActionManager : AbstractMenuManager
    {

        protected override void DoAwake()
        {
            LoadVariablesFromInitFile("configDurationJudgement.ini");
            dataCtl.SetRandomSeed(SubId);

            GameObject.Find("VideoController").GetComponent<TargetMovieControl>().num_images = dataCtl.numTargetAction;

            sessionCtl = gameObject.GetComponent<DurationJudgement.AbstractSessionManager>();

            sessionCtl.isAskingAgencyEndOfBlock = true;
            isMultiGroupMenu = true;

            numPreButtons = 2;
            numMainButtons = 24;
            numButtons = numPreButtons + numMainButtons;

            ButtonMinIndex = 0;
            ButtonMaxIndex = 10;

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
                buttonNoAction.Add(false);
            }

            selectedButton = 0;

            UpdateButtons();


        }


        // Use this for initialization
        protected override void DoStart()
        {
            //dataCtl.CreateBlockConditions();
            //dataCtl.CreatePracticeConditions();

            dataCtl.CreateBlockNoActionConditions();
            dataCtl.CreatePracticeNoActionConditions();

            for(int i=0; i < dataCtl.ResultLists.Main.Count; i++)
            {
                if(dataCtl.ResultLists.Main[i][0].actionType == DataManager.ActionyType.NO_ACTION)
                {
                    buttonNoAction[i + 2] = true;
                }
            }


            dataCtl.SetFilename(result_dir, "all", SubId);
            dataCtl.SaveHeaderToFile(HandModel);


            GameObject headposition = GameObject.Find("HandSwither");
            headposition.transform.localPosition = new Vector3(HeadOriginX, HeadOriginY, HeadOriginZ);

            LoadBlockCompleteLog();

            StartCoroutine(ChangeHand(0.5f));

        }

        
        public void ExperimentStart(int menuId)
        {
            if (isBlockRunning)
            {
                return;
            }
            Debug.Log("Experiment Start MenuId:" + menuId);
            LeapHandCtl.CurrentGroup = HandModel;

            SessionId = ExpSession.MAIN;

            curBlockId = menuId - 2;

            if (menuId == 0)
            {
                InstructionText.text = "Practice Block\n\n";
                InstructionText.text += Resources.Load<TextAsset>("Texts/DurationNoAction/E2_1_practice").text;// neeed to rewrite

                isBlockRunning = true;
                StartCoroutine(sessionCtl.CoPracticeStart(dataCtl.ResultLists.Practice));
            }
            else if (menuId == 1 )
            {
                InstructionText.text = "Training Block\n\n";
                InstructionText.text += Resources.Load<TextAsset>("Texts/DurationNoAction/E2_2_training").text; // neeed to rewrite

                isBlockRunning = true;
                StartCoroutine(sessionCtl.CoTrainingStart());

            }
            else
            {
                UnityEngine.Debug.Log("main " + curBlockId);
                if (curBlockId == 0)
                {
                    InstructionText.text = (menuId -1) + "st Main Block\n\n";
                }
                else if (curBlockId == 1)
                {
                    InstructionText.text = (menuId - 1) + "nd Main Block\n\n";
                }
                else if (curBlockId == 2)
                {
                    InstructionText.text = (menuId - 1) + "rd Main Block\n\n";
                }
                else
                {
                    InstructionText.text = (menuId - 1) + "th Main Block\n\n";
                }

                InstructionText.text += Resources.Load<TextAsset>("Texts/DurationNoAction/E2_3_main").text;// neeed to rewrite

                isBlockRunning = true;
                StartCoroutine(sessionCtl.CoExperimentStart(dataCtl.ResultLists.Main[curBlockId], SessionId, curBlockId));
            }
        }




        // Update is called once per frame
        protected override void DoUpdate()
        {
            if (!isBlockRunning)
            {
                // Power Mate Control
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (buttonActive[selectedButton])
                    {
                        ExperimentStart(selectedButton);
                    }
                }

                if (Input.GetKeyDown(KeyCode.F12))
                {
                    InstructionText.text = "Screen Capture Session\n";
                    StartCoroutine(sessionCtl.CoScreenCaptureSession());
                }
            }
        }

    }
}