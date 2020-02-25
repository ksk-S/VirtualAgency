using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;


public class DeepDreamGazingDemo : MonoBehaviour {

	SRController SRCtl;
	HeadTrackingRecorder headTracking;


	public enum Status
	{
		Nothing, Hallucinating, Reverting
	}

	Status status = Status.Nothing;
	bool previous_moving_status = false;
	// Use this for initialization
	bool isUpdateStream = true;

	void Start () {

		//OculusCamCtl = GameObject.Find ("OVRCameraController").GetComponent<OVRManager>();
		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		headTracking = GameObject.Find ("SRController").GetComponent<HeadTrackingRecorder>();

		headTracking.IsCalcHeadSpeed = true;


	}


	// Update is called once per frame
	void Update () {

		if (!SRCtl.useDomeVideo) {
			return;		
		}

		bool moving = headTracking.IsHeadMoving ();

		if(!moving && previous_moving_status){
			//Debug.Log ("Stop Moving");
			//stop moving
			if(status == Status.Nothing){

				SRCtl.curVideoCtl.Reset();
				SRCtl.curVideoCtl.isLoop = false;
				
				StartCoroutine ("WaitForStartVideo");
			}
		}

		if(moving && !previous_moving_status){
			//start moving

				StopCoroutine ("WaitForStartVideo");

				SRCtl.curVideoCtl.Stop();

				if(status == Status.Hallucinating){
					StartCoroutine ("RevertVideo");
				}

			//Debug.Log ("Start Moving");
		}

		

		if ( status == Status.Nothing && !isUpdateStream) {
			int nextVideo = UnityEngine.Random.Range (0, SRCtl.streamNum);
			Debug.Log ("goto video" + nextVideo);
			SRCtl.GoToStream (nextVideo);
			SRCtl.curVideoCtl.Reset ();
			isUpdateStream = true;
		}
		previous_moving_status = moving;

	}


	IEnumerator WaitForStartVideo () {
		Debug.Log ("Start Waiting");
		yield return new WaitForSeconds(UnityEngine.Random.Range (0.5f, 2.5f));
		Debug.Log ("End Waiting - Start Hallucinating");

		status = Status.Hallucinating;
		SRCtl.curVideoCtl.Play ();

	}

	IEnumerator RevertVideo () {
		status = Status.Reverting;
		Debug.Log ("Reverting");

		/*
		int num_video = 5;

		int revertId = 0 + num_video;
		Debug.Log (revertId);

		uint totalFrames = SRCtl.curVideoCtl.GetDurationFrame ();
		uint currentFrame = SRCtl.curVideoCtl.GetPositionFrame ();
		SRCtl.curVideoCtl.storage.AVProVideos[revertId].SetPositionFrame (totalFrames - currentFrame);
		SRCtl.curVideoCtl.storage.AVProVideos[revertId]._loop = false;
		SRCtl.curVideoCtl.storage.AVProVideos[revertId].SetPlaybackRate (2.0f);

		SRCtl.curVideoCtl.Stop();
		SRCtl.GoToStream(revertId);
		SRCtl.curVideoCtl.Play();

		*/

		status = Status.Nothing;
		SRCtl.curVideoCtl.Stop();
		SRCtl.curVideoCtl.Reset ();

		yield return new WaitForSeconds (0.7f);
		isUpdateStream = false;


	}

}
