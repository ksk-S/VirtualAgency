using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using CommonUtil;


public class ExperimentTimeBisection : ExperimentTimeInterface {
	
	public struct Param2
	{
		public int index;
		public int videoId;
		public int speed;
		public int duration;
		public float start_time;
		
		public float rotation;
		public int movement;
		public float actual_duration;
		
		public float answer_duration;
		public float[] answer_production;
		public int judgement;
		
		public float next_start_time;
		public int next_speed;
		public int next_videoId;
	}
	
	public struct BisectResult
	{
		public int duration;
		public int answer;
		public int correct;
	}

	public enum Task : int
	{
		REPRODUCTION, JUDGEMENT
	} 


	public enum ComparisonMode : int
	{
		Reminder, SingleShot
	}




	float answer_video_duration = 0.0f;

	public  Task task = Task.JUDGEMENT;

	public bool isSkipPreTask = false;
	public bool isSaveTracking = true;


	public  float[] durations = {2.2f, 2.3f, 2.4f, 2.6f, 2.7f, 2.8f};
//	public  float[] durations = {2.3f, 2.4f, 2.6f, 2.7f};

	float start_time_min = 0f;
	float start_time_max = 80f;

	public int sessionId = 0;
	//basic parameters
	public  int num_repetition = 10;
	public  int num_video = 1;
	public  int num_speed = 3;
	public  int num_duration = 6; 
	public  int num_move = 2;
	public  int num_trial_per_block = 20;
	public int num_blocks_for_braek = 3;

	
	public  float timeproduction_duration = 2.5f;

	public float break_sec_min = 0.5f;
	public float break_sec_max = 1.0f;
 
	//training circle reproduction
	public int num_training_circle_reproduction = 2;

	//training circle bisection
	public int num_training_circle_bisection = 10;

	//traing live reproduction
	public int num_training_live_minimal = 0;
	public int num_training_live_correct_answer_ratio_range = 1;
	public float num_training_live_correct_answer_ratio_to_escape = 0.0f;

	//training live bisection
	public  int num_training_bisection_minimal = 0;
	public  int  num_training_bisection_max = 30;
	public int num_comparison_correct_answer_ratio_range = 5;
	public float num_comparison_correct_answer_ratio_to_escape = 0.8f;

	public enum TrainingStage : int
	{
		Reproduction, CompEasy, CompHard, Finished
	} 

	public TrainingStage trainingStage = TrainingStage.Reproduction;

	string saved_contents_training_rep = "";
	string saved_contents_training_easy = "";
	string saved_contents_training_hard = "";

	int count_training_rep = 0;
	int count_training_easy = 0;
	int count_training_hard = 0;

	//not used
	public bool is_main_experiment_reminder = false;
	public  int num_timeproduction = 0;
	
	public int trial_counter = 0;
	public int block_counter = 0;

	//public Param2[] result_list = new Param2[num_repetition * num_video * num_speed * num_repetition];

	Param2[][] result_list ;

	float[] trainingCircleRepList ;


	List<BisectResult> trainingCircleBisectoinList = new List<BisectResult> ();
	List<float> trainingBisectionLiveList = new List<float>();
	List<int> trainingBisectionEasyList = new List<int>();
	List<int> trainingBisectionHardList = new List<int>();

	List<List<HeadTrackingRecorder.HeadTrackData>>[] HeadTrajectory ;


	float last_reproduced_duration;

	bool isRunning = false;

	string data_dir = "Results/Main";
	string tracking_data_dir = "Results/Tracking";

	AudioSource leftAudio;
	AudioSource rightAudio;
	AudioSource stayAudio;

	GameObject redCircle;
	GameObject fixationCross;
	
	string pre_training_text;
	string inst_main_mixed;
	string inst_training_live;
	string inst_training_bisection;
	string inst_training_bisection_levelup;
	
	string inst_reproduction_circle;
	string inst_reproduction_live;
	string inst_reproduction_response;
	
	string inst_main_nomove;
	string inst_main_move;
	string inst_main_judge;
	string inst_response_judge;
	string message_keypressed = "A Key Pressed";

