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

public class AbstractDelayAndShockSessionManager : InteractiveBehaviour {

    protected System.Media.SoundPlayer player = null;

    protected AbstractExperimentDelayAndShock expCtl;

    protected CollisionDetector collisionCtl;

    protected ScreenFader screenfader;
    protected Leap.Unity.HandModelManager leapCtl;

    public static Stopwatch sw;

    [System.Serializable]
    public struct HandFrames
    {
        public List<Frame> Frames;
        public int trialId;
    }

    protected List<HandFrames> FramesList;

    protected StimulusTest stimulator;

    protected GameObject SliderPanel;
    protected GameObject InstructionPanel;
    protected GameObject MenuPanel;
    protected GameObject AlertPanel;

    protected Button InstructionButton;
    protected Button InstructionBackButton;

    protected Text InstructionText;
    protected Text AlertText;


    protected Text OvrAlertTextRight;
    protected Text OvrAlertTextLeft;

    protected Slider sliderCtl;
    protected Text sliderPanelMessage;
    protected Text sliderValue;

    protected List<AbstractExperimentDelayAndShock.Result> ResultList;

    public float initialRotation = 0f;

    protected bool isSpacePressed = false;
    protected bool isSliderEnabled = false;

    protected bool isSliderButtonPressed = false;
    protected bool isInstructionButtonPressed = false;
    protected bool isInstructionBackButtonPressed = false;

    protected string tracking_data_dir;

    protected int NumPadStatus = 0;
    protected int NumPadNumber = 0;

    protected int InstButtonSelector = 1;

    protected List<string> GuidedMovementsText = new List<string>();

    protected override void DoAwake()
    {
        sw = new Stopwatch();
        stimulator = GameObject.Find("TestStimulus").GetComponent<StimulusTest>();
        leapCtl = GameObject.Find("LeapHandController").GetComponent<Leap.Unity.HandModelManager>();
        screenfader = GameObject.Find("CanvasBlackScreen").GetComponent<ScreenFader>();

        collisionCtl = GameObject.Find("VisualStimulus").GetComponent<CollisionDetector>(); 

        expCtl = gameObject.GetComponent<AbstractExperimentDelayAndShock>();
        SliderPanel = GameObject.Find("SliderPanel");
        InstructionPanel = GameObject.Find("InstructionPanel");
        MenuPanel = GameObject.Find("MenuPanel");
        AlertPanel = GameObject.Find("AlertPanel");

        InstructionButton = GameObject.Find("InstructionButton").GetComponent<Button>(); 
        InstructionBackButton = GameObject.Find("InstructionBackButton").GetComponent<Button>(); 

        InstructionText = GameObject.Find("InstructionText").GetComponent<Text>();
        sliderPanelMessage = GameObject.Find("SliderPanelMessage").GetComponent<Text>();
        AlertText = GameObject.Find("AlertText").GetComponent<Text>();


        OvrAlertTextRight = GameObject.Find("OvrAlertTextRight").GetComponent<Text>();
        OvrAlertTextLeft = GameObject.Find("OvrAlertTextLeft").GetComponent<Text>();

        sliderValue = GameObject.Find("ValueLabelText").GetComponent<Text>();

        //instruction buttons
        EventTrigger.Entry entryInstruct = new EventTrigger.Entry();
        entryInstruct.eventID = EventTriggerType.PointerDown;
        entryInstruct.callback.AddListener((eventData) => { isInstructionButtonPressed = true; });
        GameObject.Find("InstructionButton").GetComponent<EventTrigger>().triggers.Add(entryInstruct);

        EventTrigger.Entry entryInstructBack = new EventTrigger.Entry();
        entryInstructBack.eventID = EventTriggerType.PointerDown;
        entryInstructBack.callback.AddListener((eventData) => { isInstructionBackButtonPressed = true; });
        GameObject.Find("InstructionBackButton").GetComponent<EventTrigger>().triggers.Add(entryInstructBack);


        EventTrigger.Entry entrySlider = new EventTrigger.Entry();
        entrySlider.eventID = EventTriggerType.PointerDown;
        entrySlider.callback.AddListener((eventData) => { isSliderButtonPressed = true; });
        GameObject.Find("SliderPanelButton").GetComponent<EventTrigger>().triggers.Add(entrySlider);


        sliderCtl = GameObject.Find("Slider").GetComponent<Slider>();

        //        sliderCtl.onValueChanged.AddListener(SliderDrug);

            GuidedMovementsText.Add("A single finger: up and down");
            GuidedMovementsText.Add("Two fingers: up and down");
            GuidedMovementsText.Add("Rolling fingers");
            GuidedMovementsText.Add("Open / Close the whole hand");
            GuidedMovementsText.Add("Waving fingers");



        //sounds
        string waveFile = Application.dataPath + "/../" + "440Hz-100ms.wav";
        player = new System.Media.SoundPlayer(waveFile);

    }
    // Use this for initialization
    protected override IEnumerator DoStart()
    {
        GameObject.Find("AVProContainer").transform.localEulerAngles = new Vector3(0f, initialRotation, 0f);

        InitSliderForAgency();

        SliderPanel.SetActive(false);
        InstructionPanel.SetActive(false);
        MenuPanel.SetActive(true);
        AlertPanel.SetActive(false);


        yield break;

    }
    public virtual IEnumerator CoBarehandStart(int blockId)
    {
        yield break;
    }
    public virtual IEnumerator CoPracticeStart()
    {
        yield break;
    }

