using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonUtil;


public class ExperimentTimeBisectionManager : MonoBehaviour {

	
	SRController SRCtl;
	ExperimentTimeInterface expCtl;
	Text Instruction;
	Text Message;
	INIFile iniFileCtl;

	public string config_file = "config.ini";

	bool isRunExperimentManager = false;
	bool button_pressed = false;
	KeyCode pressedKey = KeyCode.Alpha0;

	string text_training = "[1] Start Training\n";
	string initial_text_odd = "[2] First Session (Move, Nomove)\n" +
							  "[3] Second Session (Nomove, Move)\n";

	string initial_text_even = "[2] First Session (Nomove, Move)\n" +
							   "[3] Second Session (Move, Nomove)\n";
	
	string initial_text_individual = "[4] Start \"Live - Move\"\n" +
				"[5] Start \"Live - Nomove\"\n" +
				"[6] Start \"Fixed - Move\"\n" +
				"[7] Start \"Fixed - Nomove\"\n" +
				"[8] Start \"Replay - Move\"\n" +
				"[9] Start \"Replay - Nomove\"\n";
			
			public int SubId = 1;
	bool odd_subject = false;

	void Awake ()
	{	
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		expCtl = gameObject.GetComponent<ExperimentTimeBisection> ();

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
		odd_subject = SubId % 2 != 0 ? false : true;

		SRCtl.disableControll = true;


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

	private IEnumerator Selector ()
	{
		if (pressedKey == KeyCode.Alpha1)
		{
			yield return StartCoroutine (expCtl.runTraining());
			yield break;
		}

		if (pressedKey == KeyCode.Alpha2)
		{
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.LIVE;
			if(odd_subject)
			{
				expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MOVE;
				yield return StartCoroutine (expCtl.runExperiment());

				expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.NOMOVE;	
				yield return StartCoroutine (expCtl.runExperiment());

			}else{
				expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.NOMOVE;
				yield return StartCoroutine (expCtl.runExperiment());
				
				expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MOVE;	
				yield return StartCoroutine (expCtl.runExperiment());
			}
			yield break;
		}

		if (pressedKey == KeyCode.Alpha3)
		{
			
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.LIVE;
			if(odd_subject)
			{	
				expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.NOMOVE;
				yield return StartCoroutine (expCtl.runExperiment());
				
				expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MOVE;	
				yield return StartCoroutine (expCtl.runExperiment());

			}else{
				expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MOVE;
				yield return StartCoroutine (expCtl.runExperiment());
				
				expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.NOMOVE;	
				yield return StartCoroutine (expCtl.runExperiment());
			}
		}



		
		if (pressedKey == KeyCode.Alpha4)
		{
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.LIVE;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MOVE;
			
			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha5)
		{
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.LIVE;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.NOMOVE;
		
			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}
		
		
		if (pressedKey == KeyCode.Alpha6)
		{
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.FIXED;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MOVE;

			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha7)
		{
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.FIXED;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.NOMOVE;
			
			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha8)
		{			
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.REPLAY;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MOVE;
			
			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}
		
		if (pressedKey == KeyCode.Alpha9)
		{			
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.REPLAY;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.NOMOVE;
			
			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}
		
		
		if (pressedKey == KeyCode.I)
		{
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.LIVE;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MIXED;

			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}
		
		if (pressedKey == KeyCode.K)
		{
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.FIXED;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MIXED;
			
			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
		}

		if (pressedKey == KeyCode.Comma)
		{			
			expCtl.Tracking = ExperimentTimeInterface.TrackingMode.REPLAY;
			expCtl.MoveInstructoin = ExperimentTimeInterface.MoveInstructionMode.MIXED;
			
			yield return StartCoroutine (expCtl.runExperiment());
			yield break;
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
	private IEnumerator WaitForAnyKeyPress ()
	{
		while (!Input.anyKey || !button_pressed ) {
			yield return new WaitForEndOfFrame();
		}

		if(Input.GetKey (KeyCode.Alpha1))
		{
			pressedKey = KeyCode.Alpha1;
		}else if(Input.GetKey (KeyCode.Alpha2))
		{
			pressedKey = KeyCode.Alpha2;
		}else if(Input.GetKey (KeyCode.Alpha3))
		{
			pressedKey = KeyCode.Alpha3;
		}else if(Input.GetKey (KeyCode.Alpha4))
		{
			pressedKey = KeyCode.Alpha4;
		}else if(Input.GetKey (KeyCode.Alpha5))
		{
			pressedKey = KeyCode.Alpha5;
		}else if(Input.GetKey (KeyCode.Alpha6))
		{
			pressedKey = KeyCode.Alpha6;
		}else if(Input.GetKey (KeyCode.Alpha7))
		{
			pressedKey = KeyCode.Alpha7;
		}else if(Input.GetKey (KeyCode.Alpha8))
		{
			pressedKey = KeyCode.Alpha8;
		}else if(Input.GetKey (KeyCode.Alpha9))
		{
			pressedKey = KeyCode.Alpha9;
		}else{
			pressedKey = KeyCode.Alpha0;
		}

		yield return 0;
	}
	
	private IEnumerator WaitForKeyDown(KeyCode keyCode)
	{
		while(!Input.GetKey (keyCode) || !button_pressed){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
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


	


	}
}
