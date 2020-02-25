using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonUtil;


public class ExperimentMagnitudeManager : MonoBehaviour {
	
	SRController SRCtl;
	ExperimentMagnitude expCtl;
	Text Instruction;
	Text Message;
	INIFile iniFileCtl;

	public string config_file = "config.ini";

	bool isRunExperimentManager = false;
	bool button_pressed = false;
	KeyCode pressedKey = KeyCode.Alpha0;
    /*
	string text_training    = "[1] Training\n"
							+ "[2] Live Practice\n"
							+ "[3] Replay Practice\n";
	
	string initial_text_odd = "[4] First Session (# Live Blocks)\n" +
							  "[5] Second Session (# Replay Blocks)\n";

	string initial_text_even = "[4] First Session (# Replay Blocks)\n" +
								"[5] Second Session (# Live Blocks)\n";
	*/
    // string initial_text_individual = "[6] Start Live Alone\"\n" +
    //								 "[7] Start Replay Alone\"\n";
    public int SubId = 100;
	public int DayId = 0;

	public int num_sessions = 3;

	public int next_session_id_live = 1;
	public int next_session_id_replay = 1;

	bool odd_subject = false;

	public GameObject controlPanel;
	public Text informationText;
	public Button TrainingButton;
	public Button LivePracticeButton;
	public Button ReplayPracticeButton;
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
		ReplayPracticeButton = GameObject.Find ("ReplayPracticeButton").GetComponent<Button> ();
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
			SubId = int.Parse (iniFileCtl ["TimePerception", "SubId"]);
			expCtl.SubId = SubId;
		}catch{
			Debug.Log ("SubId is not numerical in the Init file ");
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

		odd_subject = SubId % 2 != 0 ? true : false;

		if(SubId != 1){
			expCtl.TrajectorySubId = SubId -1;
		}else{
			SubId = 1;
		}


		next_session_id_live = expCtl.GetSessionIdLiveFromFile ();
		next_session_id_replay = expCtl.GetSessionIdReplayFromFile ();
			
		Debug.Log (" live session:" + next_session_id_live + " replay session:" + next_session_id_replay);
		if( (next_session_id_live >= num_sessions) && (next_session_id_replay >= num_sessions) ){
			DayId = 2;
		}else{
			DayId = 1;
		}

		expCtl.DayId = DayId;

		
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
		LivePracticeButton.onClick.AddListener(() => { StartCoroutine(GoLivePractice()); });
		ReplayPracticeButton.onClick.AddListener(() => { StartCoroutine(GoReplayPractice()); });

		FirstSessionButton.onClick.AddListener(() => { StartCoroutine(GoFirstSession()); });
		SecondSessionButton.onClick.AddListener(() => { StartCoroutine(GoSecondSession()); });
	
		LiveBlockButton.onClick.AddListener(() => {StartCoroutine(GoLiveSingle()); });
		ReplayBlockButton.onClick.AddListener(() => {StartCoroutine(GoReplaySingle()); });
	}
	
	private void ShowInsturction(){
		Message.text = "";
		Instruction.text = "";

		informationText.text = "Subject Id : " + SubId + "  Day " + DayId +"\n";
		if(DayId == 1){
			informationText.text += "Live# " + next_session_id_live + "  Replay# " + next_session_id_replay +"\n";
		}else{
			informationText.text += "Live# " + (next_session_id_live - num_sessions) + "  Replay# " + (next_session_id_replay - num_sessions) +"\n";
		}

		if (odd_subject ^ (DayId == 1)) {
			FirstSessionButton.GetComponentInChildren<Text>().text = " [4] 1st Session (" + num_sessions.ToString() + " Live Blocks)" ;
			SecondSessionButton.GetComponentInChildren<Text>().text = " [5] 2nd Session (" + num_sessions.ToString() + " Replay Blocks)" ;
		}else{
			FirstSessionButton.GetComponentInChildren<Text>().text = " [4] 1st Session (" + num_sessions.ToString() + " Replay Blocks)" ;
			SecondSessionButton.GetComponentInChildren<Text>().text = " [5] 2nd Session (" + num_sessions.ToString() + " Live Blocks)" ;
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
	IEnumerator GoLivePractice(){
		//training
		if(!isRunning && LivePracticeButton.interactable == true){
			isRunning = true;
			
			controlPanel.SetActive (false);
			
			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.PRACTICE;	
			expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
			yield return StartCoroutine (expCtl.runExperiment ());

			LivePracticeButton.interactable = false;
			controlPanel.SetActive (true);
			isRunning = false;
			StartCoroutine("ExperimentRun");
		}
	}

	IEnumerator GoReplayPractice(){
		//training
		if(!isRunning && ReplayPracticeButton.interactable == true){
			isRunning = true;
			
			controlPanel.SetActive (false);
			
			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.PRACTICE;	
			expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
			yield return StartCoroutine (expCtl.runExperiment ());

			ReplayPracticeButton.interactable = false;
			controlPanel.SetActive (true);
			isRunning = false;
			StartCoroutine("ExperimentRun");
		}
	}


	IEnumerator GoFirstSession(){
		if(!isRunning){
			isRunning = true;
			controlPanel.SetActive (false);

			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.MAIN;
			if (odd_subject ^ (DayId == 1)) {
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
			}else{
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
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
			
			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.MAIN;
			if (odd_subject ^ (DayId == 1)) {
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
			}else{
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
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

			expCtl.ExMode = ExperimentMagnitude.ExperimentMode.MAIN;	
			expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
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
			expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
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
			yield return StartCoroutine (GoLivePractice());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha3) {
			yield return StartCoroutine (GoReplayPractice());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha4) {
			yield return StartCoroutine (GoFirstSession());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha5) {
			yield return StartCoroutine (GoSecondSession());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha6)
		{
			yield return StartCoroutine (GoLiveSingle());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha7)
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
