using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FileManagingSpace;

public class DomeVideoMovieTexture : DomeVideoInterface {

	public MovieTexture cur_video;


	public override bool isPlaying {
		get{return cur_video.isPlaying;}
	}

	public override void Awake () {
		
		base.Awake ();

	}
	// Use this for initialization
	void Start () {
		elapsedVideoTime = new float[storage.videoNum];

		StartCoroutine ("WaitUntilVideoLoad");
	}

	
	public override IEnumerator WaitUntilVideoLoad(){

		while (!storage.isReady)
			yield return 0;

		Debug.Log ("Finish Loading Videos");

		isLoaded = true;

		SetVideo(streamId);

	}

	public override void SetVideo(int id){
		cur_video = storage.videos[id];
		GetComponent<Renderer>().material.mainTexture = cur_video;
		GetComponent<Renderer>().material.SetTexture("_MainTex", cur_video);
	}


	public override void SetForeGround(bool PlayVideoOnStart=true)
	{	
		//Debug.Log ("video stopped " + cur_video.name);

		if(PlayVideoOnStart)
		{
			cur_video.Stop ();
			elapsedVideoTime [streamId] = 0.0f;
			cur_video.Play();
		}
		foreground = true;
	}


	public override void SetBackGround(bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
		//Debug.Log ("Set Back Ground : " + gameObject.name);
		if(cur_video != null && PauseVideoOnBackground){
			cur_video.Pause ();// stop originally
		}
		
		if(ResetVideoOnStop){
			cur_video.Stop ();
		}
		foreground = false;



		if(cur_video != null && ResetVideoOnStop){
			cur_video.Stop();
		}
		foreground = false;
	}

	public override void GoToStream(int index, bool PlayVideoOnStart=true, bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
		GoToStreamBackground(index, ResetVideoOnStop, PauseVideoOnBackground);

		if(PlayVideoOnStart){
			cur_video.Play();
		}
	}

	public override void GoToStreamBackground(int index, bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
		if(cur_video != null && PauseVideoOnBackground){
			cur_video.Pause ();// stop originally
		}
		
		if(ResetVideoOnStop){
			cur_video.Stop ();
		}
		/*
		Debug.Log ("video all stopped " + cur_video.name);
		foreach(MovieTexture elem in storage.videos)
		{
			elem.Stop ();
		}
		*/
		streamId = index;
		SetVideo(streamId);
	
	}
	public override void Seek(float second){
		// not implemented
	}

	public override void Reset () {
		cur_video.Stop ();
		elapsedVideoTime [streamId] = 0.0f;
		cur_video.Play ();
	}

	public override void Reset (int id) {
		storage.videos[id].Stop ();
		elapsedVideoTime [id] = 0.0f;
	}

	public override float GetElapsedTime()
	{
		return elapsedVideoTime [streamId];
	}

	public override float GetDuration()
	{
		return cur_video.duration;
	}

	public override uint GetDurationFrame () {
		return 0;
	}

	public override void Stop () {
		cur_video.Stop ();
	}

	public override void Play () {
		cur_video.Play ();
	}

	public override void SetPositionFrame (uint frame) {
		//not implemented
	}

	public override uint GetPositionFrame () {
		return 0;
		//not implemented
	}


	// Update is called once per frame
	public override void Update () {
		//renderer.material.SetFloat ("_Param", alpha_param);
		if (cur_video != null) {

			/*
			if (AudioCtl.IsNearEnd())
			{
				AudioCtl.ReplayAudio();
				Reset ();
			}
			*/

			if(foreground)
			{
				if(loop && !cur_video.isPlaying)
				{
					cur_video.Stop ();
					elapsedVideoTime [streamId] = 0.0f;
					cur_video.Play ();
				}else{
					elapsedVideoTime [streamId] += Time.deltaTime;
				}
			}
		 }

	

	}
}
