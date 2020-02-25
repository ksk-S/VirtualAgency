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

public class SessionManager : InteractiveBehaviour {

    //System.Media.SoundPlayer player = null;

    //Arduino vibrator;

    ExperimentAbstract expCtl;
    ButtonPress RedButtonManager;

    ScreenFader screenfader;
    Leap.Unity.HandModelManager leapCtl;

    LeapHandModifier leapModifer;

    public static Stopwatch sw;


    [System.Serializable]
    public struct HandFrames
    {
        public List<Frame> Frames;
        public int trialId;
    }
    List<HandFrames> FramesListForFake;
    List<HandFrames> FramesList;

    List<int> actionOnsetList;

    GameObject SliderPanel;
    GameObject InstructionPanel;
    GameObject MenuPanel;
    GameObject AlertPanel;

    GameObject AnswerTextContainer;
    Text AnswerText;
    GameObject AnswerHandle;


    GameObject UnitLabel;
    GameObject ValueLabel;
    GameObject Slider;

    Text SubmitButtonText;

    //Text InstructionText;
    Text AlertText;

    Text sliderValue;

    Slider slider;
    Text sliderPanelMessage;

    Text UnitText;

    GameObject sliderObject;
    GameObject binaryButtonObject;
    GameObject NumPad;

    GameObject skipButton;

    Text EnteredNumbers;

    GameObject NumPadFeebackPanel;
    Text NumPadActualDuration;

    GameObject reproduceButtonObject;

    //CanvasGroup SliderController;

    GameObject ButtonGadget;
    Vector3 ButtonGadgetDefaultPosition;

    List<ExperimentAbstract.Result> ResultList;

    MeshRenderer LeftImageRenderer;
    MeshRenderer RightImageRenderer;

    public float initialRotation = 200f;

    //bool isSliderEnabled = false;
    bool isInstructionButtonPressed = false;
    bool isInstructionBackButtonPressed = false;
    bool isSubmitButtonPressed = false;
    bool isNumPadNextButtonPressed = false;
    bool isSkipButtonPressed = false;

    bool isSyncButtonPressed = false;
    bool isAsyncButtonPressed = false;

    bool isReproductionRelease = false;
    long reproduced_interval = 0;

    public enum AnswerType
    {
        BINARY = 0, SLIDER, REPRODUCT, NUMPAD
    }
    public AnswerType answerType = AnswerType.NUMPAD;

    public bool KeyboardActive = true;


    public Vector3 RedButtonPosition;

    string tracking_data_dir;

    int NumPadStatus = 0;
    int NumPadNumber = 0;

    public bool AgencyOnly = false;

    public bool isHideHandModel = false;

