using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonUtil;


public class ExperimentMagnitude : ExperimentTimeInterface {

	public struct Param
	{
		public int index;
		public int videoId;
		public int speed;
		public int duration;

		public Vector2 orientation;
		public Vector2 patch_displacement;

		public float break_seconds;

		public long actual_patch_onset;
		public long actual_patch_offset;

		public long answer_onset;
		public long answer_offset;
		public long answer_reproduction;

		public float answer_magnitude;
	}

	public struct VideoRangeAlpha{
		public static float normal   = 0f;
		public static float transit  = 120f;
		public static float altered	 = 130f;
		public static float end 	 = 370f;
	}

	public struct VideoRangeBeta{
		public static float altered  = 0f;
		public static float transit  = 240f;
		public static float normal	 = 250f;
		public static float end 	 = 370f;
	}

	int[][] videoOrder = new int[][] 
	{
		new int[] { 0, 1, 2 },
		new int[] { 0, 2, 1 },
		new int[] { 1, 0, 2 },
		new int[] { 1, 2, 0 },
		new int[] { 2, 0, 1 },
		new int[] { 2, 1, 0 }
	};
   

	List<int[]> videoSelectorList = new List<int[]> ();

	//int[,] videoSelecter = new int[4, 4] { { 0, 3, 2, 1 }, { 2, 1, 0, 3 }, { 1, 2, 3, 0}, { 3, 0, 1, 2 } };


	/*
	public struct VideoRange{
		public static float normal1  = 0f;
		public static float transit1 = 215f;
		public static float fast1	 = 235f;
		public static float transit2 = 440f;
	}
	*/
	/*
	public struct VideoRange{

		public static float normal1  = 0f;
		public static float transit1 = 85f;
		public static float fast1	 = 95f;
		public static float transit2 = 255f;
		public static float slow1 =    265f;
		public static float transit3 = 445f;
		public static float normal2 =  455f;
	}
	*/
	GameObject instCanvas;
	GameObject SliderPanel;

	PatchManager patchCtl;
	sliderController sliderCtl;

	string inst_main_start_live1;
	string inst_main_start_live2;
	string inst_main_start_replay1;
	string inst_main_start_replay2;

	string pre_training_text;

	string question_duration_text;
	string question_presence_text;
	string question_attention_text;
	string question_mood_text;
	string question_nauseous_text;
	string question_speed_text;


	//basic parameters
	public bool IsVideoSpeedFixed = false;
	public  float[] durations = {1.5f, 1.75f, 2.0f};

	public float break_sec_min = 3.0f;
	public float break_sec_max = 6.0f;
	public float keypress_expired_seconds = 1.5f;
	
	public int num_training_session = 20;

	public float practice_duration = 60f;

	public float trainig_interval_max = 3.0f;
	public float trainig_interval_min = 1.0f;

	public float training_break_sec_min = 0.5f;
	public float training_break_sec_max = 1.0f;


	public Vector2 patch_range_x = new Vector2(10f, 25f);
	public Vector2 patch_range_y = new Vector2(0f, 20f);

	public float video_duration = 300f;

	public  int num_duration = 2;  //may increase later
	public  int num_video = 1;     //may increase later (normal -> slow -> fast or normal -> fast -> slow etc)

	public  int num_repetition = 40;
	public  int num_random_repetition = 4;

	int num_actual_trials = 0;

	public  int num_trial;

	public float initialRotation;

	public int DayId = 0;
	public int sessionId = 0;
	public int num_session = 3;

	public int curVideoId = 0;

	List<List<Param>> loaded_condition_lists = new List<List<Param>>();
	List<Param> loaded_condition_list = new List<Param>();

	List<Param> condition_list = new List<Param>();

	List<Param> result_list = new List<Param>();

	List<List<HeadTrackingRecorder.HeadTrackData>> HeadTrajectory = new List<List<HeadTrackingRecorder.HeadTrackData>>();

	bool isRunning = false;

	private float timeleft;

	public string result_dir = "Results/";

	string data_dir = "Main/";
	string tracking_data_dir = "Tracking/";
	string questionnaire_data_dir = "Questionnaire/";

	int[] initialRotationRandomizer = {1, 4, 0, 3, 5, 2, 4, 0, 5, 3, 2, 1};

	public bool isChangeRedAfterClick = true;
	public float patch_color_level = 0.25f;

	GameObject redCircle;
	GameObject fixationCross;

	protected System.Random FixRn = new System.Random(1);

	double[] training_show;
	double[] training_answer;

	HeartBeat heartbeat;

	public bool pulsesensorcheck = false;

	public float CalibFrameRatioX = 0.8f;
	public float CalibFrameRatioY = 0.65f;

	protected override void DoAwake ()
	{	
		num_duration = durations.Length;


		patchCtl = GameObject.Find ("PatchController").GetComponent<PatchManager>();
		sliderCtl = GameObject.Find ("SliderController").GetComponent<sliderController> ();
	
		inst_main_start_live1 = Resources.Load<TextAsset> ("Texts/TimeMagnitude/main_start_live1").text;
		inst_main_start_live2 = Resources.Load<TextAsset> ("Texts/TimeMagnitude/main_start_live2").text;
		inst_main_start_replay1 = Resources.Load<TextAsset> ("Texts/TimeMagnitude/main_start_replay1").text;
		inst_main_start_replay2 = Resources.Load<TextAsset> ("Texts/TimeMagnitude/main_start_replay2").text;

		pre_training_text = Resources.Load<TextAsset> ("Texts/TimeMagnitude/pre_training").text;

		question_duration_text = Resources.Load<TextAsset> ("Texts/TimeMagnitude/question_duration_text").text;
		question_speed_text = Resources.Load<TextAsset> ("Texts/TimeMagnitude/question_speed_text").text;
		question_presence_text = Resources.Load<TextAsset> ("Texts/TimeMagnitude/question_presence_text").text;
		question_attention_text = Resources.Load<TextAsset> ("Texts/TimeMagnitude/question_attention_text").text; 
		question_mood_text = Resources.Load<TextAsset> ("Texts/TimeMagnitude/question_mood_text").text;
		question_nauseous_text = Resources.Load<TextAsset> ("Texts/TimeMagnitude/question_nauseous_text").text;
		
		heartbeat = GameObject.Find("CompositionEngine").GetComponent<HeartBeat>();			

		redCircle = GameObject.Find ("RedCircle");
		fixationCross = GameObject.Find ("Fixation");

		instCanvas = GameObject.Find ("InstructionCanvas");
		SliderPanel = GameObject.Find ("SliderPanel");
	}

