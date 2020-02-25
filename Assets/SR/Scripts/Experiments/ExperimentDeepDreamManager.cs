using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonUtil;


public class ExperimentDeepDreamManager : MonoBehaviour {
	
	SRController SRCtl;
	ExperimentDeepDream expCtl;
	Text Instruction;
	Text Message;
	INIFile iniFileCtl;

	public string config_file = "config.ini";

	bool isRunExperimentManager = false;
	bool button_pressed = false;
	KeyCode pressedKey = KeyCode.Alpha0;


	public int SubId = 100;

	public int num_sessions = 3;

	public int next_session_id_live = 1;
	public int next_session_id_replay = 1;

	bool odd_subject = false;

	public GameObject controlPanel;
	public Text informationText;
	public Button TrainingButton;
	public Button FirstSessionButton;
	public Button SecondSessionButton;

	bool isRunning = false;
	//public bool isUpdateSubIdAfterFinished = true;


	void Awake ()
	{	
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		expCtl = gameObject.GetComponent<ExperimentDeepDream> ();

		Instruction = GameObject.Find ("InstructionText").GetComponent<Text> ();
		Message = GameObject.Find ("MessageText").GetComponent<Text> ();

		controlPanel = GameObject.Find ("ControlPanel");
		informationText = GameObject.Find ("ExpInformation").GetComponent<Text> ();
		TrainingButton = GameObject.Find ("TrainingButton").GetComponent<Button> ();
		FirstSessionButton = GameObject.Find ("FirstSessionButton").GetComponent<Button> ();
		SecondSessionButton = GameObject.Find ("SecondSessionButton").GetComponent<Button> ();

	}
	
	IEnumerator Start () {

		while (!SRCtl.isReady) {
			yield return null;
		}

		StartExperiment ();
	}


	IEnumerator ExperimentManangerInit () {

		string initfile = Application.dataPath + "/../" + config_file;
		Debug.Log (initfile);
		iniFileCtl = new INIFile (initfile);

		try{
			SubId = int.Parse (iniFileCtl ["Experiment", "SubId"]);
			expCtl.SubId = SubId;
		}catch{
			Debug.Log ("SubId is not numerical in the Init file ");
		}

		try{
			expCtl.result_dir = iniFileCtl ["Experiment", "result_dir"].Trim ();
		}catch{
			Debug.Log ("result_dir is not numerical in the Init file ");
		}

		try{
			expCtl.video_duration = float.Parse (iniFileCtl ["TimePerception", "video_duration"]);;
			
		}catch{
			Debug.Log ("video_duration is not numerical in the Init file ");
		}

		try{
			expCtl.num_training_session = int.Parse (iniFileCtl ["TimePerception", "num_training_session"]);
		}catch{
			Debug.Log ("num_traing_session is not numerical in the Init file ");
		}

		try{
			num_sessions = int.Parse (iniFileCtl ["TimePerception", "num_sessions"]);
			expCtl.num_session = num_sessions;

		}catch{
			Debug.Log ("num_sessions is not numerical in the Init file ");
		}

		try{
			expCtl.pulsesensorcheck = iniFileCtl ["TimePerception", "pulsesensorcheck"].Trim () == "true" ? true : false;
			
		}catch{
			Debug.Log ("pulsesensorcheck is not true or false in the Init file ");
		}


		odd_subject = SubId % 2 != 0 ? true : false;

		if(SubId != 1){
			expCtl.TrajectorySubId = SubId -1;
		}else{
			SubId = 1;
		}


		next_session_id_live = expCtl.GetSessionIdLiveFromFile ();
		next_session_id_replay = expCtl.GetSessionIdReplayFromFile ();
			


		
		SRCtl.disableControll = true;
		SRCtl.DiableAudio ();
		SRCtl.TransitionToRecord();

		SRCtl.DisableOculus ();

		InitInstruction ();

		yield return StartCoroutine("ExperimentRun");
		yield return 0;

	}
	private IEnumerator ExperimentRun ()
	{
		while(!isRunning)
		{
			Debug.Log ("Loop in Experiment Manager");
			ShowInsturction ();

			yield return StartCoroutine("WaitForAnyKeyPress");
			yield return StartCoroutine ("Selector");
		}
	}

	private void InitInstruction(){

		TrainingButton.onClick.AddListener(() => { StartCoroutine(GoTraining()); });
		FirstSessionButton.onClick.AddListener(() => { StartCoroutine(GoFirstSession()); });
		SecondSessionButton.onClick.AddListener(() => { StartCoroutine(GoSecondSession()); });
	
	}
	
	private void ShowInsturction(){
		Message.text = "";
		Instruction.text = "";

		informationText.text = "Subject Id : " + SubId +"\n";

			informationText.text += "Live# " + next_session_id_live + "  Replay# " + next_session_id_replay +"\n";

		if (odd_subject) {
			FirstSessionButton.GetComponentInChildren<Text>().text = " [2] 1st Session (" + num_sessions.ToString() + " Normal)" ;
			SecondSessionButton.GetComponentInChildren<Text>().text = " [3] 2nd Session (" + num_sessions.ToString() + " Deep)" ;
		}else{
			FirstSessionButton.GetComponentInChildren<Text>().text = " [2] 1st Session (" + num_sessions.ToString() + " Deep)" ;
			SecondSessionButton.GetComponentInChildren<Text>().text = " [3] 2nd Session (" + num_sessions.ToString() + " Normal)" ;
		}
		SRCtl.GoToInstructionScreen ();

	}