    protected override void DoAwake()
    {
        sw = new Stopwatch();

        //vibrator = GetComponent<Arduino>();
        leapCtl = GameObject.Find("LeapHandController").GetComponent<Leap.Unity.HandModelManager>();
        leapModifer = GameObject.Find("LeapHandController").GetComponent<LeapHandModifier>();

        screenfader = GameObject.Find("CanvasBlackScreen").GetComponent<ScreenFader>();

        if(GameObject.Find("LeftImagePlane") != null) { 
        LeftImageRenderer = GameObject.Find("LeftImagePlane").GetComponent<MeshRenderer>();
        RightImageRenderer = GameObject.Find("RightImagePlane").GetComponent<MeshRenderer>();
        }

        ButtonGadget = GameObject.Find("ButtonGadget");
        ButtonGadgetDefaultPosition = ButtonGadget.transform.localPosition;

        expCtl = gameObject.GetComponent<ExperimentAbstract>();
        SliderPanel = GameObject.Find("SliderPanel");
        InstructionPanel = GameObject.Find("InstructionPanel");
        MenuPanel = GameObject.Find("MenuPanel");
        AlertPanel = GameObject.Find("AlertPanel");

        //InstructionText = GameObject.Find("InstructionText").GetComponent<Text>();
        sliderPanelMessage = GameObject.Find("SliderPanelMessage").GetComponent<Text>();
        SubmitButtonText = GameObject.Find("SubmitButtonText").GetComponent<Text>();
        AlertText = GameObject.Find("AlertText").GetComponent<Text>();
        UnitText = GameObject.Find("UnitText").GetComponent<Text>();
        // SliderController = GameObject.Find("SliderController").GetComponent<CanvasGroup>();

        skipButton = GameObject.Find("SkipButton");

        AnswerTextContainer = GameObject.Find("AnswerTextContainer");
        AnswerText = GameObject.Find("AnswerText").GetComponent<Text>(); ;
        AnswerHandle = GameObject.Find("AnswerHandle");

        RedButtonManager = GameObject.Find("Sensor").GetComponent<ButtonPress>();
        sliderValue = GameObject.Find("ValueLabelText").GetComponent<Text>();

        sliderObject = GameObject.Find("SliderController");
        binaryButtonObject = GameObject.Find("BinaryButtons");
        reproduceButtonObject = GameObject.Find("ReproductionButtonContainer");
        NumPad = GameObject.Find("NumPad");
        EnteredNumbers = GameObject.Find("EnteredNumbers").GetComponent<Text>();
        NumPadFeebackPanel = GameObject.Find("NumPadFeebackPanel");
        NumPadActualDuration = GameObject.Find("ActualDuration").GetComponent<Text>();


        UnitLabel = GameObject.Find("UnitLabel");
       ValueLabel = GameObject.Find("ValueLabel");
       Slider = GameObject.Find("Slider");

        //buttons
        EventTrigger.Entry entryInstruct = new EventTrigger.Entry();
        entryInstruct.eventID = EventTriggerType.PointerDown;
        entryInstruct.callback.AddListener((eventData) => { isInstructionButtonPressed = true; });
        GameObject.Find("InstructionButton").GetComponent<EventTrigger>().triggers.Add(entryInstruct);

        EventTrigger.Entry entryInstructBack = new EventTrigger.Entry();
        entryInstructBack.eventID = EventTriggerType.PointerDown;
        entryInstructBack.callback.AddListener((eventData) => { isInstructionBackButtonPressed = true; });
        GameObject.Find("InstructionBackButton").GetComponent<EventTrigger>().triggers.Add(entryInstructBack);

        if (answerType == AnswerType.BINARY)
        {
            EventTrigger.Entry entrySync = new EventTrigger.Entry();
            entrySync.eventID = EventTriggerType.PointerUp;
            entrySync.callback.AddListener((eventData) => { isSyncButtonPressed = true; });
            GameObject.Find("SyncButton").GetComponent<EventTrigger>().triggers.Add(entrySync);

            EventTrigger.Entry entryAsync = new EventTrigger.Entry();
            entryAsync.eventID = EventTriggerType.PointerUp;
            entryAsync.callback.AddListener((eventData) => { isAsyncButtonPressed = true; });
            GameObject.Find("AsyncButton").GetComponent<EventTrigger>().triggers.Add(entryAsync);

            NumPad.SetActive(false);
            reproduceButtonObject.SetActive(false);
        }
        else if (answerType == AnswerType.REPRODUCT)
        {
            EventTrigger.Entry entryReproduceStart = new EventTrigger.Entry();
            entryReproduceStart.eventID = EventTriggerType.PointerDown;
            entryReproduceStart.callback.AddListener((eventData) => { UnityEngine.Debug.Log("start"); sw.Reset(); sw.Start(); });
            GameObject.Find("ReproductionButton").GetComponent<EventTrigger>().triggers.Add(entryReproduceStart);

            EventTrigger.Entry entryReproduceStop = new EventTrigger.Entry();
            entryReproduceStop.eventID = EventTriggerType.PointerUp;
            entryReproduceStop.callback.AddListener((eventData) => { UnityEngine.Debug.Log("stop"); isReproductionRelease = true; reproduced_interval = sw.ElapsedMilliseconds; });
            GameObject.Find("ReproductionButton").GetComponent<EventTrigger>().triggers.Add(entryReproduceStop);

            NumPad.SetActive(false);
            binaryButtonObject.SetActive(false);
        }
        else if (answerType == AnswerType.NUMPAD)
        {
            for(int i=0; i<10; i++) { 
                EventTrigger.Entry entryNum = new EventTrigger.Entry();
                entryNum.eventID = EventTriggerType.PointerDown;
                int j = i;
                entryNum.callback.AddListener((eventData) => {  NumPressed(j); });
                GameObject.Find("Num" + i.ToString()).GetComponent<EventTrigger>().triggers.Add(entryNum);
            }
            EventTrigger.Entry entryBS = new EventTrigger.Entry();
            entryBS.eventID = EventTriggerType.PointerDown;
            entryBS.callback.AddListener((eventData) => { UnityEngine.Debug.Log("BS press"); NumPressed(-1); });
            GameObject.Find("BS").GetComponent<EventTrigger>().triggers.Add(entryBS);

            EventTrigger.Entry entryNext = new EventTrigger.Entry();
            entryNext.eventID = EventTriggerType.PointerUp;
            entryNext.callback.AddListener((eventData) => { isNumPadNextButtonPressed = true; });
            GameObject.Find("NumPadNextButton").GetComponent<EventTrigger>().triggers.Add(entryNext);

            EventTrigger.Entry entrySkip = new EventTrigger.Entry();
            entrySkip.eventID = EventTriggerType.PointerDown;
            entrySkip.callback.AddListener((eventData) => { isSkipButtonPressed = true; UnityEngine.Debug.Log("skip press"); });
            GameObject.Find("SkipButton").GetComponent<EventTrigger>().triggers.Add(entrySkip);


            reproduceButtonObject.SetActive(false);
            binaryButtonObject.SetActive(false);
        }

        EventTrigger.Entry entrySubmit = new EventTrigger.Entry();
        entrySubmit.eventID = EventTriggerType.PointerUp;
        entrySubmit.callback.AddListener((eventData) => { isSubmitButtonPressed = true; });
        GameObject.Find("SubmitButton").GetComponent<EventTrigger>().triggers.Add(entrySubmit);


        slider = GameObject.Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(SliderDrug);

        
        Slider sliderHeight = GameObject.Find("HeightSlider").GetComponent<Slider>();
        sliderHeight.onValueChanged.AddListener(SliderHeightDrug);
        //VRSettings.enabled = false;

        //sounds
       //string waveFile = Application.dataPath + "/../" + "440Hz-100ms.wav";
       // player = new System.Media.SoundPlayer(waveFile);

    }
    // Use this for initialization
    protected override IEnumerator DoStart()
    {
        GameObject.Find("AVProContainer").transform.localEulerAngles = new Vector3(0f, initialRotation, 0f);

        RedButtonPosition = GameObject.Find("RedButton").transform.localPosition;

        FramesListForFake = new List<HandFrames>();
        actionOnsetList = new List<int>();

        SliderPanel.SetActive(false);
        InstructionPanel.SetActive(false);
        MenuPanel.SetActive(true);
        AlertPanel.SetActive(false);

        yield break;

    }

