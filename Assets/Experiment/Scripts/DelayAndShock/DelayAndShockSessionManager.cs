using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using UnityEngine.EventSystems;

using System.Media;
using System.Diagnostics;

using Leap;

public class DelayAndShockSessionManager : AbstractDelayAndShockSessionManager
{

    public override IEnumerator CoPracticeStart()
    {
       
       // CycleHandPairs LeapCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();

        //
        //Instruction
        //
        MenuPanel.GetComponent<Fader>().FadeOut();
        InstructionPanel.GetComponent<Fader>().FadeIn();

        yield return 0;
        yield return StartCoroutine("WaitInstructionButtonPressed");
        if (isInstructionBackButtonPressed)
        {
            MenuPanel.GetComponent<Fader>().FadeIn();
            InstructionPanel.GetComponent<Fader>().FadeOut();

            isInstructionBackButtonPressed = false;

            expCtl.isBlockRunning = false;
            yield break;
        }
        InstructionPanel.GetComponent<Fader>().FadeOut();

        SetLeapPointerHide();
        
        //
        // Practice Block
        //
        screenfader.FadeIn();

        yield return new WaitForSeconds(expCtl.task_rest_secs);
        screenfader.FadeOut();

        //Delay Control
        leapCtl.recorder_.StopDelay();

        //Free movement + all guideded movement
        yield return StartCoroutine(ShowHandInstAndInduce("Start to freely move your hand. PALMS UP", 10f));
        for(int i=0; i< GuidedMovementsText.Count; i++)
        {
            yield return StartCoroutine(ShowHandInstAndInduce(GuidedMovementsText[i], 10f));
        }

        AlertText.text = "Put your hand back in a relaxed position";
        AlertText.color = Color.blue;
        AlertPanel.GetComponent<Fader>().FadeIn();
        yield return new WaitForSeconds(expCtl.task_after_task_rest_secs);
        AlertPanel.GetComponent<Fader>().FadeOut();

        yield return new WaitForSeconds(1.0f);

        AlertPanel.GetComponent<Fader>().FadeIn();
        for (int countdown = 5; countdown > 0; countdown--)
        {
            AlertText.text = "Stimulation is going to start in " + countdown + " seconds";
            yield return new WaitForSeconds(1.0f);
        }
        AlertPanel.GetComponent<Fader>().FadeOut();

        yield return new WaitForSeconds(0.5f);


        AlertText.color = Color.red;
        AlertText.text = "Tactile Stimuli\nKeep your hand still";
        AlertPanel.GetComponent<Fader>().FadeIn();

        //
        // Stimulus
        //
        UnityEngine.Debug.Log("Stimulus Count:" + expCtl.numStimulus);

        for (int stimulusId = 0; stimulusId < 3; stimulusId++)
        {
            float actual_stimulus_rest = expCtl.task_stimulus_rest_secs + UnityEngine.Random.Range(-expCtl.task_stimulus_jitter, expCtl.task_stimulus_jitter);
            yield return new WaitForSeconds(actual_stimulus_rest);

            AlertPanel.GetComponent<Fader>().FadeOut();

            stimulator.StimulatePracticeTactile();
        }
        yield return new WaitForSeconds(1.0f);

        AlertPanel.GetComponent<Fader>().FadeOut();

        AlertText.color = Color.red;
        AlertText.text = "Visual Stimuli\nKeep your hand still";
        AlertPanel.GetComponent<Fader>().FadeIn();
        for (int stimulusId = 0; stimulusId < 3; stimulusId++)
        {
            float actual_stimulus_rest = expCtl.task_stimulus_rest_secs + UnityEngine.Random.Range(-expCtl.task_stimulus_jitter, expCtl.task_stimulus_jitter);
            yield return new WaitForSeconds(actual_stimulus_rest);
            AlertPanel.GetComponent<Fader>().FadeOut();

            stimulator.StimulatePracticeVisual();
        }

        yield return 0;


        //finalizing 
        AlertPanel.GetComponent<Fader>().FadeOut();
        screenfader.FadeOut();

        expCtl.DeactivateCurrentMenu();

        MenuPanel.GetComponent<Fader>().FadeIn();
        expCtl.isBlockRunning = false;
    }