    public virtual IEnumerator CoExperimentStart(AbstractExperimentDelayAndShock.ExpSession SessionId, int blockId)
    {
        yield break;

    }

    /*
    public IEnumerator CoRepetitiveStimulus()
    {
        yield return null;
    }
    */

    protected void SavePeriodMarker(AbstractExperimentDelayAndShock.Result param, float time, int peiodLabel)
    {
        AbstractExperimentDelayAndShock.ResultStimuli paramStimulus = new AbstractExperimentDelayAndShock.ResultStimuli();
        paramStimulus.index = -1;

        paramStimulus.stimulusVisualTime = time;
        paramStimulus.stimulusTriggerTime = time;
        paramStimulus.trigerLabel = peiodLabel;

        SaveDataToFile(param, paramStimulus);

    }


    protected IEnumerator ShowHandInstAndInduce(string inst, float duration)
    {
        
        AlertText.color = Color.blue;
        AlertText.text = inst;

        AlertPanel.GetComponent<Fader>().FadeIn();
//        UnityEngine.Debug.Log("hey ");
        yield return new WaitForSeconds(expCtl.task_message_secs);
//        UnityEngine.Debug.Log("hoe ");
        AlertPanel.GetComponent<Fader>().FadeOut();

        yield return new WaitForSeconds(duration - expCtl.task_message_secs);
    }
    protected IEnumerator ShowHandInstAndInduceOvr(string inst, float duration)
    {
        OvrAlertTextLeft.color = Color.blue;
        OvrAlertTextRight.color = Color.blue;

        OvrAlertTextLeft.text = inst;
        OvrAlertTextRight.text = inst;
        yield return new WaitForSeconds(expCtl.task_message_secs);

        OvrAlertTextLeft.text = "";
        OvrAlertTextRight.text = "";

        yield return new WaitForSeconds(duration - expCtl.task_message_secs);

    }

    protected void InitSliderForAgency()
    {

        sliderCtl.minValue = 0;
        sliderCtl.maxValue = 100;
        sliderCtl.value = sliderCtl.minValue;
        sliderPanelMessage.text = "\nHow much did you feel you owned the VR hand?";

    }

    protected void ResetSiderForAgency()
    {
        sliderCtl.value = sliderCtl.minValue;
    }


    protected IEnumerator SaveTrackingFiles(List<HandFrames> SaveFrameList, string dirname)
    {

        UnityEngine.Debug.Log("Save Tracking Files #" + SaveFrameList.Count);
        for (int i = 0; i < SaveFrameList.Count; i++)
        {
            string of = dirname + "track" + SaveFrameList[i].trialId.ToString() + ".txt";

            UnityEngine.Debug.Log(i + " " + of);
            leapCtl.recorder_.SaveFramesToFile(SaveFrameList[i].Frames, of);

            yield return null;
        }
        UnityEngine.Debug.Log(dirname);

        FramesList.Clear();
        //ZipFile.CreateFromDirectory(dir, dir + "/../Tracking.zip");

    }


