using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;
using RenderingAPI;
using FileManagingSpace;

public class SRController : MonoBehaviour {

	// FrameFps
	public float fps;
	float fps_updateInterval = 1.0f; 
	float fps_accumulated   = 0.0f;
	float fps_timeUntilNextInterval; 
	int fps_numFrames = 0;

	//GameObjects
	//GameObject TrackingSpaceStream;
	//GameObject TrackingSpaceLive;

	GameObject WebCamScreen;	
	GameObject LadybugScreen;

	GameObject DomeContainer;
	GameObject DomeScreen;
	GameObject DomeScreenAlt;

	GameObject AVProContainer;
	GameObject AVProDome;
	GameObject AVProDomeAlt;

	GameObject instCanvas;

	GameObject normalCamera;

	//Scripts
	//[HideInInspector]
//	[HideInInspector]
//	public OVRManager OculusCamCtl;

//	[HideInInspector]
//	public OVRCameraRig OculusCamRig;

	[HideInInspector]
	public LadybugPluginInterface ladybugCtl;

	[HideInInspector]
	public DomeVideoInterface[] videoCtls;

	public DomeVideoInterface curVideoCtl  { get { return videoCtls[AltDomeFlag]; } }

	[HideInInspector]
	public CaliburationManager calibCtl;

	[HideInInspector]
	public FrontCameraInterface frontCamCtl;
	
	[HideInInspector]
	public AudioManager AudioCtl;
	
//	[HideInInspector]
//	public SRRenderer srRenderCtl;
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

	[HideInInspector]
	public FileManager fileCtl;

	Light LiveLight;
	//Light RecordLight;

	//status
	public bool isReady = false;
	public Status live_status = Status.LIVE;
	public int[] streamIds = {0,0};
	public int curStreamId  { get { return streamIds[AltDomeFlag]; } set{ streamIds[AltDomeFlag] = value; } }

    public int curLiveId = 0;

	public int streamNum = 0;
    public int liveNum = 0;

    public CalibrationMode caliburationMode {
		get { return calibCtl.caliburationMode; }
		set {calibCtl.caliburationMode = value;}
	}
	// calibrationmanager no atai wo syutoku suru


	//float IPDIncrement = 0.1f;
	float FOVIncrement = 0.2f;
	//float DistKIncrement = 0.005f;
	
	public float liveFOV = 116f;
	public float streamFOV = 105f;
	public float instructionFOV = 105f;

	public float ladybugFOV = 120f;

    public float liveCamScale = 0.18f; 

	public bool useAudio = true;
	
	public bool useLiveLadybug = false;
    public bool useFrontCamuEye = true;
	public bool useDomeVideo {get {return fileCtl.useDomeVideo;}}
	public bool useDomeAVPro = true;



	public bool isInstructionMode = false;

	public int AltDomeFlag = 0;
	public float mergeSec = 1.0f;

	//float merge_param = 0.0f;

	public bool disableControll = false;

	public Vector3 offset = new Vector3 (0f, 0f, 0f);

	public bool isTempralCariburationMode = false;

	public bool isMerging = false;

	public bool IsPlayVideoOnStart = true;

    public bool isPauseWhenGoToStream = true;

    public bool isResetWhenGoToStream = true;


    public bool useOculusRift = true;

	public bool usePixelation = true;

    public bool isLiveRecordSwitchable = true;