	void CreateCondition()
	{
	
		for(int i=0; i< num_repetition; i++){
			result_list[i] =  new Param2[num_video * num_speed * num_duration * num_move];

			for(int j=0; j<num_video; j++){
				for(int k=0; k<num_speed; k++){
					for(int l=0; l<num_duration; l++){
						for(int m=0; m< num_move; m++){

							//Debug.Log (i + " " + param.index);
							Param2 param = new Param2();
							param.index = j * num_speed * num_duration * num_move + k * num_duration * num_move + l * num_move + m ;
							param.videoId = j;
							param.speed   = k;
							param.duration = l; 
							param.movement = m; 
							param.start_time = UnityEngine.Random.Range(start_time_min, start_time_max);
							param.rotation = UnityEngine.Random.Range(0f, 360f); 

							param.answer_production = new float[num_timeproduction];;

							//Debug.Log (param.index + " "+ param.speed + " " + param.duration);
							result_list[i][param.index] = param;

							//Debug.Log(param.speed);
						}
					}
				}
			}
			Shuffle(result_list[i], num_video * num_speed * num_duration * num_move);
			//Debug.Log ("end of repetition");
		}

		//next start time
		for(int i=0; i< num_repetition; i++){
			for(int j=0; j < num_speed * num_duration * num_move - 1; j++){
				result_list[i][j].next_start_time = result_list[i][j+1].start_time; 
				result_list[i][j].next_speed = result_list[i][j+1].speed;
				result_list[i][j].next_videoId = result_list[i][j+1].videoId;
				
			}
		}

		for(int i=0; i< num_repetition - 1; i++){
			result_list[i][num_speed * num_duration * num_move - 1].next_start_time = result_list[i+1][0].start_time;
			result_list[i][num_speed * num_duration * num_move - 1].next_speed = result_list[i+1][0].speed;
			result_list[i][num_speed * num_duration * num_move - 1].next_videoId = result_list[i+1][0].videoId;
		}

	}
	bool LoadHeadTrackData()
	{
		for(int i=0; i< num_duration; i++)
		{
			HeadTrajectory[i] = new List<List<HeadTrackingRecorder.HeadTrackData>>();
		}

		string dir = tracking_data_dir + "/" + SubId + "/" + "LIVE-MOVE" + "/";
		Debug.Log (dir);
		if (!System.IO.Directory.Exists (dir)) {
			Debug.Log ("LIVE tracking data folder does not exist");
			return false;
		}

		string[] fs = System.IO.Directory.GetFiles (dir, "*", System.IO.SearchOption.TopDirectoryOnly);

		if(fs.Length == 0){
			Debug.Log ("LIVE tracking files do not exist");
			return false;
		}

		foreach( string file in fs )
		{

			List<HeadTrackingRecorder.HeadTrackData> tracking_data = new List<HeadTrackingRecorder.HeadTrackData>();
			//Debug.Log (file);

			FileInfo fi = new FileInfo(file);
			StreamReader sr = new StreamReader(fi.OpenRead());
			while( sr.Peek() != -1 ){
				string line = sr.ReadLine(); 
				string[] line_split = line.Split (',');

				HeadTrackingRecorder.HeadTrackData data = new HeadTrackingRecorder.HeadTrackData();
				data.frame = int.Parse(line_split[0]);
				data.time = float.Parse(line_split[1]);
				data.rotation = new Vector3(float.Parse(line_split[2]), float.Parse(line_split[3]), float.Parse(line_split[4]));
				tracking_data.Add (data);
			}
			sr.Close();
			//Debug.Log (tracking_data[0].ToString());

			//video speed
			string name = System.IO.Path.GetFileNameWithoutExtension (file);
			string[] conditions =  name.Split('-');
			int duration = int.Parse(conditions[5]);
			//Debug.Log (duration);
			HeadTrajectory[duration].Add (tracking_data);
		}
		return true;
		//HeadTrajectory;

	}

	protected override void DoAwake ()
	{	
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();

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

//		AnswerTimeObj = GameObject.Find ("AnswerTime");
//		AnswerTime =  AnswerTimeObj.GetComponent<InputField> ();
		
		redCircle = GameObject.Find ("RedCircle");
		fixationCross = GameObject.Find ("Fixation");

	
		num_duration = durations.Length;

		result_list = new Param2[num_repetition][];
		trainingCircleRepList = new float[num_training_circle_reproduction];

		HeadTrajectory = new List<List<HeadTrackingRecorder.HeadTrackData>>[num_duration];


		leftAudio = GameObject.Find ("AudioLeft").GetComponent<AudioSource> ();
		rightAudio = GameObject.Find ("AudioRight").GetComponent<AudioSource> ();
		stayAudio = GameObject.Find ("AudioStay").GetComponent<AudioSource> ();

		pre_training_text = Resources.Load<TextAsset> ("Texts/TimePerception/pre_training").text;
		inst_training_live = Resources.Load<TextAsset> ("Texts/TimePerception/training_live").text;
		inst_training_bisection = Resources.Load<TextAsset> ("Texts/TimePerception/training_bisection").text;
		inst_training_bisection_levelup = Resources.Load<TextAsset> ("Texts/TimePerception/training_bisection_levelup").text;
		
		inst_reproduction_circle = Resources.Load<TextAsset> ("Texts/TimePerception/reproduction_circle").text;
		inst_reproduction_live = Resources.Load<TextAsset> ("Texts/TimePerception/reproduction_live").text;
		inst_reproduction_response = Resources.Load<TextAsset> ("Texts/TimePerception/reproduction_response").text;
		
		inst_main_mixed = Resources.Load<TextAsset> ("Texts/TimePerception/main_miexed").text;
		inst_main_nomove = Resources.Load<TextAsset> ("Texts/TimePerception/main_nomove").text;
		inst_main_move = Resources.Load<TextAsset> ("Texts/TimePerception/main_move").text;
		inst_main_judge = Resources.Load<TextAsset> ("Texts/TimePerception/main_judge").text;
		inst_response_judge = Resources.Load<TextAsset> ("Texts/TimePerception/response_judge").text;
	}
	
//	void	Start () {
	protected override void DoStart () {
		//AnswerTimeObj.SetActive (false);

		videoCtl = GameObject.Find ("SRController").GetComponent<SRController> ().curVideoCtl;

	}

