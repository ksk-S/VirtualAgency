//for Ladybug SDK 1.9 - 6, but cannot render streams more than 2 correctly
#define LADYBUG_VERSION_110

//#define DEBUG_TIME
#if LADYBUG_VERSION_110
using LadybugAPI114;
#else
using LadybugAPI15;
#endif

using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using RenderingAPI;
using CommonUtil;
using FileManagingSpace;

public class LadybugPluginInterface : MonoBehaviour {
	

	Texture2D texture;
	//Texture
//	static int SphereWidth = 640;
//	static int SphereHeight = 512;
	
//	static int SphereWidth = 1280;
//	static int SphereHeight = 1024;
	
	
//	public static int SphereWidth= 512;
//	public static int SphereHeight  = 512;	

//	public static int SphereWidth = 1024;
//	public static int SphereHeight = 1024;
	public float alpha_param = 0.5f;

	public static int SphereWidth = 800;
	public static int SphereHeight = 800;

	//public static int SphereWidth = 512;
	//public static int SphereHeight = 512;

	//	public static int SphereWidth = 860;
//	public static int SphereHeight = 860;
	
	
//	protected static int SphereWidth = 860;
//	protected static int SphereHeight = 800;

	
	protected static int PanoWidth = 2048;
	protected static int PanoHeight = 1024;

	public  static int textureWidth = 0;
    public static int textureHeight = 0;
	
	public int streamNum = 0;
	
	//Audio 
	protected AudioManager AudioCtl;
	
	//Ladybug
	protected const string temp_config_file = "config.cal";
	
//	private List<int> ladybugContexts; 
//    private List<int> streamContexts;

	public int CurLadybugContext;
	public int CurStreamContext;	
	/*
	protected int CurLadybugContext;
	protected int CurStreamContext;	
	*/
	
	protected LadybugProcessedImage processImage;
	protected LadybugImage curImage;
	public int curFrame = 0;
	
	
	protected double imageStartTime = 0f;
	protected 	float actualStartTime = 0f;

	protected byte[] textureBuffer;
	
	public float debug_time_display;
	public float debug_time_process;

	//    private bool textureSizeChanged = false;
	
#if LADYBUG_VERSION_110	
	protected bool isHighBitDepth;
#endif

	public bool is_repeat_after_end = true;
	public bool is_reach_the_end_flag = false;

	//bit of spped up
	public bool is_use_thread = false;
	
	//Oculus Rift
	public RenderingMode render_mode = RenderingMode.Spherical;
	//public float fov = 110;
	public float fov = 120;
	
	//flags
	public bool streamReady	 = false;
	protected bool image_updated = true;
//	private bool frame_stopping = false;
	
	public bool foreground = true;
	
	public Vector3 offset = new Vector3 (0.0f, 0.0f, 0.0f);

	//head replay
	public bool is_head_replay = false;
	public Vector3 cur_head_history = new Vector3();
	public int head_replay_index = 0;

	//head delayd
	public bool is_head_delayed = false;
	public int head_delayed_frames = 10;
	private Queue<Vector3> delayedRotations = new Queue<Vector3>();

	//image delayed
	public bool is_image_delayed = false;
	public int image_delayed_frames = 10;
	private Queue<LadybugImage> delayedImages = new Queue<LadybugImage>();

	public bool is_reflected_image = false;

	//thraeding
	protected System.Object sync = new System.Object();
	protected Thread thread;
	
	// FrameFps
	public float FrameFps;
	protected int frame_numFrames = 0;
	protected float frame_lastsecond = 0;
	protected float frame_fps_updateInterval = 1.0f; 
	protected float frame_fps_timeUntilNextInterval; 
	
	// FPS calculater
	public float fps;
	protected float fps_updateInterval = 1.0f; 
	protected float fps_accumulated   = 0.0f;
	protected float fps_timeUntilNextInterval; 
	protected int fps_numFrames = 0;

	public float dRadius = 1.1f;

