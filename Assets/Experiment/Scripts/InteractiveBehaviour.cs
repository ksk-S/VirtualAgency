using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class InteractiveBehaviour : MonoBehaviour {
	
	protected bool mouse_down = false;
	protected bool button_pressed = false;
	protected KeyCode pressedKey = KeyCode.Alpha0;

	protected string data_dir = "Results/";
	protected string out_filename;
    protected string out_global_filename;

    void Awake(){
		DoAwake ();
	}

	// Use this for initialization
	IEnumerator Start () {

		StartCoroutine("DoStart");
		yield return 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			mouse_down = true;
		}else{
			mouse_down = false;
		}

		if (Input.anyKeyDown)
		{
			button_pressed = true;
		}else{
			button_pressed = false;
		}
		DoUpdate ();
	}

	protected virtual void DoAwake(){}
	protected virtual IEnumerator DoStart(){yield return 0;}
	protected virtual void DoUpdate(){}
	
	protected IEnumerator WaitForMouseDown()
	{
		
		while(!Input.GetMouseButton(0) || !mouse_down){
			
			
			if (Input.GetMouseButtonDown (0)) {
				//Debug.Log ("hoge");
			}
			yield return new WaitForEndOfFrame();

		}
		yield return 0;
	}

	protected IEnumerator WaitForAnyKeyPress ()
	{
		while (!Input.anyKey || !button_pressed || Input.GetMouseButton(0)) {
			//Debug.Log ("Waiting for keypress");
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}
	
	
	protected IEnumerator WaitForKeyDown(KeyCode keyCode)
	{
		while(!Input.GetKey (keyCode) || !button_pressed ){
			yield return new WaitForEndOfFrame();
		}
		yield return 0;
	}

	protected IEnumerator WaitForKeysDown(KeyCode keyCode1, KeyCode keyCode2)
	{
		while((!Input.GetKey (keyCode1) && !Input.GetKey (keyCode2) ) || !button_pressed){
			yield return new WaitForEndOfFrame();
		}
		if(Input.GetKey (keyCode1))
		{
			pressedKey = keyCode1;
		}
		if(Input.GetKey (keyCode2))
		{
			pressedKey = keyCode2;
		}
		yield return 0;
	}

	protected void SetFilename(string dataname, int SubId, int day, string SessionName, int block, int VR){
		
		string dir = data_dir + "/Main/" + SubId + "/";
		Directory.CreateDirectory (dir);
		
		DateTime dt = DateTime.Now;
		out_filename  = string.Format(dir + dataname + "_" + day + "_" + SessionName + "_"+ block +  "_" + VR + "_{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
	}

    public void SetFilenameGlobal(string dataname, int SubId)
    {

        string dir = data_dir + "/Main/" + SubId + "/";
        Directory.CreateDirectory(dir);

        DateTime dt = DateTime.Now;
        out_global_filename = string.Format(dir + dataname + "_{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
    }

}