	void Start () {

		videoCtl = GameObject.Find ("SRController").GetComponent<SRController> ().curVideoCtl;

	}
	void CreateCondition()
	{
		condition_list.Clear ();

		int num_random_repetition = num_duration * 2;

		for(int i=0; i< num_repetition; i++){

			int[] duration_array = new int[num_random_repetition];
			for(int k=0; k<2; k++){
				for(int m=0; m< num_duration; m++)
				{
					duration_array[m +  k * num_duration] = m;
				}
			}

			Shuffle (duration_array);

			for(int j=0; j<num_random_repetition; j++){

				Param param = new Param();
				param.duration = duration_array[j];
//				Debug.Log (param.duration);
				param.break_seconds = UnityEngine.Random.Range (break_sec_min, break_sec_max);

				int latelal_x = UnityEngine.Random.Range(0,2);

				if(latelal_x == 0){
					param.patch_displacement.x = UnityEngine.Random.Range (patch_range_x.x, patch_range_x.y);
				}else{
					param.patch_displacement.x = UnityEngine.Random.Range (-patch_range_x.y, -patch_range_x.x);
				}

				int latelal_y = UnityEngine.Random.Range(0,2);
				if(latelal_y == 0){
					param.patch_displacement.y = UnityEngine.Random.Range (patch_range_y.x, patch_range_y.y);
				}else{
					param.patch_displacement.y = UnityEngine.Random.Range (-patch_range_y.y, -patch_range_y.x);
				}	
				param.answer_onset = 0;
				param.answer_offset = 0;
				param.answer_reproduction = 0;

				condition_list.Add (param);
			}
		}
		//Shuffle (condition_list);

//		for(int i=0; i<10; i++){

	}

	void CreateVideoSelector(){
		
		for(int i=0; i<200; i++){
			Shuffle(videoOrder);
			for(int j=0; j<6; j++){
				int[] order = videoOrder[j];

				videoSelectorList.Add (order);
			}
		}



		Debug.Log ("videoselector");
		string s="";
		for (int i=0; i<videoSelectorList.Count; i++) {

			s += videoSelectorList[i][0].ToString() + ",";
		}
		Debug.Log (s);

	}

	public override IEnumerator runTraining () {

		Cursor.visible = false;

		training_show = new double[num_training_session];
		training_answer = new double[num_training_session];

		session_name = ExMode.ToString();//  Tracking.ToString ();
		sessionId = GetSessionIdFromFile (session_name);

		SetFilename (session_name, sessionId);
		
		Instruction.text = pre_training_text;
		yield return StartCoroutine(WaitForMouseDown());

		Instruction.text = "";
		Message.text = "Click to start";
		yield return StartCoroutine(WaitForMouseDown());

		Message.text = "";
		if (num_training_session > 0) {

			for (int i=0; i < num_training_session; i++) {
				
				yield return StartCoroutine (runMagnitudeTraining (i));
			}
		}
		/*
		string s ="";
		for (int i=0; i < num_training_session; i++) {
			s += training_show [i].ToString ("0.00") + " ";
		}
		s += "\n";
		for (int i=0; i < num_training_session; i++) {
			s += training_answer [i].ToString ("0.00") + " ";
		}
		Debug.Log (s);
		*/
		double corr = Correlation (training_show, training_answer);
		Debug.Log (corr);
		SaveTrainingDataCorr (corr);

		Message.text = "Score : " + corr.ToString ("0.00") + "\n";

		if (corr < 0.45) {
			Message.text += "Please try again\n";
		}

		Message.text += "\nClick to continue";

		yield return StartCoroutine(WaitForMouseDown());

		Cursor.visible = true;


		yield break;
	}




	IEnumerator runMagnitudeTraining(int index)
	{
		float exposure_time = UnityEngine.Random.Range (trainig_interval_min, trainig_interval_max);
		float break_seconds = UnityEngine.Random.Range (training_break_sec_min, training_break_sec_max);


		yield return StartCoroutine (MagnitudeEstimateStimulus (break_seconds, exposure_time));

		yield return StartCoroutine (MagnitudeEstimateSlider ());

		yield return StartCoroutine (MagnitudeEstimateFeedback (exposure_time));

		SaveTrainingData (exposure_time, sliderCtl.GetSliderValue ());

		training_show [index] = exposure_time;
		training_answer [index] = sliderCtl.GetSliderValue ();

	}
	IEnumerator MagnitudeEstimateStimulus(float break_seconds, float exposure_time)
	{
		sliderCtl.RandomizeHandle ();
		
		Instruction.text = "";
		fixationCross.GetComponent<Renderer>().enabled = true;
		
		yield return new WaitForSeconds(break_seconds);
		
		fixationCross.GetComponent<Renderer>().enabled = false;
		redCircle.GetComponent<Renderer>().enabled = true;
		
		yield return new WaitForSeconds(exposure_time);

	}


	IEnumerator MagnitudeEstimateSlider()
	{
		
		redCircle.GetComponent<Renderer>().enabled = false;	
		sliderCtl.ShowSliderForTimePerception ();
		sliderCtl.EnableSlider ();

		yield return StartCoroutine(WaitForMouseDown());
	}


	IEnumerator MagnitudeEstimateFeedback(float exposure_time)
	{

		sliderCtl.ShowFeedback (exposure_time);
		sliderCtl.DisableSlider ();

		yield return StartCoroutine(WaitForMouseDown());
		sliderCtl.HideFeedback ();

		sliderCtl.HideSlider ();


	}

	void ShowMagnitudeEstimateSliderOnScene()
	{

		SRCtl.DisablePixelation ();
		Instruction.text = "";
		Message.text = "";

		SliderPanel.GetComponent<Image> ().enabled = true;
		instCanvas.GetComponent<Canvas> ().enabled = true;

		sliderCtl.ShowSliderForTimePerception ();
		sliderCtl.EnableSlider ();


	}