	public bool isDynamicStitch = true;
	
	protected const float CONVERT_RADIAN = 2.0f * Mathf.PI /360f;
	// Use this for initialization

//	public OVRManager OculusCamCtl;

//	float pre_time = 0.0f;



	virtual public void Awake(){

	}

	protected void Init(){
		InitAudio ();
		if(is_reflected_image){
			GetComponent<Renderer>().material.mainTextureScale = new Vector2( 1f,   1f);
		}else{
			GetComponent<Renderer>().material.mainTextureScale = new Vector2( -1f,   1f);
		}
//		Debug.Log (GameObject.Find ("OVRCameraController"));
//		OculusCamCtl = GameObject.Find ("OVRCameraRig").GetComponent<OVRManager>();
	}

	virtual public void Start (){

	}
	
	virtual public void SetForeGround()
	{	
		PluginRender.SetStreamRenderStatus (true);
		foreground = true;
	}
	
	virtual public void SetBackGround()
	{
		PluginRender.SetStreamRenderStatus (false);
		foreground = false;
	}
	

	
	protected void ShowFPS(){
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
	
	protected void ShowFrameFPS()
	{
		frame_fps_timeUntilNextInterval -= Time.deltaTime;
		++frame_numFrames;
		
		if( frame_fps_timeUntilNextInterval <= 0.0 )
		{
			if( (Time.timeSinceLevelLoad - frame_lastsecond) != 0){
				FrameFps = frame_numFrames / (Time.timeSinceLevelLoad - frame_lastsecond);
			}else{
				FrameFps = 0;
			}
			
			frame_fps_timeUntilNextInterval = frame_fps_updateInterval;
			
			frame_lastsecond = Time.timeSinceLevelLoad;
			frame_numFrames = 0;
		}
		
	}
	
	
	protected void InitAudio()
	{
		GameObject AudioObject = GameObject.Find ("AudioObject") as GameObject;
		AudioCtl = AudioObject.GetComponent("AudioManager") as AudioManager;
	}
	
	
	
	public bool commonInitialize()
	{
		LadybugColorProcessingMethod colorProc = LadybugColorProcessingMethod.LADYBUG_DOWNSAMPLE4; // fast
		//LadybugColorProcessingMethod colorProc = LadybugColorProcessingMethod.LADYBUG_HQLINEAR; // slow but hiqh quality
        LadybugError error = LadybugError.LADYBUG_OK;
			
#if LADYBUG_VERSION_110	
		isHighBitDepth = dataFormat.isHighBitDepth(curImage.dataFormat);
		textureBuffer = new byte[Ladybug.LADYBUG_NUM_CAMERAS * curImage.uiRows * curImage.uiCols * 4 * (isHighBitDepth ? 2 : 1)];
#else
		textureBuffer = new byte[Ladybug.LADYBUG_NUM_CAMERAS * curImage.uiRows * curImage.uiCols * 4];
#endif		


		error = Ladybug.SetColorProcessingMethod(CurLadybugContext, colorProc);
    	handleError(error, "Ladybug.SetColorProcessingMethod");
	
     	initializeAlphaMasks();
		error = Ladybug.ConfigureOutputImages(CurLadybugContext, (0x1 << 14) ); // sphical , it does not matter?
//       		error = Ladybug.ConfigureOutputImages(CurLadybugContext, (0x1 << 12) );//LadybugOutputImage.LADYBUG_PANORAMIC
       	handleError(error, "Ladybug.ConfigureOutputImages");
		
		image_updated = false;
		streamReady = true;
        
		Debug.Log ("commonInit");
		
        return (error == 0);
	}	

	private bool initializeAlphaMasks()
    {
		LadybugError error;

		if (CurLadybugContext == 0) return false;

		LadybugColorProcessingMethod curMethod;
		Ladybug.GetColorProcessingMethod(CurLadybugContext, out curMethod);

		uint texture_width = curImage.uiCols;
		uint texture_height = curImage.uiRows;
		
		if (curMethod == LadybugColorProcessingMethod.LADYBUG_DOWNSAMPLE4 ||
			curMethod == LadybugColorProcessingMethod.LADYBUG_MONO)
		{
			texture_width  /= 2;
			texture_height /= 2;
		}
		else if (curMethod == LadybugColorProcessingMethod.LADYBUG_DOWNSAMPLE16)
        {
        	texture_width /= 4;
            texture_height /= 4;
        }
		
		// koko iranai?
		error = Ladybug.LoadAlphaMasks(CurLadybugContext, texture_width, texture_height);
		if (error != LadybugError.LADYBUG_OK)
		{
#if LADYBUG_VERSION_110
		 	error = Ladybug.InitializeAlphaMasks(CurLadybugContext, texture_width, texture_height, false);
#else
			error = Ladybug.InitializeAlphaMasks(ladybugContexts[index], texture_width, texture_height);
#endif
			handleError(error, "Ladybug.InitializeAlphaMasks");
		}
		

        return (error == 0);
	}


	virtual public void ChangeStreamStartFrame(int frame)
	{
	}


	virtual public void ResetStream()
	{
	}
	
	virtual public void GoToStream(int index)
	{
	}

	virtual public void GoToStreamBackground(int index)
	{
	}

	
	void OnApplicationQuit()
	{
		Debug.Log ("onApplicationQuit");
   	    cleanup();
		
		if( thread != null)
		{
			thread.Abort(); 
  		}
	
	}
		
	void OnDestroy()
	{
		Debug.Log ("onDestroy");
		cleanup();
	}

	virtual public void cleanup(){
	}


    [MethodImpl(MethodImplOptions.Synchronized)]
    virtual unsafe public void processLadybugImages()
    {
		#if	DEBUG_TIME
		Debug.Log(" p1 : " +  ( Time.timeSinceLevelLoad - debug_time_process ) );
		debug_time_process = Time.timeSinceLevelLoad;
#endif
				//Debug.Log ("ProcssLadybugImage");
				//Debug.Log ("processLadybugImages");

		if (image_updated) // if the previously processed image is not displayed, do nothing.
			return;

		LadybugError error;

		//Delayed Image
		if(is_image_delayed)
		{
			delayedImages.Enqueue(curImage);
			if(	delayedImages.Count > image_delayed_frames)
			{
				curImage = delayedImages.Dequeue();
			}
		}	
		//	Debug.Log (curFrame + "/" + numImages);
				//Console.WriteLine("process_and_display seqnum=" + curImage.uiSeqNum.ToString()
				//                + " seqid=" + curImage.imageInfo.ulSequenceId);
		
#if	DEBUG_TIME
		Debug.Log(" p2 : " + ( Time.timeSinceLevelLoad - debug_time_process ) );
		debug_time_process = Time.timeSinceLevelLoad;
#endif

				fixed (byte* texBufPtr = &textureBuffer[0]) {
						// this is a trick to make a pointer of arrays.
						byte** texBufPtrArray = stackalloc byte*[6];
						for (int i = 0; i < 6; i++) {
#if LADYBUG_VERSION_110
				texBufPtrArray[i] = texBufPtr + (isHighBitDepth ? i * 2 : i) * curImage.uiRows * curImage.uiCols * 4;
#else                 
								texBufPtrArray [i] = texBufPtr + i * curImage.uiRows * curImage.uiCols * 4;
#endif
						}
			
#if	DEBUG_TIME
			Debug.Log(" p3 : " + ( Time.timeSinceLevelLoad - debug_time_process ) );
			debug_time_process = Time.timeSinceLevelLoad;
#endif
			
#if LADYBUG_VERSION_110
				error = Ladybug.ConvertImage(CurLadybugContext, ref curImage, texBufPtrArray, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU));  
#else
						LadybugImageInfo imageInfo;
						error = Ladybug.ConvertToMultipleBGRU32 (CurLadybugContext, ref curImage, texBufPtrArray, out imageInfo);		
#endif
						handleError (error, "Ladybug.ConvertImage");
	
#if	DEBUG_TIME
			Debug.Log(" p4 : " + ( Time.timeSinceLevelLoad - debug_time_process ) );
			debug_time_process = Time.timeSinceLevelLoad;
#endif
			
						if (error == LadybugError.LADYBUG_OK) {
#if LADYBUG_VERSION_110				
				error = Ladybug.UpdateTextures(CurLadybugContext, Ladybug.LADYBUG_NUM_CAMERAS, texBufPtrArray, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU));
#else
								error = Ladybug.UpdateTextures (CurLadybugContext, Ladybug.LADYBUG_NUM_CAMERAS, texBufPtrArray);
				
#endif              
								handleError (error, "Ladybug.UpdateTextures");
						}

#if	DEBUG_TIME
			Debug.Log(" p5 : " + ( Time.timeSinceLevelLoad - debug_time_process ) );
			debug_time_process = Time.timeSinceLevelLoad;
#endif
            
				}
				if (error == LadybugError.LADYBUG_OK) {
						image_updated = true;		
				}

	}


	unsafe virtual public void displayLadybugImages(bool alt)
	{
		//if(!image_updated) return
	//	if (image_updated)
	//	{
			//Debug.Log ("display_dll");


		//float actualElapsed = Time.timeSinceLevelLoad - actualStartTime;
		//Debug.Log ("displayLadybugImages curFrame" + curFrame +  " " + (actualElapsed - pre_time));
		//pre_time = actualElapsed;
			 
		LadybugError error = LadybugError.LADYBUG_OK;
		//lock(sync){
		//spherical view
		if(render_mode == RenderingMode.Spherical)
		{
            //Quaternion CameraOrientation = Quaternion.identity;
            //			OVRDevice.GetPredictedOrientation(0, ref CameraOrientation);                    

            //OVRPose pose = OVRManager.display.GetHeadPose(0.0); 
            //Vector3 rotation = pose.orientation.eulerAngles;
            Vector3 rotation = new Vector3();

            if (is_head_replay)
			{
				rotation = cur_head_history;
			}

			if(is_head_delayed)
			{
				delayedRotations.Enqueue(rotation);
				if(	delayedRotations.Count > head_delayed_frames)
				{
					rotation = delayedRotations.Dequeue();
				}
			}

			if(is_reflected_image)
			{
				rotation.y = -rotation.y;
				rotation.z = -rotation.z;
			}
			float rotX = -(rotation.x - offset.x)   * CONVERT_RADIAN ;
			float rotY =  (rotation.y - offset.y)   * CONVERT_RADIAN ;
			float rotZ =  (rotation.z + offset.z)   * CONVERT_RADIAN ;
			
			#if	DEBUG_TIME
			Debug.Log(" d1 : " + ( Time.timeSinceLevelLoad - debug_time_display ) );
			debug_time_display = Time.timeSinceLevelLoad;
			#endif				
			
			error = Ladybug.SetSphericalViewParams(CurLadybugContext, fov, rotZ, rotX, rotY, 0f, 0f, 0f );
			handleError(error, "Ladybug.SetSphericalViewParams");
			
			#if	DEBUG_TIME
			Debug.Log(" d2 : " + ( Time.timeSinceLevelLoad - debug_time_display ) );
			debug_time_display = Time.timeSinceLevelLoad;
			#endif					
			
			error = Ladybug.SetOffScreenImageSize(CurLadybugContext, LadybugOutputImage.LADYBUG_SPHERICAL, (uint)textureWidth, (uint)textureHeight );
			handleError(error, "Ladybug.SetOffScreenImageSize");
			
			
			#if	DEBUG_TIME
			Debug.Log(" d3 : " + ( Time.timeSinceLevelLoad - debug_time_display ) );
			debug_time_display = Time.timeSinceLevelLoad;
			#endif					
			
			#if LADYBUG_VERSION_110				
			error = Ladybug.RenderOffScreenImage(CurLadybugContext, LadybugOutputImage.LADYBUG_SPHERICAL, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGR16 : LadybugPixelFormat.LADYBUG_BGR), out processImage);
			#else
			error = Ladybug.RenderOffScreenImage(CurLadybugContext, LadybugOutputImage.LADYBUG_SPHERICAL, out processImage);
			#endif  	
			handleError(error, "Ladybug.RenderOffScreenImage");
			
			
			#if	DEBUG_TIME
			Debug.Log(" d4 : " + ( Time.timeSinceLevelLoad - debug_time_display ) );
			debug_time_display = Time.timeSinceLevelLoad;
			#endif					
			//Debug.Log ("w:" + processImage.uiCols  + " h:" +processImage.uiRows + " pixeclformat:" + processImage.pixelFormat);
			
		}else if(render_mode == RenderingMode.Panoramic)
		{		
			error = Ladybug.SetOffScreenImageSize(CurLadybugContext, LadybugOutputImage.LADYBUG_PANORAMIC, (uint)textureWidth, (uint)textureHeight );
			handleError(error, "Ladybug.SetOffScreenImageSiz");
			
			
			#if LADYBUG_VERSION_110				
			error = Ladybug.RenderOffScreenImage(CurLadybugContext, LadybugOutputImage.LADYBUG_PANORAMIC, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGR16 : LadybugPixelFormat.LADYBUG_BGR), out processImage);
			#else
			error = Ladybug.RenderOffScreenImage(CurLadybugContext, LadybugOutputImage.LADYBUG_PANORAMIC, out processImage);
			#endif 
			handleError(error, "Ladybug.RenderOffScreenImage");
		}
		//}
		
		if(error == LadybugError.LADYBUG_OK)
		{
			//Debug.Log ("Update Texture");
			PluginRender.UpdateTextureStream (processImage.pData, alt );
		}else{
			Debug.Log ("RenderOffScreen Error");	
		}

		
		#if	DEBUG_TIME
		Debug.Log(" d5 : " + ( Time.timeSinceLevelLoad - debug_time_display ) );
		debug_time_display = Time.timeSinceLevelLoad;
		#endif					

		ShowFPS ();
		image_updated = false;
			
	//	}
	}
	
	
	// Update is called once per frame
	virtual public void Update () {
	}


	virtual public void UpdateImage()
	{
	}



	public void handleError(LadybugError errorCode, string errorPosition )
	{
        if (errorCode != LadybugError.LADYBUG_OK)
        {
            //Console.Out.WriteLine(Ladybug.ErrorToString(errorCode));
            //UnityEngine.Debug.Log(Ladybug.ErrorToString(errorCode));
			UnityEngine.Debug.Log(errorPosition + ": " + errorCode);
        }

    }

    public void handleError(CameraGUIError errorCode)
    {
        if (errorCode != CameraGUIError.PGRCAMGUI_OK)
            UnityEngine.Debug.Log("CameraGUIError error code = " + errorCode.ToString());

    }	
	
	virtual public void IncAudioLatency()
	{}
	
	virtual public void DecAudioLatency()
	{}
	
	
	protected void CreateTextureAndPassToPlugin(int mode)
	{
		// Create a texture
		texture = new Texture2D (textureWidth, textureHeight);
		texture.Apply();
		GetComponent<Renderer>().material.mainTexture = texture;
		
		// Pass texture pointer to the plugin
		PluginRender.SetTexturePtrFromUnity (texture.GetNativeTexturePtr(), textureWidth, textureHeight, mode);
	}
	
	
	virtual public void  ThreadLadyBug(){
		
	}
	
	public virtual void SetAlpha(float alpha)
	{
		alpha_param = alpha;
		GetComponent<Renderer>().material.SetFloat ("_Param", alpha_param);
	}
	
}
