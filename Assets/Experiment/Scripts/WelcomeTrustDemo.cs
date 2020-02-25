using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;


public class WelcomeTrustDemo : MonoBehaviour {

	//SRController SRCtl;
	//HeadTrackingRecorder headTracking;

    CycleHandPairs LeapCycler;
    LeapHandModifier LeapVolumer;
    Leap.Unity.HandModelManager LeapCtl;

    public bool isDemo = true;
	
	public float minimal_modified = 5.0f;
	public float additonal_modified = 5.0f;

    public float minimal_robot = 5.0f;
    public float additonal_robot = 5.0f;

    public float minimal_delay = 5.0f;
    public float additonal_delay = 5.0f;

    public float minimal_size = 5.0f;
    public float additonal_size = 5.0f;

    public float minimal_ball_break = 5.0f;
    public float additonal_ball_break = 5.0f;

    public float resting_seconds = 3.0f;

    public int max_number_of_ball = 70;

    public float ball_mass = 40f;


    // Use this for initialization
    void Awake () {


		//OculusCamCtl = GameObject.Find ("OVRCameraController").GetComponent<OVRManager>();
		//SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		//headTracking = GameObject.Find ("SRController").GetComponent<HeadTrackingRecorder>();

        LeapCycler = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();
        LeapVolumer = GameObject.Find("LeapHandController").GetComponent<LeapHandModifier>();
        LeapCtl = GameObject.Find("LeapHandController").GetComponent<Leap.Unity.HandModelManager>();


    }
    void Start()
    {
        StartCoroutine("DemoSession");
        StartCoroutine("SizeSession");
        StartCoroutine("BallSession");
    }

    IEnumerator DemoSession() {
        int prevChange = 0;
        LeapVolumer.ResetVolume();
        yield return new WaitForSeconds(1.0f);

        while (isDemo) {
            int nextChange = 0;
            do
            {
                nextChange = Random.Range(0, 3);

            }while (nextChange == prevChange);

            //nextChange = 3;

            float modified_duration = minimal_modified + Random.Range(0f, additonal_modified);
            float robot_duration = minimal_robot + Random.Range(0f, additonal_robot);
            float delay_duration = minimal_delay + Random.Range(0f, additonal_delay);

            Debug.Log(nextChange );
            switch (nextChange)
            {
                case 0: //toggle robot
                    Debug.Log("Robot starts");
                    LeapCycler.CurrentGroup = 6;
                    yield return new WaitForSeconds(robot_duration);
                    LeapCycler.CurrentGroup = Random.Range(0, 6);
                    break;
                case 1: // fat / skinny

                    Debug.Log("Fat/skinny starts");
                    if (Random.Range(0, 2) == 0)
                    {
                        float max_volume = Random.Range(0.02f, 0.04f);

                        while (LeapVolumer.sickness < max_volume)
                        {
                            //Debug.Log("hogehoge " + LeapVolumer.sickness);
                            LeapVolumer.IncVolume(0.0002f);
                            yield return null;
                        }
                        yield return new WaitForSeconds(modified_duration);
                        while (LeapVolumer.sickness > 0.0f)
                        {
                            LeapVolumer.DecVolume(0.0002f);
                            yield return null;
                        }
                    }
                    else
                    {
                        while (LeapVolumer.sickness > -0.005f)
                        {
                            //Debug.Log("hogehoge " + LeapVolumer.sickness);
                            LeapVolumer.DecVolume(0.00005f);
                            yield return null;
                        }
                        yield return new WaitForSeconds(modified_duration);
                        while (LeapVolumer.sickness < 0.0f)
                        {
                            LeapVolumer.IncVolume(0.00005f);
                            yield return null;
                        }

                    }
                    break;
                case 2: //delay

                    Debug.Log("delaystarts");
                    LeapCtl.recorder_.SetDelayMillisec((long)Random.Range(10, 20));
                    LeapCtl.recorder_.StartDelay();
                    yield return new WaitForSeconds(delay_duration);
                    LeapCtl.recorder_.StopDelay();

                    break;

                
            }

            yield return new WaitForSeconds(resting_seconds);
            prevChange = nextChange;
        }
		
	}
    IEnumerator SizeSession()
    {
        Debug.Log("Size init");
        while (isDemo)
        {
            Debug.Log("size change starts");
            float size_duration = minimal_size + Random.Range(0f, additonal_size);
            if (Random.Range(0, 2) == 0)
            {
                float max_size = Random.Range(1.5f, 2.5f);

                while (LeapVolumer.size < max_size)
                {
                    //Debug.Log("hogehoge " + LeapVolumer.sickness);
                    LeapVolumer.IncSize(0.02f);
                    yield return null;
                }
                yield return new WaitForSeconds(size_duration);
                while (LeapVolumer.size > 1.0f)
                {
                    LeapVolumer.DecSize(0.02f);
                    yield return null;
                }
            }
            else
            {
                while (LeapVolumer.size > 0.5f)
                {
                    //Debug.Log("hogehoge " + LeapVolumer.sickness);
                    LeapVolumer.DecSize(0.005f);
                    yield return null;
                }
                yield return new WaitForSeconds(size_duration);
                while (LeapVolumer.size < 1.0f)
                {
                    LeapVolumer.IncSize(0.005f);
                    yield return null;
                }

            }

            Debug.Log("size change end");
            yield return new WaitForSeconds(resting_seconds);
        }

    }