	void HideMagnitudeEstimateSliderOnScene()
	{
		SRCtl.EnablePixelation ();

		SliderPanel.GetComponent<Image> ().enabled = false;
		instCanvas.GetComponent<Canvas> ().enabled = false;
		sliderCtl.DisableSlider ();
		sliderCtl.HideSlider ();
		sliderCtl.RandomizeHandle ();
	}

	
	public override IEnumerator runExperiment () {
		if (isRunning) {
			Debug.Log ("Experiment has already started");
			yield break;
		}else{
			isRunning = true;

		}

		Cursor.visible = false;

		if(pulsesensorcheck){
			//Pulsesensor check
			Instruction.text = "";
			Message.text =  "Pulse Sensor is not attached\n";
			Message.text += "Put it on your finger\n";
			Message.text += "and wait for seconds\n";
			SRCtl.GoToInstructionScreen ();
			while (!heartbeat.healthy) {
				yield return null;
			}
		}
		CreateVideoSelector ();

		//Tracking data check
		if(Tracking == TrackingMode.REPLAY)
		{

			if(!LoadHeadTrackData()){
				Debug.Log("Experiment is canceled because the tracking data is absent ");
				Instruction.text = "";
				Message.text = "Experiment was canceled because the tracking data is absent";
				SRCtl.GoToInstructionScreen ();
				yield return StartCoroutine(WaitForMouseDown());
				
				isRunning = false;
				yield break;
			}

			if(!LoadLiveResults()){
				Debug.Log("Experiment is canceled ");
				Message.text = "Experiment was canceled because the result data is absent";
				SRCtl.GoToInstructionScreen ();
				yield return StartCoroutine(WaitForMouseDown());
				
				isRunning = false;
				yield break;
			}

		}else{

			CreateCondition ();
		}
		Message.text = "";
		Instruction.text = "";

		if (ExMode == ExperimentMode.MAIN) {
			session_name = Tracking.ToString ();
		}else{
			session_name = ExMode.ToString() + "-" + Tracking.ToString ();
		}
		Debug.Log("Experiment Start : " + session_name);

	
		sessionId = GetSessionIdFromFile (session_name);
		SetFilename (session_name, sessionId);
		Debug.Log ("Session Id = " + sessionId);
	
		SRCtl.streamFOV = fov;
		SRCtl.GoToInstructionScreen ();
		yield return 0;

		if(Tracking == TrackingMode.LIVE || Tracking == TrackingMode.LIVE_FRAME ||Tracking == TrackingMode.LIVE_BLACK )
		{
			Instruction.text = "";
			Message.text = "Please set the VR goggle on your head if you don't\n\nClick to continue";
			yield return StartCoroutine(WaitForMouseDown());


			SRCtl.EnableOculus();

		
			Message.text = "";
			Instruction.text = inst_main_start_live1;
			SRCtl.GoToInstructionScreen ();
			yield return StartCoroutine(WaitForMouseDown());
			
			Instruction.text = inst_main_start_live2;
			SRCtl.GoToInstructionScreen ();
			yield return StartCoroutine(WaitForMouseDown());

			if(Tracking == TrackingMode.LIVE_FRAME){
				caliCtl.SetFrameMode(CalibFrameRatioX, CalibFrameRatioY);
			}else if(Tracking == TrackingMode.LIVE_BLACK){
				caliCtl.SetFrameBackMode(CalibFrameRatioX, CalibFrameRatioY);
			}

		}else{
			Instruction.text = "";
			Message.text = "Please take off the VR goggle\n\nClick to continue";
			yield return StartCoroutine(WaitForMouseDown());
			SRCtl.DisableOculus();

			Message.text = "";
			Instruction.text = inst_main_start_replay1;
			SRCtl.GoToInstructionScreen ();
			yield return StartCoroutine(WaitForMouseDown());
			
			Instruction.text = inst_main_start_replay2;
			SRCtl.GoToInstructionScreen ();
			yield return StartCoroutine(WaitForMouseDown());
		}

		yield return StartCoroutine (runMain());

		Debug.Log ("runMain Finished" + " " + keyManager.GetElapsedMilliseconds());

		if(Tracking == TrackingMode.LIVE_FRAME || Tracking == TrackingMode.LIVE_BLACK){
			caliCtl.UnsetFrameMode();
		}

		yield return StartCoroutine (runQuestionnaire());

		if(ExMode == ExperimentMode.PRACTICE){
			Message.text = "Finished the practice session!\n";
		}else{
			if(sessionId == num_session - 1){
				Message.text = "End of the session!\n Take off the VR goggle\n\n";
			
			}else{
				Message.text = "Take a break!\n";
			}
		}
		Message.text += "Click to continue";

		SRCtl.GoToInstructionScreen ();

		yield return StartCoroutine(WaitForMouseDown());
		SRCtl.BackFromInstructionScreen ();


		
		//ovr0.5
        //kskk
		//OculusCamRig.IsRotationUpdate = true;

		if (ExMode != ExperimentMode.PRACTICE && sessionId == num_session - 1) {
			SRCtl.DisableOculus ();
		}

		//videoCtl.isLoop = true;

		isRunning = false;

		Cursor.visible = true;
	}

	private IEnumerator runQuestionnaire()
	{

		Directory.CreateDirectory ( result_dir  + SubId + "/" + questionnaire_data_dir + session_name);
		//SaveQuestionnaireHeaderToFile ();

		//initialize
		SRCtl.GoToInstructionScreen ();

		SRCtl.DisablePixelation ();
		Instruction.text = "";
		Message.text = "";
			
		SliderPanel.GetComponent<Image> ().enabled = true;
		instCanvas.GetComponent<Canvas> ().enabled = true;
		sliderCtl.EnableSlider ();

		List<float> Answers = new List<float> ();
		//question 1
	
		sliderCtl.ShowSliderForBlockDurationEstimation (question_duration_text);
		yield return StartCoroutine(WaitForMouseDown());
		Answers.Add (sliderCtl.GetSliderValue ());

		sliderCtl.ShowSliderForQuestionnaire (question_speed_text);
		yield return StartCoroutine(WaitForMouseDown());
		Answers.Add (sliderCtl.GetSliderValue ());

		sliderCtl.ShowSliderForQuestionnaire (question_presence_text);
		yield return StartCoroutine(WaitForMouseDown());
		Answers.Add (sliderCtl.GetSliderValue ());


		sliderCtl.ShowSliderForQuestionnaire (question_attention_text);
		yield return StartCoroutine(WaitForMouseDown());
		Answers.Add (sliderCtl.GetSliderValue ());


		sliderCtl.ShowSliderForQuestionnaire (question_mood_text);
		yield return StartCoroutine(WaitForMouseDown());
		Answers.Add (sliderCtl.GetSliderValue ());

	
		sliderCtl.ShowSliderForQuestionnaire (question_nauseous_text);
		yield return StartCoroutine(WaitForMouseDown());
		Answers.Add (sliderCtl.GetSliderValue ());

		SaveQuestionnaireDataToFile( Answers );

		//finalize
		SRCtl.EnablePixelation ();
			
		SliderPanel.GetComponent<Image> ().enabled = false;
		instCanvas.GetComponent<Canvas> ().enabled = false;
		sliderCtl.DisableSlider ();
		sliderCtl.HideSlider ();


	}