	public override IEnumerator runExperiment () {
		if (isRunning) {
			Debug.Log ("Experiment has already started");
			yield break;
		}

		isRunning = true;

		Message.text = "";
		Instruction.text = "";

		session_name = Tracking.ToString () + "-" + MoveInstructoin.ToString();



		if(MoveInstructoin == MoveInstructionMode.MIXED)
		{
			num_move = 2;
		}else{
			num_move = 1;
		}
	
		CreateCondition ();

		Debug.Log("Experiment Start : " + session_name);
	

		if(Tracking == TrackingMode.REPLAY)
		{
			if(!LoadHeadTrackData()){
				Debug.Log("Experiment is canceled ");
				Message.text = "Experiment was canceled because the tracking data is absent";
				SRCtl.GoToInstructionScreen ();
				yield return StartCoroutine(WaitForAnyKeyPress());
				yield return StartCoroutine(WaitForAnyKeyPress());

				SRCtl.BackFromInstructionScreen ();
				SRCtl.disableControll = false;

				isRunning = false;
				yield break;
			}
		}

			
		SetFilename (session_name);
		logger.StartLogHeartbeat (session_name, sessionId);

		SaveExperimentParameter ();

		SRCtl.DiableAudio ();
		SRCtl.streamFOV = fov;
		SRCtl.TransitionToRecord();
		SRCtl.GoToInstructionScreen ();
		yield return 0;


		if(MoveInstructoin == MoveInstructionMode.MIXED)
		{
			Instruction.text = inst_main_mixed;
		}else if(MoveInstructoin == MoveInstructionMode.NOMOVE)
		{
			Instruction.text = inst_main_nomove;
		}else{
			Instruction.text = inst_main_move;
		}
		
		yield return StartCoroutine(WaitForAnyKeyPress());

		Instruction.text = inst_main_judge;
		yield return StartCoroutine(WaitForAnyKeyPress());

		yield return StartCoroutine (runMain());

		logger.SaveLog();


		Message.text = "End of the session!\n\n Press any key to continue";
		SRCtl.GoToInstructionScreen ();

		yield return StartCoroutine(WaitForAnyKeyPress());
		SRCtl.BackFromInstructionScreen ();
		SRCtl.disableControll = false;

		//ovr0.5
		//OculusCamRig.IsRotationUpdate = true;

		isRunning = false;

		logger.StopLogHeartbeat ();
	}


	public override IEnumerator runTraining()
	{
		SetFilename ("Training");
		
		Instruction.text = pre_training_text;
		yield return StartCoroutine(WaitForAnyKeyPress());

		if (num_training_circle_reproduction > 0) {
			for (int j=0; j < num_training_circle_reproduction; j++) {
					
				yield return StartCoroutine (runCircleReproduction ());
					
					trainingCircleRepList [j] = last_reproduced_duration;
			}

			SaveFileTrainingCircleReproduction ();
		}

		Instruction.text = inst_training_bisection;
		yield return StartCoroutine(WaitForAnyKeyPress());

		if (num_training_circle_bisection > 0) {
			for (int j=0; j < num_training_circle_bisection; j++) {
				yield return StartCoroutine (runCircleBisection ());
			}
			
			SaveFileTrainingCircleBisection ();
		}

		yield break;
	
	//	yield return StartCoroutine (runTrainingBisectionLevelUp ());
	//	SaveFileTrainingAll();
	}



