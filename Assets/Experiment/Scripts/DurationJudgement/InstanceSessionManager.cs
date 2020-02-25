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

namespace DurationJudgement
{
    public class InstanceSessionManager : AbstractSessionManager
    {

        public override IEnumerator CoScreenCaptureSession()
        {
            #region Instruction
            MenuPanel.GetComponent<Fader>().FadeOut();

            SelectRightButton();
            InstructionPanel.GetComponent<Fader>().FadeIn();

            yield return 0;
            yield return StartCoroutine("WaitInstructionButtonPressed");
            if (isInstructionBackButtonPressed || InstButtonSelector == 0)
            {
                MenuPanel.GetComponent<Fader>().FadeIn();
                InstructionPanel.GetComponent<Fader>().FadeOut();

                isInstructionBackButtonPressed = false;

                expCtl.isBlockRunning = false;
                yield break;
            }
            InstructionPanel.GetComponent<Fader>().FadeOut();
            #endregion

            int videoNum = expCtl.dataCtl.numTargetAction;

            #region Set Screen Capture Session

            GameObject AVProContainer = GameObject.Find("AVProContainer");
            GameObject Avatar = GameObject.Find("Avatar");
            CycleHandPairs HandCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();
            AVProContainer.SetActive(false);
            Avatar.SetActive(false);
            HandCtl.CurrentGroup = 6;
            #endregion


            #region Iterations
            for (int videoId = 0; videoId < videoNum; videoId++)
            {

                yield return new WaitForSeconds(expCtl.task_rest_secs);

                //Alert Screen
                AlertText.text = "Palm down and spread your fingers";
                AlertText.color = Color.red;
                AlertPanel.GetComponent<Fader>().FadeIn();
                yield return new WaitForSeconds(expCtl.task_message_secs);
                AlertPanel.GetComponent<Fader>().FadeOut();

                yield return new WaitForSeconds(expCtl.task_wait_secs);
                

                //********************
                // Target Video
                //********************
                videoCtl.PlayVideo(videoId);

                TargetPanel.GetComponent<Fader>().FadeIn();

                yield return new WaitForSeconds(expCtl.task_target_secs);

                TargetPanel.GetComponent<Fader>().FadeOut();


                yield return new WaitForSeconds(expCtl.task_wait_secs);

                //start capturing
                soundPlayer.Play();
                ScreenCaptureControl.StartCapture(videoId);

                yield return new WaitForSeconds(expCtl.task_action_secs);

                ScreenCaptureControl.StopCapture();
                soundPlayer.Play();
            }
            #endregion

            yield return new WaitForSeconds(2.0f);

            #region Get Back Normal
            AVProContainer.SetActive(true);
            Avatar.SetActive(true);
            HandCtl.CurrentGroup = 0;
            #endregion


            #region finalising



            screenfader.FadeOut();

            expCtl.DeactivateCurrentMenu();

            MenuPanel.GetComponent<Fader>().FadeIn();
            expCtl.isBlockRunning = false;
            #endregion

        }