	private IEnumerator runMain()
	{	//-------------
		// init
		//-------------
		SetTrackingFilename (session_name, sessionId);


		if(ExMode == ExperimentMode.PRACTICE){

			curVideoId = 0;
		
		}else{

			int sessionDivision =  (int)Mathf.Floor ((float)sessionId / 3);

			if(Tracking == TrackingMode.LIVE || Tracking == TrackingMode.LIVE_FRAME)
			{
				curVideoId  = videoSelectorList[    SubId * 2 + (sessionDivision * 128) ][sessionId % 3];

			}else if(Tracking == TrackingMode.REPLAY || Tracking == TrackingMode.LIVE_BLACK){
				curVideoId  = videoSelectorList[1 + SubId * 2 + (sessionDivision * 128) ][sessionId % 3];
				//int subType = (SubId + 1) % 4;
				//curVideoId  = videoSelecter[subType, ( sessionId % 4)];
			}
			Debug.Log ( SubId + " " + sessionDivision + " key:" + (SubId * 2 + (sessionDivision * 128) + " videoId:" + curVideoId));

		}
		Debug.Log ("curVideoId = " + curVideoId);

		int offsetSeparater = (Tracking == TrackingMode.LIVE || Tracking == TrackingMode.LIVE_FRAME) ? 0 : 1;

		SRCtl.GoToStream(curVideoId);
		int offset =  ( SubId + sessionId + (3 * offsetSeparater) ) % initialRotationRandomizer.Length ;

		int offsetR = initialRotationRandomizer[ offset ];
		Debug.Log ("initial offset id "  + sessionId + " " + offset+ " " + offsetR);
		initialRotation = UnityEngine.Random.Range ( (offsetR * 60f), (offsetR + 1) * 60f);
		GameObject.Find ("AVProContainer").transform.localEulerAngles = new Vector3(0f, initialRotation, 180f);
		//OculusCamRig.transform.localRotation = Quaternion.AngleAxis(initialRotation, Vector3.up);
		
		Instruction.text = "";
		Message.text = "Click to Start";
		SRCtl.GoToInstructionScreen ();
		result_list.Clear ();

		yield return StartCoroutine(WaitForMouseDown());
		//-------------
		// start
		//-------------
		//float start_time = UnityEngine.Time.realtimeSinceStartup;
		//float duration = 60;// videoCtl.GetDuration ();
		
		keyManager.ResetTimer ();
		HeadTracker.StartHeadRecording(); 
		videoCtl.SetForeGround();
		//videoCtl.isLoop = true;

		logger.StartLogHeartbeat (session_name, sessionId);
		logger.SetLog("start", 0, 0, 0); //todo

		SaveExperimentParameter ();

		if(Tracking == TrackingMode.REPLAY)
		{
			Debug.Log ("Loading Trajectory");
			Debug.Log ("sessionId : " + sessionId + " num trajectories : " + loaded_condition_lists.Count);

			int previousId = sessionId % loaded_condition_lists.Count;
			Debug.Log (" using "+previousId);

			List<HeadTrackingRecorder.HeadTrackData> trajectory = HeadTrajectory[previousId];
			HeadTracker.SetTrajectoryForReplay(trajectory); 
			HeadTracker.StartHeadReplay(); 
		
			loaded_condition_list = loaded_condition_lists[previousId];
			//back from Instruction but tracking is disable
			SRCtl.BackFromInstructionScreen (true);

			keyManager.SetMouseHook();
			
			yield return StartCoroutine (runReplayTrials());

			keyManager.UnsetMouseHook();
		}else{
			//OculusCamRig.transform.localRotation = Quaternion.AngleAxis(param.rotation, Vector3.up);

			//back from Instruction but tracking is aviable
			SRCtl.BackFromInstructionScreen (false);

			keyManager.SetMouseHook();
			yield return StartCoroutine (runLiveTrials());
			
			keyManager.UnsetMouseHook();
		}

		Message.text = "End of Block\nWait for a moment";
		SRCtl.GoToInstructionScreen ();

		//-------------
		// finished
		//-------------

		Debug.Log ("Saving Data " + " " + keyManager.GetElapsedMilliseconds());
		SaveExperimentParameter ();
		SaveAllDataToFile ();
		Debug.Log ("The data data was sucessfully saved");

		Debug.Log ("Go To Background" + " " + keyManager.GetElapsedMilliseconds());
		videoCtl.SetBackGround();

		if(Tracking == TrackingMode.REPLAY){ HeadTracker.StopHeadReplay(); }

		Debug.Log ("Stopping Head Recording" + " " + keyManager.GetElapsedMilliseconds());

		HeadTracker.StopHeadRecording();
		Debug.Log ("Saving Tracking File" + " " + keyManager.GetElapsedMilliseconds());
		SaveTrackingFile();

		Debug.Log ("Session Finished" + " " + keyManager.GetElapsedMilliseconds());
		logger.SetLog("session finish", 0, 0, 0); 

		logger.StopLogHeartbeat ();	

		logger.SaveLog();
	}

