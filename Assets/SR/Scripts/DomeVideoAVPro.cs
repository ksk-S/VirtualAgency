using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FileManagingSpace;

public class DomeVideoAVPro : DomeVideoInterface {

	//public MovieTexture cur_video;
	public AVProWindowsMediaMovie curVideo;
	public SRController srCtl;

	//public System.EventHandler loopEvent;
	//public System.EventHandler finishEvent;

	public override bool isPlaying {
		get{return (curVideo != null) ? curVideo.isPlaying : false;}
	}

	public override bool isLoop
	{
		set{
			loop = value;
			if(curVideo != null){
				curVideo._loop = loop;
			}
		}
		get{
			if(curVideo != null){
				return curVideo._loop;
			}else{
				return true;
			}
		}
	}

	public override void Awake () {
		
		base.Awake ();
		storage = GameObject.Find ("DomeVideoStorage").GetComponent<DomeVideoStorage>();
		srCtl = GameObject.Find ("SRController").GetComponent<SRController>();

	}
	// Use this for initialization
	void Start () {

		//renderer.material.SetFloat ("_Param", alpha_param);

		Debug.Log ("start avpro : num video " + storage.videoNum);
		StartCoroutine ("WaitUntilVideoLoad");
	}

	
	public override IEnumerator WaitUntilVideoLoad(){
		
		while (!storage.isReady)
			yield return 0;
		
		Debug.Log ("Finish Loading Videos");

		isLoaded = true;
		
		//SetVideo(streamId);
		
	}

	public override void SetVideo(int id){
        //Debug.Log (id);
        //		Debug.Log (gameObject.name);

        //Debug.Log("numver of vidoes: " + storage.ContentContainer.Count);
        VideoImageElement curElem = storage.ContentContainer[id];

        if(curElem.isImage)
        {
           
            Vector3 rot = GameObject.Find("AVProContainer").transform.eulerAngles;
            rot.z = 0f;
            GameObject.Find("AVProContainer").transform.eulerAngles = rot; 
            
            //   Debug.Log("Set Image");
            curVideo = null;
            GetComponent<Renderer>().material.mainTexture = curElem.imageTexture;
        }
        else
        {
            /*
            Vector3 rot = GameObject.Find("AVProContainer").transform.eulerAngles;
            rot.z = 180f;
            GameObject.Find("AVProContainer").transform.eulerAngles = rot;
            */


            //GameObject.Find("AVProContainer").transform.eulerAngles = new Vector3(0f, 270f, 180f);

            // Debug.Log("Set Movie");
            curVideo = curElem.AVProVideo;
            curVideo._volume = 0.0f;
           
            //curVideo._loop = true;

            //Debug.Log ("Set Vdieo : " + id);

            //EventHandler Flash

            for (int i = 0; i < storage.videoNum; i++)
            {
                //Debug.Log ("Remove Handler");
                if (!storage.ContentContainer[i].isImage)
                {
                    storage.ContentContainer[i].AVProVideo.loopEvent -= onLoop;
                    storage.ContentContainer[i].AVProVideo.finishEvent -= onFinish;
                }
            }
            curVideo.loopEvent += onLoop;
            curVideo.finishEvent += onFinish;

            if (curVideo.OutputTexture != null)
                GetComponent<Renderer>().material.mainTexture = curVideo.OutputTexture;
        }

    }



	public override void SetForeGround(bool PlayVideoOnStart=true)
	{	
//		Debug.Log ("Set Fore Ground : " + gameObject.name);
		//cur_video.Stop ();
		//elapsedVideoTime [streamId] = 0.0f;
		if(PlayVideoOnStart && curVideo != null)
        {
			curVideo.Play();

            VideoImageElement curElem = storage.ContentContainer[srCtl.curStreamId];
            if (curElem.isEmbededAudio)
            {
                curVideo._volume = 1.0f;
            }
        }
		foreground = true;
	}
	