    public IEnumerator CoTrainingStart(ExperimentAbstract.ExpSession SessionId)
    {
        yield break;
        /*

            CycleHandPairs LeapCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();
            if (SessionId == ExperimentAbstract.ExpSession.TRAINING)
            {
                SetFilename("training", expCtl.SubId, expCtl.day, SessionId, 0, LeapCtl.CurrentGroup);
            }
            else
            {
                SetFilename("tone", expCtl.SubId, expCtl.day, SessionId, 0, LeapCtl.CurrentGroup);
            }
            SaveHeaderToFile();

            UnityEngine.Debug.Log("sessionId" + " " + (int)SessionId);
            UnityEngine.Debug.Log(expCtl.ResultLists.Count());


            ResultList = expCtl.ResultLists[(int)SessionId][0];

            MenuPanel.GetComponent<Fader>().FadeOut();
            InstructionPanel.GetComponent<Fader>().FadeIn();

            yield return StartCoroutine("WaitInstructionButtonPressed");
            if (isInstructionBackButtonPressed)
            {
                MenuPanel.GetComponent<Fader>().FadeIn();
                InstructionPanel.GetComponent<Fader>().FadeOut();

                isInstructionBackButtonPressed = false;
                yield break;
            }

            RedButtonManager.DisableButton();

            InstructionPanel.GetComponent<Fader>().FadeOut();

            int max = ResultList.Count;
            //debug
            if (expCtl.debugMode) max = expCtl.debug_mode_num_trials;
            for (int i = 0; i < max; i++)
            {

                ExperimentAbstract.Result param = ResultList[i];

                AnswerTextContainer.SetActive(false);
                AnswerHandle.SetActive(false);

                screenfader.FadeIn();
                SetLeapPointerHide();

                yield return new WaitForSeconds(expCtl.training_rest_secs);

                player.Play();
                //SystemSounds.Beep.Play();
                UnityEngine.Debug.Log((float)param.actual_interval / 1000f);
                yield return new WaitForSeconds((float)param.actual_interval / 1000f);
                player.Play();
                //SystemSounds.Beep.Play();


                yield return new WaitForSeconds(expCtl.task_answer_rest_secs);

                //slider 
                if (answerType == AnswerType.BINARY)
                {
                    isSyncButtonPressed = false;
                    isAsyncButtonPressed = false;

                    sliderObject.SetActive(false);
                    binaryButtonObject.SetActive(true);
                    sliderPanelMessage.text = "Did a button press and a tone occured at the same time?";
                }
                else if (answerType == AnswerType.REPRODUCT)
                {
                    isReproductionRelease = false;

                    sliderObject.SetActive(false);
                    reproduceButtonObject.SetActive(true);
                    sliderPanelMessage.text = "Reproduce the interval between a button press and a tone";

                }
                else if (answerType == AnswerType.NUMPAD)
                {
                    NumPadStatus = 0;
                    NumPadNumber = 0;
                    EnteredNumbers.text = "";
                    sliderObject.SetActive(false);
                    NumPad.SetActive(true);
                }
                else {
                    SetSliderForTrainingEstimation();
                }

                screenfader.FadeOut();
                SetLeapPointerShow();
                SliderController.interactable = true;
                SliderPanel.GetComponent<Fader>().FadeIn();

                UnityEngine.Debug.Log(SessionId);

                if (SessionId == ExperimentAbstract.ExpSession.TRAINING)
                {
                    yield return StartCoroutine("WaitSubmitButtonPressed");

                    SliderController.interactable = false;

                    SubmitButtonText.text = "Next";
                    AnswerTextContainer.SetActive(true);
                    AnswerHandle.SetActive(true);
                    AnswerText.text = "The actual interval was " + param.actual_interval.ToString() + "ms";

                    //move the feedback handle
                    RectTransform t = GameObject.Find("Handle Slide Area").GetComponent<RectTransform>();
                    Vector3 position = AnswerHandle.transform.localPosition;
                    float xPos = Mathf.Lerp(t.rect.xMin, t.rect.xMax, (param.actual_interval / slider.maxValue));
                    position.x = xPos;
                    AnswerHandle.transform.localPosition = position;
                    //
                }

                yield return StartCoroutine("WaitSubmitButtonPressed");

                param.reported_interval = slider.value;
                SaveDataToFile(param);
                SliderPanel.GetComponent<Fader>().FadeOut();
            }

            RedButtonManager.EnableButton();
            MenuPanel.GetComponent<Fader>().FadeIn();

        */
    }

