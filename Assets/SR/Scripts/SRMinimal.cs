using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;

public class SRMinimal : MonoBehaviour {
	
	//GameObjects
	GameObject OculusCamera;
	GameObject WebCamScreen;	
	GameObject DomeScreen;
	GameObject AVProDome;



	//Scripts
//	OVRManager OculusCamCtl;
//	OVRCameraRig OculusCamRig;

	public DomeVideoInterface videoCtl;

    FrontCameraInterface frontCamCtl;
	
	[HideInInspector]
	public Camera OVRLiveCameraRight;
	[HideInInspector]
	public Camera OVRLiveCameraLeft;
	[HideInInspector]
	public Camera OVRStreamCameraRight;
	[HideInInspector]
	public Camera OVRStreamCameraLeft;
	[HideInInspector]
	public GameObject OVRCentreAnchor;


	//status
	public Status live_status = Status.LIVE;
	public int streamId = 0;
	public int streamNum = 0;
    public bool useFrontCamuEye = true;
	public bool disable_controll = false;
	
	public bool useDomeAVPro = true;


	// FrameFps
	public float fps;
	 float fps_updateInterval = 1.0f; 
	 float fps_accumulated   = 0.0f;
	 float fps_timeUntilNextInterval; 
	 int fps_numFrames = 0;

	

	void Awake ()
	{	

		//OculusCamCtl = GameObject.Find ("OVRCameraRig").GetComponent<OVRManager>();
		//OculusCamRig = GameObject.Find ("OVRCameraRig").GetComponent<OVRCameraRig>();

		WebCamScreen = GameObject.Find ("FrontCamScreen") as GameObject;
		DomeScreen = GameObject.Find ("DomeVideoScreen") as GameObject;
		AVProDome = GameObject.Find ("AVProDome") as GameObject;
	}
	
	IEnumerator	Start () {
//	Enumerator Start () {

	
	
		//Eye separation
		//OculusCamCtl.monoscopic = true;
		//OculusCamCtl.usePositionTracking = true;

	//	OVRCameraRig rig = GameObject.Find ("OVRCameraRigLive").GetComponent<OVRCameraRig> ();
	//	rig.IsPositionUpdate = false;
	//	rig.IsRotationUpdate = false;
		
	//	OculusCamRig.IsPositionUpdate = false;


		WebCamScreen.SetActive (true);

		if(useDomeAVPro)
		{
			AVProDome.SetActive (true);
			DomeScreen.SetActive(false);

			videoCtl = AVProDome.GetComponent ("DomeVideoAVPro") as DomeVideoInterface;
		}else{
			AVProDome.SetActive (false);
			DomeScreen.SetActive (true);

			videoCtl = DomeScreen.GetComponent ("DomeVideoMovieTexture") as DomeVideoInterface;
		}
		videoCtl.enabled = true;


		//Front Camera Selector
		if(useFrontCamuEye)
		{
			frontCamCtl = WebCamScreen.GetComponent("FrontCamerauEye") as FrontCameraInterface;
			frontCamCtl.enabled = true;
		}else
		{
			OVRLiveCameraRight.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
			OVRLiveCameraLeft.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
			OVRLiveCameraRight.GetComponent<Camera>().backgroundColor = Color.black;
			OVRLiveCameraLeft.GetComponent<Camera>().backgroundColor = Color.black;
			WebCamScreen.transform.localScale = new Vector3(5f,  1f, 3.75f);
			WebCamScreen.transform.localPosition = new Vector3(0f,  0f, 25f);

			BarrelMesh bm = WebCamScreen.GetComponent<BarrelMesh>();
			bm.k1 = 1.0f; bm.k2 = 0.0f; bm.k3 = 0.0f; bm.k4 = 0.0f; 
			bm.Reset ();

			frontCamCtl = WebCamScreen.GetComponent("FrontCameraWebCam") as FrontCameraInterface;
			frontCamCtl.enabled = true;
		}

		//file loading
		DomeVideoStorage domeStorage = GameObject.Find ("DomeVideoStorage").GetComponent<DomeVideoStorage>();
		if(useDomeAVPro)
		{
			yield return domeStorage.StartCoroutine("LoadAVProVideos");
		}else{
			yield return domeStorage.StartCoroutine("LoadVideos");
		}
		streamNum = domeStorage.videoNum;
		Debug.Log ("Video Ready!!!");

		GoToStream (0);
		if(live_status == Status.LIVE)
		{
			ToLive ();
		}else if(live_status == Status.STREAM){
			ToRecord ();
		}


		//Debug.Log ("Ready!!!");
		//StartCoroutine("SetRenderingStart");

//		Render.SetHook ();
	}

	public void TransitionToLive(){
		ToLive ();
	}
	public void TransitionToRecord(){

		ToRecord ();
	}





	
	public void ToLive(){
		
		
		live_status = Status.LIVE;

	//	OculusCamRig.IsRotationUpdate = false;
		
		videoCtl.SetBackGround ();
		frontCamCtl.SetForeGround();


		WebCamScreen.GetComponent<Renderer>().enabled = true;
			

	}
	