	override public void SetBackGround(bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
		Debug.Log ("AVPro SetBackGround: reset " + ResetVideoOnStop + ", pause " + PauseVideoOnBackground);
//		Debug.Log ("Set Back Ground : " + gameObject.name);
		if(curVideo != null && PauseVideoOnBackground){
			curVideo.Pause ();// stop originally
		}

		if(ResetVideoOnStop && curVideo != null)
        {
			curVideo.Reset ();
		}
		foreground = false;
	}

	public override void GoToStream(int index, bool PlayVideoOnStart=true, bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
		GoToStreamBackground(index, ResetVideoOnStop, PauseVideoOnBackground);
		if(PlayVideoOnStart && curVideo != null)
        {
			curVideo.Play();

            VideoImageElement curElem = storage.ContentContainer[index];
            if (curElem.isEmbededAudio)
            {
                curVideo._volume = 1.0f;
            }
        }
	}
	public override void GoToStreamBackground(int index, bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
		if(curVideo != null && PauseVideoOnBackground){
			curVideo.Pause ();// stop originally
		}
		
		if(curVideo != null && ResetVideoOnStop){
			curVideo.Reset ();
		}

	
		streamId = index;
		SetVideo(streamId);
	}

	public override void Reset () {
		//cur_video.Stop ();
		if(curVideo != null){
			curVideo.Reset ();
		}
//		Debug.Log (streamId);
	}

	public override void Reset (int id) {
        if(!storage.ContentContainer[id].isImage)
            storage.ContentContainer[id].AVProVideo.Reset ();
	}

	
	
	public override void Stop () {
		if(curVideo != null){
			curVideo.Pause ();
		}
	}
	
	public override void Play () {
		if (curVideo != null) {
			curVideo.Play ();
		}
	}


	public override float GetElapsedTime()
	{
		if (curVideo != null) { 
			return curVideo.GetElapsedSeconds ();
		} else {
			return 0;
		}
	}

	public override float GetDuration()
	{
		return curVideo.GetDuration ();
	}

	public override uint GetDurationFrame () {
		if(curVideo != null) return curVideo.GetDurationFrame ();
		return 0;
	}

	public override void Seek(float second){

		curVideo.SetElapsedSeconds (second);
		// not implemented
	}

	public override void SetPositionFrame (uint frame) {

		if(curVideo != null)
			curVideo.SetPositionFrame (frame);
	}
	
	public override uint GetPositionFrame () {
		if(curVideo != null)
			return curVideo.GetPositionFrame ();

		return 0;
	}

	public override void SetPlaybackRate(float speed)
	{
		if(curVideo != null)	curVideo.SetPlaybackRate (speed);
	}

	public override float GetPlaybackRate()
	{
		if(curVideo != null) return curVideo.GetPlaybackRate ();
		return 1.0f;
	}


	public override void SetAlpha(float alpha)
	{
		alpha_param = alpha;
		GetComponent<Renderer>().material.SetFloat ("_Param", alpha_param);
	}

	private void onLoop(object sender, System.EventArgs e){
		//Debug.Log ("AVPro video onLoop");
		//loopEvent.Invoke(this, System.EventArgs.Empty);
		
		if (srCtl.useAudio) {
			srCtl.AudioCtl.ReplayAudio();
		}
	}

	public void onFinish(object sender, System.EventArgs e){
		//Debug.Log ("AVPro video onFnish");
		//finishEvent.Invoke(this, System.EventArgs.Empty);


	}

	// Update is called once per frame
	public override void Update () {

		/*
		if (AudioCtl.IsNearEnd())
		{
			AudioCtl.ReplayAudio();
			Reset ();
		}
*/

		//if(curVideo != null)
		//	Debug.Log (" Current Video :" + curVideo.GetElapsedSeconds() + " / " + curVideo.GetDuration ());

	//	renderer.material.SetFloat ("_Param", alpha_param);

		/* TODO

		if (curVideo != null) {


			isPlaying = curVideo.isPlaying;
			duration = curVideo.duration;

			if(foreground)
			{
				if(!curVideo.isPlaying)
				{
					//curVideo.Stop (); how to stop
					elapsedVideoTime [streamId] = 0.0f;
					curVideo.Play ();
				}else{
					elapsedVideoTime [streamId] += Time.deltaTime;
				}
			}
		 }
*/
	

	}
}