    protected string getDirForTracking(int subId, int sessionId)
    {
        DateTime dt = DateTime.Now;
        string now = string.Format("-{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        string dir = data_dir + "Tracking/" + subId.ToString() + "/Session" + sessionId.ToString() + now + "/";
        Directory.CreateDirectory(dir);


        return dir;
    }
    protected string getFilenameForFrame(string dataname, int subId, int sessionId, int trial)
    {
        DateTime dt = DateTime.Now;
        string now = string.Format("-{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        string dir = data_dir + "/" + subId.ToString() + "/Tracking/Session" + sessionId.ToString() + now + "/";
        Directory.CreateDirectory(dir);

        out_filename = dir + dataname + trial.ToString() + ".txt";

        return out_filename;
    }


    public void SaveHeaderToGlobalFile()
    {
        
        string text = "#";
        text += "numBlocks:" + expCtl.num_blocks + ";";
        text += "numTrials:" + expCtl.numTrial + ";";
        text += "numStimuli:" + expCtl.numStimulus + ";";
        text += "Hand:" +  expCtl.HandModel  + ";";
        text += "delayShort:" + expCtl.delayFrameShort + ";";
        text += "delayLong:" + expCtl.delayFrameLong + ";";
        text += Environment.NewLine;
        text += "#sessionId, blockId, trialId, VRcondition, stimulusType, inductionStarttime, inductionEndtime, stimulusStarttime, stimulusEndtime, agency, numActulStimuli, averageDelay";
        text += Environment.NewLine;
        File.AppendAllText(out_global_filename, text);
    }


    protected string GetTextFromParamGlobal(AbstractExperimentDelayAndShock.Result p)
    {
        string text = "";
        text += p.sessionId + ", ";
        text += p.blockId + ", ";
        text += p.trialId + ", ";
        text += p.VRcondition + ", ";
        text += p.stimulusType + ", ";
        text += p.inductionStartTime + ", ";
        text += p.inductionEndTime + ", ";
        text += p.stimulusStartTime + ", ";
        text += p.stimulusEndTime + ", ";
        text += p.agency + ", ";
        text += p.numActualStimulus + ", ";
        text += p.averageDelay + "";
        text += Environment.NewLine;
        return text;
    }
    protected void SaveDataToGlobalFile(AbstractExperimentDelayAndShock.Result p)
    {
        File.AppendAllText(out_global_filename, GetTextFromParamGlobal(p));
    }

    protected void SaveHeaderToFile()
    {
        string text = "";
        text += "#sessionId, blockId, trialId, VRcondition, stimulusType, index, stimulusVisualTime, stimulusTriggerTime, trigerLabel";
        text += Environment.NewLine;
        File.AppendAllText(out_filename, text);
    }

    protected string GetTextFromParam(AbstractExperimentDelayAndShock.Result p, AbstractExperimentDelayAndShock.ResultStimuli s)
    {
        string text = "";
        text += p.sessionId + ", ";
        text += p.blockId + ", ";
        text += p.trialId + ", ";
        text += p.VRcondition + ", ";
        text += p.stimulusType + ", ";
        text += s.index + ", ";
        text += s.stimulusVisualTime + ", ";
        text += s.stimulusTriggerTime + ", ";
        text += s.trigerLabel + "";
        text += Environment.NewLine;
        return text;
    }
    protected void SaveDataToFile(AbstractExperimentDelayAndShock.Result p, AbstractExperimentDelayAndShock.ResultStimuli s)
    {
        //UnityEngine.Debug.Log("SaveDataToFile:" + out_filename);
        File.AppendAllText(out_filename, GetTextFromParam(p,s));
    }

    // Update is called once per frame
    protected override void DoUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            leapCtl.recorder_.StopRecording();
            HandFrames handframe = new HandFrames();
            handframe.Frames = new List<Frame>(leapCtl.recorder_.framesOBJ_);
            FramesList.Add(handframe);
            leapCtl.ResetRecording();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            leapCtl.recorder_.framesOBJ_ = FramesList[0].Frames;
            leapCtl.PlayRecording();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            leapCtl.recorder_.framesOBJ_ = FramesList[1].Frames;
            leapCtl.PlayRecording();
        }
        */


        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            screenfader.FadeIn();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            screenfader.FadeOut();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SetLeapPointerHide();
        }


