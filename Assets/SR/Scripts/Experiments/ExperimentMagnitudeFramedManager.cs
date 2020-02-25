using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonUtil;


public class ExperimentMagnitudeFramedManager : MonoBehaviour {
	
	SRController SRCtl;
	ExperimentMagnitude expCtl;
	Text Instruction;
	Text Message;
	INIFile iniFileCtl;

	public string config_file = "config.ini";

	bool isRunExperimentManager = false;
	bool button_pressed = false;
	KeyCode pressedKey = KeyCode.Alpha0;

	public int SubId = 100;
	public int DayId = 0;

	public int num_sessions = 3;

	public int next_session_id_live = 1;
	public int next_session_id_black = 1;

	public int effective_num_sessions_live = 3;
	public int effective_num_sessions_black = 3;


	bool odd_subject = false;

	public GameObject controlPanel;
	public Text informationText;
	public Button TrainingButton;
	public Button LivePracticeButton;
	//public Button ReplayPracticeButton;
	public Button FirstSessionButton;
	public Button SecondSessionButton;
	public Button LiveBlockButton;
	public Button ReplayBlockButton;

	bool isRunning = false;
	//public bool isUpdateSubIdAfterFinished = true;


	void Awake ()
	{	
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		expCtl = gameObject.GetComponent<ExperimentMagnitude> ();

		Instruction = GameObject.Find ("InstructionText").GetComponent<Text> ();
		Message = GameObject.Find ("MessageText").GetComponent<Text> ();

		controlPanel = GameObject.Find ("ControlPanel");
		informationText = GameObject.Find ("ExpInformation").GetComponent<Text> ();
		TrainingButton = GameObject.Find ("TrainingButton").GetComponent<Button> ();
		LivePracticeButton = GameObject.Find ("LivePracticeButton").GetComponent<Button> ();
		FirstSessionButton = GameObject.Find ("FirstSessionButton").GetComponent<Button> ();
		SecondSessionButton = GameObject.Find ("SecondSessionButton").GetComponent<Button> ();
		LiveBlockButton = GameObject.Find ("LiveBlockButton").GetComponent<Button> ();
		ReplayBlockButton = GameObject.Find ("ReplayBlockButton").GetComponent<Button> ();

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
			expCtl.video_duration = float.Parse (iniFileCtl ["TimePerception", "video_duration"]);;

		}catch{
			Debug.Log ("video_duration is not numerical in the Init file ");
		}

		try{
			expCtl.CalibFrameRatioX = float.Parse (iniFileCtl ["TimePerception", "CalibFrameRatioX"]);
			expCtl.CalibFrameRatioY = float.Parse (iniFileCtl ["TimePerception", "CalibFrameRatioY"]);
		}catch{
			Debug.Log ("CalibFrameRatio is not numerical in the Init file ");
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
		LivePracticeButton.onClick.AddListener(() => { StartCoroutine(GoPractice()); });

		FirstSessionButton.onClick.AddListener(() => { StartCoroutine(GoFirstSession()); });
		SecondSessionButton.onClick.AddListener(() => { StartCoroutine(GoSecondSession()); });
	
		LiveBlockButton.onClick.AddListener(() => {StartCoroutine(GoLiveSingle()); });
		ReplayBlockButton.onClick.AddListener(() => {StartCoroutine(GoReplaySingle()); });
	}
	
	private void ShowInsturction(){
		Message.text = "";
		Instruction.text = "";

		next_session_id_live = expCtl.GetSessionIdLiveFramedFromFile ();
		next_session_id_black = expCtl.GetSessionIdLiveBlackedFromFile ();
		
		Debug.Log (" live session:" + next_session_id_live + " replay session:" + next_session_id_black);
		if( (next_session_id_live >= num_sessions) && (next_session_id_black >= num_sessions) ){
			DayId = 2;
			
			effective_num_sessions_live = 2 * num_sessions - next_session_id_live;
			effective_num_sessions_black = 2 * num_sessions - next_session_id_black;
		}else{
			DayId = 1;
			
			effective_num_sessions_live = num_sessions - next_session_id_live;
			effective_num_sessions_black = num_sessions - next_session_id_black;
			
		}
		
		expCtl.DayId = DayId;


		informationText.text = "Subject Id : " + SubId + "  Day " + DayId +"\n";
		if(DayId == 1){
			informationText.text += "Live# " + next_session_id_live + "  Black# " + next_session_id_black +"\n";
		}else{
			informationText.text += "Live# " + (next_session_id_live - num_sessions) + "  Black# " + (next_session_id_black - num_sessions) +"\n";
		}

		if (odd_subject ^ (DayId == 1)) {
			FirstSessionButton.GetComponentInChildren<Text>().text = " [3] 1st Session (" + effective_num_sessions_live.ToString() + " Live)" ;
			SecondSessionButton.GetComponentInChildren<Text>().text = " [4] 2nd Session (" +  effective_num_sessions_black.ToString() + " Black)" ;
		}else{
			FirstSessionButton.GetComponentInChildren<Text>().text = " [3] 1st Session (" +  effective_num_sessions_black.ToString() + " Black)" ;
			SecondSessionButton.GetComponentInChildren<Text>().text = " [4] 2nd Session (" +  effective_num_sessions_live.ToString() + " Live)" ;
		}
		SRCtl.GoToInstructionScreen ();

	}
		
	IEnumerator GoTraining(){
		//training
		if(!isRunning){
			isRunning = true;

			controlPanel.SetActive (false);

			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.TRAINING;	
			yield return StartCoroutine (expCtl.runTraining());

			controlPanel.SetActive (true);
			isRunning = false;
			StartCoroutine("ExperimentRun");
		}
	}

	
	IEnumerator GoPractice(){
	

		if(!isRunning && LivePracticeButton.interactable == true){
			isRunning = true;
			
			controlPanel.SetActive (false);
			
			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.PRACTICE;	
			expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE_FRAME;
			yield return StartCoroutine (expCtl.runExperiment ());

			LivePracticeButton.interactable = false;
			controlPanel.SetActive (true);
			isRunning = false;
			StartCoroutine("ExperimentRun");
		}
	}



	IEnumerator GoFirstSession(){
		if(!isRunning){
			isRunning = true;
			controlPanel.SetActive (false);

			int num_actual_sessions = 0;
			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.MAIN;
			if (odd_subject ^ (DayId == 1)) {
				num_actual_sessions = effective_num_sessions_live; 
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE_FRAME;
			}else{
				num_actual_sessions = effective_num_sessions_black; 
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE_BLACK;
			}	

			for(int i=0; i< num_actual_sessions; i++){

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
			
			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.MAIN;

			int num_actual_sessions = 0;
			if (odd_subject ^ (DayId == 1)) {
				num_actual_sessions = effective_num_sessions_black; 
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE_BLACK;
			}else{
				num_actual_sessions = effective_num_sessions_live; 
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE_FRAME;
			}	
			
			for(int i=0; i< num_actual_sessions; i++){
				
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

			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.MAIN;	
			expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE_FRAME;
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
			
			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.MAIN;	
			expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE_BLACK;
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
			yield return StartCoroutine (GoPractice());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha3) {
			yield return StartCoroutine (GoFirstSession());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha4) {
			yield return StartCoroutine (GoSecondSession());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha5) {
			yield return StartCoroutine (GoLiveSingle());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha6)
		{
			yield return StartCoroutine (GoReplaySingle());
			yield break;
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