	private IEnumerator runLiveTrials()
	{
		int counter = 0;
		float max_duration;
		if(ExMode == ExperimentMode.PRACTICE)
		{
			max_duration = practice_duration;
		}else{
			max_duration = video_duration;
		}
		Debug.Log (max_duration);
		Debug.Log ("actual video " + videoCtl.GetDuration ()  + " video length " + max_duration);

		while(videoCtl.GetElapsedTime() < max_duration){

			Debug.Log ("start of trial: counter: "+ counter + " " +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds() + " " + videoCtl.GetElapsedTime());
		//for(int i=0; i< num_trial; i++){
			Param param = condition_list[counter];

			param.videoId = curVideoId;

			float duration_sec = durations[param.duration];

			//yield return StartCoroutine(WaitUntilTransitionEnd());
			//Debug.Log ("Transition wait end " +  getSpeedinVideo()+ " " + keyManager.GetElapsedMilliseconds() + " " + videoCtl.GetElapsedTime());
			yield return new WaitForSeconds(param.break_seconds);
			//-------------
			//onset
			Debug.Log ("actual onset!"  + " " +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds() + " " + videoCtl.GetElapsedTime());
			logger.SetLog("onset", counter, 1, param.duration); //todo
		
			param.actual_patch_onset = keyManager.GetElapsedMilliseconds();
			param.speed = getSpeedinVideo();

			// live
			Vector3 orientation = patchCtl.GetHeadOrientation();
			param.orientation.x = orientation.x;
			param.orientation.y = orientation.y;
			patchCtl.SetPatchRandomInSight (param.patch_displacement.x, param.patch_displacement.y);
			// live end

			if(isChangeRedAfterClick)
			{
				yield return StartCoroutine(WaitForMouseDownOrSeconds(keypress_expired_seconds));
				param.answer_onset = LowLevelInputManager.lastKeyDownStopWatch;

				if( param.answer_onset < param.actual_patch_onset){
					logger.SetLog("miss", counter, 1, param.duration);
					patchCtl.RemovePatch();
					Debug.Log ("missed onset! " + param.answer_onset + " " +  param.actual_patch_onset);
					param.answer_onset = 0;
				}else{
					logger.SetLog("click", counter, 1, param.duration);

					SRCtl.SetPixelationColor(patch_color_level);
					yield return new WaitForSeconds(duration_sec);
					param.actual_patch_offset = keyManager.GetElapsedMilliseconds();

					Debug.Log ("actual offset!" +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds()+  " " + videoCtl.GetElapsedTime());
					patchCtl.RemovePatch();
					SRCtl.SetPixelationColor(0.0f);
					logger.SetLog("offset", counter, 1, param.duration); //todo
				

					yield return new WaitForSeconds(0.3f);
					
					ShowMagnitudeEstimateSliderOnScene();
					yield return StartCoroutine (WaitForMouseDown());
					
					Debug.Log (sliderCtl.GetSliderValue());
					param.answer_magnitude = sliderCtl.GetSliderValue();
					
					HideMagnitudeEstimateSliderOnScene();
				}

			}else{

				yield return new WaitForSeconds(duration_sec);

				//-------------
				//offset
				Debug.Log ("actual offset!" +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds()+  " " + videoCtl.GetElapsedTime());
				patchCtl.RemovePatch();
				logger.SetLog("offset", counter, 1, param.duration); //todo
				param.answer_onset = LowLevelInputManager.lastKeyDownStopWatch;
				//todo: actual patch onset ä»¥é™ã§æœ€åˆã®key pressã‚’æŒã£ã¦ãã‚‹
				param.actual_patch_offset = keyManager.GetElapsedMilliseconds();
			
				if( param.answer_onset < param.actual_patch_onset){
					Debug.Log ("missed onset! " + param.answer_onset + " " +  param.actual_patch_onset);
					param.answer_onset = 0;
				}else{
					yield return new WaitForSeconds(0.3f);
			
					ShowMagnitudeEstimateSliderOnScene();
					yield return StartCoroutine (WaitForMouseDown());

					Debug.Log (sliderCtl.GetSliderValue());
					param.answer_magnitude = sliderCtl.GetSliderValue();
							
					HideMagnitudeEstimateSliderOnScene();
				}
			}

			if(getSpeedinVideo() == -1) param.speed = -1;


			//Debug.Log (" actual onset:" + param.actual_patch_onset + " actual offset:" + param.actual_patch_offset + " answer onset:" + param.answer_onset + 
			//           " ansser offset:" + param.answer_offset + " ansser reproduction:" + param.answer_reproduction );
			SaveDataToFile(param);
			result_list.Add (param);

			SaveTrackingFile();
			logger.SaveLog();

			Debug.Log ("end of trial: " +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds() + " " + videoCtl.GetElapsedTime());
			counter++;
		}

		num_actual_trials = counter;
	}
	