	void Awake ()
	{	

		//Render.SetHook ();
		//TrackingSpaceStream = GameObject.Find ("OVRCameraRig").transform.FindChild ("TrackingSpace").gameObject;
		//TrackingSpaceLive = GameObject.Find("OVRCameraRigLive").transform.FindChild("TrackingSpace").gameObject;

		fileCtl =  GameObject.Find ("FileManager").GetComponent<FileManager> ();
		AudioCtl = GameObject.Find ("AudioObject").GetComponent<AudioManager> ();

		WebCamScreen = GameObject.Find ("FrontCamScreen") as GameObject;

		DomeContainer = GameObject.Find ("DomeContainer") as GameObject;
		DomeScreen = GameObject.Find ("DomeVideoScreen") as GameObject;
		DomeScreenAlt = GameObject.Find ("DomeVideoScreenAlt") as GameObject;


		AVProContainer = GameObject.Find ("AVProContainer") as GameObject;;
		AVProDome = GameObject.Find ("AVProDome") as GameObject;
		AVProDomeAlt = GameObject.Find ("AVProDomeAlt") as GameObject;

		LadybugScreen =GameObject.Find ("SphericalScreen") as GameObject;
	

		//ladybugScreenDistance = LadybugScreen.transform.localPosition.z;
		//webCamScreenDistance = WebCamScreen.transform.localPosition.z;


		videoCtls = new DomeVideoInterface[2];
		streamIds = new int[2]{0,0};

		AudioCtl.cur_audio_index = curStreamId;


	}
	
//	void	Start () {
	IEnumerator Start () {

		bool is_plugin_rendering = false;
		Application.targetFrameRate = 120;

		instCanvas = GameObject.Find ("InstructionCanvas");
		normalCamera = GameObject.Find ("NormalCamera");
        if (GameObject.Find("LightInLive")) { 
		    LiveLight = GameObject.Find ("LightInLive").GetComponent<Light>();
        }
        //RecordLight = GameObject.Find ("LightInRecord").GetComponent<Light>();
        //		srRenderCtl = GetComponent("SRRenderer") as SRRenderer;

        //	OculusCamCtl = GameObject.Find ("OVRCameraRigLive").GetComponent<OVRManager>();
        //	OculusCamRig = GameObject.Find ("OVRCameraRig").GetComponent<OVRCameraRig>();

        calibCtl = gameObject.GetComponent<CaliburationManager> ();

		OculusCameraInit ();
		OculusInit ();

		// Dome Video (MovieTexture or AVPro) or Ladybug Stream (Live or Stream)
		if(useDomeVideo){
			//LadybugScreen.SetActive (false);

			if(useDomeAVPro)
			{
				AVProContainer.SetActive (true);
				AVProDome.SetActive (true);
				AVProDome.transform.localEulerAngles = offset;
				AVProDomeAlt.transform.localEulerAngles = offset;

				DomeContainer.SetActive(false);

				videoCtls[0] = AVProDome.GetComponent ("DomeVideoAVPro") as DomeVideoInterface;
				videoCtls[1] = AVProDomeAlt.GetComponent ("DomeVideoAVPro") as DomeVideoInterface;
			}else{

				DomeContainer.SetActive(true);
				DomeScreen.SetActive (true);
				DomeScreen.transform.localEulerAngles = offset;
				DomeScreenAlt.transform.localEulerAngles = offset;

				AVProContainer.SetActive (false);

				videoCtls[0] = DomeScreen.GetComponent ("DomeVideoMovieTexture") as DomeVideoInterface;
				videoCtls[1] = DomeScreenAlt.GetComponent ("DomeVideoMovieTexture") as DomeVideoInterface;
			}

			int BackFlag = AltDomeFlag == 0 ? 1 : 0;
			videoCtls[AltDomeFlag].enabled = true;
			videoCtls[BackFlag].enabled = true;

			videoCtls [AltDomeFlag].SetAlpha (1.0f);
			videoCtls [BackFlag].SetAlpha (0.0f);
			
		}else{
			AVProContainer.SetActive (false);
			DomeContainer.SetActive (false);

			//Ladybug Selector
			LadybugPluginCamera ladybugcam = LadybugScreen.GetComponent("LadybugPluginCamera") as LadybugPluginCamera;
			LadybugPluginStream ladybugstream = LadybugScreen.GetComponent("LadybugPluginStream") as LadybugPluginStream ;
			if(useLiveLadybug)
			{
				ladybugcam.enabled = true;
				ladybugstream.enabled = false;
				ladybugCtl = LadybugScreen.GetComponent("LadybugPluginCamera") as LadybugPluginInterface;
			}else
			{
				ladybugcam.enabled = false;
				ladybugstream.enabled = true;
				ladybugCtl = LadybugScreen.GetComponent("LadybugPluginStream") as LadybugPluginInterface;
				Debug.Log (ladybugCtl);
			}

			LadybugScreen.SetActive (true);
			ladybugCtl.SetAlpha (1.0f);
			ladybugCtl.offset = offset;
		}	

       
		//Front Camera Selector
		WebCamScreen.SetActive (true);
		if(useFrontCamuEye)
		{
			frontCamCtl = WebCamScreen.GetComponent("FrontCamerauEye") as FrontCameraInterface;
			//Render.SetLiveTransparent(false, 255);
			is_plugin_rendering = true;

		}else
		{
            
			//WebCamScreen.transform.localScale = new Vector3(5f,  1f, 3.75f);
			//WebCamScreen.transform.localPosition = new Vector3(0f,  0f, 25f);


			//BarrelMesh bm = WebCamScreen.GetComponent<BarrelMesh>();
			//bm.k1 = 1.0f; bm.k2 = 0.0f; bm.k3 = 0.0f; bm.k4 = 0.0f; 
			//bm.Reset ();
           
			frontCamCtl = WebCamScreen.GetComponent("FrontCameraWebCam") as FrontCameraInterface;

		}
		frontCamCtl.enabled = true;

       
		//audio 
		AudioCtl.RotateMultiAudioSource(offset.y);
		// wait until ready
		while( !AudioCtl.audioReady )
		{
			yield return null;
		}
		Debug.Log ("Audio Ready!!!");
        

        //Loading files
        if (useDomeVideo)
		{

            DomeVideoStorage domeStorage = GameObject.Find ("DomeVideoStorage").GetComponent<DomeVideoStorage>();
			if(useDomeAVPro)
			{
				yield return domeStorage.StartCoroutine("LoadAVProVideos");
			}else{
				yield return domeStorage.StartCoroutine("LoadVideos");
			}
			streamNum = domeStorage.videoNum;
            liveNum = domeStorage.liveNum;

            Debug.Log ("Video Ready!!!");
		} else {

			streamNum = ladybugCtl.streamNum;
			
			SetLadybugFov(ladybugFOV);
			Debug.Log ("Stream Ready!!!");
		
			is_plugin_rendering = true;
		}



        if (is_plugin_rendering)	StartCoroutine("SetRenderingStart");

        yield return new WaitForSeconds(0.5f);
        Debug.Log ("SRController: goto stream 0");
		GoToStream (0);

		Debug.Log (live_status);
		if(live_status == Status.LIVE)
		{
			yield return StartCoroutine ("ToLive");
		}else if(live_status == Status.STREAM){
			yield return StartCoroutine ("ToRecord");
		}

		Debug.Log ("SRController isReady");

		isReady = true;
	}

	
	void OculusInit()
	{
	//	OculusCamCtl.monoscopic = true;
	//	OculusCamCtl.usePositionTracking = true;
		
		//for live
	//	OVRCameraRig rig = GameObject.Find ("OVRCameraRigLive").GetComponent<OVRCameraRig> ();
	//	rig.IsPositionUpdate = false;
	//	rig.IsRotationUpdate = false;
		
	//	OculusCamRig.IsPositionUpdate = false;
	}
	
	void OculusCameraInit()
	{
        /*
		OVRCentreAnchor = TrackingSpaceStream.transform.FindChild ("CenterEyeAnchor").gameObject;
		
		Camera[] cameras = TrackingSpaceStream.GetComponentsInChildren<Camera> ();
		for (int i = 0; i < cameras.Length; i++)
		{
			if(cameras[i].name == "LeftEyeAnchor")
				OVRStreamCameraLeft = cameras[i];
			
			if(cameras[i].name == "RightEyeAnchor")
				OVRStreamCameraRight = cameras[i];
		}
		
		Camera[] cameras2 = TrackingSpaceLive.GetComponentsInChildren<Camera> ();
		for (int i = 0; i < cameras2.Length; i++)
		{
			if(cameras2[i].name == "LeftEyeAnchor")
				OVRLiveCameraLeft = cameras2[i];
			
			if(cameras2[i].name == "RightEyeAnchor")
				OVRLiveCameraRight = cameras2[i];
		}
		*/
	}