	private IEnumerator runTrainingBisectionLevelUp()
	{
			do{
			if( num_training_live_minimal > 0 && trainingStage == TrainingStage.Reproduction)
			{
				SRCtl.TransitionToLive();
				SRCtl.GoToInstructionScreen ();

				Instruction.text = inst_training_live;
				yield return StartCoroutine(WaitForAnyKeyPress());	

				yield return StartCoroutine(runTrainingLiveSession());

				SRCtl.TransitionToRecord();
				SRCtl.GoToInstructionScreen ();

			}

			if( num_training_bisection_minimal > 0 &&  trainingStage == TrainingStage.CompEasy)
			{
				SRCtl.TransitionToLive();
				SRCtl.GoToInstructionScreen ();

				Instruction.text = inst_training_bisection;
				yield return StartCoroutine(WaitForAnyKeyPress());

				yield return StartCoroutine(runTrainingBisectionSession(0));
			
				SRCtl.TransitionToRecord();
				SRCtl.GoToInstructionScreen ();

			}

			if( num_training_bisection_minimal > 0 && trainingStage == TrainingStage.CompHard)
			{
				SRCtl.TransitionToLive();
				SRCtl.GoToInstructionScreen ();

				Instruction.text = inst_training_bisection_levelup; 
				yield return StartCoroutine(WaitForAnyKeyPress());
				
				yield return StartCoroutine(runTrainingBisectionSession(1));
				
				SRCtl.TransitionToRecord();
				SRCtl.GoToInstructionScreen ();

			}
		}while(trainingStage != TrainingStage.Finished);

	}

	
	private IEnumerator runCircleReproduction()
	{
		Instruction.text = inst_reproduction_circle;
		yield return StartCoroutine(WaitForAnyKeyPress());

		yield return StartCoroutine(runReproductionPre());

		redCircle.GetComponent<Renderer>().enabled = true;
		
		yield return new WaitForSeconds(timeproduction_duration);

		redCircle.GetComponent<Renderer>().enabled = false;

		yield return StartCoroutine(runReproductionPost(0));
	}


	
	private IEnumerator runTrainingLiveSession()
	{
		float correct_ratio = 0.0f;
		do{
			yield return StartCoroutine(runLiveReproduction());
			
			int correct_answers = 0;
			if(trainingBisectionLiveList.Count >= num_training_live_minimal )
			{
				int counter = 0;
				for(int i=trainingBisectionLiveList.Count-1; i >= trainingBisectionLiveList.Count - num_training_live_correct_answer_ratio_range; i--)
				{
					counter++;
					if(Math.Abs (trainingBisectionLiveList[i] - timeproduction_duration) < 0.1f)
					{
						correct_answers++;
					}
				}
				correct_ratio = (float)correct_answers / (float)counter;
				Debug.Log (correct_answers + "/" + counter + " : " + correct_ratio  + " " +  num_training_live_correct_answer_ratio_to_escape);
			}
		}while( correct_ratio < num_training_live_correct_answer_ratio_to_escape || trainingBisectionLiveList.Count < num_training_live_minimal );
		
		
		trainingStage = TrainingStage.CompEasy;
		StoreTrainingReproduction();
	}

	private IEnumerator runLiveReproduction()
	{
		
		Instruction.text = inst_reproduction_live;
		yield return StartCoroutine(WaitForAnyKeyPress());

		yield return StartCoroutine(runReproductionPre());

		SRCtl.BackFromInstructionScreen ();
	
		yield return new WaitForSeconds(timeproduction_duration);

		SRCtl.GoToInstructionScreen ();
		
		yield return StartCoroutine(runReproductionPost(1));
	}

	private IEnumerator runReproductionPre()
	{
		//show fixation
		SRCtl.GoToInstructionScreen ();
		Instruction.text = "";
		fixationCross.GetComponent<Renderer>().enabled = true;

		float break_seconds = UnityEngine.Random.Range (break_sec_min,break_sec_max);
		yield return new WaitForSeconds(break_seconds);

		fixationCross.GetComponent<Renderer>().enabled = false;
		//sound.isPlaying = true;
	}
	private IEnumerator runReproductionPost(int mode )
	{
		//sound.isPlaying = false;
		//reproduction instruction
		Instruction.text = inst_reproduction_response;
		keyManager.SetKeyboardHook();
		yield return StartCoroutine(WaitForAnyKeyPress());

		//reproduction start
		Instruction.text = "";

		redCircle.GetComponent<Renderer>().enabled = true;
//		Message.text = message_keypressed;
		//sound.isPlaying = true;
		
		yield return StartCoroutine(WaitAnyKeyPressing());

		redCircle.GetComponent<Renderer>().enabled = false;

		last_reproduced_duration = LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown;
		Debug.Log (last_reproduced_duration);

		if(mode == 1)
		{
			trainingBisectionLiveList.Add(last_reproduced_duration);
		}

		if (Math.Abs (last_reproduced_duration - timeproduction_duration) < 0.1f) {
			Message.text = "Your reproduction is very accurate!!";
		}else if (Math.Abs (last_reproduced_duration - timeproduction_duration) < 0.2f) {
			Message.text = "Your reproduction is relatively accurate";

		}else{
			if( last_reproduced_duration > timeproduction_duration)
			{
				Message.text = "Your reproduction is too LONG";
			}else{
				Message.text = "Your reproduction is too SHORT";;
			}
		}
		if(mode == 1)
		{
			Message.text += "\n\n";
		}
		if(trainingBisectionLiveList.Count >0){
			for(int i=0; i < trainingBisectionLiveList.Count; i++)
			{
				if (Math.Abs (trainingBisectionLiveList[i]  - timeproduction_duration) < 0.2f)
				{
					Message.text += "C ";
				}else{
					Message.text += "W " ;

				}
			}
		}
		Message.text += "\n\nPress any key to continue";

		yield return StartCoroutine(WaitForAnyKeyPress());
		Message.text = "";
		//reproduction end
		//sound.isPlaying = false;

		
		keyManager.UnsetKeyboardHook();
	}


