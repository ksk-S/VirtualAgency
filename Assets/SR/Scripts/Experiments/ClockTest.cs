using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using CommonUtil;

public class ClockTest : MonoBehaviour {

	LowLevelInputManager keyManager;

	bool isRunning = true;
	bool button_pressed = false;
	string out_filename;

	List<float> result = new List<float> ();

	void Awake ()
	{	
		keyManager = gameObject.GetComponent<LowLevelInputManager> ();
	}
	
//	void	Start () {
	IEnumerator Start () {

		Application.targetFrameRate = 120;


		SetFilename ();

		keyManager.SetKeyboardHook();
		while(isRunning)
		{
			yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));
			Debug.Log (LowLevelInputManager.lastKeyUp + " " + LowLevelInputManager.lastKeyDown);
			result.Add(LowLevelInputManager.lastKeyDown);
		}
		keyManager.UnsetKeyboardHook();

	}


	public void SetFilename(){
		DateTime dt = DateTime.Now;
		out_filename  = string.Format("Results/clockTest-{0}-{1}-{2}-{3}-{4}-{5}.txt",  dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
	}


	void SaveFile(){
		string saved_contents = "";

		foreach(float value in result)
		{
			saved_contents += value + Environment.NewLine;
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
		
	void Update()
	{
		if (Input.anyKeyDown)
		{
			button_pressed = true;
		}else{
			button_pressed = false;
		}


		float yr = 20 * Time.deltaTime;
		transform.Rotate(0, yr, 0);
		if (Time.frameCount % Application.targetFrameRate == 0)
		{
			print("FPS=" + 1 / Time.deltaTime);
		}

		if (Input.GetKeyDown (KeyCode.Escape))
		{
			SaveFile();
			
			Debug.Log ("finished");
			Application.Quit ();
		}


	}




	
}