        public override IEnumerator CoPracticeStart(List<DataManager.Result> ResultList)
        {
            #region Instruction
            MenuPanel.GetComponent<Fader>().FadeOut();

            SelectRightButton();
            InstructionPanel.GetComponent<Fader>().FadeIn();

            yield return 0;
            yield return StartCoroutine("WaitInstructionButtonPressed");
            if (isInstructionBackButtonPressed || InstButtonSelector == 0)
            {
                MenuPanel.GetComponent<Fader>().FadeIn();
                InstructionPanel.GetComponent<Fader>().FadeOut();

                isInstructionBackButtonPressed = false;

                expCtl.isBlockRunning = false;
                yield break;
            }
            InstructionPanel.GetComponent<Fader>().FadeOut();

            trackingCtl.SetDirForPractice(expCtl.SubId);
            #endregion

            HideAvatarHand();
            leapSelfCtl.RightHandEnabled = false;

            #region Iterations
            int max = ResultList.Count;
            for (int trialId = 0; trialId < max; trialId++)
            {
                DataManager.Result param = ResultList[trialId];

                // setting Leap Recorder based on perspectives
                if (param.perspectiveType == DataManager.PerspectiveType.SELF)
                {
                    localRedFrameCtl = RedFrameSelfCtl;
                    localLeapRecorder = leapSelfCtl.recorder_;

                    ShowOwnHand();
                    HideAvatarHand();
                }
                else
                {
                    localRedFrameCtl = RedFrameAvatarCtl;
                    localLeapRecorder = leapAvatarCtl.recorder_;

                    ShowAvatarHand();
                    HideOwnHand();
                }
                yield return new WaitForSeconds(expCtl.task_rest_secs);

                //Alert Screen
                AlertText.text = "Palm down and spread your fingers";
                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    AlertText.color = Color.blue;
                }
                else
                {
                    AlertText.color = Color.white;
                }

                AlertPanel.GetComponent<Fader>().FadeIn();
                yield return new WaitForSeconds(expCtl.task_message_secs);

                //Check Position
                yield return StartCoroutine(CheckHandPosition());


                AlertPanel.GetComponent<Fader>().FadeOut();

                yield return new WaitForSeconds(expCtl.task_wait_secs);


                //********************
                // Choose Target Image
                //********************
                videoCtl.PlayVideo(param.targetActionId);


                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    TargetTitleText.text = "Don't Move Your Hand";
                    TargetTitleText.color = Color.blue;
                }
                else
                {
                    TargetTitleText.text = "Target Action";
                    TargetTitleText.color = Color.white;
                }


                TargetPanel.GetComponent<Fader>().FadeIn();


                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    RedX.SetActive(true);
                }

