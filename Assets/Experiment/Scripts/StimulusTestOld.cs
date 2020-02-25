using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusTestOld : MonoBehaviour
{
    //Leap.Unity.HandModelManager leapCtl;

    //int counter = 0;

    // CollisionDetector collisionDetector;
    public int stimulusLabel = 0;

    GameObject LeftHand;
    GameObject visualStimulus;

    Vector3 parmPos = new Vector3();

    Renderer visualStimulusRenderer;
    CollisionDetector cd;

    AbstractExperimentDelayAndShock expCtl;
    
    public System.Diagnostics.Stopwatch sw;
    public int count_stimuli_send = 0;
    public int count_stimuli_catch = 0;
    public int count_stimuli_catch_dist = 0;
    private void Awake()
    {
        //leapCtl = GameObject.Find("LeapHandController").GetComponent<Leap.Unity.HandModelManager>();
        //collisionDetector = GameObject.Find("VisualStimulus").GetComponent<CollisionDetector>();
        expCtl = GameObject.Find("Experiment").GetComponent<AbstractExperimentDelayAndShock>();

        visualStimulusRenderer = GameObject.Find("MyFingerStimulusSkin").GetComponent<Renderer>();
    }

    // Use this for initialization
    void Start()
    {
        ParallelPortControl.PortControl.Output(0x0378, 0);

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
        while (LeftHand == null)
        {
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

    public int SetStimulusLabel(int ShockMarker, int EventMarker,  int StimulusMarker, int DelayMarker, int OrderMarker)
    {
        stimulusLabel = OrderMarker * 64 + DelayMarker * 16 + StimulusMarker * 4 + EventMarker * 2 + ShockMarker;   

        Debug.Log("Stimulus Label:" + stimulusLabel + " ( Shock:" + ShockMarker + " Event:" + EventMarker + " Stimulus:" + StimulusMarker + " Delay:" + DelayMarker + " Order:" + OrderMarker + ")");

        return stimulusLabel;
    }

    public int SetStimulusLabelNew(int ShockMarker, int EventMarker, int StimulusMarker, int DelayMarker, int OrderMarker)
    {
        stimulusLabel = ShockMarker + EventMarker * 2 + StimulusMarker * 4 + DelayMarker * 16 + OrderMarker * 64;

        Debug.Log("Stimulus Label:" + stimulusLabel + " ( Shock:" + ShockMarker + " Event:" + EventMarker + " Stimulus:" + StimulusMarker + " Delay:" + DelayMarker + " Order:" + OrderMarker + ")");

        return stimulusLabel;
    }

    public void SendParallelSignal()
    {
        ParallelPortControl.PortControl.Output(888, stimulusLabel);

        StartCoroutine(SendResetSignal());

    }

    public IEnumerator SendResetSignal()
    {
        yield return null;
        ParallelPortControl.PortControl.Output(888, 0);

    }

    public int SendPeriodMarker(int orderId, int delayId, int stimuId, int repeatId)
    {
        int dId = delayId == 0 ? 0 : 1;
        SetStimulusLabel(0, 0, orderId, stimuId, dId + 2 * repeatId);

        //        SetStimulusLabel(0, 0, orderId, delayId, stimuId + 2 * repeatId);
        SendParallelSignal();
        return stimulusLabel;
    }


    public int StimulateTactile(int delayId, int orderId)
    {
        SetStimulusLabel(1, 0, 0, delayId, orderId);
        SendParallelSignal();
        return stimulusLabel;
    }

    public int StimulateVisual(int delayId, int orderId)
    {
        SetStimulusLabel(0, 1, 0, delayId, orderId);

        SetVisualStimulus();

        return stimulusLabel;
    }

    public int StimulateVisualTactile(int delayId, int orderId)
    {
        SetStimulusLabel(1, 1, 0, delayId, orderId);

        SetVisualStimulus();
        return stimulusLabel;
    }

    public void StimulatePracticeTactile()
    {
        SetStimulusLabel(1, 0, 0, 0, 0);
        SendParallelSignal();
    }

    public void StimulatePracticeVisual()
    {
        SetStimulusLabel(0, 0, 0, 0, 0);
        SetVisualStimulus();
    }
    public void StimulatePracticeVisualTactile()
    {
        SetStimulusLabel(1, 0, 0, 0, 0);
        SetVisualStimulus();
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

    IEnumerator TestPeriodMarkers()
    {
        for (int rId = 0; rId < 2; rId++)
        {
            for(int dId=0; dId <3; dId++)
            {
                for (int sId = 0; sId < 3; sId++)
                {
                    for (int oId=1; oId<4; oId++)
                    {
                        if(dId != 1) {
                            Debug.Log(dId + " " + sId + " " + oId);
                            SendPeriodMarker(oId, dId, sId, rId);
                            yield return null;
                        }
                    }
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //ParallelPortControl.PortControl.Output(0x0378, 0);
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            SetStimulusLabel(1, 1, 1 , 1, 1);
            SendParallelSignal();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(TestMarkers2());
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {

            StartCoroutine(TestPeriodMarkers());
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StimulateVisual(0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StimulateTactile(0, 0);
        }


    }
}
