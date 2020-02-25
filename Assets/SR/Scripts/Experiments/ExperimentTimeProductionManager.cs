using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonUtil;


public class ExperimentTimeProductionManager : MonoBehaviour {

	
	SRController SRCtl;
	ExperimentMagnitude expCtl;
	Text Instruction;
	Text Message;
	INIFile iniFileCtl;

	public string config_file = "config.ini";

	bool isRunExperimentManager = false;
	bool button_pressed = false;
	KeyCode pressedKey = KeyCode.Alpha0;

	string text_training = "[1] Start Training\n";
	string initial_text_odd = "[2] First Session (3 Live Blocks)\n" +
							  "[3] Second Session (3 Replay Blocks)\n";

	string initial_text_even = "[2] First Session (3 Replay Blocks)\n" +
								"[3] Second Session (3 Live Blocks)\n";
	string initial_text_individual = "[4] Start Live Alone\"\n" +
									 "[5] Start Replay Alone\"\n";
	public int SubId = 1;
	bool odd_subject = false;

	//public bool isUpdateSubIdAfterFinished = true;

	void Awake ()
	{	
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		expCtl = gameObject.GetComponent<ExperimentMagnitude> ();

		Instruction = GameObject.Find ("InstructionText").GetComponent<Text> ();
		Message = GameObject.Find ("MessageText").GetComponent<Text> ();

	}
	
	void	Start () {
	}


	IEnumerator ExperimentManangerInit () {


		string initfile = Application.dataPath + "/" + config_file;
		Debug.Log (initfile);
		iniFileCtl = new INIFile (initfile);

		try{
			SubId = int.Parse (iniFileCtl ["TimePerception", "SubId"]);
		}catch{
			Debug.Log ("SubId is not numerical in the Init file ");
		}



		expCtl.SubId = SubId;
		odd_subject = SubId % 2 != 0 ? true : false;

		if(SubId != 1){
			expCtl.TrajectorySubId = SubId -1;
		}else{
			SubId = 1;
		}
		
		SRCtl.disableControll = true;
		SRCtl.DiableAudio ();
		SRCtl.TransitionToRecord();

		yield return StartCoroutine("ExperimentRun");
		yield return 0;

	}
	private IEnumerator ExperimentRun ()
	{
		while(true)
		{
			Debug.Log ("Loop in Experiment Manager");
			ShowInsturction ();

			yield return StartCoroutine("WaitForAnyKeyPress");
			yield return StartCoroutine ("Selector");
		}
	}

	
	private void ShowInsturction(){
		Message.text = "";
		
		Instruction.text = "Subject Id : " + SubId + "\n";
		
		Instruction.text += text_training;
		if (odd_subject) {
			Instruction.text += initial_text_odd;
		}else{
			Instruction.text += initial_text_even;
		}
		Instruction.text += initial_text_individual;
		SRCtl.GoToInstructionScreen ();
		
	}

	private IEnumerator Selector ()
	{
		UnityEngine.Debug.Log (pressedKey);
		if (pressedKey == KeyCode.Alpha1) {

			expCtl.Tracking = ExperimentMagnitude.TrackingMode.PRACTICE;
			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha2) {
			if (odd_subject) {
						
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
				yield return StartCoroutine (expCtl.runExperiment ());

				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
				yield return StartCoroutine (expCtl.runExperiment ());

				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
				yield return StartCoroutine (expCtl.runExperiment ());
				
			} else {

				expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
				yield return StartCoroutine (expCtl.runExperiment ());

				expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
				yield return StartCoroutine (expCtl.runExperiment ());

				expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
				yield return StartCoroutine (expCtl.runExperiment ());
			}
			yield break;
		}

		if (pressedKey == KeyCode.Alpha3) {

			if (odd_subject) {	
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
				yield return StartCoroutine (expCtl.runExperiment ());

				expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
				yield return StartCoroutine (expCtl.runExperiment ());

				expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
				yield return StartCoroutine (expCtl.runExperiment ());

			} else {
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
				yield return StartCoroutine (expCtl.runExperiment ());
		
				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
				yield return StartCoroutine (expCtl.runExperiment ());

				expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
				yield return StartCoroutine (expCtl.runExperiment ());
			}
		}

		if (pressedKey == KeyCode.Alpha4)
		{

			expCtl.Tracking = ExperimentMagnitude.TrackingMode.LIVE;
			yield return StartCoroutine (expCtl.runExperiment ());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha5)
		{	
			expCtl.Tracking = ExperimentMagnitude.TrackingMode.REPLAY;
			yield return StartCoroutine (expCtl.runExperiment ());
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
		}else{
			pressedKey = KeyCode.Alpha0;
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
			if(!isRunExperimentManager){
				StartCoroutine("ExperimentManangerInit");
				isRunExperimentManager = true;
			}
		}

		if(Input.GetKeyDown (KeyCode.Escape)){
			StopCoroutine("ExperimentRun");
			isRunExperimentManager = false;
			SRCtl.disableControll = false;
			SRCtl.BackFromInstructionScreen ();

		}
	}
}
