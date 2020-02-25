using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using CommonUtil;


public class ExperimentTimePerception : MonoBehaviour {
	
	public struct Param
	{
		public int index;
		public int videoId;
		public int speed;
		public float duration;
		public int start_frame;
		
		public float answer_duration;
		public float[] answer_production;
	}

	SRController SRCtl;
	Text Instruction;
	GameObject AnswerTimeObj;
	InputField AnswerTime;

	LowLevelInputManager keyManager;
	PlayWave sound;

	string inst_text1 = "In the experiment\n" +
						"You will watch several videos.\n" +
						"You can freely look around during the video playing.\n" +
						"\n" + 
						"Press a button to Continue";

	string inst_text2 = "After watching video,\n" +
						"You are asked to estimate the duration of the video.\n" +
						"Please not count time during the video .\n" +
						"\n" + 
						"Press a button to Continue";


	string inst_text3 = "In the beginning of the experiment, " +
						"You will hear a tone lasting for several seconds. " +
						"At each point, you are asked to reproduce the same length of time by pressing button. " +
						"Again, please not count time when you hear the sound.\n\n" +
						"Press a button to Continue";


	string time_reproduction1 = "You will hear a tone lasting for several seconds\n\n" +
								"Pree Enter to Start";

	string time_reproduction2 = "Now, duplicate the time of sounds by keeping button pressed \n\n" +
								"Hold a button";


	string inst_text_test1 = "How long the movie was?";

	string inst_text_test2 = "Duplicate the time of sounds you heard at the beginning  by pressing a button";

	string inst_text_test3 = "Button Pressed";


	string out_filename;
	int SubId = 1;

	float answer_video_duration = 0.0f;

	public static float duration_min = 30f;
	public static float duration_max = 50f;

	public static int frame_min = 0;
	public static int frame_max = 200;


	public static int num_video = 2;
	public static int num_speed = 3;
	public static int num_timeproduction = 3;

	public static float timeproduction_duration = 3.0f;

	public Param[] result_list = new Param[num_video * num_speed];
	public float[] preTask_list = new float[num_timeproduction];

	System.Random Rn = new System.Random(System.Environment.TickCount);

	bool button_pressed = false;

	bool isRunning = false;

	void CreateCondition()
	{
		for(int i=0; i< num_video; i++){
			for(int j=0; j<num_speed; j++){
				Param param = new Param();
				param.index   = i * num_speed + j;
				param.videoId = i;
				param.speed   = j;
				param.duration = UnityEngine.Random.Range(duration_min, duration_max);
				param.start_frame = UnityEngine.Random.Range(frame_min, frame_max);
				param.answer_duration = -1;
				param.answer_production = new float[num_timeproduction];;

				//Debug.Log (param.index + " "+ param.speed + " " + param.duration);
				result_list[i * num_speed + j] = param;
			}
		}
		Shuffle(result_list, num_video * num_speed);


	}

	void Awake ()
	{	
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		Instruction = GameObject.Find ("InstructionText").GetComponent<Text> ();
		keyManager = gameObject.GetComponent<LowLevelInputManager> ();
		sound = GameObject.Find ("CameraRight").GetComponent<PlayWave> ();

		AnswerTimeObj = GameObject.Find ("AnswerTime");
		AnswerTime =  AnswerTimeObj.GetComponent<InputField> ();

	}
	
//	void	Start () {
	void Start () {
		AnswerTimeObj.SetActive (false);

		CreateCondition ();

	}
	
