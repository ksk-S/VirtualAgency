using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;


/* Deep dream demo mode automatic transition
 * 
 * 
 */

public class DeepDreamAutomaticDemo : MonoBehaviour {

	SRController SRCtl;
	HeadTrackingRecorder headTracking;

	bool isIdeling = false;

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
		//Debug.Log (SRCtl.curVideoCtl.isPlaying);
		if(SRCtl.curVideoCtl && !SRCtl.curVideoCtl.isPlaying)
		{
			if(isIdeling == false) StartCoroutine("StartVideo");
		}
	}
	IEnumerator StartVideo () {

		isIdeling = true;

		int nextVideo = UnityEngine.Random.Range (0, SRCtl.streamNum);
		if(SRCtl.curStreamId == nextVideo){
			nextVideo = UnityEngine.Random.Range (0, SRCtl.streamNum);
		}

		Debug.Log ("DeepDreamDemo2 : goto video : " + nextVideo);

		while (!SRCtl.isReady) {
			yield return null;
		}
		SRCtl.GoToStream (nextVideo);

		Debug.Log ("Start Waiting");
		yield return new WaitForSeconds(UnityEngine.Random.Range (1.5f, 4.5f));
		Debug.Log ("End Waiting - Start Hallucinating");

		SRCtl.curVideoCtl.isLoop = false;
		SRCtl.curVideoCtl.Reset ();
		SRCtl.curVideoCtl.Play ();
		isIdeling = false;
	}
}
 