	public void TransitionToLive(){

		StartCoroutine ("ToLive");
	}
	public void TransitionToRecord(){
		
		StartCoroutine ("ToRecord");
	}


	//rendering starts after 1second after everything ready.
	//which avoinding the abnormal application crush, when two image are rendered altenately.
	//it is not the fundamental solution, but seems ok for a while.
	IEnumerator  SetRenderingStart()
	{
		yield return new WaitForSeconds(0.5f);
		//srRenderCtl.isReady = true;

	}
	
	
	public IEnumerator  ToLive(){

		live_status = Status.LIVE;
		LiveLight.enabled = true;
	//	RecordLight.enabled = false;
//		OculusCamRig.IsRotationUpdate = false;
		BackFromInstructionScreen ();
		SetOVRFovLive (liveFOV);

		if(useAudio)
		{
			StartCoroutine(AudioCtl.CrossFade(live_status, curLiveId));
		}

		// live start
        frontCamCtl.SetForeGround();
		frontCamCtl.SetAlpha (1.0f);
        
		//TrackingSpaceStream.GetComponentInChildren<AudioListener> ().enabled = false;
		//TrackingSpaceLive.SetActive (true);
		//TrackingSpaceLive.GetComponentInChildren<AudioListener> ().enabled = true;
		WebCamScreen.GetComponent<Renderer>().enabled = true;

		yield return 0;

		//TrackingSpaceStream.SetActive (false);

		// stream stop
		if(useDomeVideo){
			if(useDomeAVPro)
			{
				AVProContainer.SetActive (false);
			}else{
				DomeContainer.SetActive (false);
			}
			curVideoCtl.SetAlpha (0.0f );
			curVideoCtl.SetBackGround (isResetWhenGoToStream, isPauseWhenGoToStream);
        }else{
			LadybugScreen.GetComponent<Renderer>().enabled = false;
			ladybugCtl.SetBackGround();
		}

        /*
		if( useFrontCamuEye ){
			srRenderCtl.isReady = true;
		}else{
			srRenderCtl.isReady = false;
		}
        */

		if(usePixelation && normalCamera != null){
			Pixelation pix = normalCamera.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.enabled = false;
		}
		//SetParentToChildObjects (TrackingSpaceLive);
		yield return 0;
	}



	public IEnumerator  ToRecord(){
		
		live_status = Status.STREAM;
        if (LiveLight != null)
        {
            LiveLight.enabled = false;
        }
	//	RecordLight.enabled = true;

		//OculusCamRig.IsRotationUpdate = true;
		BackFromInstructionScreen ();
		SetOVRFovStream (streamFOV);


		//record start
		if(useDomeVideo){

			curVideoCtl.SetForeGround (IsPlayVideoOnStart);
			
			curVideoCtl.SetAlpha (1.0f );

			if(useDomeAVPro)
			{
				AVProContainer.SetActive (true);
			}else{
				DomeContainer.SetActive (true);
			}
		}else{
			ladybugCtl.ResetStream();
			ladybugCtl.SetForeGround();

			LadybugScreen.GetComponent<Renderer>().enabled = true;
		}

		if(useAudio && IsPlayVideoOnStart)
		{
			StartCoroutine(AudioCtl.CrossFade(live_status, curStreamId, isResetWhenGoToStream, isPauseWhenGoToStream));
		}

        /*
		AudioListener a = TrackingSpaceLive.GetComponentInChildren<AudioListener> ();
		if(a != null){
			a.enabled = false;
		}
        */

		//TrackingSpaceStream.SetActive (true);
		//TrackingSpaceStream.GetComponentInChildren<AudioListener> ().enabled = true;
		
		yield return 0;

		//live stop
		//TrackingSpaceLive.SetActive (false);
		WebCamScreen.GetComponent<Renderer>().enabled = false;
		frontCamCtl.SetAlpha (0.0f);
		frontCamCtl.SetBackGround();

        /*
		if( useDomeVideo ){
			srRenderCtl.isReady = false;
		}else{
			srRenderCtl.isReady = true;
		}
        */

		if (usePixelation && normalCamera != null) {
			Pixelation pix = normalCamera.GetComponent ("Pixelation") as Pixelation;
			if (pix != null)
				pix.enabled = true;
		}

		//SetParentToChildObjects (TrackingSpaceStream);
	}


	public IEnumerator  MergeToLive(){
		
		live_status = Status.LIVE;
        if (LiveLight != null)
        {
            LiveLight.enabled = true;
        }
	//	RecordLight.enabled = false;
	//	OculusCamRig.IsRotationUpdate = false;
		BackFromInstructionScreen ();
		SetOVRFovLive (liveFOV);
		
		if(useAudio)
		{

            StartCoroutine(AudioCtl.CrossFade(live_status, curLiveId));
		}
		
		// live start
		frontCamCtl.SetForeGround();
		frontCamCtl.SetAlpha (1.0f);

	//	SetCameraDontClear(OVRLiveCameraRight);
	//	SetCameraDontClear(OVRLiveCameraLeft);

		//TrackingSpaceLive.SetActive (true);
		//TrackingSpaceStream.GetComponentInChildren<AudioListener> ().enabled = false;
		//TrackingSpaceLive.GetComponentInChildren<AudioListener> ().enabled = true;

		WebCamScreen.GetComponent<Renderer>().enabled = true;

		yield return StartCoroutine ("ProcessMergeLive");
		
		//TrackingSpaceStream.SetActive (false);
		
		// stream stop
		if(useDomeVideo){
			if(useDomeAVPro)
			{
				AVProContainer.SetActive (false);
			}else{
				DomeContainer.SetActive (false);
			}
			curVideoCtl.SetAlpha (0.0f );
            curVideoCtl.SetBackGround (isResetWhenGoToStream, isPauseWhenGoToStream);
		}else{
			LadybugScreen.GetComponent<Renderer>().enabled = false;
			ladybugCtl.SetBackGround();
		}

		SetCameraDepthOnly(OVRLiveCameraRight);
		SetCameraDepthOnly(OVRLiveCameraLeft);

		//SetParentToChildObjects (TrackingSpaceLive);

		yield return 0;

	}