	IEnumerator GoTraining(){
		//training
		if(!isRunning){
			isRunning = true;

			controlPanel.SetActive (false);

			expCtl.ExMode = ExperimentDeepDream.ExperimentMode.TRAINING;	
			yield return StartCoroutine (expCtl.runTraining());

			controlPanel.SetActive (true);
			isRunning = false;
			StartCoroutine("ExperimentRun");
		}
	}

	IEnumerator GoFirstSession(){
		if(!isRunning){
			isRunning = true;
			controlPanel.SetActive (false);

			expCtl.ExMode = ExperimentDeepDream.ExperimentMode.MAIN;
			if (odd_subject) {
				expCtl.Tracking = ExperimentDeepDream.TrackingMode.LIVE;
			}else{
				expCtl.Tracking = ExperimentDeepDream.TrackingMode.REPLAY;
			}	

			for(int i=0; i< num_sessions; i++){

				yield return StartCoroutine (expCtl.runExperiment ());
			}
		
			FirstSessionButton.interactable = false;	

			controlPanel.SetActive (true);
			isRunning = false;
			StartCoroutine("ExperimentRun");
		}
	}

	IEnumerator GoSecondSession(){
		if(!isRunning){
			isRunning = true;
			
			controlPanel.SetActive (false);
			
			expCtl.ExMode = ExperimentDeepDream.ExperimentMode.MAIN;
			if (odd_subject) {
				expCtl.Tracking = ExperimentDeepDream.TrackingMode.REPLAY;
			}else{
				expCtl.Tracking = ExperimentDeepDream.TrackingMode.LIVE;
			}	
			
			for(int i=0; i< num_sessions; i++){
				
				yield return StartCoroutine (expCtl.runExperiment ());
			}

			Instruction.text = "";
			Message.text = ("Experiment Done!");
			SRCtl.GoToInstructionScreen ();
		
			while (!Input.anyKey) {
				yield return new WaitForEndOfFrame();
			}
			Debug.Log ("Experiment Finishes Correctly");
			Application.Quit ();	

			//SecondSessionButton.interactable = false;	
			//controlPanel.SetActive (true);
			//isRunning = false;
			//StartCoroutine("ExperimentRun");
		}
	}
	



	IEnumerator GoLiveSingle(){
		if(!isRunning){
			isRunning = true;
			
			controlPanel.SetActive (false);

			expCtl.ExMode = ExperimentDeepDream.ExperimentMode.MAIN;	
			expCtl.Tracking = ExperimentDeepDream.TrackingMode.LIVE;
			yield return StartCoroutine (expCtl.runExperiment ());

			controlPanel.SetActive (true);
			isRunning = false;
			StartCoroutine("ExperimentRun");
		}
	}
	IEnumerator GoReplaySingle(){
		if(!isRunning){
			isRunning = true;
			
			controlPanel.SetActive (false);
			
			expCtl.ExMode = ExperimentDeepDream.ExperimentMode.MAIN;	
			expCtl.Tracking = ExperimentDeepDream.TrackingMode.REPLAY;
			yield return StartCoroutine (expCtl.runExperiment ());
			
			controlPanel.SetActive (true);
			isRunning = false;
			StartCoroutine("ExperimentRun");

		}
	}

	private IEnumerator Selector ()
	{
		UnityEngine.Debug.Log (pressedKey);
		if (pressedKey == KeyCode.Alpha1) {
			yield return StartCoroutine (GoTraining());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha2) {
			yield return StartCoroutine (GoFirstSession());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha3) {
			yield return StartCoroutine (GoSecondSession());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha4) {
		//	yield return StartCoroutine (GoFirstSession());
		//	yield break;
		}

	}


	private IEnumerator WaitForAnyKeyPress ()
	{
		while (!Input.anyKey || !button_pressed ) {
			yield return new WaitForEndOfFrame();
		}
		
		if (Input.GetKey (KeyCode.Alpha1)) {
			pressedKey = KeyCode.Alpha1;
		}else if (Input.GetKey (KeyCode.Alpha2)) {
			pressedKey = KeyCode.Alpha2;
		}else if (Input.GetKey (KeyCode.Alpha3)) {
			pressedKey = KeyCode.Alpha3;
		}else if(Input.GetKey (KeyCode.Alpha4))  {
			pressedKey = KeyCode.Alpha4;
		}else if(Input.GetKey (KeyCode.Alpha5)) {
			pressedKey = KeyCode.Alpha5;
		}else if(Input.GetKey (KeyCode.Alpha6)) {
			pressedKey = KeyCode.Alpha6;
		}else if(Input.GetKey (KeyCode.Alpha7)) {
			pressedKey = KeyCode.Alpha7;
		}else{
			pressedKey = KeyCode.Alpha0;
		}
	}

	public void StartExperiment()
	{
		
		if(!isRunExperimentManager){
			StartCoroutine("ExperimentManangerInit");
			isRunExperimentManager = true;
		}
	}

	void Update()
	{
		if (Input.anyKeyDown)
		{
			button_pressed = true;
		}else{
			button_pressed = false;
		}
		
		if(Input.GetKeyDown (KeyCode.Return)){
			StartExperiment();

		}

		if(Input.GetKeyDown (KeyCode.Escape)){
			StopCoroutine("ExperimentRun");
			isRunExperimentManager = false;
			SRCtl.disableControll = false;
			SRCtl.BackFromInstructionScreen ();
			Application.Quit ();
		}
	}
}
