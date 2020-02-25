using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonUtil;


public class ExperimentDeepDream : ExperimentTimeInterface {

	public struct Param
	{
		public int index;
		public int videoId;
		public int type;
		public int duration;
		public float break_seconds;

		public long tone_onset;

		public long answer_onset;
		public long answer_offset;
		public float answer_production;
	}

	AudioSource[] tones = new AudioSource[3];

	GameObject instCanvas;
	GameObject SliderPanel;

	// PatchManager patchCtl;
	sliderController sliderCtl;

	string inst_main_start_live1;
	string inst_main_start_live2;

	string pre_training_text;

	string question_duration_text;
	string question_presence_text;
	string question_attention_text;
	string question_mood_text;
	string question_nauseous_text;
	string question_speed_text;


	//basic parameters
	public  float[] durations = {1.0f, 2.00f, 4.00f};

	public float break_sec_min = 2.0f;
	public float break_sec_max = 4.0f;
	
	public int num_training_session = 20;

	public float practice_duration = 60f;

	public float trainig_interval_max = 3.0f;
	public float trainig_interval_min = 1.0f;

	public float training_break_sec_min = 0.5f;
	public float training_break_sec_max = 1.0f;


	public Vector2 patch_range_x = new Vector2(10f, 25f);
	public Vector2 patch_range_y = new Vector2(0f, 20f);

	public float video_duration = 180f;

	public  int num_duration = 3;  //may increase later
	public  int num_video = 1;     //may increase later (normal -> slow -> fast or normal -> fast -> slow etc)

	public  int num_repetition = 40;
	public  int num_random_repetition = 4;

	int num_actual_trials = 0;

	public  int num_trial;

	public float initialRotation;

	public int sessionId = 0;
	public int num_session = 3;

	public int curVideoId = 0;


	List<Param> condition_list = new List<Param>();
	List<Param> result_list = new List<Param>();

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
	// GameObject fixationCross;

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

		 //patchCtl = GameObject.Find ("PatchController").GetComponent<PatchManager>();
		sliderCtl = GameObject.Find ("SliderController").GetComponent<sliderController> ();
	
		inst_main_start_live1 = Resources.Load<TextAsset> ("Texts/TimeDeepDream/main_start_live1").text;
		inst_main_start_live2 = Resources.Load<TextAsset> ("Texts/TimeDeepDream/main_start_live2").text;

		pre_training_text = Resources.Load<TextAsset> ("Texts/TimeDeepDream/pre_training").text;

		question_duration_text = Resources.Load<TextAsset> ("Texts/TimeDeepDream/question_duration_text").text;
		question_speed_text = Resources.Load<TextAsset> ("Texts/TimeDeepDream/question_speed_text").text;
		question_presence_text = Resources.Load<TextAsset> ("Texts/TimeDeepDream/question_presence_text").text;
		question_attention_text = Resources.Load<TextAsset> ("Texts/TimeDeepDream/question_attention_text").text; 
		question_mood_text = Resources.Load<TextAsset> ("Texts/TimeDeepDream/question_mood_text").text;
		question_nauseous_text = Resources.Load<TextAsset> ("Texts/TimeDeepDream/question_nauseous_text").text;
		
		heartbeat = GameObject.Find("CompositionEngine").GetComponent<HeartBeat>();			

		redCircle = GameObject.Find ("RedCircle");
		// fixationCross = GameObject.Find ("Fixation");

		instCanvas = GameObject.Find ("InstructionCanvas");
		SliderPanel = GameObject.Find ("SliderPanel");

		tones[0] = GameObject.Find ("AudioSourceLow").GetComponent<AudioSource> ();
		tones[1] = GameObject.Find ("AudioSourceMiddle").GetComponent<AudioSource> ();
		tones[2] = GameObject.Find ("AudioSourceHigh").GetComponent<AudioSource> ();
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
				param.break_seconds = UnityEngine.Random.Range (break_sec_min, break_sec_max);


				param.answer_onset = 0;
				param.answer_offset = 0;
				param.answer_production = 0;

				condition_list.Add (param);
			}
		}
		//Shuffle (condition_list);

