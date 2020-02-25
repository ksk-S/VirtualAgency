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

    public class AbstractSessionManager : InteractiveBehaviour
    {

        protected DurationJudgement.AbstractMenuManager expCtl;
        protected Leap.Unity.HandModelManager leapSelfCtl;
        protected LeapAvatarHands.IKOrionLeapHandController leapAvatarCtl;

        protected RedFrameControl localRedFrameCtl;
        protected Leap.Unity.LeapRecorder localLeapRecorder;

        protected Leap.Unity.LeapRecorder unusedlocalLeapRecorder;

        protected TrackingManager trackingCtl = new TrackingManager();

        public RedFrameControl RedFrameSelfCtl;
        public RedFrameControl RedFrameAvatarCtl;
        public RedFrameControl RedFrameTrainingCtl;

        protected Leap.Unity.CapsuleHand capsuleHand;

        protected TargetMovieControl videoCtl;

        public static Stopwatch sw;

        protected System.Media.SoundPlayer soundPlayer = null;
        protected ScreenFader screenfader;

        protected GameObject SliderPanel;
        protected GameObject InstructionPanel;
        protected GameObject MenuPanel;
        protected GameObject AlertPanel;
        protected GameObject TargetPanel;

        protected GameObject BoundingBox;


        protected Button InstructionButton;
        protected Button InstructionBackButton;

        protected Text InstructionText;
        protected Text AlertText;

        protected Slider sliderCtl;
        protected GameObject FeedbackHandle;

        protected Text sliderPanelMessage;
        protected Text sliderValue;
        protected Text ValueLabelLeft;
        protected Text ValueLabelRight;
        protected Text ValueLabelCentre;
        protected Text ValueLabelUnit;

        protected GameObject RedX;
        protected Text TargetTitleText;
        

        protected List<DataManager.Result> ResultList;

        public float initialRotation = 0f;

        protected bool isSpacePressed = false;
        protected bool isSliderEnabled = false;

        protected bool isSliderButtonPressed = false;
        protected bool isInstructionButtonPressed = false;
        protected bool isInstructionBackButtonPressed = false;

        protected int InstButtonSelector = 1;

        protected bool isFPSrun = false;
        protected float fps = 0.0f;
        protected int frameCount = 0;
        protected float dt = 0.0f;

        public bool isShowingCrossInRedFrame = false;

        private float last_time_stick = 0f;
        public float stick_interval = 0.1f;
        public float stick_sensitivity = 0.05f;

        public bool isIncremental = false;

        public bool isAskingAgencyEndOfBlock = false;

        protected override void DoAwake()
        {
            sw = new Stopwatch();
            leapSelfCtl = GameObject.Find("LeapHandController").GetComponent<Leap.Unity.HandModelManager>();
            leapAvatarCtl = GameObject.Find("Mixamo Rigged VR Mode").GetComponent<LeapAvatarHands.IKOrionLeapHandController>();
            screenfader = GameObject.Find("CanvasBlackScreen").GetComponent<ScreenFader>();

            expCtl = gameObject.GetComponent<AbstractMenuManager>();
            SliderPanel = GameObject.Find("SliderPanel");
            InstructionPanel = GameObject.Find("InstructionPanel");
            MenuPanel = GameObject.Find("MenuPanel");
            AlertPanel = GameObject.Find("AlertPanel");
            TargetPanel = GameObject.Find("TargetPanel");

            InstructionButton = GameObject.Find("InstructionButton").GetComponent<Button>();
            InstructionBackButton = GameObject.Find("InstructionBackButton").GetComponent<Button>();

            InstructionText = GameObject.Find("InstructionText").GetComponent<Text>();
            sliderPanelMessage = GameObject.Find("SliderPanelMessage").GetComponent<Text>();
            AlertText = GameObject.Find("AlertText").GetComponent<Text>();

            BoundingBox = GameObject.Find("BoundingBox");
            BoundingBox.GetComponent<MeshRenderer>().enabled = false;

            Transform[] trs = GameObject.Find("LeapHandController").GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trs)
            {
                if (t.name == "CapsuleHand_L")
                {
                    capsuleHand = t.gameObject.GetComponent<Leap.Unity.CapsuleHand>();
                }
            }


            RedX = GameObject.Find("RedX");
            RedX.SetActive(false);

            TargetTitleText = GameObject.Find("TargetTitleText").GetComponent<Text>();

            FeedbackHandle = GameObject.Find("FeedbackHandle");

            sliderValue = GameObject.Find("ValueLabelText").GetComponent<Text>();
            ValueLabelLeft = GameObject.Find("ValueLabelTextLeft").GetComponent<Text>(); 
            ValueLabelRight = GameObject.Find("ValueLabelTextRight").GetComponent<Text>();
            ValueLabelCentre = GameObject.Find("ValueLabelTextCentre").GetComponent<Text>();
            ValueLabelUnit = GameObject.Find("ValueLabelTextUnit").GetComponent<Text>();

            //RedFrameCtl = GameObject.Find("RedFrame").GetComponent<RedFrameControl>();
            videoCtl = GameObject.Find("VideoController").GetComponent<TargetMovieControl>();

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
            //sounds
            string waveFile = Application.dataPath + "/../" + "440Hz-100ms.wav";
            soundPlayer = new System.Media.SoundPlayer(waveFile);

        }


        // Use this for initialization
        protected override IEnumerator DoStart()
        {
            GameObject.Find("AVProContainer").transform.localEulerAngles = new Vector3(0f, initialRotation, 0f);
            SetLeapPointerHide();

            trackingCtl.LoadRecordedMovemnts(leapSelfCtl.recorder_, expCtl.dataCtl.numTargetAction);

            MenuPanel.SetActive(true);

            SliderPanel.SetActive(false);
            InstructionPanel.SetActive(false);
            AlertPanel.SetActive(false);
            TargetPanel.SetActive(false);
            yield break;
        }

        #region Virtual Functions
        public virtual IEnumerator CoScreenCaptureSession()
        {
            yield break;
        }
        public virtual IEnumerator CoTrainingStart()
        {
            yield break;
        }
        public virtual IEnumerator CoPracticeStart(List<DataManager.Result> ResultList)
        {
            yield break;
        }

        public virtual IEnumerator CoExperimentStart(List<DataManager.Result> ResultList, AbstractMenuManager.ExpSession SessionId, int blockId)
        {
            yield break;
        }
        #endregion

        #region UI Controls
        protected void InitSliderDuration()
        {
            sliderCtl.wholeNumbers = true;
            sliderCtl.minValue = 1000;
            sliderCtl.maxValue = 3000;

            sliderCtl.value =  UnityEngine.Random.Range(0, 2) == 0 ? sliderCtl.minValue : sliderCtl.maxValue;

            sliderPanelMessage.text = "\nHow long was the red frame presented?";
            ValueLabelLeft.text = "1000ms";
            ValueLabelRight.text = "3000ms";
            ValueLabelCentre.text = "";
            ValueLabelUnit.text = "ms";
            isSpacePressed = false;
            isSliderButtonPressed = false;

            sliderValue.enabled = true;
            isIncremental = false;
        }

        protected void InitSliderForControl()
        {
            sliderCtl.wholeNumbers = true;
            sliderCtl.minValue = 1;
            sliderCtl.maxValue = 7;
            sliderCtl.value = 4;
            sliderPanelMessage.text = "\nHow much did you feel you control VR hand?";
            ValueLabelLeft.text = "Strongly disagree";
            ValueLabelRight.text = "Strongly agree";
            ValueLabelCentre.text = "Neither";
            ValueLabelUnit.text = "";

            isSpacePressed = false;
            isSliderButtonPressed = false;


            sliderValue.enabled = false;
            isIncremental = true;
        }

        protected void InitSliderForAuthorship()
        {
            sliderCtl.wholeNumbers = true;
            sliderCtl.minValue = 1;
            sliderCtl.maxValue = 7;
            sliderCtl.value = 4;
            sliderPanelMessage.text = "\nHow much did you feel as if the VR hand was part of your body?";
            ValueLabelLeft.text = "Strongly disagree";
            ValueLabelRight.text = "Strongly agree";
            ValueLabelCentre.text = "Neither";
            ValueLabelUnit.text = "";

            isSpacePressed = false;
            isSliderButtonPressed = false;

            sliderValue.enabled = false;
            isIncremental = true;
        }


        public void ShowFeedback(float feedback)
        {
            isSliderEnabled = false;
            sliderCtl.interactable = false;

            SliderPanel.GetComponent<Fader>().FadeIn();

            RectTransform t = GameObject.Find("Handle Slide Area").GetComponent<RectTransform>();

            Vector3 position = FeedbackHandle.transform.localPosition;
            
            float xPos = Mathf.Lerp(t.rect.xMin, t.rect.xMax, ((feedback - sliderCtl.minValue )/ (sliderCtl.maxValue - sliderCtl.minValue) ));

            position.x = xPos;// - (int)(FeedbackHandle.GetComponent<RectTransform>().rect.width / 2);

            //UnityEngine.Debug.Log(t.rect.xMin + " " + t.rect.xMax + " " + (feedback / sliderCtl.maxValue));
            //UnityEngine.Debug.Log(xPos / (t.rect.xMax - t.rect.xMin) ) ;


            FeedbackHandle.transform.localPosition = position;

            FeedbackHandle.GetComponent<UnityEngine.UI.Image>().enabled = true;
            //  SliderPanel.GetComponent<Fader>().FadeIn();
            sliderPanelMessage.color = Color.red;
            sliderPanelMessage.text = "\nActual interval was " + feedback.ToString("0") + " ms";

        }

        public void HideFeedback()
        {
            sliderPanelMessage.color = Color.white;
            FeedbackHandle.GetComponent<UnityEngine.UI.Image>().enabled = false;
            SliderPanel.GetComponent<Fader>().FadeOut();

        }

        public string SetTrainingFilename(string data_dir, string dataname, int SubId)
        {

            string dir = data_dir +  SubId + "/";
            Directory.CreateDirectory(dir);

            DateTime dt = DateTime.Now;
            return string.Format(dir + dataname + "_{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

        public void SavTrainingToFile(string filename, List<double> array1, List<double> array2, double score)
        {
            string text = "";
            text += "#trialId, actual_duration, reported_duraton";
            text += Environment.NewLine;

            for (int i = 0; i < array1.Count; i++)
            {
                text += array1[i] + ", " + array2[i];
                text += Environment.NewLine;
            }

            text += "Score:" + score + Environment.NewLine;

            File.AppendAllText(filename, text);

        }


        public double CalcStandardScore(List<double> array1, List<double> array2)
        {
            const double M = 243.6232f;
            const double SD = 201.9328f;

            double sum = 0;
            for(int i=0; i < array1.Count; i++)
            {
                sum += Math.Abs(array1[i] - array2[i]);
            }
            double mean = sum / array1.Count;

            double standard_score = 10 * (M - mean) / SD + 50;

            return standard_score;

        }

        public double Correlation(double[] array1, double[] array2)
        {
            double[] array_xy = new double[array1.Length];
            double[] array_xp2 = new double[array1.Length];
            double[] array_yp2 = new double[array1.Length];

            for (int i = 0; i < array1.Length; i++)
                array_xy[i] = array1[i] * array2[i];
            for (int i = 0; i < array1.Length; i++)
                array_xp2[i] = Math.Pow(array1[i], 2.0);
            for (int i = 0; i < array1.Length; i++)
                array_yp2[i] = Math.Pow(array2[i], 2.0);
            double sum_x = 0;
            double sum_y = 0;
            foreach (double n in array1)
                sum_x += n;
            foreach (double n in array2)
                sum_y += n;
            double sum_xy = 0;
            foreach (double n in array_xy)
                sum_xy += n;
            double sum_xpow2 = 0;
            foreach (double n in array_xp2)
                sum_xpow2 += n;
            double sum_ypow2 = 0;
            foreach (double n in array_yp2)
                sum_ypow2 += n;
            double Ex2 = Math.Pow(sum_x, 2.00);
            double Ey2 = Math.Pow(sum_y, 2.00);

            double top = (array1.Length * sum_xy - sum_x * sum_y);
            double bottom = Math.Sqrt((array1.Length * sum_xpow2 - Ex2) * (array1.Length * sum_ypow2 - Ey2));

            if (bottom != 0)
            {
                return top / bottom;
            }
            else
            {
                return 0;
            }
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
        #endregion


        #region Avatar/Hand Control

        protected void ShowOwnHand()
        {
            leapSelfCtl.enabled = true;
            

            GameObject.Find("LeapHandController").GetComponent<LeapHandModifier>().UnsetHandTransparent();
            GameObject.Find("LeapHandController").GetComponent<Leap.Unity.LeapXRServiceProvider>().enabled = true;
            StartCoroutine(expCtl.ChangeHand(0f));

        }

        protected void HideOwnHand()
        {
            leapSelfCtl.enabled = false;
            GameObject.Find("LeapHandController").GetComponent<LeapHandModifier>().SetHandTransparent();
            GameObject.Find("LeapHandController").GetComponent<Leap.Unity.LeapXRServiceProvider>().enabled = false;
        }

        protected void ShowAvatarHand()
        {
            leapAvatarCtl.enabled = true;
            if (GameObject.Find("AvatarCamera"))
            {
                GameObject.Find("AvatarCamera").GetComponent<Leap.Unity.LeapXRServiceProvider>().enabled = true;
            }
        }

        protected void HideAvatarHand()
        {
            leapAvatarCtl.enabled = false;
            if (GameObject.Find("AvatarCamera"))
            {
                GameObject.Find("AvatarCamera").GetComponent<Leap.Unity.LeapXRServiceProvider>().enabled = false;
            }
        }


        #endregion
        protected void StartCalcFps()
        {
            isFPSrun = true;
        }

        protected float GetFpsAndStop()
        {
            fps = frameCount / dt;

            frameCount = 0;
            dt = 0;
            return fps;


        }


        // Update is called once per frame
        protected override void DoUpdate()
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

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

            if (Input.GetKeyDown(KeyCode.G))
            {
                ShowOwnHand();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                HideOwnHand();
                //SetLeapPointerHide();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                ShowAvatarHand();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                HideAvatarHand();
                //SetLeapPointerHide();
            }


            // Power Mate Control
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                isSpacePressed = true;
            }

            if (isSliderEnabled)
            {
                //float d = Input.GetAxis("Mouse ScrollWheel");
                float d = Input.GetAxis("HorizontalRStick");

                if (isIncremental)
                {
                    if (Time.time - last_time_stick > stick_interval && d != 0f)
                    {
                        if (d > 0.1f)
                        {
                            sliderCtl.value++;
                        }
                        else if (d < -0.1f)
                        {
                            sliderCtl.value--;
                        }
                        last_time_stick = Time.time;
                    }

                }
                else
                {
                    sliderCtl.value += d * stick_sensitivity * sliderCtl.maxValue;

                }
                sliderValue.text = sliderCtl.value.ToString();

            }

            if (isFPSrun)
            {
                frameCount++;
                dt += Time.unscaledDeltaTime;
            }

            // UnityEngine.Debug.Log(Input.GetAxis("HorizontalRStick"));

        }


        protected void SetLeapPointerHide()
        {
            // Leap.Unity.InputModule.LeapInputModule inputmodule = GameObject.Find("LeapEventSystem").GetComponent<Leap.Unity.InputModule.LeapInputModule>();
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
        }


        public IEnumerator WaitInstructionButtonPressed()
        {
            isSpacePressed = false;
            UnityEngine.Debug.Log(" space bar " + isSpacePressed);
            while (!isInstructionButtonPressed && !isInstructionBackButtonPressed && !isSpacePressed)
            {
                float d = Input.GetAxis("HorizontalRStick"); //Input.GetAxis("Mouse ScrollWheel");

                if (Time.time - last_time_stick > stick_interval && d != 0f)
                {

                    if (d > 0.9)
                    {
                        SelectRightButton();
                    }
                    else if (d < -0.9)
                    {
                        SelectLeftButton();
                    }
                    last_time_stick = Time.time;
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


    }
}