	private IEnumerator runCircleBisection ()
	{
				
		int duration_index = UnityEngine.Random.Range (0, 6);
		Debug.Log ("duration_index : " + duration_index);
		int is_duration_longer = duration_index > 2 ? 1 : 0;
		float stimulus_duration = durations[duration_index];

		//show fixation
		SRCtl.GoToInstructionScreen ();
		Instruction.text = "";
		fixationCross.GetComponent<Renderer>().enabled = true;

		float break_seconds = UnityEngine.Random.Range (break_sec_min,break_sec_max);
		yield return new WaitForSeconds(break_seconds);
		
		fixationCross.GetComponent<Renderer>().enabled = false;
		redCircle.GetComponent<Renderer>().enabled = true;

		float time = UnityEngine.Time.timeSinceLevelLoad;
		yield return new WaitForSeconds(stimulus_duration);
		
		Debug.Log (UnityEngine.Time.timeSinceLevelLoad - time);
		redCircle.GetComponent<Renderer>().enabled = false;
		
		Message.text = inst_response_judge;
		yield return StartCoroutine(WaitForKeysDown(KeyCode.UpArrow, KeyCode.DownArrow));
		
		int answer =  pressedKey == KeyCode.UpArrow ? 1 : 0;
		bool correct = !((is_duration_longer == 1) ^ (answer == 1));
		
		//Debug.Log (is_duration_longer + " " + answer + " " + correct);


		BisectResult result = new BisectResult();

		result.duration = duration_index;
		result.answer = answer;
		result.correct = Convert.ToInt32 (correct);

		trainingCircleBisectoinList.Add (result);

		Message.text = correct ? "Your answer is Correct!\n\n" : "Your answer is Wrong\n\n";
		
		if(trainingCircleBisectoinList.Count >0){
			for(int i=0; i < trainingCircleBisectoinList.Count; i++)
			{
				Message.text += trainingCircleBisectoinList[i].correct == 1 ? "C " : "W " ;
			}
		}
		Message.text += "\n\nPress any key to continue";
		yield return StartCoroutine(WaitForAnyKeyPress());
		
		Message.text = "";    
	}

	private IEnumerator runTrainingBisectionSession(int mode)
	{
		List<int> resultList;
		if (mode == 0) {
			resultList = trainingBisectionEasyList;
		}else{
			resultList = trainingBisectionHardList;
		}

		float correct_ratio = 0.0f;
		do{
			if(resultList.Count > num_training_bisection_max)
			{
				Instruction.text = "Back to the previous level";
				yield return StartCoroutine(WaitForAnyKeyPress());

				if (mode == 0) {
					trainingStage = TrainingStage.Reproduction;
					StoreTrainingEasy();

				}else{
					trainingStage = TrainingStage.CompEasy;
					StoreTrainingHard();
				}
				yield break;
			}

			yield return StartCoroutine(runTrainingBisection(mode));

			int correct_answers = 0;
			if(resultList.Count  >= num_training_bisection_minimal )
			{
				int counter = 0;
				for(int i=resultList.Count-1; i >= resultList.Count - num_comparison_correct_answer_ratio_range; i--)
				{
					counter++;
					correct_answers += resultList[i];
				}
				correct_ratio = (float)correct_answers / (float)counter;
				Debug.Log (correct_answers + "/" + counter + " : " + correct_ratio  + " " +  num_comparison_correct_answer_ratio_to_escape);
			}
		}while( correct_ratio < num_comparison_correct_answer_ratio_to_escape || resultList.Count  < num_training_bisection_minimal );

		if (mode == 0) {
			trainingStage = TrainingStage.CompHard;
			StoreTrainingEasy();
		}else{
			trainingStage = TrainingStage.Finished;
			StoreTrainingHard();
		}
		
	}
	
