using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using CommonUtil;


public class ExperimentTimeInterface : MonoBehaviour {

	public enum TrackingMode : int
	{
		LIVE, FIXED, REPLAY, LIVE_FRAME, LIVE_BLACK, PRACTICE
	}

	public enum ExperimentMode : int
	{
		MAIN, PRACTICE, TRAINING
	}

	
	public enum MoveInstructionMode : int
	{
		MOVE, NOMOVE, MIXED
	}


	protected SRController SRCtl;
	protected CaliburationManager caliCtl;

	protected HeadTrackingRecorder HeadTracker;
	//protected OVRCameraRig OculusCamRig;
	protected DomeVideoInterface videoCtl;
	protected Logger logger;

	public int SubId = 1;
	public int TrajectorySubId = 1;

	public int fov = 120;
	public TrackingMode Tracking = TrackingMode.LIVE;
	public ExperimentMode ExMode = ExperimentMode.MAIN;
	public MoveInstructionMode MoveInstructoin = MoveInstructionMode.MIXED;

	protected Text Instruction;
	protected Text Message;

	protected LowLevelInputManager keyManager;
	protected PlayWave sound;

	protected string out_filename;
	protected string tracking_filename;
	protected string session_name = "";
	protected bool button_down = false;
	protected KeyCode pressedKey = KeyCode.Alpha0;

	protected bool mouse_down = false;

	protected System.Random Rn = new System.Random(System.Environment.TickCount);

	void Awake ()
	{	
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		caliCtl = GameObject.Find ("SRController").GetComponent<CaliburationManager>();

		HeadTracker = GameObject.Find ("SRController").GetComponent<HeadTrackingRecorder>();
		Instruction = GameObject.Find ("InstructionText").GetComponent<Text> ();
		Message = GameObject.Find ("MessageText").GetComponent<Text> ();
		keyManager = gameObject.GetComponent<LowLevelInputManager> ();
		logger = gameObject.GetComponent<Logger> ();

		GameObject g = GameObject.Find ("OVRCameraRig");
		GameObject c = g.transform.Find ("TrackingSpace").Find ("CenterEyeAnchor").gameObject;
		sound = c.GetComponent<PlayWave> ();;
		
		//OculusCamCtl = GameObject.Find ("OVRCameraRig").GetComponent<OVRManager>();
		//OculusCamRig = GameObject.Find ("OVRCameraRig").GetComponent<OVRCameraRig>();

		DoAwake ();
	}

	void Start () {
		videoCtl = GameObject.Find ("SRController").GetComponent<SRController> ().curVideoCtl;

		DoStart ();
	}
	
	void Update()
	{
		if (Input.anyKeyDown)
		{
			button_down = true;
		}else{
			button_down = false;
		}

		if (Input.GetMouseButtonDown(0)) {
			mouse_down = true;
		}else{
			mouse_down = false;
		}
		DoUpdate ();
	}

	protected virtual void DoAwake(){}
	protected virtual void DoStart(){}
	protected virtual void DoUpdate(){}

	public virtual IEnumerator runTraining(){yield return null;}
	public virtual IEnumerator runExperiment(){yield return null;}



	protected IEnumerator WaitForKeyDown(KeyCode keyCode)
	{
		while(!Input.GetKey (keyCode) || !button_down){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	protected IEnumerator WaitForKeysDown(KeyCode keyCode1, KeyCode keyCode2)
	{
		while((!Input.GetKey (keyCode1) && !Input.GetKey (keyCode2) ) || !button_down){
			yield return new WaitForEndOfFrame();
		}
		if(Input.GetKey (keyCode1))
		{
			pressedKey = keyCode1;
		}
		if(Input.GetKey (keyCode2))
		{
			pressedKey = keyCode2;
		}
		yield return 0;
	}
	
	protected IEnumerator WaitForKeyPressing(KeyCode keyCode)
	{
		while(Input.GetKey (keyCode)){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	protected IEnumerator WaitAnyKeyPressing()
	{
		while(Input.anyKey){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}


	protected IEnumerator WaitForKeyUp(KeyCode keyCode)
	{
		while(Input.GetKey (keyCode) || button_down){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	protected IEnumerator WaitForAnyKeyPress ()
	{
		while (!Input.anyKey || !button_down ) {
			//Debug.Log ("Waiting for keypress");
			yield return new WaitForEndOfFrame();
         }
         yield return 0;
	}

	protected IEnumerator WaitForAnyKeyPressOrSeconds (float seconds)
	{
		float start_sec = UnityEngine.Time.realtimeSinceStartup;
		while ( (!Input.anyKey || !button_down ) && UnityEngine.Time.realtimeSinceStartup - start_sec < seconds)  {
			//Debug.Log ("Waiting for keypress");
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	protected IEnumerator WaitForAnyKeyUpOrSeconds (float seconds)
	{
		//Debug.Log ("START - WaitForAnyKeyUpOrSeconds : " + keyManager.GetElapsedMilliseconds());
		//Debug.Log (Input.anyKey );

		float start_sec = UnityEngine.Time.realtimeSinceStartup;
		while ( Input.anyKey && UnityEngine.Time.realtimeSinceStartup - start_sec < seconds)  {
			//Debug.Log ("Waiting for keypress");

			yield return new WaitForEndOfFrame();
		}

//		Debug.Log ("END - WaitForAnyKeyUpOrSeconds : " + keyManager.GetElapsedMilliseconds());
		yield return 0;
	}


	protected IEnumerator WaitForMousePressing()
	{
		while(Input.GetMouseButton(0) ){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	protected IEnumerator WaitForMouseDown()
	{
		while(!Input.GetMouseButton(0) || !mouse_down){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	protected IEnumerator WaitForMouseDownOrSeconds (float seconds)
	{
		float start_sec = UnityEngine.Time.realtimeSinceStartup;
		while ( (!Input.GetMouseButton(0) || !mouse_down) && UnityEngine.Time.realtimeSinceStartup - start_sec < seconds)  {
			//Debug.Log ("Waiting for keypress");
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}
}