    IEnumerator BallSession()
    {
        while (isDemo)
        {
            float break_seconds = minimal_ball_break + Random.Range(0f, additonal_ball_break);

            yield return new WaitForSeconds(break_seconds);



            GameObject[] objects = new GameObject[max_number_of_ball];
            for (int i = 0; i < max_number_of_ball; i++)
            {
                float scale = Random.Range(0.04f, 0.10f);
                objects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                objects[i].transform.localScale = new Vector3(scale, scale, scale);

                Vector3 head_pos = GameObject.Find("CenterEyeAnchor").transform.position;
                head_pos.y = head_pos.y + 0.4f;
                head_pos.z = head_pos.z + 1.2f;
                head_pos.x = head_pos.x + Random.Range(-0.15f, 0.15f);
                objects[i].transform.position = head_pos;

                switch (i % 3)
                {
                    case 0: objects[i].GetComponent<Renderer>().material = Resources.Load<Material>("Materials/RedBall"); break;
                    case 1: objects[i].GetComponent<Renderer>().material = Resources.Load<Material>("Materials/BlueBall"); break;
                    case 2: objects[i].GetComponent<Renderer>().material = Resources.Load<Material>("Materials/GreenBall"); break;

                }

                Rigidbody rb = objects[i].AddComponent<Rigidbody>(); // Add the rigidbody.
                rb.mass = ball_mass; // Set the GO's mass to 5 via the Rigidbody.
                rb.AddForce(new Vector3(0f, 0f, Random.Range(-100f, -150f)), ForceMode.Impulse);
                yield return new WaitForSeconds(0.1f);

            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < max_number_of_ball; i++)
            {
                Destroy(objects[i]);
            }
        }
    }

        // Update is called once per frame
        void Update () {
		/*
		if ( (Input.GetKey (KeyCode.L) || Input.GetKey(KeyCode.JoystickButton8))&& !isDemo  ) {
			StopCoroutine("DemoSession");
			StopCoroutine("LiveToRecord");
			StopCoroutine("RecordToLiveAfterFinish");
			StopCoroutine("SwitchToRecord");

			Debug.Log ("Demo Finish");
			isDemo = false;
		}

		if ( (Input.GetKey (KeyCode.O) || Input.GetKey (KeyCode.JoystickButton9))&& !isDemo  ) {
			StopCoroutine("DemoSession");
			StopCoroutine("LiveToRecord");
			StopCoroutine("RecordToLiveAfterFinish");
			StopCoroutine("SwitchToRecord");
			isDemo = true;

			Debug.Log ("Demo Start");
			StartCoroutine("DemoSession");
		}
		*/




	}




}
