using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;


public class DeepDreamBlendingDemo : MonoBehaviour {

	SRController SRCtl;
	// HeadTrackingRecorder headTracking;
	
	public float normal_video_minimal_secs = 2.0f;
	public float normal_video_random_sec = 2.0f;

	public float dream_video_minimal_secs = 5.0f;
	public float dream_video_random_sec = 5.0f;

	public float transition_secs = 3.0f;

	INIFile iniFileCtl;
	public string config_file = "config.ini";



    CycleHandPairs LeapCtl;
    // Use this for initialization
    void Awake () {

		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		// headTracking = GameObject.Find ("SRController").GetComponent<HeadTrackingRecorder>();

        LeapCtl = GameObject.Find("LeapHandController").GetComponent<CycleHandPairs>();

        ExperimentManangerInit ();
	}

	void ExperimentManangerInit () {
		
		string initfile = Application.dataPath + "/../" + config_file;
		Debug.Log (initfile);
		iniFileCtl = new INIFile (initfile);
		
		try{
			normal_video_minimal_secs = float.Parse (iniFileCtl ["DeepDream", "normal_video_minimal_secs"]);
		}catch{
			Debug.Log ("normal_video_minimal_secs is not numerical in the Init file ");
		}

		
		try{
			normal_video_random_sec = float.Parse (iniFileCtl ["DeepDream", "normal_video_random_sec"]);
		}catch{
			Debug.Log ("normal_video_random_sec is not numerical in the Init file ");
		}

		try{
			dream_video_minimal_secs = float.Parse (iniFileCtl ["DeepDream", "dream_video_minimal_secs"]);
		}catch{
			Debug.Log ("dream_video_minimal_secs is not numerical in the Init file ");
		}
		
		
		try{
			dream_video_random_sec = float.Parse (iniFileCtl ["DeepDream", "dream_video_random_sec"]);
		}catch{
			Debug.Log ("dream_video_random_sec is not numerical in the Init file ");
		}

		try{
			transition_secs = float.Parse (iniFileCtl ["DeepDream", "transition_secs"]);
		}catch{
			Debug.Log ("transition_secs is not numerical in the Init file ");
		}
	}

	IEnumerator Start () {
		
		while (!SRCtl.isReady) {
			yield return null;
		}

		StartCoroutine ("StartDemo");
	}

	IEnumerator StartDemo (){

		SRCtl.mergeSec = transition_secs;
		SRCtl.isPauseWhenGoToStream = false;
		SRCtl.isResetWhenGoToStream = false;

		yield return null;

		SRCtl.videoCtls [0].SetVideo (0);
		SRCtl.videoCtls [1].SetVideo (1);

		
		yield return null;

		SRCtl.videoCtls [0].SetForeGround ();
		SRCtl.videoCtls[1].SetForeGround ();

		yield return null;
		
		SRCtl.IsPlayVideoOnStart    = true;

		SRCtl.TransitionToRecord();

		SRCtl.IsPlayVideoOnStart   = false;

		StartCoroutine ("StartTransition");
	}

	IEnumerator StartTransition (){


		//headTracking.StartDetectHeadMotion (minimal_secs_since_last_switch + UnityEngine.Random.Range (0.0f, additonal_random_sec_range));
		float t = 0f;
		if (SRCtl.curStreamId == 1) {
			t = normal_video_minimal_secs + UnityEngine.Random.Range (0.0f, normal_video_random_sec);
		} else {
			t = dream_video_minimal_secs + UnityEngine.Random.Range (0.0f, dream_video_random_sec);
		}

		yield return new WaitForSeconds (t);

        if (SRCtl.curStreamId == 1)
        {
            LeapCtl.CurrentGroup = 0;
        }
        else
        {
            LeapCtl.CurrentGroup = 6;
        }

        SRCtl.IncreaseMerge ();

      

        StartCoroutine ("StartTransition");
	}

	// Update is called once per frame
	void Update () {



	}




}