	private IEnumerator runReplayTrials()
	{
		int counter = 0;
		float max_duration;
		if(ExMode == ExperimentMode.PRACTICE)	
		{
			max_duration = practice_duration;
		}else{
			max_duration = video_duration;
		}

		//while(videoCtl.GetElapsedTime() < duration){
		for(int i=0; i< loaded_condition_list.Count; i++){

			if(videoCtl.GetElapsedTime() > max_duration){
				yield break;
			}

			Param param = loaded_condition_list[i];

			if(param.actual_patch_offset != 0 ){

			Param result = new Param();
			result.videoId = curVideoId;

			result.duration = param.duration;
			result.patch_displacement = param.patch_displacement;
			result.break_seconds = param.break_seconds;

			float duration_sec =  durations[param.duration];

			yield return StartCoroutine(WaitUntilStopWatch(param.actual_patch_onset));
			//-------------
			//onset
			result.actual_patch_onset = keyManager.GetElapsedMilliseconds();

			logger.SetLog("onset", i, 1, param.duration); //todo
		
			result.speed = getSpeedinVideo();

			Debug.Log ("actual onset : " + param.actual_patch_onset + " " + keyManager.GetElapsedMilliseconds() + " diff: " + ( keyManager.GetElapsedMilliseconds() - param.actual_patch_onset ));
			// live
			Vector3 orientation = patchCtl.GetHeadOrientation();
			result.orientation.x = orientation.x;
			result.orientation.y = orientation.y;
			patchCtl.SetPatchRandomInSight (param.patch_displacement.x, param.patch_displacement.y);
			// live end

			//Debug.Log ("actual onset end! " + keyManager.GetElapsedMilliseconds());

			if(isChangeRedAfterClick)
			{
				yield return StartCoroutine(WaitForMouseDownOrSeconds(keypress_expired_seconds));
				result.answer_onset = LowLevelInputManager.lastKeyDownStopWatch;
			
				if( result.answer_onset < result.actual_patch_onset){
					logger.SetLog("miss", counter, 1, param.duration); //todo
					patchCtl.RemovePatch();
					Debug.Log ("missed onset! " + result.answer_onset + " " +  result.actual_patch_onset);
					result.answer_onset = 0;
				}else{
					logger.SetLog("click", counter, 1, param.duration); //todo

					Debug.Log ("click : " + result.answer_onset + " rt: " + ( result.answer_onset - result.actual_patch_onset));

					SRCtl.SetPixelationColor(patch_color_level);
					yield return new WaitForSeconds(duration_sec);
					result.actual_patch_offset = keyManager.GetElapsedMilliseconds();
					
					Debug.Log ("offset : " +  keyManager.GetElapsedMilliseconds() + " duration:" + duration_sec + " actual duration:" + (result.actual_patch_offset - result.answer_onset) );
					patchCtl.RemovePatch();
					SRCtl.SetPixelationColor(0.0f);
					logger.SetLog("offset", counter, 1, param.duration); //todo

					yield return new WaitForSeconds(0.3f);
					
					ShowMagnitudeEstimateSliderOnScene();

					//Param next_param = loaded_condition_list[i+1];
					//Debug.Log ("remaining time to the next patch : " + (next_param.actual_patch_onset - keyManager.GetElapsedMilliseconds()) );

					yield return StartCoroutine(WaitForMouseDownOrSeconds(4.5f));
					if( LowLevelInputManager.lastKeyDownStopWatch  < result.actual_patch_offset){
						Debug.Log ("missed answer" );

						result.answer_magnitude = -1;
					}else{
						result.answer_magnitude = sliderCtl.GetSliderValue();
					}				
					HideMagnitudeEstimateSliderOnScene();
				}

			}else{

				yield return StartCoroutine(WaitUntilStopWatch(param.actual_patch_offset));
				//-------------
				//offset

				patchCtl.RemovePatch();
				result.actual_patch_offset = keyManager.GetElapsedMilliseconds();
				result.answer_onset = LowLevelInputManager.lastKeyDownStopWatch;
				logger.SetLog("offset", i, 1, result.duration); //todo

				//Debug.Log ("actual offset : " + param.actual_patch_offset + " " + keyManager.GetElapsedMilliseconds() + " diff: " + ( keyManager.GetElapsedMilliseconds() - param.actual_patch_offset ) );
				//Debug.Log ("live duration : " + (param.actual_patch_offset - param.actual_patch_onset )+ " replay duration : " + (result.actual_patch_offset - result.actual_patch_onset) );

				if( result.answer_onset < result.actual_patch_onset){
					Debug.Log ("missed onset!");
					result.answer_onset = 0;
				}else{

					Debug.Log ("!! Onset : " + result.answer_onset );
				
					yield return new WaitForSeconds(0.3f);
				
					ShowMagnitudeEstimateSliderOnScene();
				
					yield return StartCoroutine (WaitForMouseDown());
				
					result.answer_magnitude = sliderCtl.GetSliderValue();
					Debug.Log (result.answer_magnitude);

					HideMagnitudeEstimateSliderOnScene();

				}
			}

			if(getSpeedinVideo() == -1) result.speed = -1;
			

			//Debug.Log (" actual onset:" + param.actual_patch_onset + " actual offset:" + param.actual_patch_offset + " answer onset:" + param.answer_onset + 
			 //          " ansser offset:" + param.answer_offset + " ansser reproduction:" + param.answer_reproduction );
			result_list.Add (result);
			SaveDataToFile(result);

			SaveTrackingFile();
			logger.SaveLog();

			}
			counter++;
		}
		num_actual_trials = counter;
	}