	IEnumerator ProcessMergeLive()
	{
		isMerging = true;
		float start_time = Time.timeSinceLevelLoad;
		float end_time = start_time + mergeSec;
		float param = 1.0f;
		while (Time.timeSinceLevelLoad <= end_time) {
			param = Mathf.Lerp (1.0f, 0.0f, (Time.timeSinceLevelLoad - start_time)  / mergeSec );
			ChangeTransparencyRecord(param);
			yield return 0;
		}
		ChangeTransparencyRecord(0.0f);
		isMerging = false;
	}

	public IEnumerator  MergeToRecord(){
		live_status = Status.STREAM;
        if (LiveLight != null)
        {
            LiveLight.enabled = false;
        }
		//RecordLight.enabled = true;

		SetOVRFovStream (streamFOV);

	//	OculusCamRig.IsRotationUpdate = true;
		BackFromInstructionScreen ();


		// video start
		if(useDomeVideo){
			curVideoCtl.SetForeGround (IsPlayVideoOnStart);

			if(useDomeAVPro)
			{
				AVProContainer.SetActive (true);
			}else{
				DomeContainer.SetActive (true);
			}
		}else{
			ladybugCtl.ResetStream();
			ladybugCtl.SetForeGround();

			LadybugScreen.GetComponent<Renderer>().enabled = true;
		}

		if(useAudio && IsPlayVideoOnStart)
		{
			StartCoroutine(AudioCtl.CrossFade(live_status, curStreamId));
		}

		yield return 0;

		// render start
		//TrackingSpaceStream.SetActive (true);
		//TrackingSpaceLive.GetComponentInChildren<AudioListener> ().enabled = false;
		//TrackingSpaceStream.GetComponentInChildren<AudioListener> ().enabled = true;

		yield return StartCoroutine ("ProcessMergeRecord");

		//finish merge
		WebCamScreen.GetComponent<Renderer>().enabled = false;
		//TrackingSpaceLive.SetActive (false);

		frontCamCtl.SetBackGround();

		//SetParentToChildObjects (TrackingSpaceStream);

	}

	IEnumerator ProcessMergeRecord()
	{
		isMerging = true;
		float start_time = Time.timeSinceLevelLoad;
		float end_time = start_time + mergeSec;
		float param = 0.0f;
		while (Time.timeSinceLevelLoad <= end_time) {
			param = Mathf.Lerp (0.0f, 1.0f, (Time.timeSinceLevelLoad - start_time)  / mergeSec );
			ChangeTransparencyRecord(param);
			yield return 0;
		}
		ChangeTransparencyRecord(1.0f);
		isMerging = false;
	}

	void ChangeTransparencyRecord(float param)
	{
		//add in case of ladybug stream
		if(useDomeVideo){
			videoCtls [AltDomeFlag].SetAlpha (param );
		}else{
			ladybugCtl.SetAlpha(param);
		}

		frontCamCtl.SetAlpha (1 - param);
	}


	void SetParentToChildObjects(GameObject rig)
	{
		if(instCanvas != null)
		{
			instCanvas.transform.SetParent (rig.transform.Find ("CenterEyeAnchor").gameObject.transform);
			instCanvas.transform.localPosition = new Vector3 (0f, 0f, 1f);
			instCanvas.transform.localRotation = new Quaternion(0f,0f,0f,0f);
		}

		if(normalCamera != null){

			normalCamera.transform.SetParent (rig.transform.Find ("CenterEyeAnchor").gameObject.transform);
			normalCamera.transform.localPosition = new Vector3 (0f, 0f, 0f);
			normalCamera.transform.localRotation = new Quaternion(0f,0f,0f,0f);
		}
	}



	void SetLadybugFov(float fov)
	{
		ladybugCtl.fov = fov;
	}

	void SetOVRFovLive(float fov)
	{
		//OVRLiveCameraLeft.fieldOfView = fov;
		//OVRLiveCameraRight.fieldOfView = fov;
	}
	void SetOVRFovStream(float fov)
	{
        //kskk
		//OVRStreamCameraLeft.fieldOfView = fov;
		//OVRStreamCameraRight.fieldOfView = fov;
		//OculusCamCtl.SetVerticalFOV(fov);
	}

	public void DiableAudio()
	{
		useAudio = false;
		AudioCtl.StopAll ();
	}