    public IEnumerator CoExperimentStart(ExperimentAbstract.ExpSession SessionId, int blockId)
    {
        FramesList = new List<HandFrames>();
        tracking_data_dir = getDirForTracking(expCtl.SubId, (int)SessionId);

        sliderPanelMessage.text = "Estimate the interval between a button press and a tone";
        AnswerTextContainer.SetActive(false);
        AnswerHandle.SetActive(false);

        UnityEngine.Debug.Log("SessionId:"+SessionId.ToString() + " BlockId:" + blockId);
        ResultList = expCtl.ResultLists[(int)SessionId][blockId];

        CycleHandPairs HandPairs = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();

        SetFilename("main", expCtl.SubId, expCtl.day, SessionId.ToString(), blockId, HandPairs.CurrentGroup);
        SaveHeaderToFile();
        
        //MenuPanel.SetActive(false);
        MenuPanel.GetComponent<Fader>().FadeOut();
        InstructionPanel.GetComponent<Fader>().FadeIn();

        yield return StartCoroutine("WaitInstructionButtonPressed");

        if (isInstructionBackButtonPressed)
        {
            MenuPanel.GetComponent<Fader>().FadeIn();
            InstructionPanel.GetComponent<Fader>().FadeOut();

            isInstructionBackButtonPressed = false;

            expCtl.isBlockRunning = false;
            yield break;
        }

        RedButtonManager.isButtonActiveAfterPress = false;
        if (isHideHandModel) { 
            leapModifer.SetHandTransparent();
        }
        InstructionPanel.GetComponent<Fader>().FadeOut();

        int max = ResultList.Count;
        UnityEngine.Debug.Log("Trial Count:" + max);
        //debug
        if (expCtl.debugMode) max = expCtl.debug_mode_num_trials;
        for (int i = 0; i < max; i++)
        {
            UnityEngine.Debug.Log("i:" + i);

            ExperimentAbstract.Result param = ResultList[i];
            param.agency = -1;
            param.fake_latency = -1;
            param.actionBeforeFake = -1;
            param.time_before_action = -1;
            param.actual_interval = -1;
            param.reported_interval = -1;
            param.trialId = i;

            int latency = UnityEngine.Random.Range(expCtl.PassiveLatencyMin, expCtl.PassiveLatencyMax);
            param.fake_latency = latency;

            screenfader.FadeIn();
            if(LeftImageRenderer != null)
            {
                LeftImageRenderer.enabled = false;
                RightImageRenderer.enabled = false;
            }

            SetLeapPointerHide();

            if (param.actionId == 0)
            {
                if (param.VRconditionId == 1)
                {
                    // UnityEngine.Debug.Log("with vr passive");
                    int index = UnityEngine.Random.Range(0, FramesListForFake.Count);

                    int search_latency_count = 0;
                    while ((actionOnsetList[index] < latency) && (search_latency_count < 5))
                    {
                        index = UnityEngine.Random.Range(0, FramesListForFake.Count);
                        UnityEngine.Debug.Log("search other actionOnsetList " + search_latency_count);
                        search_latency_count++;
                    }

                    int start_timing = (actionOnsetList[index] - latency);
                    if (start_timing < 0)
                    {
                        start_timing = 0;
                        param.fake_latency = actionOnsetList[index];
                    }


                    leapCtl.recorder_.SetRecordFrame(FramesListForFake[index].Frames);
                    

                    leapCtl.recorder_.StopReplay();

                    int tracking_index = 1;
                    int actualtime = (int)((float)(FramesListForFake[index].Frames[tracking_index].Timestamp - FramesListForFake[index].Frames[0].Timestamp) / 1000f);
                    while (actualtime < start_timing)
                    {
                        long h = FramesListForFake[index].Frames[tracking_index].Timestamp - FramesListForFake[index].Frames[0].Timestamp;
                        actualtime = (int)(h / 1000f);
                        tracking_index++;
                    }
                    UnityEngine.Debug.Log("start timing=" + start_timing);
                    UnityEngine.Debug.Log("actualtime=" + actualtime);
                    UnityEngine.Debug.Log("tracking_index=" + tracking_index);
                    UnityEngine.Debug.Log("latency=" + latency);
                    UnityEngine.Debug.Log("actionOnset=" + actionOnsetList[index]);

                    leapCtl.recorder_.SetIndex(tracking_index);

                }
            }

            RedButtonManager.isToneOnSet = false;
            RedButtonManager.automatic_button_success = false;

            if(SessionId == ExperimentAbstract.ExpSession.PRACTICE)
            {
                sw.Reset(); sw.Start();
                leapCtl.recorder_.StartRecording();

                yield return new WaitForSeconds(expCtl.task_rest_secs + 1.0f);
                screenfader.FadeOut();
                if (LeftImageRenderer != null)
                {
                    LeftImageRenderer.enabled = true;
                    RightImageRenderer.enabled = true;
                }
            }
            else
            {
                yield return new WaitForSeconds(expCtl.task_rest_secs);
                screenfader.FadeOut();
                if (LeftImageRenderer != null)
                {
                    LeftImageRenderer.enabled = true;
                    RightImageRenderer.enabled = true;
                }
                sw.Reset(); sw.Start();

                if (expCtl.recordTracking)
                {
                    leapCtl.recorder_.StartRecording();
                }

            }



            if (param.actionId == 0)
            {
                switch (param.VRconditionId)
                {
                    case 1:
                        leapCtl.recorder_.StartReplay();
                        RedButtonManager.EnableButton();
                        break;
                    case 0:
                        RedButtonManager.AutomaticButtonPress(latency);
                        RedButtonManager.DisableButton();
                        break;
                    case 2:
                        RedButtonManager.AutomaticTone(latency);
                        RedButtonManager.DisableButton();
                        break;
                    case 3:
                        RedButtonManager.AutomaticTactile(latency);
                        RedButtonManager.DisableButton();
                        break;
                }
            }
            else
            {
                RedButtonManager.EnableButton();
            }

            UnityEngine.Debug.Log(param.planned_interval);
            RedButtonManager.interval = param.planned_interval;

            yield return StartCoroutine("WaitUntilToneRing");

            if (param.actionId == 0)
            {
                if (param.VRconditionId == 1)
                {
                    int diff = latency - RedButtonManager.buttonPressedTime;
                    if (diff > 300) param.actionBeforeFake = 1;
                    else param.actionBeforeFake = 0;
                }
                else {
                    param.actionBeforeFake = RedButtonManager.automatic_button_success ? 0 : 1;
                }
            }

            yield return new WaitForSeconds(expCtl.task_answer_rest_secs);
            SetLeapPointerShow();

            //Recording Hand positions START
            if (SessionId == ExperimentAbstract.ExpSession.PRACTICE)
            {
                
                if (i > 1)
                {
                    leapCtl.recorder_.StopRecording();
                    UnityEngine.Debug.Log(RedButtonManager.buttonPressedTime);

                    if (RedButtonManager.buttonPressedTime > expCtl.PassiveLatencyMax)
                    {
                        HandFrames handframe = new HandFrames();
                        handframe.trialId = i;
                        handframe.Frames = new List<Frame>(leapCtl.recorder_.frameRecordList);
                        FramesListForFake.Add(handframe);
                        leapCtl.recorder_.ResetRecording();
                        
                        actionOnsetList.Add(RedButtonManager.buttonPressedTime);
                    }
                }
                else
                {
                    leapCtl.recorder_.StopRecording();
                    leapCtl.recorder_.ResetRecording();
                }
            }
            else
            {
                if (expCtl.recordTracking)
                {
                    leapCtl.recorder_.StopRecording();

                    HandFrames handframe = new HandFrames();
                    handframe.trialId = i;
                    handframe.Frames = new List<Frame>(leapCtl.recorder_.frameRecordList);
                    FramesList.Add(handframe);

                    leapCtl.recorder_.ResetRecording();
                }
            }
            //Recording Hand positions END

            if (param.actionId == 0)
            {
                leapCtl.recorder_.StopReplay();
            }

            // interval estimation
            if (!AgencyOnly)
            {

                ShowIntervalEstimation();

                SliderPanel.GetComponent<Fader>().FadeIn();

                float? answer = null;
                yield return StartCoroutine(WaitIntervalEstimation(r => answer = r));
                param.reported_interval = answer.Value;

                param.time_before_action = RedButtonManager.buttonPressedTime;
                param.actual_interval = (RedButtonManager.toneTime - RedButtonManager.buttonPressedTime);

                //  UnityEngine.Debug.Log(answer);


                // UnityEngine.Debug.Log("SESSION ID" + SessionId.ToString());
                if (SessionId == ExperimentAbstract.ExpSession.TRAINING)
                {
                    yield return StartCoroutine(ShowFeedback(param));
                }


            }
            // asking agency, not using now
            
            if (param.askAgency == 1 && !isSkipButtonPressed && SessionId != ExperimentAbstract.ExpSession.PRACTICE)
            {
                UnitLabel.SetActive(true);
                ValueLabel.SetActive(true);
                Slider.SetActive(true);

                SliderPanel.GetComponent<Fader>().FadeOut();
                yield return new WaitForSeconds(SliderPanel.GetComponent<Fader>().duration);

                NumPad.SetActive(false);
                SetSiderForAgency(param.question);

                SliderPanel.GetComponent<Fader>().FadeIn();
                
                yield return StartCoroutine("WaitSubmitButtonPressed");
                param.agency = slider.value;

            }
            isSkipButtonPressed = false;


            SliderPanel.GetComponent<Fader>().FadeOut();
            SaveDataToFile(param);

            //take asbreak
            if ((i != 0) && ((i + 1) % expCtl.brakAfterTrials == 0) && (max- i) > 10)
            {
                //SaveTrackingFilesSync(FramesList, tracking_data_dir);

                if (expCtl.recordTracking)
                {
                    StartCoroutine(SaveTrackingFiles(FramesList, tracking_data_dir));
                }

                AlertPanel.GetComponent<Fader>().FadeIn();

                for (int j = expCtl.breakSeconds; j > 0; j--)
                {
                    AlertText.text = "Take a break\n The next trial will start in " + j + " secs";
                    yield return new WaitForSeconds(1.0f);
                }

                AlertPanel.GetComponent<Fader>().FadeOut();
            }

            if (SessionId == ExperimentAbstract.ExpSession.PRACTICE)
            {
                if(FramesListForFake.Count >= expCtl.numFakeRecording)
                {
                    break;
                }
            }

        }

        //Save Tracking Data at the end of session
        screenfader.FadeIn();
        if (LeftImageRenderer != null)
        {
            LeftImageRenderer.enabled = false;
            RightImageRenderer.enabled = false;
        }

        AlertPanel.SetActive(true);
        AlertText.text = "Saving Data to a file\nWait for seconds...";


        if (expCtl.recordTracking)
        {
            if (SessionId == ExperimentAbstract.ExpSession.PRACTICE)
            {
                yield return StartCoroutine(SaveTrackingFiles(FramesListForFake, tracking_data_dir));
            }
            else
            {
                yield return StartCoroutine(SaveTrackingFiles(FramesList, tracking_data_dir));

            }
        }
        //finalizing 
        AlertPanel.GetComponent<Fader>().FadeOut();
        screenfader.FadeOut();
        if (LeftImageRenderer != null)
        {
            LeftImageRenderer.enabled = true;
            RightImageRenderer.enabled = true;
        }

        RedButtonManager.isButtonActiveAfterPress = true;
        expCtl.DeactivateCurrentMenu();
        if (isHideHandModel)
        {
            leapModifer.UnsetHandTransparent();
        }
        MenuPanel.GetComponent<Fader>().FadeIn();
        if (SessionId == ExperimentAbstract.ExpSession.PRACTICE)
        {
            GameObject.Find("HeightSlider").SetActive(false);
        }
        expCtl.isBlockRunning = false;
    }