	IEnumerator runExperiment () {
	
		Debug.Log("Experiment Start");
		SetFilename ();
		SRCtl.disableControll = true;

		Instruction.text = inst_text1;
		SRCtl.GoToInstructionScreen ();

		yield return 0;

		yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));
	
		Instruction.text = inst_text2;

		yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));

		Instruction.text = inst_text3;
		yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));


		yield return StartCoroutine (runPreTask());

		yield return StartCoroutine (runMain());

		AnswerTimeObj.SetActive (false);

		Instruction.text = "Finished!";
		SRCtl.GoToInstructionScreen ();

		yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));
		SRCtl.BackFromInstructionScreen ();
		SRCtl.disableControll = false;
		isRunning = false;
		
	}

	private IEnumerator runPreTask()
	{
		Instruction.text = time_reproduction1;

		yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));

		sound.isPlaying = true;
		yield return new WaitForSeconds(timeproduction_duration);

		sound.isPlaying = false;


		for( int j=0; j < num_timeproduction; j++){
			
			Instruction.text = time_reproduction2;
			keyManager.SetKeyboardHook();
			yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));
			
			Instruction.text = inst_text_test3;
			sound.isPlaying = true;
			
			yield return StartCoroutine(WaitForKeyPressing(KeyCode.Return));
			
			sound.isPlaying = false;
			preTask_list[j] = LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown;
		//	result_list[i].answer_production[j] = LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown;

			keyManager.UnsetKeyboardHook();
		}
		
		SaveFilePreTask();

	}

	private IEnumerator runMain()
	{
		for (int i=0; i< num_video * num_speed; i++) 
		{
			Instruction.text = "Block " + (i+1) + "\n" + "Press Enter to Start";
			SRCtl.GoToStream(result_list[i].index);
			SRCtl.ChangeStartFrame(result_list[i].start_frame);
		

			yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));

			SRCtl.BackFromInstructionScreen ();
			SRCtl.TransitionToRecord();
			Debug.Log (i + " " + +result_list[i].index + " " +result_list[i].duration);
			yield return new WaitForSeconds(result_list[i].duration);

			//answer duration
			Instruction.text = inst_text_test1;
			SRCtl.GoToInstructionScreen ();
			AnswerTimeObj.SetActive (true);

			yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));
			result_list[i].answer_duration = answer_video_duration;
			answer_video_duration = 0.0f;
			AnswerTimeObj.SetActive (false);

			for( int j=0; j < num_timeproduction; j++){

				Instruction.text = inst_text_test2;
				keyManager.SetKeyboardHook();
				yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));
				
				Instruction.text = inst_text_test3;
				sound.isPlaying = true;

				yield return StartCoroutine(WaitForKeyPressing(KeyCode.Return));

				sound.isPlaying = false;
				result_list[i].answer_production[j] = LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown;
				Debug.Log ((LowLevelInputManager.lastKeyUp - LowLevelInputManager.lastKeyDown));
				keyManager.UnsetKeyboardHook();
			}

			SaveFile(i);
		}
	}

	public void SetFilename(){
		DateTime dt = DateTime.Now;
		out_filename  = string.Format("Results/sub{0}-{1}-{2}-{3}-{4}-{5}-{6}.txt", SubId, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
	}


	void SaveFilePreTask(){
		string saved_contents = "";
		for (int i=0; i< num_timeproduction; i++) {
			saved_contents += preTask_list[i] + ", ";
		}
		saved_contents +=   Environment.NewLine;
		File.AppendAllText(out_filename, saved_contents);
	}

	void SaveFile(int trial)
	{
		int i = trial;
		string saved_contents = "";
		saved_contents += i + ", ";
		saved_contents += result_list[i].index + ", ";
		saved_contents += result_list[i].videoId + ", ";
		saved_contents += result_list [i].speed + ", ";
		saved_contents += result_list [i].duration + ", ";
		saved_contents += result_list [i].answer_duration + ", ";
		for (int j=0; j< num_timeproduction; j++) {
			saved_contents += result_list[i].answer_production[j] + ", ";
		}
		saved_contents +=   Environment.NewLine;
		
		File.AppendAllText(out_filename, saved_contents);
	}


	private IEnumerator WaitForKeyDown(KeyCode keyCode)
	{
		while(!Input.GetKey (keyCode) || !button_pressed){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	
	private IEnumerator WaitForKeyPressing(KeyCode keyCode)
	{
		while(Input.GetKey (keyCode)){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	private IEnumerator WaitForKeyUp(KeyCode keyCode)
	{
		while(!Input.GetKey (keyCode) || button_pressed){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	private IEnumerator WaitForAnyKeyPress ()
	{
		while (!Input.anyKey || !button_pressed ) {
			//Debug.Log ("Waiting for keypress");
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


		if (Input.GetKeyDown (KeyCode.Return) && !isRunning)
		{
			isRunning = true;
			StartCoroutine ("runExperiment");
		}

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

		AnswerTime.text = string.Format ("{0,3:F1}", answer_video_duration);

	}



	public void Shuffle(Param[] ary, int num_ary)
	{
		int n=num_ary;
		while (n > 1)
		{
			n--;
			int k = Rn.Next(n + 1);
			Param tmp = ary[k];
			ary[k] = ary[n];
			ary[n] = tmp;
		}
		
	}

	
}
