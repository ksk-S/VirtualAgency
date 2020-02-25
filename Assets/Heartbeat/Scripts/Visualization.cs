using UnityEngine;
using System.Collections;
using System.Diagnostics; 
using Heartbeat;
using System;

namespace Heartbeat
{
	
	public class ObjectSet
	{	
		public GameObject obj;
		public Vector3 scale;
		
		public ObjectSet(GameObject o, Vector3 s)
		{
			obj = o;
			scale = s;
		}
	}
	
class Visualization : MonoBehaviour
{
	public HeartBeat heartbeat;
	
	//--------------------------------------
	//Parameters
	//--------------------------------------

	public float flash_duration_half = 0.15f;
	
	//--------------------------------------
	//variables
	//--------------------------------------
	public GameObject own;
	int counter=0;
	
	
	public Visualization()
	{	
	}
	// Use this for initialization
	void Start ()
	{
		own = GameObject.Find("CompositionEngine");
		heartbeat = GameObject.Find("CompositionEngine").GetComponent("HeartBeat") as HeartBeat;	
		
		//event handlers
		if(heartbeat.use_correct_feedback){
			heartbeat.feedback_correct.BeatOccur += new Heartbeat.Feedback.BeatOccurHandler(OnCorrectBeat);
		}		
			
		if(heartbeat.use_delayed_feedback){
			heartbeat.feedback_delayed.BeatOccur += new Heartbeat.Feedback.BeatOccurHandler(OnDelayedBeat);
		}	
			
		if(heartbeat.use_fast_feedback){
			heartbeat.feedback_fast.BeatOccur += new Heartbeat.Feedback.BeatOccurHandler(OnFastBeat);
		}
			
		if(heartbeat.use_slow_feedback){
			heartbeat.feedback_slow.BeatOccur += new Heartbeat.Feedback.BeatOccurHandler(OnSlowBeat);
		}	
			
		if(heartbeat.use_same_feedback){
			heartbeat.feedback_same.BeatOccur += new Heartbeat.Feedback.BeatOccurHandler(OnSameBeat);
		}	
	}
	
	// Update is called once per frame
	void Update ()
	{
		counter++;
		
		if(heartbeat.raw_beat_flag){
			FlashObjectByProperty(FeedbackType.Raw);
			//FlashObjectByTag("RawObjects");
			heartbeat.raw_beat_flag = false;
		}
	}
	
	void OnCorrectBeat()
	{
		FlashObjectByProperty(FeedbackType.Sync);
	}
		
	void OnDelayedBeat()
	{
		FlashObjectByProperty(FeedbackType.Delay);
	}
		
	void OnFastBeat()
	{
		FlashObjectByProperty(FeedbackType.Fast);
	}	
		
	void OnSlowBeat()
	{
		FlashObjectByProperty(FeedbackType.Slow);
	}		
	
	void OnSameBeat()
	{
		FlashObjectByProperty(FeedbackType.Same);
	}	
		
	void FLashObject(GameObject obj)
	{
		FlashProperty property = obj.GetComponent ("FlashProperty") as FlashProperty;

		if(property.ChangeSize)
		{
			ChangeObjectSize(obj, property.delay, property.ratio);
		}
		if(property.ChangeColor)
		{
			ChangeObjectColor(obj, property.delay);	
		}

	}
		
	void FlashObjectByProperty(FeedbackType type)
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("HeartbeatObjects")) 
		{	
			FlashProperty property = obj.GetComponent ("FlashProperty") as FlashProperty;
			if(property.feedback_type == type)
			{	
				if(property.ChangeSize)
				{
					ChangeObjectSize(obj, property.delay, property.ratio);
				}
				if(property.ChangeColor)
				{
					ChangeObjectColor(obj, property.delay);	
				}
			}
		}
	}
	
	/*
	void FlashObjectByTag(string tagName)
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag(tagName)) 
		{		
			FlashProperty property = obj.GetComponent ("FlashProperty") as FlashProperty;
 			
			ChangeObjectSize(obj, property.delay);
			ChangeObjectColor(obj, property.delay);		
		}
	}
	*/
	
	void ChangeObjectSize(GameObject obj, float delay, float ratio)
	{
		Vector3 scale = Vector3.Scale (obj.transform.localScale, new Vector3(ratio, ratio, ratio));
		//UnityEngine.Debug.Log (obj);
		ObjectSet oset = new ObjectSet(obj, obj.transform.localScale);
		Hashtable hash = iTween.Hash(
   			"scale", scale,
			"time", flash_duration_half,
			"delay", delay,
			"oncomplete","onCompleteScale",
			"oncompletetarget", own,
   			"oncompleteparams",oset);
  		iTween.ScaleTo (obj, hash);

	}
	void onCompleteScale(ObjectSet oset)
	{
		Hashtable hash = iTween.Hash(
   			"scale", oset.scale,
			"time", flash_duration_half
		);
  		iTween.ScaleTo (oset.obj, hash);
	}

	
	
	void ChangeObjectColor(GameObject obj, float delay)
	{
		FlashProperty property = obj.GetComponent ("FlashProperty") as FlashProperty;
		Color col = property.target_color;
		Hashtable hash = iTween.Hash(
   			"color", col,
			"time", flash_duration_half,
			"delay", delay,
			"oncomplete","onCompleteColorChange",
			"oncompletetarget",own,
   			"oncompleteparams",obj);
  		iTween.ColorTo(obj, hash);
	}
	
	void onCompleteColorChange(GameObject obj)
	{
		FlashProperty property = obj.GetComponent ("FlashProperty") as FlashProperty;
 			
		Color col = property.original_color;
		Hashtable hash = iTween.Hash(
   			"color", col,
			"time", flash_duration_half
			);
  		iTween.ColorTo(obj, hash);
	}
	
}

}