	public void  ToRecord(){
		
		live_status = Status.STREAM;		

	//	OculusCamRig.IsRotationUpdate = true;

		frontCamCtl.SetBackGround();
		videoCtl.SetForeGround ();

	}

	// Update is called once per frame
	void Update () {
		if(disable_controll) return;

		ManageKeyboard();
		ShowFPS ();
	}
	
	public void GoToStream(int id)
	{
		streamId = id;

		if(live_status == Status.STREAM)
		{
			videoCtl.GoToStream(streamId);
		}else if(live_status == Status.LIVE){

			videoCtl.GoToStreamBackground(streamId);
		}
	}
	//Calibration Modes
	void SetObjTextureTiling(GameObject obj, Vector2 scale, Vector2 offset)
	{
		obj.GetComponent<Renderer>().material.mainTextureScale = scale;
		obj.GetComponent<Renderer>().material.mainTextureOffset = offset;
	}


	
	/* 
	Keyboard Assign
	*/
	void KeyboardUEyeParam()
	{
		//uEye camera parameters
		if(Input.GetKey(KeyCode.F1))
		{
            frontCamCtl.DecExposure();
		}else if(Input.GetKey(KeyCode.F2))
		{
            frontCamCtl.IncExposure();
		}else if(Input.GetKey(KeyCode.F3))
		{
            frontCamCtl.DecGain();
		}else if(Input.GetKey(KeyCode.F4))
		{
            frontCamCtl.IncGain();
		}else if(Input.GetKey(KeyCode.F5))
		{
            frontCamCtl.DecSaturation();
		}else if(Input.GetKey(KeyCode.F6))
		{
            frontCamCtl.IncSaturation();
		}else if(Input.GetKey(KeyCode.F7))
		{
            frontCamCtl.DecGamma();
		}else if(Input.GetKey(KeyCode.F8))
		{
            frontCamCtl.IncGamma();
		}else if(Input.GetKey(KeyCode.F9))
		{
            frontCamCtl.DecTemp();
		}else if(Input.GetKey(KeyCode.F10))
		{
            frontCamCtl.IncTemp();
		}else if(Input.GetKey(KeyCode.F11))
		{
            frontCamCtl.DecAOI();
		}else if(Input.GetKey(KeyCode.F12))
		{
            frontCamCtl.IncAOI();
		}
	}

	
	void IncreaseStreamId()
	{

		streamId++;
			
		if(streamId > streamNum - 1)
		{
			streamId = 0;
		}
		GoToStream(streamId);	
	}
	
	void DecreaseStreamId()
	{	

		streamId--;
		if(streamId < 0)
		{
			streamId = streamNum - 1;
		}
		GoToStream(streamId);	
	}

	
	void KeyboardSRControl()
	{
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            Debug.Log("up - live");
			ToLive();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            Debug.Log("down - rocord");
			ToRecord();
		}

		if (Input.GetKeyDown(KeyCode.T) ) //Goto Streams
		{
		//	OculusCamRig.transform.localRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);
			//OculusCamCtl.SetOrientationOffset(Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up));
		
			//OVRDevice.ResetOrientation ();
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.JoystickButton4))
    	{
			DecreaseStreamId();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.JoystickButton6))
		{
			IncreaseStreamId();
   		}else if (Input.GetKeyDown("1") ) //Goto Streams
		{
			Debug.Log("GoTo 1");
			GoToStream(0);
		}else if (Input.GetKeyDown("2") )
		{
			Debug.Log("GoTo 2");
			GoToStream(1);
   		}else if (Input.GetKeyDown("3"))
    	{
			Debug.Log("GoTo 3");
			GoToStream(2);
   		}else if (Input.GetKeyDown("4") )	
    	{
			Debug.Log("GoTo 4");
			GoToStream(3);
   		}else if (Input.GetKeyDown("5"))
    	{
			Debug.Log("GoTo 5");
			GoToStream(4);
   		}else if (Input.GetKeyDown("6"))
    	{
			Debug.Log("GoTo 6");
			GoToStream(5);
   		}else if (Input.GetKeyDown("7"))
    	{
			Debug.Log("GoTo 7");
			GoToStream(6);
   		}else if (Input.GetKeyDown("8"))
    	{
			Debug.Log("GoTo 8");
			GoToStream(7);
   		}else if  (Input.GetKeyDown("9"))
    	{
			Debug.Log("GoTo 9");
			GoToStream(8);
   		}
	}


	
	void ManageKeyboard()
	{
		KeyboardSRControl();
		if(Input.GetKey (KeyCode.LeftControl))
		{
			KeyboardUEyeParam();
		}
	}

	
	
	void ShowFPS(){
		fps_timeUntilNextInterval -= Time.deltaTime;
		fps_accumulated += Time.timeScale / Time.deltaTime;
		++fps_numFrames;
		
		if( fps_timeUntilNextInterval <= 0.0 )
		{
			fps = fps_accumulated / fps_numFrames;
			fps_timeUntilNextInterval = fps_updateInterval;
			fps_accumulated = 0.0F;
			fps_numFrames = 0;
		}
	}
}