	private IEnumerator runTrainingBisection(int mode)
	{

		List<int> resultList;
		if (mode == 0) {
			resultList = trainingBisectionEasyList;
		}else{
			resultList = trainingBisectionHardList;
		}

		int is_duration_longer = UnityEngine.Random.Range (0, 2);
		float stimulus_duration = 0f;

		//show fixation
		SRCtl.GoToInstructionScreen ();
		Instruction.text = "";
		fixationCross.GetComponent<Renderer>().enabled = true;

		float break_seconds = UnityEngine.Random.Range (break_sec_min,break_sec_max);
		yield return new WaitForSeconds(break_seconds);
		
		fixationCross.GetComponent<Renderer>().enabled = false;
		SRCtl.BackFromInstructionScreen ();

		if(is_duration_longer == 1)
		{
			if(mode == 0)
			{
				stimulus_duration = durations[3]; //longest
			}else{
				stimulus_duration = durations[2]; //longer
			}
		}else{
			if(mode == 0)
			{
				stimulus_duration = durations[0]; //shortest
			}else{
				stimulus_duration = durations[1]; //shorter
			}

		}

		float time = UnityEngine.Time.timeSinceLevelLoad;
		yield return new WaitForSeconds(stimulus_duration);

		Debug.Log (UnityEngine.Time.timeSinceLevelLoad - time);
		SRCtl.GoToInstructionScreen ();

		Message.text = inst_response_judge;
		yield return StartCoroutine(WaitForKeysDown(KeyCode.UpArrow, KeyCode.DownArrow));

		int answer =  pressedKey == KeyCode.UpArrow ? 1 : 0;
		bool correct = !((is_duration_longer == 1) ^ (answer == 1));

		//Debug.Log (is_duration_longer + " " + answer + " " + correct);

		resultList.Add (Convert.ToInt32 (correct));

		Message.text = correct ? "Your answer is Correct!\n\n" : "Your answer is Wrong\n\n";

		if(resultList.Count >0){
			for(int i=0; i < resultList.Count; i++)
			{
				Message.text += resultList[i] == 1 ? "C " : "W " ;
			}
		}
		Message.text += "\n\nPress any key to continue";
		yield return StartCoroutine(WaitForAnyKeyPress());

		Message.text = "";    
//		
	}

	private IEnumerator runMain()
	{
		if (isSaveTracking) {
			Directory.CreateDirectory ( tracking_data_dir + "/" + SubId + "/" + session_name);
		}

		bool lastHeadInstOrientation = false;
	
		Instruction.text = "";
		trial_counter = 0;
		block_counter = 0;

		Param2 first_param = result_list[0][0];
		SRCtl.GoToStream(first_param.videoId * num_speed + first_param.speed);
		if(SRCtl.useDomeVideo)
		{
			videoCtl.Seek(first_param.start_time);
		}
		
		for (int i=0; i< num_repetition; i++) 
		{
			for(int j=0; j < num_speed * num_duration * num_move; j++){

				if(trial_counter % num_trial_per_block == 0)
				{
					if(block_counter != 0 && block_counter % num_blocks_for_braek == 0)
					{
						Message.text = "Take off the head set and take a braek\n\nPress any key to restart";
						yield return StartCoroutine(WaitForAnyKeyPress());
						Message.text = "";
					}

					if(is_main_experiment_reminder)
					{
						Instruction.text = inst_reproduction_circle;
						yield return StartCoroutine(WaitForAnyKeyPress());
						yield return StartCoroutine(runCircleReproduction());
					}

					Message.text = "Block " + (++block_counter) + "\n" + "Press Any Key to Start";
					Debug.Log ("************************");
					Debug.Log ("** Block " + block_counter + "**");
					Debug.Log ("************************");
					yield return StartCoroutine(WaitForAnyKeyPress());
					Message.text = "";

				}
				//*******************************
				//  Fixation Periods (0.5s)
				//*******************************
				Param2 param = result_list[i][j];
				Message.text = "";
				fixationCross.GetComponent<Renderer>().enabled = true;

				//instruction
				if(MoveInstructoin == MoveInstructionMode.MOVE)
				{
					//AudioSource audio = UnityEngine.Random.Range (0,2) == 1 ? leftAudio : rightAudio;

					AudioSource audio = (trial_counter % 2 == 0) ? leftAudio : rightAudio;
					audio.Play ();
				}else if(MoveInstructoin == MoveInstructionMode.MIXED)
				{
					AudioSource audio;
					if(param.movement  == 0 )
					{
						audio = stayAudio;
					}else{
						audio = lastHeadInstOrientation ? leftAudio : rightAudio;
						lastHeadInstOrientation = !lastHeadInstOrientation;
					}
					audio.Play ();
				}


                //kskk
				//OculusCamRig.transform.localRotation = Quaternion.AngleAxis(param.rotation, Vector3.up);

				videoCtl.SetForeGround();
				float break_seconds = UnityEngine.Random.Range (break_sec_min, break_sec_max);
				yield return new WaitForSeconds(break_seconds);

				//*******************************
				//  Show Stimulus
				//*******************************
				fixationCross.GetComponent<Renderer>().enabled = false;

				if(Tracking == TrackingMode.REPLAY)
				{
					int num_trajectory = HeadTrajectory[result_list[i][j].duration].Count;
					List<HeadTrackingRecorder.HeadTrackData> trajectory = HeadTrajectory[result_list[i][j].duration][UnityEngine.Random.Range(0, num_trajectory-1)];
					HeadTracker.SetTrajectoryForReplay(trajectory); 
					HeadTracker.StartHeadReplay(); 

					SRCtl.BackFromInstructionScreen (true);

				}else if(Tracking == TrackingMode.FIXED)
				{
					SRCtl.BackFromInstructionScreen (true);
				}else{
					SRCtl.BackFromInstructionScreen (false);
				}


				if(isSaveTracking) { HeadTracker.StartHeadRecording(); }

				float time = UnityEngine.Time.timeSinceLevelLoad;
				//	Debug.Log (i + " index:" + +result_list[i][j].index + " video:" +result_list[i][j].videoId + " speed:" +result_list[i][j].speed + " duration:" +result_list[i][j].duration);

				logger.SetLog("onset", trial_counter, result_list[i][j].speed, result_list[i][j].duration);

				yield return new WaitForSeconds(durations[result_list[i][j].duration]);

				result_list[i][j].actual_duration = UnityEngine.Time.timeSinceLevelLoad - time;
			
				//goto next video 
				SRCtl.GoToStream(param.next_videoId * num_speed + param.next_speed);
				if(SRCtl.useDomeVideo)
				{
					videoCtl.Seek(param.next_start_time);
				}
				videoCtl.SetBackGround();

				if(isSaveTracking) { HeadTracker.StopHeadRecording(); }
				if(Tracking == TrackingMode.REPLAY)
				{
					HeadTracker.StopHeadReplay();
				}

				//*******************************
				//  Time duration estimation
				//*******************************
				SRCtl.GoToInstructionScreen ();
			
				logger.SetLog("offset", trial_counter, result_list[i][j].speed, result_list[i][j].duration);

				if(task == Task.REPRODUCTION){

					for(int k=0; k< num_timeproduction; k++){

						Instruction.text = inst_reproduction_response;         
						keyManager.SetKeyboardHook();
						yield return StartCoroutine(WaitForAnyKeyPress());
				
						Instruction.text = "";
						Message.text = message_keypressed;
						sound.isPlaying = true;
						yield return StartCoroutine(WaitForAnyKeyPress());

						Message.text = "";
						sound.isPlaying = false;
						result_list[i][j].answer_production[k] = LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown;
						//Debug.Log ((LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown));
						keyManager.UnsetKeyboardHook();
					}
				}else{
					Message.text = inst_response_judge;
					yield return StartCoroutine(WaitForKeysDown(KeyCode.UpArrow, KeyCode.DownArrow));

					result_list[i][j].judgement = pressedKey == KeyCode.DownArrow ? 1 : 0;
					Message.text = "";    
				}
				SaveFile(result_list[i][j], block_counter);

				logger.SetLog("response", trial_counter, result_list[i][j].speed, result_list[i][j].duration);

				if(isSaveTracking){ SaveTrackingFile(result_list[i][j], i, j); }

		

				trial_counter++;
			}
		}
	}

