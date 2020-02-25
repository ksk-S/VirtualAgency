using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusTest : MonoBehaviour
{
   // Leap.Unity.HandModelManager leapCtl;

    //int counter = 0;

   // CollisionDetector collisionDetector;
    public int stimulusLabel = 0;

    GameObject LeftHand;
    GameObject visualStimulus;

    Vector3 parmPos = new Vector3();

    Renderer visualStimulusRenderer;
    CollisionDetector cd;

    AbstractExperimentDelayAndShock expCtl;

    //int portAddress = 0x0378;
    int portAddress = 0xC050;
    //int portAddress = 0xC040;


    public System.Diagnostics.Stopwatch sw;
    public int count_stimuli_send = 0;
    public int count_stimuli_catch = 0;
    public int count_stimuli_catch_dist = 0;

    public enum ShockType
    {
        NOSHOCK = 0, SHOCK
    }
    public enum StimulusType
    {
        NOVISUAL = 0, VISUAL
    }

    public enum DelayType
    {
        SYNC=0, ASYNC
    }

    public enum HandType
    {
        VR = 0, BAREHAND1, BAREHAND2
    }

    public enum PeriodType
    {
        PRACTICE = 0, FIRST, SECOND, THIRD

    }

    public enum MarkerType
    {
        STIMULUS = 0, PERIOD
    }

    private void Awake()
    {
       // leapCtl = GameObject.Find("LeapHandController").GetComponent<Leap.Unity.HandModelManager>();
       // collisionDetector = GameObject.Find("VisualStimulus").GetComponent<CollisionDetector>();
        expCtl = GameObject.Find("Experiment").GetComponent<AbstractExperimentDelayAndShock>();

        visualStimulusRenderer = GameObject.Find("MyFingerStimulusSkin").GetComponent<Renderer>();
    }

    // Use this for initialization
    void Start()
    {
        ParallelPortControl.PortControl.Output(portAddress, 0);

        visualStimulus = GameObject.Find("VisualStimulus");

        cd = visualStimulus.GetComponent<CollisionDetector>();


        sw = new System.Diagnostics.Stopwatch();
    }

    public void SetVisualStimulus()
    {

        StartCoroutine(FindLeftHandCoroutine());
    }

    IEnumerator FindLeftHandCoroutine()
    {
        LeftHand = null;
        
        while (LeftHand == null)
        {
            LeftHand = GameObject.Find("RigidRoundHand_L");
            if (LeftHand == null)
            {
                LeftHand = GameObject.Find("RigidRoundHand_L(Clone)");
                if (LeftHand == null)
                {
                    LeftHand = GameObject.Find("RigidRoundHand_R");
                    if (LeftHand == null)
                    {
                        LeftHand = GameObject.Find("RigidRoundHand_R(Clone)");
                    }
                }
            }
            //            Debug.Log("cannot find left hand");
            yield return null;
        }

        cd.IsActive = true;
        int numChildren = LeftHand.transform.childCount;
        for (int i = 0; i < numChildren; ++i)
        {
            if (LeftHand.transform.GetChild(i).gameObject.name == "palm")
            {
                parmPos = LeftHand.transform.GetChild(i).position;
            }
        }

        // Debug.Log(expCtl.stimulus_offset_x + " "+ expCtl.stimulus_offset_y);

        Vector3 StimulusHandPos = new Vector3(
            parmPos.x + expCtl.stimulus_offset_x,
            parmPos.y + expCtl.stimulus_height,
            parmPos.z + expCtl.stimulus_offset_y);

        visualStimulus.transform.localPosition = StimulusHandPos;
        visualStimulusRenderer.enabled = true;
        //visualStimulusHandModel.GetComponent<Renderer>().enabled = true;


        sw.Reset(); sw.Start();
        count_stimuli_send++;
        Debug.Log("stimuli Starts Fall Off:" + count_stimuli_send);

        StartCoroutine(SendVisualStimulationSignal());
    }

    IEnumerator SendVisualStimulationSignal()
    {

        float dist = visualStimulus.transform.localPosition.y - parmPos.y;

        while (dist > 0.03)
        {
            dist = visualStimulus.transform.localPosition.y - parmPos.y;
            yield return 0;
        }
        if (cd.IsActive) {
            SendParallelSignal();
            count_stimuli_catch_dist++;
            Debug.Log("Hand Collision based on Distance :" + count_stimuli_catch_dist + " time:" + sw.ElapsedMilliseconds);

            cd.IsActive = false;
        }
    }

    public int SetStimulusLabel(ShockType ShockMarker, StimulusType StimulusMarker,  DelayType DelayMarker, HandType HandMarker, PeriodType Periodmarker, MarkerType TypeMarker)
    {
        stimulusLabel = (int)ShockMarker + (int)StimulusMarker * 2 + (int)DelayMarker * 4 + (int)HandMarker * 8 + (int)Periodmarker * 32 + (int)TypeMarker * 128  ;   

        Debug.Log("Stimulus Label:" + stimulusLabel + " ( Shock:" + ShockMarker + " Stimulus:" + StimulusMarker + " Delay:" + DelayMarker + " Hand:" + HandMarker + " Period:" + Periodmarker + " Type:"+ TypeMarker+ ")");

        return stimulusLabel;
    }

    public void SendParallelSignal()
    {
        ParallelPortControl.PortControl.Output(portAddress, stimulusLabel);

        StartCoroutine(SendResetSignal());

    }

    public IEnumerator SendResetSignal()
    {
        yield return null;
        ParallelPortControl.PortControl.Output(portAddress, 0);

    }

    public int SendPeriodMarker(int orderId, int delayId, int HandId, int repeatId)
    {
        //old not used
        DelayType delay = delayId == 0 ? DelayType.SYNC : DelayType.ASYNC;
        SetStimulusLabel(ShockType.NOSHOCK, StimulusType.NOVISUAL, delay, (HandType)HandId, (PeriodType)orderId, MarkerType.PERIOD);

        SendParallelSignal();
        return stimulusLabel;
    }


    public int SendPeriodMarker2(int stimulusId, int delayId, int HandId, int orderId)
    {
        DelayType delay = delayId == 0 ? DelayType.SYNC : DelayType.ASYNC;
        SetStimulusLabel(ShockType.NOSHOCK, (StimulusType)stimulusId, delay, (HandType)HandId, (PeriodType)orderId, MarkerType.PERIOD);

        SendParallelSignal();
        return stimulusLabel;
    }

    public int StimulateTactile2(int delayId, int HandId, int orderId)
    {

        DelayType delay = delayId == 0 ? DelayType.SYNC : DelayType.ASYNC;
        SetStimulusLabel(ShockType.SHOCK, StimulusType.NOVISUAL, delay, (HandType)HandId, (PeriodType)orderId, MarkerType.STIMULUS);

        SendParallelSignal();
        return stimulusLabel;
    }

   
    public int StimulateVisualTactile2(int delayId, int HandId, int orderId)
    {
        DelayType delay = delayId == 0 ? DelayType.SYNC : DelayType.ASYNC;
        SetStimulusLabel(ShockType.SHOCK, StimulusType.VISUAL, delay, (HandType)HandId, (PeriodType)orderId, MarkerType.STIMULUS);

        SetVisualStimulus();
        return stimulusLabel;
    }

    public int StimulateTactile(int delayId, int orderId)
    {
        return stimulusLabel;
    }
    public int StimulateVisual(int delayId, int orderId)
    {
        return stimulusLabel;
    }
    public int StimulateVisualTactile(int delayId, int orderId)
    {
        return stimulusLabel;
    }
        public void StimulatePracticeTactile()
    {
    }
    public void StimulatePracticeVisual()
    {
    }
   
    public void StimulatePracticeVisualTactile()
    {
    }



    IEnumerator TestMarkers2()
    {
        stimulusLabel = 7;
        for(int i=0;i <5; i++)
        {
            yield return new WaitForSeconds(0.2f);
            SendParallelSignal();
        }

        for (int i = 0; i < 100; i++)
        {
            stimulusLabel = i;
            SendParallelSignal();
            Debug.Log("send" + i );
            yield return new WaitForSeconds(0.25f);

        }
    }

    // Update is called once per frame
    void Update()
    {
        //ParallelPortControl.PortControl.Output(0x0378, 0);
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetStimulusLabel(ShockType.SHOCK, StimulusType.NOVISUAL, DelayType.ASYNC, HandType.VR, PeriodType.PRACTICE, MarkerType.STIMULUS);
            SendParallelSignal();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(TestMarkers2());
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StimulateVisualTactile2(0, 0, 0);
        }



    }
}