	protected override void DoUpdate()
	{
		if(isRunning){
			timeleft -= Time.deltaTime;
			if (timeleft <= 0.0) {
				timeleft = 10.0f;
				logger.SetFPSLog(SRCtl.fps); 
			}

		}
	}
	public void SetFilename(string mode, int sessionId){

		string filename = result_dir + SubId + "/" + data_dir + mode + "/";
		Directory.CreateDirectory (filename );

		DateTime dt = DateTime.Now;
		out_filename  = string.Format(filename + "/data-{0}-{1}-{2}-{3}-{4}-{5}-{6}.txt", sessionId, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
	}


	public void SetTrackingFilename(string mode, int sessionId){

		Directory.CreateDirectory ( result_dir  + SubId + "/" + tracking_data_dir +  mode);

		DateTime dt = DateTime.Now;
		tracking_filename  = string.Format("head-{0}-{1}-{2}-{3}-{4}-{5}-{6}.txt", sessionId, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

	}


	public int GetSessionIdLiveFromFile(){
		return GetSessionIdFromFile (TrackingMode.LIVE.ToString());
	}
	public int GetSessionIdReplayFromFile(){
		return GetSessionIdFromFile (TrackingMode.REPLAY.ToString());
	}


	public int GetSessionIdLiveFramedFromFile(){
		return GetSessionIdFromFile (TrackingMode.LIVE_FRAME.ToString());
	}

	public int GetSessionIdLiveBlackedFromFile(){
		return GetSessionIdFromFile (TrackingMode.LIVE_BLACK.ToString());
	}

	public int GetSessionIdFromFile(string mode){

		int max_id = -1;
		try{
			string[] files = Directory.GetFiles(result_dir + SubId + "/" + questionnaire_data_dir +  mode + "/" );
			foreach (string file in files) {
				string filename = Path.GetFileNameWithoutExtension(file);
				string[] tokens = filename.Split ('-');
				int id = int.Parse(tokens[1]);
				if( id > max_id ) max_id = id;
			}	
		}catch(Exception e){
			Debug.Log ("no such directory : " + e.Message);
		}

		Debug.Log ("max_id=" + max_id );
		return (max_id + 1);
	}



	string GetExperimentParameter()
	{
		string text = "#";
		text += "Tracking=" + Tracking.ToString () +  ";";

		text += "Durations=";
		for(int i=0; i< durations.Length; i++)
		{
			text += durations[i];

			if(i == durations.Length -1)
				text += ";";
			else
				text += ",";
		}

		text += "NumTrials=" + num_actual_trials +  ";";
		text += "videoId=" + curVideoId +  ";";

		text += "initialOrientation=" + initialRotation + ";";
		text +=   Environment.NewLine;

		return text;

	}

	void SaveExperimentParameter()
	{
		File.AppendAllText(out_filename, GetExperimentParameter());
	}

	string GetTextFromParam(Param p){

		string text = "";
		text += p.index + ", ";
		text += p.videoId + ", ";
		text += p.speed + ", ";
		text += p.duration + ", ";
		text += p.break_seconds + ", ";
		text += p.orientation.x + ", " + p.orientation.y + ", ";
		text += p.patch_displacement.x + ", " + p.patch_displacement.y + ", ";
		
		text += p.actual_patch_onset + ", ";
		text += p.actual_patch_offset + ", ";
		
		text += p.answer_onset + ", ";
		text += p.answer_offset + ", ";
		text += p.answer_magnitude + ", ";
		text += (p.actual_patch_offset - p.answer_onset) + ", ";
		
		text +=   Environment.NewLine;
		return text;
	}

	void SaveDataToFile(Param p)
	{

		File.AppendAllText(out_filename, GetTextFromParam (p));
	}

	void SaveAllDataToFile()
	{	
		string saved_contents = GetExperimentParameter();
		for(int i=0; i< result_list.Count; i++){
			saved_contents += GetTextFromParam(result_list[i]);
		}
		saved_contents +=   Environment.NewLine;
		File.WriteAllText (out_filename, saved_contents);
	}

	void SaveTrainingData(float actual, float answer)
	{
		string text = actual.ToString ("0.00000") + ", " + answer.ToString ("0.00000") +  Environment.NewLine;
		File.AppendAllText(out_filename, text);
	}

	void SaveTrainingDataCorr(double corr)
	{
		string text = corr.ToString ("0.00000") + Environment.NewLine + Environment.NewLine;
		File.AppendAllText(out_filename, text);
	}



	void SaveTrackingFile()
	{
		string saved_contents = "";
		foreach (HeadTrackingRecorder.HeadTrackData data in HeadTracker.GetRecordedTrajectory()) 
		{
			saved_contents += data.frame + "," + data.time + "," +  + data.rotation.x + "," + data.rotation.y + "," + data.rotation.z + Environment.NewLine;
		}

		File.AppendAllText(result_dir + SubId + "/" + tracking_data_dir  +  session_name + "/" + tracking_filename , saved_contents);

		HeadTracker.ClearHeadRecordingData ();
	}

	void SaveQuestionnaireHeaderToFile()
	{
		string saved_contents = "# tracking, sessionId, videoId, duration, presence, attention, mood, nausia";
		string filename = "questionnaire.txt";
		File.AppendAllText(result_dir + SubId + "/" + questionnaire_data_dir + session_name + "/" + filename , saved_contents);

	}

	void SaveQuestionnaireDataToFile(List<float> Answers)
	{
		//if(ExMode == ExperimentMode.MAIN){
		
		//	Debug.Log ("V" +curVideoId);
			string saved_contents = "";
			saved_contents += (int)Tracking + ", ";
			saved_contents += sessionId + ", ";
			saved_contents += curVideoId + ", ";

			for(int i=0; i< Answers.Count; i++)
			{
				saved_contents += Answers[i] + ", ";
			}
			saved_contents += Environment.NewLine;

			DateTime dt = DateTime.Now;
			string filename  = string.Format("questionnaire-{0}-{1}-{2}-{3}-{4}-{5}-{6}.txt", sessionId, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);


			File.AppendAllText(result_dir + SubId + "/" + questionnaire_data_dir + session_name + "/" + filename , saved_contents);
		//}
	}



	bool LoadLiveResults()
	{


		string dir = result_dir  + TrajectorySubId + "/" + data_dir + "LIVE" + "/";
		Debug.Log ("Load Live Results : " + dir);
		loaded_condition_lists.Clear ();
		if (!System.IO.Directory.Exists (dir)) {
			Debug.Log ("LIVE Results data folder for Sub" + TrajectorySubId + " does not exist");
			return false;
		}
		
		string[] fs = System.IO.Directory.GetFiles (dir, "*", System.IO.SearchOption.TopDirectoryOnly);

		for(int i=0; i< fs.Length; i++){
			
			loaded_condition_lists.Add (new List<Param>());
		}

		foreach( string file in fs )
		{
			List<Param> loaded_data = new List<Param>();
			
			FileInfo fi = new FileInfo(file);
			StreamReader sr = new StreamReader(fi.OpenRead());
			while( sr.Peek() != -1 ){
				string line = sr.ReadLine(); 
				if(line != "" && line.Substring(0,1) != "#"){
					string[] line_split = line.Split (',');
					//Debug.Log (line);
					//Debug.Log (line_split.Length);
					Param data = new Param();
					data.duration = int.Parse(line_split[3]);
					data.orientation.x = float.Parse(line_split[5]);
					data.orientation.x = float.Parse(line_split[6]);
					data.patch_displacement.x = float.Parse(line_split[7]);
					data.patch_displacement.y = float.Parse(line_split[8]);
					data.actual_patch_onset = long.Parse(line_split[9]);
					data.actual_patch_offset = long.Parse(line_split[10]);
					data.answer_onset = long.Parse(line_split[11]);
					loaded_data.Add (data);
				}
			}
			sr.Close();
			
			string name = System.IO.Path.GetFileNameWithoutExtension (file);
			string[] token =  name.Split('-');
			int sessionId = int.Parse(token[1]);
			Debug.Log ("Results file was loaded : sessionId=" + sessionId + " num trials=" + loaded_data.Count);

			loaded_condition_lists[sessionId] = loaded_data;
			//HeadTrajectory.Add (tracking_data);
		}
		return true;

	}

	bool LoadHeadTrackData()
	{
		string dir = result_dir  + TrajectorySubId + "/" + tracking_data_dir + "LIVE" + "/";
		Debug.Log ("Load Tracking Data : " + dir);
		HeadTrajectory.Clear ();
		if (!System.IO.Directory.Exists (dir)) {
			Debug.Log ("LIVE tracking data folder for Sub" + TrajectorySubId + " does not exist");
			return false;
		}

		string[] fs = System.IO.Directory.GetFiles (dir, "*", System.IO.SearchOption.TopDirectoryOnly);
		
		if(fs.Length < num_session){
			Debug.Log ("LIVE tracking files do not exist. the number of tracking files is " + fs.Length);
			return false;
		}

		for(int i=0; i< fs.Length; i++){
			HeadTrajectory.Add (new List<HeadTrackingRecorder.HeadTrackData>());
		}

		foreach( string file in fs )
		{
			List<HeadTrackingRecorder.HeadTrackData> tracking_data = new List<HeadTrackingRecorder.HeadTrackData>();
			//Debug.Log (file);
			
			FileInfo fi = new FileInfo(file);
			StreamReader sr = new StreamReader(fi.OpenRead());
			while( sr.Peek() != -1 ){
				//Debug.Log ("loading tracking data : " + file);
				string line = sr.ReadLine(); 
				string[] line_split = line.Split (',');
				
				HeadTrackingRecorder.HeadTrackData data = new HeadTrackingRecorder.HeadTrackData();
				data.frame = int.Parse(line_split[0]);
				data.time = float.Parse(line_split[1]);
				data.rotation = new Vector3(float.Parse(line_split[2]), float.Parse(line_split[3]), float.Parse(line_split[4]));
				tracking_data.Add (data);
			}
			sr.Close();

			string name = System.IO.Path.GetFileNameWithoutExtension (file);
			string[] token =  name.Split('-');
			int sessionId = int.Parse(token[1]);
			Debug.Log ("tracking file was loaded : sessionId=" + sessionId);
			HeadTrajectory[sessionId] = tracking_data;
		}
		return true;
		
	}

	int getSpeedinVideo(){

		return curVideoId;
	}
	/*
	int getSpeedinVideo(){
		float now = videoCtl.GetElapsedTime ();

		switch(curVideoId){
		case 0:
			if( now > VideoRangeAlpha.normal && now < VideoRangeAlpha.transit ){
				return 0;
			}else if( now > VideoRangeAlpha.transit && now < VideoRangeAlpha.altered ){
				return -1;
			}else if( now > VideoRangeAlpha.altered && now < VideoRangeAlpha.end){
				return 1;
			}
			break;
		case 1:
			if( now > VideoRangeBeta.altered && now < VideoRangeBeta.transit ){
				return 1;
			}else if( now > VideoRangeBeta.transit && now < VideoRangeBeta.normal ){
				return -1;
			}else if( now > VideoRangeBeta.normal && now < VideoRangeBeta.end){
				return 0;
			}
			break;
		case 2:
			if( now > VideoRangeAlpha.normal && now < VideoRangeAlpha.transit ){
				return 0;
			}else if( now > VideoRangeAlpha.transit && now < VideoRangeAlpha.altered ){
				return -1;
			}else if( now > VideoRangeAlpha.altered && now < VideoRangeAlpha.end){
				return 2;
			}
			break;
		case 3:
			if( now > VideoRangeBeta.altered && now < VideoRangeBeta.transit ){
				return 2;
			}else if( now > VideoRangeBeta.transit && now < VideoRangeBeta.normal ){
				return -1;
			}else if( now > VideoRangeBeta.normal && now < VideoRangeBeta.end){
				return 0;
			}
			break;

		default:
			return -3;
		}

		return -3;
	}
*/

	/*
	int getSpeedinVideo(){

		if(curVideoId == 2){
			return 0;
		}
		
		float now = videoCtl.GetElapsedTime ();
		if( now > VideoRange.normal1 && now < VideoRange.transit1 ){
			return 0;
		}else if( now > VideoRange.transit1 && now < VideoRange.fast1 ){
			return -1;
		}else if( now > VideoRange.fast1 && now < VideoRange.transit2 ){
			if(curVideoId == 0){
				return 1;
			}else if(curVideoId == 1){
				return 2;
			}else{
				return 0;
			}
		}else if( now > VideoRange.transit2 && now < VideoRange.slow1 ){
			return -1;
		}else if( now > VideoRange.slow1 && now < VideoRange.transit3 ){
			if(curVideoId == 0){
				return 2;
			}else if(curVideoId == 1){
				return 1;
			}else{
				return 0;
			}
		}else if( now > VideoRange.transit3 && now < VideoRange.normal2 ){
			return -1;
		}else if( now > VideoRange.normal2 ){
			return 0;
		}

		return -3;
	}
*/




	IEnumerator WaitUntilTransitionEnd()
	{
		while(getSpeedinVideo() == -1 ){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	IEnumerator WaitUntilStopWatch(long time)
	{
		while(videoCtl.isPlaying && keyManager.GetElapsedMilliseconds() < time){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

			          
	
	public void Shuffle(List<Param> ary)
	{
		int n = ary.Count;
		while (n > 1)
		{
			n--;
			int k = Rn.Next(n + 1);
			Param tmp = ary[k];
			ary[k] = ary[n];
			ary[n] = tmp;
		}
	}

	
	public void Shuffle(int[] ary)
	{
		int n = ary.Length;
		while (n > 1)
		{
			n--;
			int k = Rn.Next(n + 1);
			int tmp = ary[k];
			ary[k] = ary[n];
			ary[n] = tmp;
		}
	}

	public void Shuffle(int[][] ary)
	{
		int n = ary.Length;
		while (n > 1)
		{
			n--;
			int k = FixRn.Next(n + 1);
			int[] tmp = ary[k];
			ary[k] = ary[n];
			ary[n] = tmp;
		}
	}

	public double Correlation(double[] array1, double[] array2)
	{
		double[] array_xy = new double[array1.Length];
		double[] array_xp2 = new double[array1.Length];
		double[] array_yp2 = new double[array1.Length];

		for (int i = 0; i < array1.Length; i++)
			array_xy[i] = array1[i] * array2[i];
		for (int i = 0; i < array1.Length; i++)
			array_xp2[i] = Math.Pow(array1[i], 2.0);
		for (int i = 0; i < array1.Length; i++)
			array_yp2[i] = Math.Pow(array2[i], 2.0);
		double sum_x = 0;
		double sum_y = 0;
		foreach (double n in array1)
			sum_x += n;
		foreach (double n in array2)
			sum_y += n;
		double sum_xy = 0;
		foreach (double n in array_xy)
			sum_xy += n;
		double sum_xpow2 = 0;
		foreach (double n in array_xp2)
			sum_xpow2 += n;
		double sum_ypow2 = 0;
		foreach (double n in array_yp2)
			sum_ypow2 += n;
		double Ex2 = Math.Pow(sum_x, 2.00);
		double Ey2 = Math.Pow(sum_y, 2.00);

		double top = (array1.Length * sum_xy - sum_x * sum_y);
		double bottom = Math.Sqrt ((array1.Length * sum_xpow2 - Ex2) * (array1.Length * sum_ypow2 - Ey2));

		if (bottom != 0) {
			return top / bottom;
		} else {
			return 0;
		}
	}

}