//		for(int i=0; i<10; i++){
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

		keyManager.ResetTimer ();

		keyManager.SetMouseHook();

		Message.text = "";
		if (num_training_session > 0) {

			for (int i=0; i < num_training_session; i++) {
				
				yield return StartCoroutine (runReproductionTraining (i));
			}
		}

		keyManager.UnsetMouseHook();

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


	IEnumerator runReproductionTraining(int index)
	{
		int duration_index = UnityEngine.Random.Range (0, num_duration);
		float break_seconds = UnityEngine.Random.Range (training_break_sec_min, training_break_sec_max);

		Message.text = "+";
		yield return new WaitForSeconds(break_seconds);

		Message.text = "Hold a mouse button";

		tones[duration_index].Play ();
		Debug.Log ("training actual onset!"  + " " +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds() + " " + videoCtl.GetElapsedTime());
		//long tone_onset = keyManager.GetElapsedMilliseconds();

		//-------------
		//production
		yield return StartCoroutine(WaitForMouseDown());
		long answer_onset = keyManager.GetElapsedMilliseconds();
		Message.text = "";

		yield return StartCoroutine(WaitForMousePressing());
		
		long answer_offset = keyManager.GetElapsedMilliseconds();
		float answer_production = LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown;
		Debug.Log ( (answer_offset - answer_onset) + " " +  answer_production);

		SaveTrainingData (durations[duration_index], answer_production);
		
		training_show [index] = durations[duration_index];
		training_answer [index] = answer_production;


		yield return StartCoroutine(ShowProductionFeedback(durations[duration_index], answer_production));
	}

	IEnumerator ShowProductionFeedback(float correct, float answer)
	{
		redCircle.GetComponent<Renderer>().enabled = false;	
		sliderCtl.ShowSliderForTimePerception ();
		sliderCtl.EnableSlider ();

		sliderCtl.ShowProductionFeedback (correct, answer);
		sliderCtl.DisableSlider ();
		
		yield return StartCoroutine(WaitForMouseDown());
		sliderCtl.HideFeedback ();
		sliderCtl.HideSlider ();
		
		
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
		CreateCondition ();

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

	
		{
			Instruction.text = "";
			Message.text = "Please set the VR goggle on your head if you don't\n\nClick to continue";
			yield return StartCoroutine (WaitForMouseDown ());


			SRCtl.EnableOculus ();

		
			Message.text = "";
			Instruction.text = inst_main_start_live1;
			SRCtl.GoToInstructionScreen ();
			yield return StartCoroutine (WaitForMouseDown ());
			
			Instruction.text = inst_main_start_live2;
			SRCtl.GoToInstructionScreen ();
			yield return StartCoroutine (WaitForMouseDown ());

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
			if(Tracking == TrackingMode.LIVE){

				curVideoId = 0;

			}else{

				curVideoId = 1;
			}

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


		//Experiment Start
		SRCtl.BackFromInstructionScreen (false);
		
		keyManager.SetMouseHook();

		yield return StartCoroutine (runLiveTrials());
		
		keyManager.UnsetMouseHook();
		

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
		float max_duration = video_duration;

		Debug.Log (max_duration);
		Debug.Log ("actual video " + videoCtl.GetDuration ()  + " video length " + max_duration);

		float start_time = (float)keyManager.GetElapsedMilliseconds() / 1000f;

		while( (float)keyManager.GetElapsedMilliseconds() / 1000f - start_time < max_duration){

			Debug.Log ("start of trial: counter: "+ counter + " " +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds() + " " + videoCtl.GetElapsedTime());
		//for(int i=0; i< num_trial; i++){
			Param param = condition_list[counter];

			param.videoId = curVideoId;
			//float duration_sec = durations[param.duration];
		
			yield return new WaitForSeconds(param.break_seconds);

			//-------------
			//onset
			tones[param.duration].Play ();
			Debug.Log ("actual onset!"  + " " +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds() + " " + videoCtl.GetElapsedTime());
			logger.SetLog("onset", counter, 1, param.duration); //todo	
			param.tone_onset = keyManager.GetElapsedMilliseconds();


			//-------------
			//production
			yield return StartCoroutine(WaitForMouseDown());
			param.answer_onset = keyManager.GetElapsedMilliseconds();

			yield return StartCoroutine(WaitForMousePressing());

			param.answer_offset = keyManager.GetElapsedMilliseconds();
			param.answer_production = LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown;
			Debug.Log (param.answer_production);


			SaveDataToFile(param);
			result_list.Add (param);

			SaveTrackingFile();
			logger.SaveLog();

			Debug.Log ("end of trial: " +  getSpeedinVideo() + " " + keyManager.GetElapsedMilliseconds() + " " + videoCtl.GetElapsedTime());
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
		text += p.duration + ", ";
		text += p.break_seconds + ", ";
		text += p.tone_onset + ", ";
		text += p.answer_onset + ", ";
		text += p.answer_offset + ", ";
		text += p.answer_production + ", ";
		
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


	int getSpeedinVideo(){

		return curVideoId;
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