using UnityEngine;
using System.Collections;
using System.Media;
using System.Timers;


public class ButtonPress : MonoBehaviour
{

    Arduino vibrator;
    private System.Timers.Timer toneTimer;
    public System.Timers.Timer autoButtonTimer;
    public System.Timers.Timer autoToneTimer;
    public System.Timers.Timer autoTactileTimer;

    //GameObject ButtonGadget;
    GameObject Button;
    Rigidbody ButtonRigidbody;

    public double interval = 900;
    public bool buttonEnabled = true;

    public bool isToneOnSet = false;

    public int buttonPressedTime = 0;
    public int toneTime = 0;

    bool isAutomaticButtonFlag = false;

    public bool isButtonActiveAfterPress = true;

    public bool automatic_button_success = false;

    System.Media.SoundPlayer player = null;


    void Start()
    {
        vibrator = GameObject.Find("Experiment").GetComponent<Arduino>();

        toneTimer = new System.Timers.Timer(900);
        toneTimer.Elapsed += OnFollowingToneEvent;
        toneTimer.AutoReset = false;

        autoButtonTimer = new System.Timers.Timer();
        autoButtonTimer.Elapsed += OnAutomaticEvent;
        autoButtonTimer.AutoReset = false;

        autoToneTimer = new System.Timers.Timer();
        autoToneTimer.Elapsed += OnAutoToneEvent;
        autoToneTimer.AutoReset = false;


        autoTactileTimer = new System.Timers.Timer();
        autoTactileTimer.Elapsed += OnAutoTactileEvent;
        autoTactileTimer.AutoReset = false;


        Button = GameObject.Find("RedButton");
        ButtonRigidbody = Button.GetComponent<Rigidbody>();

        Button.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        Button.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);

        //ButtonGadget = GameObject.Find("ButtonGadget");

        //読み込む
        string waveFile = Application.dataPath + "/../" + "880Hz-50ms.wav";
        player = new System.Media.SoundPlayer(waveFile);

    }

    public void AutomaticButtonPress(int waiting_time)
    {
        automatic_button_success = false;
        Debug.Log("automatic button press start");
        autoButtonTimer.Interval = waiting_time;
        autoButtonTimer.Start();
    }

    public void CancelAutomaticButtunPress()
    {
        autoButtonTimer.Stop();
        Debug.Log("fake button press is cancelled");
    }

    public void AutomaticTone(int waiting_time)
    {
        automatic_button_success = false;
        Debug.Log("automatic tone start");
        autoToneTimer.Interval = waiting_time;
        autoToneTimer.Start();

    }

    public void AutomaticTactile(int waiting_time)
    {
        automatic_button_success = false;
        Debug.Log("automatic tactile start");
        autoTactileTimer.Interval = waiting_time;
        autoTactileTimer.Start();

    }

    public void KeyboardPress()
    {
        buttonPressedTime = (int)SessionManager.sw.ElapsedMilliseconds;
        toneTimer.Interval = interval;
        toneTimer.Start();
    }

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.name == "RedButton")
        {

            if (buttonEnabled)
            {
                vibrator.Vibrate();
                CancelAutomaticButtunPress();

                buttonPressedTime = (int)SessionManager.sw.ElapsedMilliseconds;
                Debug.Log("Button is pressed physically" );
                //SystemSounds.Beep.Play();

                if (interval > 0)
                {
                    toneTimer.Interval = interval;
                    toneTimer.Start();
                }
                //DisableButton();

                buttonEnabled = false;
                Button.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.gray);
            }

        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "RedButton")
        {
            Button.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }

    void OnFollowingToneEvent(System.Object source, ElapsedEventArgs e)
    {
        toneTime = (int)SessionManager.sw.ElapsedMilliseconds;

//        buttonSound.Play();
        player.Play();
        //SystemSounds.Beep.Play();
        //Debug.Log(e.SignalTime);

        if (isButtonActiveAfterPress) { 
            buttonEnabled = true;
        }
        isToneOnSet = true;

    }

    void OnAutomaticEvent(System.Object source, ElapsedEventArgs e)
    {
        isAutomaticButtonFlag = true;

        automatic_button_success = true;
        Debug.Log("fake button press");
    }

    void OnAutoToneEvent(System.Object source, ElapsedEventArgs e)
    {
        buttonPressedTime = (int)SessionManager.sw.ElapsedMilliseconds;
        player.Play();

        automatic_button_success = true;
        toneTimer.Interval = interval;
        toneTimer.Start();
        buttonEnabled = false;
        Debug.Log("first tone");
    }

    void OnAutoTactileEvent(System.Object source, ElapsedEventArgs e)
    {
        isAutomaticButtonFlag = true;

        buttonPressedTime = (int)SessionManager.sw.ElapsedMilliseconds;
        vibrator.Vibrate();

        automatic_button_success = true;
        toneTimer.Interval = interval;
        toneTimer.Start();
        buttonEnabled = false;
        Debug.Log("vibrate");
    }


    public void EnableButton()
    {
        buttonEnabled = true;
        ButtonRigidbody.isKinematic = false;
    }

    public void DisableButton()
    {
        buttonEnabled = false;
        ButtonRigidbody.isKinematic = true;
    }


    void MoveButtonBack()
    {
        Button.transform.localPosition = new Vector3(0f, -0.0041f, 0f);

    }

    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Return))
        {
            MoveButtonBack();
        }

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            if (buttonEnabled)
            {
                DisableButton();
            }
            else {
                EnableButton();
            }
        }


        if (isAutomaticButtonFlag)
        {
            EnableButton();
            Rigidbody rb = Button.GetComponent<Rigidbody>();
            rb.AddForce(0, -1, 0, ForceMode.Impulse);
            isAutomaticButtonFlag = false;
        }

    }
}