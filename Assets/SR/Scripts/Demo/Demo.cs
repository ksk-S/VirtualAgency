using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;


public class Demo : MonoBehaviour {

	SRController SRCtl;
	//OVRManager OculusCamCtl;
	LadybugPluginInterface ladybugCtl;
	HeadTrackingRecorder headTracking;

	public bool isDemo = false;
	public float secsUnitlSwitch = 0.10f;
	
	public float minimal_secs_since_last_switch = 5.0f;
	public float additonal_random_sec_range = 5.0f;



	// Use this for initialization
	void Start () {


		//OculusCamCtl = GameObject.Find ("OVRCameraController").GetComponent<OVRManager>();
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		ladybugCtl = SRCtl.ladybugCtl;
		headTracking = GameObject.Find ("SRController").GetComponent<HeadTrackingRecorder>();

		if(!SRCtl.useDomeVideo){
			if(SRCtl.useLiveLadybug)
			{
				ladybugCtl = GameObject.Find ("SphericalScreen").GetComponent("LadybugPluginCamera") as LadybugPluginInterface;
			}else
			{
				ladybugCtl = GameObject.Find ("SphericalScreen").GetComponent("LadybugPluginStream") as LadybugPluginInterface;
			}
		}

	

	}

	IEnumerator DemoSession () {
		int sceneId = 0;
		while(isDemo && sceneId < 5){
			SRCtl.GoToStreamBackground(sceneId);
			yield return StartCoroutine("LiveToRecord");

			yield return  StartCoroutine ("RecordToLiveAfterFinish");

			sceneId++;
		}

		isDemo = false;

	}

	IEnumerator LiveToRecord () {

		SRCtl.TransitionToLive();

		headTracking.StartDetectHeadMotion (minimal_secs_since_last_switch + UnityEngine.Random.Range (0.0f, additonal_random_sec_range));

		while(!headTracking.switching_timing)
		{
			yield return new WaitForEndOfFrame();
		}

		yield return  StartCoroutine ("SwitchToRecord");

	
	}


	IEnumerator RecordToLiveAfterFinish () {
		
		ladybugCtl.is_repeat_after_end = false;
		while(!ladybugCtl.is_reach_the_end_flag)
		{
			yield return new WaitForEndOfFrame();
		}

		headTracking.StartDetectHeadMotion (0.0f);
		while(!headTracking.switching_timing)
		{
			yield return new WaitForEndOfFrame();
		}

		SRCtl.TransitionToLive();
		ladybugCtl.is_repeat_after_end = true;

	}
	
	IEnumerator SwitchToRecord () {
		yield return new WaitForSeconds(secsUnitlSwitch);
		
		SRCtl.TransitionToRecord();
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