        // Power Mate Control
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSpacePressed = true;
        }

        if (isSliderEnabled) { 
            float d = Input.GetAxis("Mouse ScrollWheel");
            sliderCtl.value += (int)(d * 10);
        }
    }
    protected void Swap<T>(List<T> list, int index1, int index2) {
        var a = list[index1];
        list[index1] = list[index2];
        list[index2] = a;
    }

    protected void Shuffle<T>(List<T> list) {
        var rnd = new System.Random();
        Enumerable.Range(1, list.Count).Reverse().ToList().ForEach(i => Swap(list, rnd.Next(i), i - 1));
    }

    protected void SliderDrug(float value)
    {
        sliderValue.text = value.ToString();

    }

    protected void SetLeapPointerHide()
    {
        //Leap.Unity.InputModule.LeapInputModule inputmodule = GameObject.Find("LeapEventSystem").GetComponent<Leap.Unity.InputModule.LeapInputModule>();

        // TODO: ksk
        /*
        for (int i = 0; i < inputmodule.pointerRenderers.Count; i++)
        {
            inputmodule.pointerRenderers[i].enabled = false;
        }
        */
    }
    protected void SetLeapPointerShow()
    {
        //Leap.Unity.InputModule.LeapInputModule inputmodule = GameObject.Find("LeapEventSystem").GetComponent<Leap.Unity.InputModule.LeapInputModule>();

        // TODO: ksk
        /*
        for (int i = 0; i < inputmodule.pointerRenderers.Count; i++)
        {
            inputmodule.pointerRenderers[i].enabled = true;
        }
        */
    }

    protected void SelectLeftButton()
    {
        ColorBlock cb1 = InstructionButton.colors;
        cb1.normalColor = expCtl.defaultColor;
        InstructionButton.colors = cb1;

        ColorBlock cb2 = InstructionBackButton.colors;
        cb2.normalColor = expCtl.selectedColor;
        InstructionBackButton.colors = cb2;

        InstButtonSelector = 0;
    }
    protected void SelectRightButton()
    {
        ColorBlock cb1 = InstructionButton.colors;
        cb1.normalColor = expCtl.selectedColor;
        InstructionButton.colors = cb1;

        ColorBlock cb2 = InstructionBackButton.colors;
        cb2.normalColor = expCtl.defaultColor;
        InstructionBackButton.colors = cb2;

        InstButtonSelector = 1;
    }

    public IEnumerator WaitInstructionButtonPressed()
    {

        isSpacePressed = false;
        UnityEngine.Debug.Log(" space bar " + isSpacePressed);
        while (!isInstructionButtonPressed && !isInstructionBackButtonPressed && !isSpacePressed)
        {

            float d = Input.GetAxis("Mouse ScrollWheel");

            if (d > 0.1)
            {
                SelectRightButton();
            }
            else if (d < -0.1)
            {
                SelectLeftButton();
            }

            yield return new WaitForEndOfFrame();
        }
       // UnityEngine.Debug.Log(isInstructionButtonPressed + " " + isInstructionBackButtonPressed);
        isInstructionButtonPressed = false;
    }


    protected IEnumerator WaitForPowerMateDown()
    {
        while (!isSpacePressed && !isSliderButtonPressed)
        {
            yield return new WaitForEndOfFrame();
        }
        isSpacePressed = false;
        isSliderButtonPressed = false;
        yield return 0;
    }

    public void EnableAR()
    {
        GameObject.Find("OvrAlertTextLeft").GetComponent<Text>().enabled = true;
        GameObject.Find("OvrAlertTextRight").GetComponent<Text>().enabled = true;
        GameObject.Find("BlackBackground").GetComponent<Renderer>().enabled = true;
        GameObject.Find("LeftImagePlane").GetComponent<Renderer>().enabled = true;
        GameObject.Find("RightImagePlane").GetComponent<Renderer>().enabled = true;


    }

    public void DisableAR()
    {
        GameObject.Find("OvrAlertTextLeft").GetComponent<Text>().enabled = false;
        GameObject.Find("OvrAlertTextRight").GetComponent<Text>().enabled = false;
        GameObject.Find("BlackBackground").GetComponent<Renderer>().enabled = false;
        GameObject.Find("LeftImagePlane").GetComponent<Renderer>().enabled = false;
        GameObject.Find("RightImagePlane").GetComponent<Renderer>().enabled = false;


    }



    protected void SetFilename(string dataname, int SubId, int block, string SessionName)
    {

        string dir = data_dir + "/Main/" + SubId + "/";
        Directory.CreateDirectory(dir);

        DateTime dt = DateTime.Now;
        out_filename = string.Format(dir + dataname + "_" + block + "_" + SessionName + "_{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
    }
}
