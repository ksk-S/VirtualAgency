using UnityEngine;
using System.Collections;

public class WebCamera : MonoBehaviour {
	
	public GameObject AudioObject;
	public AudioPlay audioCtl;
	
	WebCamDevice[] deviceMem;
	WebCamTexture webcamTexture;
	WebCamTexture webcamTextureOrg;
	
	
	void Awake(){
		audioCtl = AudioObject.GetComponent("AudioPlay") as AudioPlay;	
	}
	// Use this for initialization
	void Start () {
		
		WebCamDevice[] devices = WebCamTexture.devices;
		if (devices.Length > 0)
		{

			webcamTextureOrg = new WebCamTexture(devices[0].name);
			Debug.Log ("WebCam Width:" + webcamTextureOrg.width + " Height:" + webcamTextureOrg.height + " FPS:" + webcamTextureOrg.requestedFPS);
        	webcamTexture = new WebCamTexture(devices[0].name, 640, 480, 75);
			
			GetComponent<Renderer>().material.mainTexture = webcamTexture;
			
			webcamTexture.Play();
			
		} else {
			Debug.LogError("can't find a web camera");
		}
		
	}
}