	public void SetFilename(string mode){

		Directory.CreateDirectory (data_dir + "/" + SubId + "/" );
		DateTime dt = DateTime.Now;
		out_filename  = string.Format(data_dir + "/" + SubId + "/sub{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}.txt", SubId, mode, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
	}

	void SaveExperimentParameter()
	{
		string saved_contents = "#";
		saved_contents += "Task=" + task.ToString () + ";";
		saved_contents += "Tracking=" + Tracking.ToString () +  ";";
		saved_contents += "HeadMove=" + MoveInstructoin.ToString() +  ";";

		saved_contents += "Durations=";
		for(int i=0; i< durations.Length; i++)
		{
			saved_contents += durations[i];

			if(i == durations.Length -1)
				saved_contents += ";";
			else
				saved_contents += ",";
		}
		/*
		saved_contents += "Speeds=";
		for(int i=0; i< speeds.Length; i++)
		{
			saved_contents += speeds[i];
			if(i == speeds.Length -1)
				saved_contents += ";";
			else
				saved_contents += ",";
		}
		*/

		saved_contents += "NumRepeat=" + num_repetition +  ";";
		saved_contents += "NumVideo=" + num_video +  ";";
		saved_contents += "TimeProductionDuration=" + timeproduction_duration +  ";";

		saved_contents += "NumTrialPerBlock=" + num_trial_per_block +  ";";
		saved_contents += "NumCircleReproduction=" + num_training_circle_reproduction +  ";";
		saved_contents += "NumCircleBisection=" + num_training_circle_bisection +  ";";

		saved_contents += "NumLiveBisection=" + num_training_live_minimal +  ";";
		saved_contents += "NumTrainingBisectionMinimal=" + num_training_bisection_minimal +  ";";
		saved_contents += "NumCorrectAnswerRatioRange=" + num_comparison_correct_answer_ratio_range +  ";";
		saved_contents += "NumCorrectAnswerRatioToEscape=" + num_comparison_correct_answer_ratio_to_escape +  ";";
		saved_contents +=   Environment.NewLine;

		File.AppendAllText(out_filename, saved_contents);
	}

	void SaveFileTrainingCircleReproduction(){
		string saved_contents = "#CircleRep" + Environment.NewLine;
		for (int i=0; i< num_training_circle_reproduction; i++) {
			saved_contents += trainingCircleRepList [i] + Environment.NewLine;
		}
		saved_contents += Environment.NewLine;

		try{
			File.AppendAllText(out_filename, saved_contents);
		}catch{

		}
	}

	void SaveFileTrainingCircleBisection(){
		string saved_contents = "#CircleBisection" + Environment.NewLine;
		for (int i=0; i< num_training_circle_bisection; i++) {
			BisectResult b = trainingCircleBisectoinList [i];
			saved_contents += b.duration +", "+ b.answer + Environment.NewLine;
		}
		saved_contents += Environment.NewLine;
		try{
			File.AppendAllText(out_filename, saved_contents);
		}catch{

		}
	}

	void StoreTrainingReproduction(){
		saved_contents_training_rep += "Live" + (++count_training_rep) + ":" ;
		for (int i=0; i< trainingBisectionLiveList.Count; i++) {
			saved_contents_training_rep += trainingBisectionLiveList[i] + ", ";
		}
		saved_contents_training_rep +=   ";";
		trainingBisectionLiveList.Clear ();
	}

	void StoreTrainingEasy(){
		saved_contents_training_easy += "Easy"+ (++count_training_easy) + ":" ;
		for (int i=0; i< trainingBisectionEasyList.Count; i++) {
			saved_contents_training_easy += trainingBisectionEasyList[i] + ", ";
		}
		saved_contents_training_easy +=   ";";
		trainingBisectionEasyList.Clear ();
	}

	void StoreTrainingHard(){
		saved_contents_training_hard += "Hard"+ (++count_training_hard) + ":" ;
		for (int i=0; i< trainingBisectionHardList.Count; i++) {
			saved_contents_training_hard += trainingBisectionHardList[i] + ", ";
		}
		saved_contents_training_hard +=   ";";
		trainingBisectionHardList.Clear ();
	}

	void SaveFileTrainingAll(){
		string saved_contents = "";
		saved_contents += saved_contents_training_rep + Environment.NewLine;
		saved_contents += saved_contents_training_easy +  Environment.NewLine;
		saved_contents += saved_contents_training_hard +  Environment.NewLine;
		File.AppendAllText(out_filename, saved_contents);
	}


	void SaveFile(Param2 result, int block)
	{
		string saved_contents = "";
		saved_contents += block + ", ";
		saved_contents += result.index + ", ";
		saved_contents += result.videoId + ", ";
		saved_contents += result.speed + ", ";
		saved_contents += result.duration + ", ";
		saved_contents += result.movement + ", ";
		saved_contents += result.rotation + ", ";

		if(task == Task.REPRODUCTION){
			for(int i=0; i< num_timeproduction; i++){
				saved_contents += result.answer_production[i] + ",";
			}
		}else{
			saved_contents += result.judgement + ", ";	
		}

		saved_contents += result.actual_duration + ""; 

		saved_contents +=   Environment.NewLine;
		File.AppendAllText(out_filename, saved_contents);
	}

	void SaveTrackingFile(Param2 result, int block, int trial)
	{
		string saved_contents = "";
		foreach (HeadTrackingRecorder.HeadTrackData data in HeadTracker.GetRecordedTrajectory()) 
		{
			saved_contents += data.frame + "," + data.time + "," +  + data.rotation.x + "," + data.rotation.y + "," + data.rotation.z + Environment.NewLine;
		}
		string filename = "head-" + block + "-" + trial + "-" + result.videoId + "-" + result.speed + "-" + result.duration + "-" + HeadTracker.GetRecordedTrajectory().Count +".txt";
		File.AppendAllText(tracking_data_dir + "/" + SubId + "/" + session_name + "/" + filename , saved_contents);
	}

	protected override void DoUpdate()
	{

		if (Input.GetKey(KeyCode.UpArrow))
		{
			answer_video_duration += 0.1f;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			answer_video_duration -= 0.1f;
			if(answer_video_duration <= 0.0f)
			{
				answer_video_duration = 0.0f;
			}
		}

		float rawVertical = Input.GetAxisRaw("VerticalLStick");
		answer_video_duration += rawVertical;

		//AnswerTime.text = string.Format ("{0,3:F1}", answer_video_duration);

	}

	public void Shuffle(Param2[] ary, int num_ary)
	{
		int n=num_ary;
		while (n > 1)
		{
			n--;
			int k = Rn.Next(n + 1);
			Param2 tmp = ary[k];
			ary[k] = ary[n];
			ary[n] = tmp;
		}
		
	}
}
