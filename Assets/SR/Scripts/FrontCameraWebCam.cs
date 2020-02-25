using UnityEngine;
using System.Collections;

public class FrontCameraWebCam : FrontCameraInterface
{
	WebCamDevice[] deviceMem;
	WebCamTexture webcamTexture;
	WebCamTexture webcamTextureOrg;

	// Use this for initialization
	public override void Start () {

       GetComponent<Renderer>().material.mainTextureScale = new Vector2( 1f, 1f);
       GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0f);
		

		WebCamDevice[] devices = WebCamTexture.devices;
		if (devices.Length > 0)
		{

			webcamTextureOrg = new WebCamTexture(devices[1].name);
			Debug.Log ("WebCam Width:" + webcamTextureOrg.width + " Height:" + webcamTextureOrg.height + " FPS:" + webcamTextureOrg.requestedFPS);
        	webcamTexture = new WebCamTexture(devices[1].name, 640, 480, 75);
			
			GetComponent<Renderer>().material.mainTexture = webcamTexture;
			
			webcamTexture.Play();
			
		} else {
			Debug.LogError("can't find a web camera");
		}
		
	}
}