	// Update is called once per frame
	void Update () {

		ShowFPS ();

		/*
		if(UnityEditor.SceneView.lastActiveSceneView != null)
		{
			if(live_status == Status.STREAM)
			{
				UnityEditor.SceneView.lastActiveSceneView.AlignViewToObject (OVRStreamCameraLeft.transform);
			}else{
				UnityEditor.SceneView.lastActiveSceneView.AlignViewToObject (OVRLiveCameraLeft.transform);
			}
		}
		*/

		if(disableControll) return;
		ManageKeyboard();
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

	public void SetPixelationColor(float redness)
	{
		if (!usePixelation)
			return;

		if(!useOculusRift)
		{
			Pixelation pix = normalCamera.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.redness = redness;
			
		}else{
			
			Pixelation pix = OVRLiveCameraLeft.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.redness = redness;
			
			pix = OVRStreamCameraLeft.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.redness = redness;
		}
	}


	public void EnablePixelation(){

		if (!usePixelation)
			return;


		if(!useOculusRift)
		{

			Pixelation pix = normalCamera.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.enabled = true;
		
		}else{
		
			Pixelation pix = OVRLiveCameraLeft.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.enabled = true;	

			pix = OVRStreamCameraLeft.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.enabled = true;

		}
	}

	public void DisablePixelation(){

		
		if (!usePixelation)
			return;

		if(!useOculusRift)
		{
			Pixelation pix = normalCamera.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.enabled = false;
		}else{


			Pixelation pix = OVRLiveCameraLeft.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.enabled = false;	

			pix = OVRStreamCameraLeft.GetComponent("Pixelation") as Pixelation;
			if(pix != null) pix.enabled = false;

		}
	}

	public void GoToInstructionScreen(){

        if (instCanvas != null)
        {
            instCanvas.GetComponent<Canvas>().enabled = true;
        }

		if(!useOculusRift)
		{
			if (live_status == Status.LIVE) {
				normalCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer ("LiveScene"));
			}else{
				normalCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer ("RecordScene"));

				Pixelation pix = normalCamera.GetComponent("Pixelation") as Pixelation;
				if(pix != null) pix.enabled = false;
			}
			return;
		}

		if (live_status == Status.LIVE) {
			SetOVRFovLive(instructionFOV);
//			SetCameraSolidColor(OVRLiveCameraRight);
//			SetCameraSolidColor(OVRLiveCameraLeft);

//			OVRLiveCameraRight.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer ("LiveScene"));
//			OVRLiveCameraLeft.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer ("LiveScene"));

			//instCanvas.layer = LayerMask.NameToLayer("LiveScene");
		}else{
			SetOVRFovStream(instructionFOV);
//			SetCameraSolidColor(OVRStreamCameraRight);
//			SetCameraSolidColor(OVRStreamCameraLeft);

//			OVRStreamCameraRight.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer ("RecordScene"));
//			OVRStreamCameraLeft.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer ("RecordScene"));

	//		OculusCamRig.IsRotationUpdate = false;

//			Pixelation pix = OVRStreamCameraLeft.GetComponent("Pixelation") as Pixelation;
	//		if(pix != null) pix.enabled = false;

			//instCanvas.layer = LayerMask.NameToLayer("RecordScene");
		}