                yield return new WaitForSeconds(expCtl.task_target_secs);


                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    RedX.SetActive(false);
                }


                TargetPanel.GetComponent<Fader>().FadeOut();


                yield return new WaitForSeconds(expCtl.task_wait_secs);


                #region Frame Location

                if (param.frameType == DataManager.FrameType.NEAR)
                {
                    localRedFrameCtl.MakeFrameNear();
                }
                else
                {
                    localRedFrameCtl.MakeFrameOver();
                }

                #endregion


                if (param.randomType == DataManager.RandomType.RANDOM)
                {
                    capsuleHand.StartRandomise();
                }


                //********************
                // Cross Onset
                //********************

                //Cross Onset
                AlertText.text = "Wait";
                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    AlertText.color = Color.blue;
                }
                else
                {
                    AlertText.color = Color.white;
                }
                AlertPanel.GetComponent<Fader>().FadeIn();

                localRedFrameCtl.ShowCross();
                yield return new WaitForSeconds(expCtl.task_cross_secs /2.0f);
                
                AlertPanel.GetComponent<Fader>().FadeOut();

                yield return new WaitForSeconds(expCtl.task_cross_secs / 2.0f);

                //Cross Offset
                if (!isShowingCrossInRedFrame)
                {
                    localRedFrameCtl.HideCross();
                }

                //********************
                // Action Onset
                //********************

                soundPlayer.Play();

                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    AlertText.text = "Don't move your hand";
                    AlertText.color = Color.blue;
                }
                else
                {
                    AlertText.text = "Mimic the hand movement";
                    AlertText.color = Color.white;
                }

                AlertPanel.GetComponent<Fader>().FadeIn();

                localRedFrameCtl.ShowFrame();

                //yield return new WaitForSeconds(0.5f);

                if (expCtl.recordTrackingPractice)
                {
                    localLeapRecorder.StartRecording();
                }
                StartCalcFps();

                yield return new WaitForSeconds(expCtl.task_action_secs);

                //Action Offset
                AlertPanel.GetComponent<Fader>().FadeOut();

                localRedFrameCtl.HideFrame();
                if (isShowingCrossInRedFrame)
                {
                    localRedFrameCtl.HideCross();
                }


                if (param.randomType == DataManager.RandomType.RANDOM)
                {
                    capsuleHand.StopRandomise();
                }

                //stop recording
                if (expCtl.recordTrackingPractice)
                {
                    localLeapRecorder.StopRecording();
                }
                UnityEngine.Debug.Log("Average FPS : " + GetFpsAndStop());


                if (expCtl.recordTrackingPractice)
                {
                    trackingCtl.SaveTrackingPractice(localLeapRecorder, param.perspectiveType, param.targetActionId, param.index);
                }

                HideAvatarHand();
                HideOwnHand();
                yield return new WaitForSeconds(0.25f);


            }
            #endregion

            #region finalising
            //finalizing


            leapSelfCtl.RightHandEnabled = true;

            screenfader.FadeOut();

            expCtl.DeactivateCurrentMenu();

            MenuPanel.GetComponent<Fader>().FadeIn();
            expCtl.isBlockRunning = false;
            #endregion
        }

        public override IEnumerator CoTrainingStart()
        {
            #region Instruction
            MenuPanel.GetComponent<Fader>().FadeOut();

            SelectRightButton();
            InstructionPanel.GetComponent<Fader>().FadeIn();

            yield return 0;
            yield return StartCoroutine("WaitInstructionButtonPressed");
            if (isInstructionBackButtonPressed || InstButtonSelector == 0)
            {
                MenuPanel.GetComponent<Fader>().FadeIn();
                InstructionPanel.GetComponent<Fader>().FadeOut();

                isInstructionBackButtonPressed = false;

                expCtl.isBlockRunning = false;
                yield break;
            }
            InstructionPanel.GetComponent<Fader>().FadeOut();
            #endregion

            #region Set Training

            HideAvatarHand();
            HideOwnHand();

            #endregion

            List<double> ActualDurations = new List<double>();
            List<double> ReportedDurations = new List<double>();


            #region Iterations
            int num_training = expCtl.dataCtl.numTraining;
            for (int i = 0; i < num_training; i++)
            {
                yield return new WaitForSeconds(expCtl.task_rest_secs);


                //Cross Onset
                RedFrameTrainingCtl.ShowCross();
                yield return new WaitForSeconds(expCtl.task_cross_secs);
                //Cross Offset
                RedFrameTrainingCtl.HideCross();

                //Action Onset
                soundPlayer.Play();
                RedFrameTrainingCtl.ShowFrame();
                float start = UnityEngine.Time.time;

                // randomise duration
                int duration_index = UnityEngine.Random.Range(0, expCtl.dataCtl.TrainingDurations.Length);
                float exposed_duration = expCtl.dataCtl.TrainingDurations[duration_index] / 1000f;
                float jitter = UnityEngine.Random.Range(-0.05f, 0.05f);

                //UnityEngine.Debug.Log(exposed_duration + " " + jitter);

                yield return new WaitForSeconds(exposed_duration + jitter);

                //Action Offset
                RedFrameTrainingCtl.HideFrame();
                int actual_duration = (int)((UnityEngine.Time.time - start) * 1000);

                #region Ask Long/Short
                yield return new WaitForSeconds(expCtl.task_wait_secs);

                {
                    InitSliderDuration();

                    isSliderEnabled = true;
                    SliderPanel.GetComponent<Fader>().FadeIn();

                    yield return StartCoroutine("WaitForPowerMateDown");
                          
                    SliderPanel.GetComponent<Fader>().FadeOut();
                }
                #endregion



                #region Feedback

                yield return new WaitForSeconds(expCtl.task_wait_secs);
                ShowFeedback((float)actual_duration);

                yield return new WaitForSeconds(1.0f);

                yield return StartCoroutine("WaitForPowerMateDown");
                HideFeedback();
                #endregion

                ActualDurations.Add((double)actual_duration);
                ReportedDurations.Add((double)sliderCtl.value);

            }
            #endregion

            #region Show Results

            double score = CalcStandardScore(ActualDurations.GetRange(5, ActualDurations.Count() - 5), ReportedDurations.GetRange(5, ActualDurations.Count() - 5));

            //double[] actuals = ActualDurations.GetRange(5, ActualDurations.Count() - 5).ToArray();
            //double[] reports = ReportedDurations.GetRange(5, ActualDurations.Count() - 5).ToArray();

            //double score = Correlation(actuals, reports) * 100f;

            AlertText.text = "Your score is " + score.ToString("00.0");

            if(score < 40)
            {
                AlertText.text += "\n You will need to try it again";
            }

            AlertText.text += "\n Press button to continue";

            AlertText.color = Color.red;
            AlertPanel.GetComponent<Fader>().FadeIn();
            yield return StartCoroutine("WaitForPowerMateDown");
            AlertPanel.GetComponent<Fader>().FadeOut();

            #endregion

            #region Save Training Data
            string out_name = SetTrainingFilename(expCtl.result_dir, "training", expCtl.SubId);
            SavTrainingToFile(out_name, ActualDurations, ReportedDurations, score);

            #endregion



            #region Get Back Normal

            ShowOwnHand();
            ShowAvatarHand();

            #endregion

            #region finalising
            //finalizing
            screenfader.FadeOut();

            if (score > 40)
            {
                expCtl.DeactivateCurrentMenu();
            }

            MenuPanel.GetComponent<Fader>().FadeIn();
            expCtl.isBlockRunning = false;
            #endregion

        }

        public override IEnumerator CoExperimentStart(List<DataManager.Result> ResultList, DurationJudgement.AbstractMenuManager.ExpSession SessionId, int blockId)
        {
            #region Instruction
            MenuPanel.GetComponent<Fader>().FadeOut();

            SelectRightButton();
            InstructionPanel.GetComponent<Fader>().FadeIn();

            yield return 0;
            yield return StartCoroutine("WaitInstructionButtonPressed");
            if (isInstructionBackButtonPressed || InstButtonSelector == 0)
            {
                MenuPanel.GetComponent<Fader>().FadeIn();
                InstructionPanel.GetComponent<Fader>().FadeOut();

                isInstructionBackButtonPressed = false;

                expCtl.isBlockRunning = false;
                yield break;
            }
            InstructionPanel.GetComponent<Fader>().FadeOut();
            #endregion

            #region Initialize

            // result lists
            UnityEngine.Debug.Log("SessionId:" + SessionId.ToString() + " BlockId:" + blockId + " Param Blockid:" + ResultList[0].blockId);

            // tracking record
            
            trackingCtl.SetDirForMain(expCtl.SubId, SessionId, blockId);


            sw.Reset(); sw.Start();
            #endregion

            leapSelfCtl.RightHandEnabled = false;

            #region Trial Iterations
            int max = ResultList.Count;
            //debug
            if (expCtl.debugMode) max = expCtl.debug_mode_num_trials;
            for (int trialId = 0; trialId < max; trialId++)
            {
                DataManager.Result param = ResultList[trialId];

                param.blockId = blockId;
                param.trialId = trialId;

                UnityEngine.Debug.Log(" Persperctive:" + param.perspectiveType + " Action:" + param.replayType + " Delay:" + param.delayId + " Random:" + param.randomType + " Frame:" + param.frameType);

                // setting Leap Recorder based on perspectives
                if (param.perspectiveType == DataManager.PerspectiveType.SELF)
                {
                    localRedFrameCtl = RedFrameSelfCtl;
                    localLeapRecorder = leapSelfCtl.recorder_;

                    if (expCtl.HandModel != 6)
                    {
                        ShowOwnHand();
                        HideAvatarHand();
                    }
                }
                else
                {
                    localRedFrameCtl = RedFrameAvatarCtl;
                    localLeapRecorder = leapAvatarCtl.recorder_;

                    ShowAvatarHand();
                    HideOwnHand();
                }

                yield return new WaitForSeconds(expCtl.task_rest_secs);

                //Alert Screen
                AlertText.text = "Palm down and spread your fingers";
                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    AlertText.color = Color.blue;
                }
                else
                {
                    AlertText.color = Color.white;
                }

                AlertPanel.GetComponent<Fader>().FadeIn();
                yield return new WaitForSeconds(expCtl.task_message_secs);

                //Check Position
                yield return StartCoroutine(CheckHandPosition());

                AlertPanel.GetComponent<Fader>().FadeOut();

                yield return new WaitForSeconds(expCtl.task_wait_secs);

                //********************
                // Choose Target Image
                //********************
                videoCtl.PlayImage(param.targetActionId);



                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    TargetTitleText.text = "Don't Move Your Hand";
                    TargetTitleText.color = Color.blue;
                }
                else
                {
                    TargetTitleText.text = "Target Action";
                    TargetTitleText.color = Color.white;
                }

                TargetPanel.GetComponent<Fader>().FadeIn();

                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    RedX.SetActive(true);
                }

                yield return new WaitForSeconds(expCtl.task_target_image_secs);

                TargetPanel.GetComponent<Fader>().FadeOut();

                if (param.actionType == DataManager.ActionyType.NO_ACTION)
                {
                    RedX.SetActive(false);
                }

                yield return new WaitForSeconds(expCtl.task_wait_secs);


                #region Replay Preparation
                if (param.replayType == DataManager.ReplayType.REPLAY)
                {
                    int index = UnityEngine.Random.Range(0, trackingCtl.num_samples);
                    trackingCtl.SetDefaultMovement(localLeapRecorder, (int)param.perspectiveType, param.targetActionId, index);
                }

                #endregion

                #region Frame Location

                if (param.frameType == DataManager.FrameType.NEAR)
                {
                    localRedFrameCtl.MakeFrameNear();
                }
                else
                {
                    localRedFrameCtl.MakeFrameOver();
                }

                #endregion

                #region Delay / Replay Control

                if (param.randomType == DataManager.RandomType.RANDOM)
                {
                    capsuleHand.StartRandomise();
                }


                localLeapRecorder.meanDelaySec = 0.0f;
                if (param.replayType == DataManager.ReplayType.LIVE)
                {
                    if (param.delayId != 0)
                    {
                        localLeapRecorder.SetDelayMillisec(expCtl.dataCtl.DelayArray[param.delayId]);
                        localLeapRecorder.StartDelay();
                    }
                }
                else
                {
                    long wait_time = 0;
                    if (param.delayId == 0)
                    {
                        wait_time = (long)(expCtl.task_cross_secs * 1000);
                    }
                    else
                    {
                        wait_time = expCtl.dataCtl.DelayArray[param.delayId] + (long)(expCtl.task_cross_secs * 1000);
                    }

                    if (expCtl.HandModel == 6)
                    {
                        localLeapRecorder.StartReplayOnLivePosCapsuleHandwithPause(wait_time);
                    }
                    else
                    {
                        localLeapRecorder.StartReplaywithPause(wait_time);
                    }
                }


                #endregion

                #region Show Cross
                //Cross Onset
                localRedFrameCtl.ShowCross();
                yield return new WaitForSeconds(expCtl.task_cross_secs);
                //Cross Offset

                if(!isShowingCrossInRedFrame)
                { 
                    localRedFrameCtl.HideCross();
                }


                #region Action Recording
                if (expCtl.recordTrackingMain)
                {
                    localLeapRecorder.StartRecording();
                }
                #endregion


                //Action Onset
                soundPlayer.Play();
                localRedFrameCtl.ShowFrame();

                float start = UnityEngine.Time.time;
                StartCalcFps();

                yield return new WaitForSeconds(expCtl.dataCtl.DurationArray[param.durationId] / 1000f); 

                //Action Offset
                localRedFrameCtl.HideFrame();
                if (isShowingCrossInRedFrame)
                {
                    localRedFrameCtl.HideCross();
                }

                param.actual_duration = (int)((UnityEngine.Time.time - start)*1000);
                param.averageFPS = GetFpsAndStop();

                param.averageDelay = localLeapRecorder.meanDelaySec;

                //stop delay
                if (param.replayType == DataManager.ReplayType.LIVE)
                {
                    if (param.delayId != 0)
                    {
                        localLeapRecorder.StopDelay();
                    }
                }
                else
                {
                    localLeapRecorder.StopReplay();
                }

                //stop recording
                if (expCtl.recordTrackingMain)
                {
                    localLeapRecorder.StopRecording();
                }

                if (param.randomType == DataManager.RandomType.RANDOM)
                {
                    capsuleHand.StopRandomise();
                }
                #endregion



                #region Ask Long/Short
                yield return new WaitForSeconds(expCtl.task_wait_secs);
                if (param.perspectiveType == DataManager.PerspectiveType.OTHER)
                {
                    HideAvatarHand();
                }
                {
                    InitSliderDuration();

                    isSliderEnabled = true;
                    SliderPanel.GetComponent<Fader>().FadeIn();

                    yield return StartCoroutine("WaitForPowerMateDown");
                    param.repoted_duration = (int)sliderCtl.value;

                    isSliderEnabled = false;
                    SliderPanel.GetComponent<Fader>().FadeOut();
                }
                #endregion


                if (!isAskingAgencyEndOfBlock || (isAskingAgencyEndOfBlock && (trialId == ResultList.Count - 1)))
                {
                    yield return new WaitForSeconds(expCtl.task_wait_secs);

                    #region Ask Control
                    yield return new WaitForSeconds(expCtl.task_wait_secs);

                    {
                        InitSliderForControl();

                        isSliderEnabled = true;
                        SliderPanel.GetComponent<Fader>().FadeIn();

                        yield return StartCoroutine("WaitForPowerMateDown");
                        param.control = sliderCtl.value - 4;

                        isSliderEnabled = false;
                        SliderPanel.GetComponent<Fader>().FadeOut();
                    }
                    #endregion

                    #region Ask Authorsip
                    yield return new WaitForSeconds(expCtl.task_wait_secs);

                    {
                        InitSliderForAuthorship();

                        isSliderEnabled = true;
                        SliderPanel.GetComponent<Fader>().FadeIn();

                        yield return StartCoroutine("WaitForPowerMateDown");
                        param.authorship = sliderCtl.value - 4;

                        isSliderEnabled = false;
                        SliderPanel.GetComponent<Fader>().FadeOut();
                    }
                    #endregion

                }

                //HideAvatarHand();
                //HideOwnHand();


                #region Save Data
                expCtl.dataCtl.SaveDataToFile(param);
                if (expCtl.recordTrackingMain)
                {
                    trackingCtl.SaveTrackingMain(localLeapRecorder, trialId, param.perspectiveType, param.replayType, param.actionType, param.targetActionId);
                }
                #endregion
                if (param.perspectiveType == DataManager.PerspectiveType.OTHER)
                {
                    ShowAvatarHand();
                }
            }
            #endregion

            /*
            #region Save Tracking Data
            if (expCtl.recordTracking)
            {
                screenfader.FadeIn();
                AlertPanel.SetActive(true);
                AlertText.text = "Saving Data to a file\nWait for seconds...";

                yield return StartCoroutine(SaveTrackingFiles(FramesList, tracking_data_dir));
            }
            #endregion
            */

            #region Finalizing

            //finalizing

            ShowOwnHand();
            ShowAvatarHand();
            leapSelfCtl.RightHandEnabled = true;

            AlertPanel.GetComponent<Fader>().FadeOut();
            screenfader.FadeOut();

            expCtl.DeactivateCurrentMenu();

            MenuPanel.GetComponent<Fader>().FadeIn();
            expCtl.isBlockRunning = false;

            #endregion
        }



        IEnumerator CheckHandPosition()
        {
            Vector3 hand = localRedFrameCtl.facingControl.TargetPos.transform.position;
            if (hand.x > 0.2 || hand.x < -0.2 || hand.y > 0 || hand.y < -0.3 || hand.z > 0.4 || hand.z < 0.2)
            {
                BoundingBox.GetComponent<MeshRenderer>().enabled = true;
                AlertText.text = "Locate your hand inside the blue box";
                while (hand.x > 0.2 || hand.x < -0.2 || hand.y > 0 || hand.y < -0.3 || hand.z > 0.4 || hand.z < 0.2)
                {
                    //UnityEngine.Debug.Log(" hand:" + hand.x + ", " + hand.y + ", " + hand.z);

                    yield return null;
                    hand = localRedFrameCtl.facingControl.TargetPos.transform.position;
                }
                BoundingBox.GetComponent<MeshRenderer>().enabled = false;
            }

            GameObject HeadObject = GameObject.Find("LeapHandController");
            Vector3 head = HeadObject.transform.localPosition;
            if (head.z > 0.05)
            {
                AlertText.text = "Move your head backward";
                while (head.z > 0.05)
                {
                    yield return null;
                    head = HeadObject.transform.localPosition;
                }
            }
            if (head.z < -0.05)
            {
                AlertText.text = "Move your head forward";
                while (head.z < -0.05)
                {
                    yield return null;
                    head = HeadObject.transform.localPosition;
                }
            }

        }

    }
}