    public override IEnumerator CoExperimentStart(AbstractExperimentDelayAndShock.ExpSession SessionId, int blockId)
    {

        //hand tracking
        FramesList = new List<HandFrames>();
        tracking_data_dir = getDirForTracking(expCtl.SubId, (int)SessionId);

        UnityEngine.Debug.Log("SessionId:"+SessionId.ToString() + " BlockId:" + blockId);
        ResultList = expCtl.ResultLists[(int)SessionId][blockId];

        //CycleHandPairs LeapCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();

        //
        //Instruction
        //
        MenuPanel.GetComponent<Fader>().FadeOut();
        InstructionPanel.GetComponent<Fader>().FadeIn();

        yield return 0;
        yield return StartCoroutine("WaitInstructionButtonPressed");
        if (isInstructionBackButtonPressed)
        {
            MenuPanel.GetComponent<Fader>().FadeIn();
            InstructionPanel.GetComponent<Fader>().FadeOut();

            isInstructionBackButtonPressed = false;

            expCtl.isBlockRunning = false;
            yield break;
        }
        InstructionPanel.GetComponent<Fader>().FadeOut();

        SetLeapPointerHide();

        sw.Reset(); sw.Start();

        //
        //Experiment 
        //
        int max = ResultList.Count;
        //debug
        if (expCtl.debugMode) max = expCtl.debug_mode_num_trials;
        for (int i = 0; i < max; i++)
        {
            AbstractExperimentDelayAndShock.Result param = ResultList[i];
            param.blockId = blockId + 1;
            SetFilename("Stimulus", expCtl.SubId, param.blockId, param.VRcondition.ToString() + "_" + param.stimulusType.ToString(), 0, 0);
            SaveHeaderToFile();

            
            screenfader.FadeIn();

            yield return new WaitForSeconds(expCtl.task_rest_secs);
            screenfader.FadeOut();

            if (expCtl.recordTracking)
            {
                leapCtl.recorder_.StartRecording();
            }

            //Delay Control
            UnityEngine.Debug.Log(param.VRcondition + " " + param.stimulusType + " " + param.repeatId);
            leapCtl.recorder_.meanDelaySec = 0.0f;
            switch (param.VRcondition)
            {
                case AbstractExperimentDelayAndShock.VRCondition.SYNC:
                    leapCtl.recorder_.StopDelay();
                    break;
                case AbstractExperimentDelayAndShock.VRCondition.DELAY_S:
                    leapCtl.recorder_.StartDelayingWithFrame(expCtl.delayFrameShort);
                    break;
                case AbstractExperimentDelayAndShock.VRCondition.DELAY_L:
                    leapCtl.recorder_.StartDelayingWithFrame(expCtl.delayFrameLong);
                    break;

            }

            //don't use the same id for movement instruction
            int handMovementId1 = UnityEngine.Random.Range(0, GuidedMovementsText.Count);
            int handMovementId2 = 0;
            do
            {
                handMovementId2 = UnityEngine.Random.Range(0, GuidedMovementsText.Count);
            } while (handMovementId2 == handMovementId1);
                

            param.inductionStartTime = sw.ElapsedMilliseconds;
            int peiodLabel = stimulator.SendPeriodMarker(1, (int)param.VRcondition, (int)param.stimulusType, param.repeatId);
            SavePeriodMarker(param, sw.ElapsedMilliseconds, peiodLabel);


            yield return StartCoroutine(ShowHandInstAndInduce("Start to freely move your hand. PALMS UP", expCtl.task_induction_secs));
            yield return StartCoroutine(ShowHandInstAndInduce(GuidedMovementsText[handMovementId1], expCtl.task_induction_secs));
            yield return StartCoroutine(ShowHandInstAndInduce(GuidedMovementsText[handMovementId2], expCtl.task_induction_secs));
            yield return StartCoroutine(ShowHandInstAndInduce("Free Movements", expCtl.task_induction_secs));

            param.inductionEndTime = sw.ElapsedMilliseconds;
            peiodLabel = stimulator.SendPeriodMarker(2, (int)param.VRcondition, (int)param.stimulusType, param.repeatId);
            SavePeriodMarker(param, sw.ElapsedMilliseconds, peiodLabel);

            param.averageDelay = leapCtl.recorder_.meanDelaySec;


            AlertText.color = Color.blue;
            AlertText.text = "Put your hand back in a relaxed position";
            AlertPanel.GetComponent<Fader>().FadeIn();
            yield return new WaitForSeconds(expCtl.task_after_task_rest_secs);
            AlertPanel.GetComponent<Fader>().FadeOut();

            yield return new WaitForSeconds(1.0f);

            AlertPanel.GetComponent<Fader>().FadeIn();
            for(int countdown=5; countdown > 0; countdown--)
            {
                AlertText.text = "Stimulation is going to start in " + countdown + " seconds";
                yield return new WaitForSeconds(1.0f);
            }
            AlertPanel.GetComponent<Fader>().FadeOut();

            yield return new WaitForSeconds(0.5f);

            AlertText.color = Color.red;
            AlertText.text = "Keep your hand still";
            AlertPanel.GetComponent<Fader>().FadeIn();
            //
            // Stimulus
            //
            UnityEngine.Debug.Log("Stimulus Count:" + expCtl.numStimulus);

            //stimulator.SendStimulusPeriodStart((int)param.VRcondition);

            for (int stimulusId = 0; stimulusId < expCtl.numStimulus; stimulusId++)
            {
                AbstractExperimentDelayAndShock.ResultStimuli paramStimulus = new AbstractExperimentDelayAndShock.ResultStimuli();
                paramStimulus.index = stimulusId;

                int orderId = 0;
                if(stimulusId < (int) ((float)expCtl.numStimulus / 3.0f) )
                {
                    orderId = 0;
                }
                else if (stimulusId < (int)(2.0f * (float)expCtl.numStimulus / 3.0f))
                {
                    orderId = 1;
                }
                else
                {
                    orderId = 2;
                }
                int stimLabel = 0;

                float actual_stimulus_rest = expCtl.task_stimulus_rest_secs + UnityEngine.Random.Range(-expCtl.task_stimulus_jitter, expCtl.task_stimulus_jitter);

                yield return new WaitForSeconds(actual_stimulus_rest);

                paramStimulus.stimulusVisualTime = sw.ElapsedMilliseconds;
                switch (param.stimulusType) {
                    case AbstractExperimentDelayAndShock.StimulusType.VISUALTACTILE:
                        stimLabel = stimulator.StimulateVisualTactile((int)param.VRcondition, orderId);
                        break;
                    case AbstractExperimentDelayAndShock.StimulusType.VISUAL:
                        stimLabel = stimulator.StimulateVisual((int)param.VRcondition, orderId);
                        break;
                    case AbstractExperimentDelayAndShock.StimulusType.TACTILE:
                        stimLabel = stimulator.StimulateTactile((int)param.VRcondition, orderId);
                        break;
                }

                if(param.stimulusType == AbstractExperimentDelayAndShock.StimulusType.VISUAL || param.stimulusType == AbstractExperimentDelayAndShock.StimulusType.VISUALTACTILE )
                {
                    while (collisionCtl.IsActive || sw.ElapsedMilliseconds - paramStimulus.stimulusVisualTime < 2.0f)
                    {
                        yield return null;
                    }
                }

                if (collisionCtl.IsActive)
                {
                    paramStimulus.stimulusTriggerTime = -1;
                }
                else
                {
                    paramStimulus.stimulusTriggerTime = sw.ElapsedMilliseconds;
                }

                paramStimulus.trigerLabel = stimLabel;
                param.StimuliList.Add(paramStimulus);
                SaveDataToFile(param, paramStimulus);
            }
            

            param.numActualStimulus = param.StimuliList.Count;

            leapCtl.recorder_.StopDelay();
            UnityEngine.Debug.Log("Before Agency:");
            //
            // asking agency
            //
            yield return new WaitForSeconds(expCtl.task_answer_rest_secs);
            peiodLabel = stimulator.SendPeriodMarker(3, (int)param.VRcondition, (int)param.stimulusType, param.repeatId);
            SavePeriodMarker(param, sw.ElapsedMilliseconds, peiodLabel);

            {
                ResetSiderForAgency();
                isSliderEnabled = true;
                SliderPanel.GetComponent<Fader>().FadeIn();

                isSpacePressed = false;
                isSliderButtonPressed = false;

                yield return StartCoroutine("WaitForPowerMateDown");
                param.agency = sliderCtl.value;
                
                isSliderEnabled = false;
                SliderPanel.GetComponent<Fader>().FadeOut();
                
            }
            SaveDataToGlobalFile(param);

        }

        //Save Tracking Data at the end of session
       
        if (expCtl.recordTracking)
        {
            screenfader.FadeIn();
            AlertPanel.SetActive(true);
            AlertText.text = "Saving Data to a file\nWait for seconds...";

            yield return StartCoroutine(SaveTrackingFiles(FramesList, tracking_data_dir));
        }

        //finalizing 
        AlertPanel.GetComponent<Fader>().FadeOut();
        screenfader.FadeOut();

        expCtl.DeactivateCurrentMenu();

        MenuPanel.GetComponent<Fader>().FadeIn();
        expCtl.isBlockRunning = false;

        gameObject.GetComponent<Arduino>().Vibrate();

    }

}