    void ShowIntervalEstimation()
    {
        switch (answerType)
        {
            case AnswerType.BINARY:
                isSyncButtonPressed = false;
                isAsyncButtonPressed = false;

                sliderObject.SetActive(false);
                binaryButtonObject.SetActive(true);
                sliderPanelMessage.text = "Did a button press and a tone occured at the same time?";
                break;
            case AnswerType.SLIDER:
                sliderObject.SetActive(true);
                binaryButtonObject.SetActive(false);

                UnitText.text = "ms";
                slider.minValue = 0;
                slider.maxValue = 1200;
                slider.value = slider.minValue;
                sliderValue.text = slider.value.ToString();
                sliderPanelMessage.text = "Estimate the interval between a button press and a tone";
                break;
            case AnswerType.REPRODUCT:
                isReproductionRelease = false;
                sliderObject.SetActive(false);
                reproduceButtonObject.SetActive(true);
                sliderPanelMessage.text = "Reproduce the interval between a button press and a tone";
                break;
            case AnswerType.NUMPAD:

                EnteredNumbers.text = "";
                NumPadStatus = 0;
                NumPadNumber = 0;
                NumPadFeebackPanel.SetActive(false);
                sliderObject.SetActive(false);
                NumPad.SetActive(true);
                sliderPanelMessage.text = "Estimate the interval between a button press and a tone";
                break;
        }
    }

