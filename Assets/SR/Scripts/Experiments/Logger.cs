using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using CommonUtil;

public class Logger : MonoBehaviour {

	
	public struct Log
	{
		public long time;
		public string eventname;
		public int index;
		public int speed;
		public int duration;
		public float fps;
	}

	List<Log> logList = new List<Log> ();

	HeartBeat heartbeat;
	ExperimentTimeInterface expCtl;

	public string data_dir = "Results/";	
	string out_filename;

	
	LowLevelInputManager keyManager;

	public  void Awake ()
	{	
		
		keyManager = gameObject.GetComponent<LowLevelInputManager> ();
		heartbeat = GameObject.Find("CompositionEngine").GetComponent<HeartBeat>();	

		expCtl = gameObject.GetComponent<ExperimentMagnitude> ();

		if(expCtl == null){
			expCtl = gameObject.GetComponent<ExperimentTimeBisection> ();
		}
		if(expCtl == null){
			expCtl = gameObject.GetComponent<ExperimentMagnitude> ();
		}
		if(expCtl == null){
			expCtl = gameObject.GetComponent<ExperimentDeepDream> ();
		}
	}

	public  void Start ()
	{	
	}

	public void Update(){
	}
	public void StartLogHeartbeat(string mode, int sessionId)
	{

		SetFilename (mode, sessionId);
		heartbeat.RawBeatOccur += OnBeat;
	}
	
	public void StopLogHeartbeat()
	{
		heartbeat.RawBeatOccur -= OnBeat;
	}


	public void SetLog(string eventName, int index, int speed, int duration){
		
		Log log = new Log ();
		
		log.time = keyManager.GetElapsedMilliseconds();
		log.eventname = eventName;
		log.duration = duration;
		log.speed = speed;
		log.index = index;
		logList.Add (log);
	}

	public void SetFPSLog(float fps){
		
		Log log = new Log ();
		
		log.time = keyManager.GetElapsedMilliseconds();
		log.eventname = "fps";
		log.duration = 0;
		log.speed = 0;
		log.index = 0;
		log.fps = fps;
		logList.Add (log);
	}
	

	
	void OnBeat()
	{
		Log log = new Log ();
		log.eventname = "heartbeat";
		log.time = keyManager.GetElapsedMilliseconds();
		log.duration = -1;
		log.speed = -1;
		log.index = -1;
		//Debug.Log ("beat " +  keyManager.GetElapsedMilliseconds());
		logList.Add (log);
	}
	
	public void SetFilename(string mode, int sessionId){
//		Debug.Log (expCtl);


		Directory.CreateDirectory (data_dir + "/" + expCtl.SubId + "/" );
		DateTime dt = DateTime.Now;
		out_filename  = string.Format(data_dir + "/" + expCtl.SubId + "/log-{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}.txt", sessionId, mode, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
	}

	public void SaveLog(){
		if(logList.Count > 0){
			string saved_contents = "";
			for(int i=0; i< logList.Count; i++)
			{
				Log _data = logList[i];
				saved_contents += _data.time + ", ";
				saved_contents += _data.eventname + ", ";
				saved_contents += _data.index + ", ";
				saved_contents += _data.speed + ", ";
				saved_contents += _data.duration + ", ";
				saved_contents += _data.fps + "";
				saved_contents += Environment.NewLine;
			}
			File.AppendAllText(out_filename, saved_contents);
			logList.Clear ();
		}
	}
}