	//	StartCoroutine ("ToLive");

	}

	public void BackFromInstructionScreen(bool not_orientation_turn_on = false){
        if (instCanvas != null)
        {
            instCanvas.GetComponent<Canvas>().enabled = false;
        }
		if(!useOculusRift)
		{
			if (live_status == Status.LIVE) {
				normalCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("LiveScene");
			}else{
				normalCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer ("RecordScene");

				if(usePixelation){
					Pixelation pix = normalCamera.GetComponent("Pixelation") as Pixelation;
					if(pix != null) pix.enabled = true;
				}
			}
			return;
		}
		
		if (live_status == Status.LIVE) {
			SetOVRFovLive(liveFOV);
			SetCameraDepthOnly(OVRStreamCameraRight);
			SetCameraDepthOnly(OVRStreamCameraLeft);
//			OVRLiveCameraRight.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("LiveScene");
//			OVRLiveCameraLeft.GetComponent<Camera>().cullingMask  |= 1 << LayerMask.NameToLayer("LiveScene");
		}else
		{
			SetOVRFovStream(streamFOV);
			SetCameraDepthOnly(OVRStreamCameraRight);
			SetCameraDepthOnly(OVRStreamCameraLeft);
			//OVRStreamCameraRight.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer ("RecordScene");
			//OVRStreamCameraLeft.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer ("RecordScene");

			if(!not_orientation_turn_on)
			{
				//OculusCamRig.IsRotationUpdate = true;
				//OculusCamCtl.EnableOrientation = true;
			}

            /*
			if(usePixelation){
				Pixelation pix = OVRStreamCameraLeft.GetComponent("Pixelation") as Pixelation;
				if(pix != null) pix.enabled = true;
			}
            */
		}

	}

	private void SetCameraSolidColor(Camera cam)
	{
		cam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
		cam.GetComponent<Camera>().backgroundColor = Color.black;
	}

	private void SetCameraDontClear(Camera cam)
	{
		cam.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
	}

	private void SetCameraDepthOnly(Camera cam)
	{
		//cam.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
	}


	public void ChangeStartFrame(int frame){
		ladybugCtl.ChangeStreamStartFrame(frame);
	}


	public void LoadStream(int id)
	{
		if(AudioCtl.is_streaming_audio){

			StartCoroutine(AudioCtl.LoadAudioStream (curStreamId));
			Debug.Log ("load audio file in foreground");
		}
	}
    public void GoToLiveAudio(int id)
    {
        if (live_status == Status.LIVE)
        {
            //corss fade
            if (useAudio)
            {
                curLiveId = id;
                StartCoroutine(CoGoToLiveAudio());

            }
        }
    }

    public IEnumerator CoGoToLiveAudio()
    {
        yield return StartCoroutine(AudioCtl.CrossFade(live_status, curLiveId));
    }


    public void GoToStream(int id)
	{
		if(useAudio && AudioCtl.is_transition)
		{
			return;
		}
		curStreamId = id;
		if(live_status == Status.STREAM)
		{
			StartCoroutine(GoToStreamForeground(streamIds[AltDomeFlag]));
		}else if(live_status == Status.LIVE){
			StartCoroutine(GoToStreamBackground(streamIds[AltDomeFlag]));
		}
	}
		
	public IEnumerator GoToStreamForeground(int id)
	{
		if(AudioCtl.is_streaming_audio){
			yield return StartCoroutine(AudioCtl.LoadAudioStream (curStreamId));
		}

		if(useDomeVideo){
			if(isResetWhenGoToStream){	
				curVideoCtl.Reset ();
			}

			curVideoCtl.GoToStream(curStreamId, IsPlayVideoOnStart, isResetWhenGoToStream, isPauseWhenGoToStream);

		}else{
			ladybugCtl.GoToStream(curStreamId);
			// not resetting does not work in the stream files. I need to store the current frame for each scene 

		}

		//corss fade
		if(useAudio && IsPlayVideoOnStart)
		{	
			yield return StartCoroutine(AudioCtl.CrossFade(live_status, curStreamId, isResetWhenGoToStream, isPauseWhenGoToStream));
		
		}
	}

	public IEnumerator GoToStreamBackground(int id)
	{
		if(useDomeVideo){
			curVideoCtl.GoToStreamBackground(curStreamId, isResetWhenGoToStream, isPauseWhenGoToStream);
		}else{
			ladybugCtl.GoToStreamBackground(curStreamId);
		}
		// load next audio
		if(AudioCtl.is_streaming_audio){
			
			yield return StartCoroutine(AudioCtl.LoadAudioStream (curStreamId));
			
			Debug.Log ("load audio file in background");
		}
//		yield return StartCoroutine(AudioCtl.UnloadAudio(live_status, streamId));
	}

	
	void IncreaseStreamId()
	{
		
		curStreamId++;
		
		if(curStreamId > streamNum - 1)
		{
			curStreamId = 0;
		}
		GoToStream(curStreamId);
	}
	
	void DecreaseStreamId()
	{	
		
		curStreamId--;
		if(curStreamId < 0)
		{
			curStreamId = streamNum - 1;
		}
		GoToStream(curStreamId);
	}
	
	public void IncreaseMerge()
	{
		if (isMerging) return;
		
		Debug.Log ("inc merge");
		
		int BackFlag = AltDomeFlag == 0 ? 1 : 0;
		
		streamIds [BackFlag] = streamIds [AltDomeFlag] + 1;
		if(streamIds [BackFlag] > streamNum - 1) streamIds [BackFlag] = 0;
		
		
		MergeStream ();
	}
	
	public void DecreaseMerge()
	{	
		if (isMerging) return;
		
		Debug.Log ("dec merge");
		
		int BackFlag = AltDomeFlag == 0 ? 1 : 0;
		streamIds [BackFlag] = streamIds [AltDomeFlag] - 1;
		if(streamIds [BackFlag] < 0) streamIds [BackFlag] = streamNum - 1;;
		
		MergeStream ();
	}
	
	
	public void MergeStreamWithId(int id)
	{
		if (isMerging) return;
		
		int BackFlag = AltDomeFlag == 0 ? 1 : 0;
		
		streamIds [BackFlag] = id;
		
		MergeStream ();
		
	}
	
	
	void MergeStream()
	{
		int BackFlag = AltDomeFlag == 0 ? 1 : 0;

		videoCtls[BackFlag].GoToStream (streamIds[BackFlag], IsPlayVideoOnStart, isResetWhenGoToStream, isPauseWhenGoToStream);
		videoCtls[BackFlag].SetForeGround ();
		
		if(useAudio && IsPlayVideoOnStart)
		{
			StartCoroutine(AudioCtl.CrossFade(live_status, streamIds[BackFlag], isResetWhenGoToStream, isPauseWhenGoToStream));
		}
		
		StartCoroutine ("ProcessMerge");
		//corss fade
		
		
	}
	
	IEnumerator ProcessMerge()
	{
		isMerging = true;
		int BackFlag = AltDomeFlag == 0 ? 1 : 0;
		
		float start_time = Time.timeSinceLevelLoad;
		float end_time = start_time + mergeSec;
		float param = 0.0f;
		while (Time.timeSinceLevelLoad <= end_time) {
			param = Mathf.Lerp (0.0f, 1.0f, (Time.timeSinceLevelLoad - start_time)  / mergeSec );
			ChangeTransparency(param);
			yield return 0;
		}
		ChangeTransparency (1.0f);
		Debug.Log ("finish merge");


		videoCtls [AltDomeFlag].SetBackGround (isResetWhenGoToStream, isPauseWhenGoToStream);

		AltDomeFlag = BackFlag;
		isMerging = false;
	}

    void IncLiveAudio()
    {
        curLiveId++;

        if (curLiveId > liveNum - 1)
        {
            curLiveId = 0;
        }
        GoToLiveAudio(curLiveId);
    }
    void DecLiveAudio()
    {
        curLiveId--;
        if (curLiveId < 0)
        {
            curLiveId = liveNum - 1;
        }
        GoToLiveAudio(curLiveId);
    }
	
	void ChangeTransparency(float param)
	{
		int BackFlag = AltDomeFlag == 0 ? 1 : 0;
		//videoCtls[BackFlag].alpha_param =  param; 
		//videoCtls[AltDomeFlag].alpha_param = 1 - param ;
		videoCtls [BackFlag].SetAlpha (param);
		videoCtls [AltDomeFlag].SetAlpha (1 - param );
	}

	/* 
		Keyboard Assign
	*/

	public void EnableOculus()
	{

		useOculusRift = true;

		OVRLiveCameraRight.enabled =true;
		OVRLiveCameraLeft.enabled =true;
		OVRStreamCameraRight.enabled =true;
		OVRStreamCameraLeft.enabled =true;
		
		//OculusCamCtl.enabled = true;
		//OculusCamCtl.gameObject.GetComponent<Camera>().enabled = true;

		normalCamera.GetComponent<Camera>().enabled = false;
	}

	public void DisableOculus()
	{

		useOculusRift = false;

		OVRLiveCameraRight.enabled =false;
		OVRLiveCameraLeft.enabled =false;
		OVRStreamCameraRight.enabled =false;
		OVRStreamCameraLeft.enabled =false;

		
	//	OculusCamCtl.enabled = false;
	//	OculusCamCtl.gameObject.GetComponent<Camera>().enabled = false;

		normalCamera.GetComponent<Camera>().enabled = true;


	}

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

	void KeyboardLadybugFov()
	{
        if (Input.GetKey(KeyCode.F9))
        {
            if (liveCamScale > 0.0f)
            {
                liveCamScale -= 0.005f;
            }
            GameObject.Find("FrontCamScreen").transform.localScale = new Vector3(liveCamScale, liveCamScale, liveCamScale);


        }
        else if (Input.GetKey(KeyCode.F10))
        {
            liveCamScale += 0.005f;

            GameObject.Find("FrontCamScreen").transform.localScale = new Vector3(liveCamScale, liveCamScale, liveCamScale);
        }


        if (Input.GetKey (KeyCode.F11))
		{
			ladybugFOV -= FOVIncrement;
			SetLadybugFov(ladybugFOV);
			Debug.Log("ladybug fov:" + ladybugFOV);
		}else if(Input.GetKey (KeyCode.F12))
		{	
			ladybugFOV += FOVIncrement;
			SetLadybugFov(ladybugFOV);
			Debug.Log("ladybug fov:" + ladybugFOV);
		}
	}
	
	void KeyboardOVRFov(ref float fov, ref Camera camL, ref Camera camR)
	{

		if(Input.GetKey (KeyCode.F9))
  	  	{
			fov -= FOVIncrement;
			if(camL.name == "CameraLiveLeft"){
				Debug.Log("fov live:" + fov);
			}else{
				Debug.Log("fov stream:" + fov);
			}
			//OculusCamCtl.SetCameras( camL,  camR);
			//OculusCamCtl.SetVerticalFOV(fov);
		
        }else if (Input.GetKey (KeyCode.F10))
        {
			fov += FOVIncrement;
			if(camL.name == "CameraLiveLeft"){
				Debug.Log("fov live:" + fov);
			}else{
				Debug.Log("fov stream:" + fov);
			}
			//OculusCamCtl.SetCameras(camL,camR);
			//OculusCamCtl.SetVerticalFOV(fov);            
		}
	}


    void GamePadCaliburation()
	{
		if(isTempralCariburationMode)
		{
	        //Stream Image Move
    	    float rawHorizontal = Input.GetAxisRaw("HorizontalLStick");
        	float rawVertical = Input.GetAxisRaw("VerticalLStick");

	        float rawHorizontal2 = Input.GetAxisRaw("HorizontalRStick");

			if(useDomeVideo){

				offset.y  -= rawHorizontal * 1.0f;

				if(useDomeAVPro)
				{
					AVProDome.transform.localEulerAngles = offset;
					AVProDomeAlt.transform.localEulerAngles = offset;
				
				}else{
					DomeScreen.transform.localEulerAngles = offset;
					DomeScreenAlt.transform.localEulerAngles = offset;
				}
			}else{
				offset.y -= rawHorizontal * 2.0f;
				offset.x += rawVertical * 2.0f;
				offset.z -= rawHorizontal2 * 1.0f;

				ladybugCtl.offset = offset;
			}



	        //FOV for Ladybug (left/right on plus arrow keys)
			if(!useDomeVideo){
	    	    float rawHorizontalPad = Input.GetAxisRaw("HorizontalPad");
				ladybugFOV += rawHorizontalPad * 0.3f;

				SetLadybugFov (ladybugFOV);
			}

			//FOV for Oculus (up/down on plus arrow keys)
			float rawVerticalPad = Input.GetAxisRaw("VerticalPad");
			if (rawVerticalPad > 0 || rawVerticalPad < 0)
			{

				if (live_status == Status.STREAM) 
    	    	{
					streamFOV += rawVerticalPad * 0.3f;
					SetOVRFovStream(streamFOV);
				}else{
					liveFOV += rawVerticalPad * 0.3f;
					SetOVRFovLive(liveFOV);
				}
        	}

			//change exposure by right stick (up-down)
			float rawVertical2 = Input.GetAxisRaw("VerticalRStick");
			if (rawVertical2 > 0)
			{
				frontCamCtl.IncExposure();
			}
			else if (rawVertical2 < 0)
			{
				frontCamCtl.DecExposure();
			}	
		}
		//scene change

        if (Input.GetKey(KeyCode.JoystickButton0))
        {
			MergeStreamWithId(0);
			Debug.Log ("Switch to 1");
		}
		else if (Input.GetKey(KeyCode.JoystickButton1))
		{
			MergeStreamWithId(1);
			Debug.Log ("Switch to 2");
		}
		else if (Input.GetKey(KeyCode.JoystickButton2))
        {
			MergeStreamWithId(2);
			Debug.Log ("Switch to 3");
		}
		else if (Input.GetKey(KeyCode.JoystickButton3))
        {
			MergeStreamWithId(3);
			Debug.Log ("Switch to 4");
		}else if(Input.GetKey(KeyCode.JoystickButton8))
		{
			MergeStreamWithId(4);
			Debug.Log ("Switch to 5");
		}else if(Input.GetKey(KeyCode.JoystickButton9))
		{
			MergeStreamWithId(5);
			Debug.Log ("Switch to 6");

		}
	}
	
	void KeyboardSRControl()
	{
		GamePadCaliburation();
		//SR controlf
		if (Input.GetKeyDown (KeyCode.V)) {
			
			if(useOculusRift){
				DisableOculus();

				Debug.Log ("Oculus Disabled");
			}else{
				EnableOculus();
				useOculusRift = true;
				Debug.Log ("Oculus Eabled");
			}
		}

		if(caliburationMode == CalibrationMode.None)
		{
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (live_status != Status.LIVE)
                {
                    StartCoroutine("MergeToLive");
                }
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (live_status == Status.LIVE)
                {
                    StartCoroutine("MergeToRecord");
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.JoystickButton6))
            {
                if (isLiveRecordSwitchable)
                {
                    if (live_status != Status.LIVE)
                    {
                        Debug.Log("up - live");
                        StartCoroutine("ToLive");
                    }
                }

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.JoystickButton7))
            {
                if (isLiveRecordSwitchable)
                {
                    Debug.Log("down - rocord");
                    StartCoroutine("ToRecord");

                }
            }
        }

		if (Input.GetKeyDown(KeyCode.T) ) //Goto Streams
		{
            //kskk
		//	OculusCamRig.transform.localRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);
			//OculusCamCtl.SetOrientationOffset(Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up));
		
			//OVRDevice.ResetOrientation ();
		}



        if (Input.GetKeyDown(KeyCode.PageDown))
            IncLiveAudio(); 
        else if (Input.GetKeyDown(KeyCode.PageUp))
            DecLiveAudio();

        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.LeftArrow) ) 
			DecreaseMerge();
		else if (Input.GetKey (KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.RightArrow)) 
			IncreaseMerge();

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
			DecreaseStreamId();
        else if (Input.GetKeyDown(KeyCode.RightArrow) )
			IncreaseStreamId();

        /*
		for( int i = 1; i < 10; ++i )
		{
			if ( Input.GetKeyDown( "" + i ) )
			{
				Debug.Log("GoTo " + i);
				GoToStream (i-1);
			}
		}
        
			
		if (Input.GetKey (KeyCode.LeftShift) ) 
		{
			for ( int i = 1; i < 10; ++i )
			{
				if ( Input.GetKeyDown( "" + i ) )
				{
					Debug.Log("Merge to " + i);
					GoToStream (i-1);
				}
			}
		}
        */

		if(Input.GetKey (KeyCode.Minus))
			ladybugCtl.DecAudioLatency();
		else if(Input.GetKey (KeyCode.Equals))
			ladybugCtl.IncAudioLatency();
		else if(Input.GetKey (KeyCode.LeftBracket))
			AudioCtl.DecRadius();		
		if(Input.GetKey (KeyCode.RightBracket))
			AudioCtl.IncRadius(); 	
		
		if  (Input.GetKeyDown(KeyCode.X))
    	{
		}

		if (Input.GetKeyDown (KeyCode.B)) {
			isInstructionMode = !isInstructionMode;
			if(isInstructionMode){
				GoToInstructionScreen();
			}else{
				BackFromInstructionScreen();
			}

		}
	

		/*
		if (Input.GetKeyDown (KeyCode.Z)) 
		{
			isTempralCariburationMode = !isTempralCariburationMode;
		}
		*/


		//Ladybug rotation offset
        if(Input.GetKey (KeyCode.W) )
        {
			if(!useDomeVideo){
				offset.x += 2.0f;
				ladybugCtl.offset = offset;
			}

		}else if(Input.GetKey (KeyCode.S) ) 
		{
			if(!useDomeVideo){
				offset.x -= 2.0f;
				ladybugCtl.offset = offset;	
			}
		
		}else if(Input.GetKey (KeyCode.A)  ) 
		{
			if(useDomeVideo){
				if(useDomeAVPro)
				{
					offset.y  -= 1.0f;
					AVProDome.transform.localEulerAngles = offset;
					AVProDomeAlt.transform.localEulerAngles = offset;
					
				}else{
					offset.y  -= 1.0f;
					DomeScreen.transform.localEulerAngles = offset;
					DomeScreenAlt.transform.localEulerAngles = offset;
				}
			}else{
				offset.y  += 1.0f;
				ladybugCtl.offset = offset;	
			}
			AudioCtl.RotateMultiAudioSource(1.0f);


		}else if(Input.GetKey (KeyCode.D)   )
		{

			if(useDomeVideo){
				if(useDomeAVPro)
				{
					offset.y  += 1.0f;
					AVProDome.transform.localEulerAngles = offset;
					AVProDomeAlt.transform.localEulerAngles = offset;
					
				}else{
					offset.y  += 1.0f;
					DomeScreen.transform.localEulerAngles = offset;
					DomeScreenAlt.transform.localEulerAngles = offset;
				}
			}else{
				offset.y  -= 1.0f;
				ladybugCtl.offset = offset;	
			}
			AudioCtl.RotateMultiAudioSource(-1.0f);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
			if(!useDomeVideo){
				offset.z -= 1.0f;
				ladybugCtl.offset = offset;
			}
        }
        else if (Input.GetKey(KeyCode.E))
        {
			if(!useDomeVideo){
				offset.z += 1.0f;
				ladybugCtl.offset = offset;
			}
        }
	}
	
	void ManageKeyboard()
	{
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MergeStreamWithId(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MergeStreamWithId(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MergeStreamWithId(3);
        }

        if (Input.GetKey (KeyCode.LeftShift) ) 
		{
			/*
			if( Input.GetKeyDown (KeyCode.Alpha0))
			{
				LoadStream(0);
				Debug.Log ("load stream ");
				return;
			}
			if( Input.GetKeyDown (KeyCode.Alpha1))
			{
				LoadStream(1);
				Debug.Log ("load stream ");
				return;
				
			}
			if( Input.GetKeyDown (KeyCode.Alpha2))
			{
				LoadStream(2);
				Debug.Log ("load stream ");
				return;
			}
			*/
		}

		KeyboardSRControl();
		
		if(Input.GetKey (KeyCode.LeftControl))
		{
			KeyboardUEyeParam();
			
		}else{
			KeyboardLadybugFov();

            /*
			if(caliburationMode != CalibrationMode.None)
			{
				if(Input.GetKey (KeyCode.LeftShift))
				{
					KeyboardOVRFov(ref streamFOV, ref OVRStreamCameraLeft, ref OVRStreamCameraRight);

				}else{
					KeyboardOVRFov(ref liveFOV,ref OVRLiveCameraLeft,ref OVRLiveCameraRight);	
				}
			}else if(caliburationMode == CalibrationMode.None)
			{
				if(live_status == Status.LIVE)
				{
					//live oculus parameter
					KeyboardOVRFov(ref liveFOV, ref OVRLiveCameraLeft, ref OVRLiveCameraRight);	
				}else if(live_status == Status.STREAM)
				{
					//stream oculus parameter
					KeyboardOVRFov(ref streamFOV, ref OVRStreamCameraLeft, ref OVRStreamCameraRight);
				}
			}
            */


		}
	}
}