    IEnumerator WaitIntervalEstimation(Action<float> callback)
    {
        float answer = 0;
        switch (answerType)
        {
            case AnswerType.BINARY:
                yield return StartCoroutine("WaitSyncAsyncButtonPressed");
                if (isAsyncButtonPressed) answer = 1;
                else if (isSyncButtonPressed) answer = 0;

                isSyncButtonPressed = false;
                isAsyncButtonPressed = false;
                break;

            case AnswerType.SLIDER:
                yield return StartCoroutine("WaitSubmitButtonPressed");
                answer = slider.value;
                break;

            case AnswerType.REPRODUCT:

                yield return StartCoroutine("WaitReproductionRelease");
                //                yield return StartCoroutine("WaitSubmitButtonPressed");
                answer = reproduced_interval;
                UnityEngine.Debug.Log(" interval " + reproduced_interval);

                break;
            case AnswerType.NUMPAD:
                yield return StartCoroutine("WaitNumPadFinished");
                if (isSkipButtonPressed) { answer = -1; }
                else { answer = NumPadNumber; }

                break;
        }
        callback(answer);
    }

    IEnumerator ShowFeedback(ExperimentAbstract.Result param)
    {
        SliderPanel.GetComponent<Fader>().FadeOut();
        yield return new WaitForSeconds(SliderPanel.GetComponent<Fader>().duration);
        SliderPanel.GetComponent<Fader>().FadeIn();

        sliderPanelMessage.text = "Feedback";
        //only slider button is used
        UnitLabel.SetActive(false);
        ValueLabel.SetActive(false);
        Slider.SetActive(false);

        switch (answerType)
        {
            case AnswerType.BINARY:
                sliderObject.SetActive(true);
                binaryButtonObject.SetActive(false);

                AnswerTextContainer.SetActive(true);
                AnswerText.text = "The tone was ";
                UnityEngine.Debug.Log(param.intervalId);
                if (param.intervalId == 0)
                {
                    AnswerText.text += "on time";
                }
                else
                {
                    AnswerText.text += "delayed";
                }

                if ((param.reported_interval == 1 && param.intervalId == 0) || (param.reported_interval == 0 && param.intervalId != 0))
                {
                    AnswerText.color = Color.red;
                }
                else
                {
                    AnswerText.color = Color.white;
                }

                SubmitButtonText.text = "Next";

                yield return StartCoroutine("WaitSubmitButtonPressed");
                break;

            case AnswerType.SLIDER:
                break;
            case AnswerType.REPRODUCT:
                sliderObject.SetActive(true);
                reproduceButtonObject.SetActive(false);

                AnswerTextContainer.SetActive(true);
                AnswerText.text = "Actual interval was ";
                AnswerText.text += param.actual_interval + " ms";

                AnswerText.text += "\n";
                AnswerText.text += "Your reproduction was ";
                AnswerText.text += param.reported_interval + " ms";

                SubmitButtonText.text = "Next";

                yield return StartCoroutine("WaitSubmitButtonPressed");
                break;
            case AnswerType.NUMPAD:
                skipButton.SetActive(false);
                NumPadFeebackPanel.SetActive(true);
                NumPadActualDuration.text = param.actual_interval.ToString();

                yield return StartCoroutine("WaitNumPadNextButtonPressed");
                skipButton.SetActive(true);
                break;
        }
        SliderPanel.GetComponent<Fader>().FadeOut();
    }

    void SetSiderForAgency(int question)
    {
        sliderObject.SetActive(true);
        binaryButtonObject.SetActive(false);

        UnitText.text = "";
        slider.minValue = 1;
        slider.maxValue = 5;
        slider.value = 3;

        if (question == 0)
        {
            sliderPanelMessage.text = "\nHow much did you feel your action caused the tone to occur?";
            sliderPanelMessage.text += "\n 1: Not at all  5: Definitely";
        }
        else
        {
            sliderPanelMessage.text = "\nTo what degree did the moving hand cause the tone to occur?";
            sliderPanelMessage.text += "\n 1: Not at all  5: Definitely";

        }
        
    }

    void NumPressed(int num)
    {
        if(num == -1)
        {
            switch (NumPadStatus)
            {
                case 0:
                    break;
                case 1:
                    NumPadNumber = 0;
                    EnteredNumbers.text = "";
                    NumPadStatus--;
                    break;
                case 2:
                    NumPadNumber = NumPadNumber / 10;
                    EnteredNumbers.text = NumPadNumber.ToString("0");
                    NumPadStatus--;
                    break;
            }
        }
        else {
            switch (NumPadStatus)
            {
                case 0:
                    NumPadNumber = num;
                    EnteredNumbers.text = NumPadNumber.ToString("0");
                    NumPadStatus++;
                    break;
                case 1:
                    NumPadNumber = NumPadNumber * 10 + num;
                    EnteredNumbers.text = NumPadNumber.ToString("00");
                    NumPadStatus++;
                    break;
                case 2:
                    NumPadNumber = NumPadNumber * 10 + num;
                    EnteredNumbers.text = NumPadNumber.ToString("000");
                    NumPadStatus++;
                    break;
            }
        }
    }


    void SaveTrackingFilesSync(List<HandFrames> SaveFrameList, string dirname)
    {

        for (int i = 0; i < SaveFrameList.Count; i++)
        {
            string out_filename = dirname + "track" + SaveFrameList[i].trialId.ToString() + ".txt";
            leapCtl.recorder_.SaveFramesToFile(SaveFrameList[i].Frames, out_filename);
        }
        UnityEngine.Debug.Log(dirname);

        FramesList.Clear();

        //ZipFile.CreateFromDirectory(dir, dir + "/../Tracking.zip");
    }

    IEnumerator SaveTrackingFiles(List<HandFrames> SaveFrameList, string dirname)
    {

        UnityEngine.Debug.Log("Save Tracking Files #" + SaveFrameList.Count);
        for (int i = 0; i < SaveFrameList.Count; i++)
        {
            string out_filename = dirname + "track" + SaveFrameList[i].trialId.ToString() + ".txt";

            UnityEngine.Debug.Log(i + " " + out_filename);
            leapCtl.recorder_.SaveFramesToFile(SaveFrameList[i].Frames, out_filename);

            yield return null;
        }
        UnityEngine.Debug.Log(dirname);

        FramesList.Clear();
        //ZipFile.CreateFromDirectory(dir, dir + "/../Tracking.zip");

    }


    string getDirForTracking(int subId, int sessionId)
    {
        DateTime dt = DateTime.Now;
        string now = string.Format("-{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        string dir = data_dir + "Tracking/" + subId.ToString() + "/Session" + sessionId.ToString() + now + "/";
        Directory.CreateDirectory(dir);


        return dir;
    }
    string getFilenameForFrame(string dataname, int subId, int sessionId, int trial)
    {
        DateTime dt = DateTime.Now;
        string now = string.Format("-{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        string dir = data_dir + "/" + subId.ToString() + "/Tracking/Session" + sessionId.ToString() + now + "/";
        Directory.CreateDirectory(dir);

        out_filename = dir + dataname + trial.ToString() + ".txt";

        return out_filename;
    }

    void SaveHeaderToFile()
    {
        string text = "#";

        text += "intervals:";
        for (int i = 0; i < expCtl.intervals.Length; i++) {
            text += expCtl.intervals[i] + ",";
        }
        text += ";";
        text += "numPractice:" + (expCtl.numPractice * expCtl.intervals.Length) + ";";
        text += "numTraining:" + (expCtl.numTraining * expCtl.intervals.Length) + ";";
        if (expCtl.GetType().FullName == "ExperimentAgencyMagnitude")
        {
            text += "numActive:" + (expCtl.numFakeActive * expCtl.intervals.Length) + ";";
            text += "numPassive:" + (expCtl.numFakePassive * expCtl.intervals.Length) + ";";
        }
        else if (expCtl.GetType().FullName == "ExperimentAgencyFakeBinary")
        {
            text += "numActive:" + (expCtl.numActive * expCtl.intervals.Length) + ";";
            text += "numPassive:" + (expCtl.numPassive * expCtl.intervals.Length) + ";";

        }else if (expCtl.GetType().FullName == "ExperimentAgencyFakeReproductionBlocked" || expCtl.GetType().FullName == "ExperimentAgencyFakeBlocked")
        {
            text += "numActive:" + (expCtl.numActive * expCtl.intervals.Length) + ";";
            text += "numPassive:" + (expCtl.numPassive * expCtl.intervals.Length) + ";";
            text += "numPassiveHand:" + (expCtl.numPassiveHand * expCtl.intervals.Length) + ";";
        }
        text += "Hand:" +  expCtl.HandModel  + ";";
        text += "Robot:" + (expCtl.isRobot ? "Yes" : "No") + ";";
        text += Environment.NewLine;

        text += "#trialId, sessionId, blockId, VRconditionId, actionId, intervalId, time_before_action, planned_interval, actual_interval, reported_interval, agency, actionBeforeFake, fake_latency, question";

        text += Environment.NewLine;
        File.AppendAllText(out_filename, text);
    }
    void SaveTrainingHeaderToFile()
    {
        string text = "#";

        text += "intervals:";
        for (int i = 0; i < expCtl.intervals.Length; i++)
        {
            text += expCtl.intervals[i] + ",";
        }
        text += ";";
        text += "numPractice:" + (expCtl.numPractice * expCtl.intervals.Length) + ";";
        text += "numTraining:" + (expCtl.numTraining * expCtl.intervals.Length) + ";";
        text += "numActive:" + (expCtl.numFakeActive * expCtl.intervals.Length) + ";";
        text += "numPassive:" + (expCtl.numFakePassive * expCtl.intervals.Length) + ";";
        text += "binaryChoice:" + ( answerType.ToString() );
        text += Environment.NewLine;

        text += "#sessionId, actual_interval, reported_interval";

        text += Environment.NewLine;
        File.AppendAllText(out_filename, text);

    }


    void SaveDataToFile(ExperimentAbstract.Result p)
    {

        File.AppendAllText(out_filename, GetTextFromParam(p));
    }

    string GetTextFromParam(ExperimentAbstract.Result p)
    {
        string text = "";
        text += p.trialId + ", ";
        text += p.sessionId + ", ";
        text += p.blockId + ", ";
        text += p.VRconditionId + ", ";
        text += p.actionId + ", ";
        text += p.intervalId + ", ";
        text += p.time_before_action + ", ";
        text += p.planned_interval + ", ";
        text += p.actual_interval + ", ";
        text += p.reported_interval + ", ";
        text += p.agency + ", ";
        text += p.actionBeforeFake + ", ";
        text += p.fake_latency + ", ";
        text += p.question + "";
        text += Environment.NewLine;
        return text;
    }

    // Update is called once per frame
    protected override void DoUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            leapCtl.recorder_.StopRecording();
            HandFrames handframe = new HandFrames();
            handframe.Frames = new List<Frame>(leapCtl.recorder_.frameRecordList);
            FramesList.Add(handframe);
            leapCtl.recorder_.ResetRecording();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            leapCtl.recorder_.frameRecordList = FramesList[0].Frames;
            leapCtl.recorder_.StartReplay();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

            leapCtl.recorder_.frameRecordList = FramesList[1].Frames;
            leapCtl.recorder_.StartReplay();

        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RedButtonManager.AutomaticButtonPress(1000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            screenfader.FadeIn();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            screenfader.FadeOut();
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        if (Input.GetKeyDown(KeyCode.G))
        {
            ResetRedButton();
        }

        if (KeyboardActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RedButtonManager.KeyboardPress();
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SetLeapPointerHide();
        }


    }
    void ResetRedButton()
    {

        GameObject.Find("RedButton").transform.localPosition = RedButtonPosition;

    }

    void Swap<T>(List<T> list, int index1, int index2) {
        var a = list[index1];
        list[index1] = list[index2];
        list[index2] = a;
    }

    void Shuffle<T>(List<T> list) {
        var rnd = new System.Random();
        Enumerable.Range(1, list.Count).Reverse().ToList().ForEach(i => Swap(list, rnd.Next(i), i - 1));
    }

    void SliderDrug(float value)
    {
        sliderValue.text = value.ToString();

    }

    void SliderHeightDrug(float value)
    {
        Vector3 pos = new Vector3(ButtonGadgetDefaultPosition.x, ButtonGadgetDefaultPosition.y + value, ButtonGadgetDefaultPosition.z);
        ButtonGadget.transform.localPosition = pos;

    }

    void SetLeapPointerHide()
    {
        Leap.Unity.InputModule.LeapInputModule inputmodule = GameObject.Find("LeapEventSystem").GetComponent<Leap.Unity.InputModule.LeapInputModule>();
        inputmodule.enabled = false;

        //TODO: ksk
        /*
        for (int i = 0; i < inputmodule.pointerRenderers.Count; i++)
        {
            inputmodule.pointerRenderers[i].enabled = false;
        }
        */

    }
    void SetLeapPointerShow()
    {
        Leap.Unity.InputModule.LeapInputModule inputmodule = GameObject.Find("LeapEventSystem").GetComponent<Leap.Unity.InputModule.LeapInputModule>();
        inputmodule.enabled = true;

        //TODO: ksk
        /*
        for (int i = 0; i < inputmodule.pointerRenderers.Count; i++)
        {
            inputmodule.pointerRenderers[i].enabled = true;
        }\
        */
    }

    public IEnumerator WaitSubmitButtonPressed()
    {
        while (!isSubmitButtonPressed)
        {
            //UnityEngine.Debug.Log("wait submit pressed");
            yield return new WaitForEndOfFrame();
        }
        isSubmitButtonPressed = false;
    }


    public IEnumerator WaitNumPadNextButtonPressed()
    {
        while (!isNumPadNextButtonPressed)
        {
            //UnityEngine.Debug.Log("wait submit pressed");
            yield return new WaitForEndOfFrame();
        }
        isNumPadNextButtonPressed = false;
    }


    public IEnumerator WaitReproductionRelease()
    {
        while (!isReproductionRelease)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator WaitSyncAsyncButtonPressed()
    {
        while (!isSyncButtonPressed && !isAsyncButtonPressed)
        {
            //UnityEngine.Debug.Log("wait submit pressed");
            yield return new WaitForEndOfFrame();
        }

    }
    public IEnumerator WaitNumPadFinished()
    {
        while(NumPadStatus != 3 && !isSkipButtonPressed) { 
            yield return null;
        }
    }



    public IEnumerator WaitInstructionButtonPressed()
    {
        while (!isInstructionButtonPressed && !isInstructionBackButtonPressed)
        {
            yield return new WaitForEndOfFrame();
        }
       // UnityEngine.Debug.Log(isInstructionButtonPressed + " " + isInstructionBackButtonPressed);
        isInstructionButtonPressed = false;
    }

    public IEnumerator WaitUntilToneRing()
    {
        while(!RedButtonManager.isToneOnSet)
        {
            yield return new WaitForEndOfFrame();
        }
        RedButtonManager.isToneOnSet = false;
    }
}
