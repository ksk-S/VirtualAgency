//for Ladybug SDK 1.9 - 6, but cannot render streams more than 2 correctly
#define LADYBUG_VERSION_110
//#define DEBUG_TIME
#if LADYBUG_VERSION_110
using LadybugAPI110;
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

public class DomeStreamPlugin : MonoBehaviour {
	
	FileManager files;
	//Unity	
	Texture2D texture;
	//Texture
//	static int SphereWidth = 640;
//	static int SphereHeight = 512;
		
//	static int SphereWidth = 1280;
//	static int SphereHeight = 1024;
	
	static int SphereWidth = 1024;
	static int SphereHeight = 720;

	static int PanoWidth = 2048;
	static int PanoHeight = 1024;

	static int textureWidth = 1024;
	static int textureHeight = 1024;
	
	//Audio 
	private AudioManager AudioCtl;
	
	//Ladybug
	const string temp_config_file = "config.cal";
	
//	private List<int> ladybugContexts; 
//    private List<int> streamContexts;
	
	private int CurLadybugContext;
	private int CurStreamContext;	
	
	public List<int> numImages;
	public List<int> startFrames;
	public List<int> endFrames;
	
	public int streamNum;
	public int streamId = 0;
	

	
	private LadybugProcessedImage processImage;
	private LadybugImage curImage;
	public int curFrame = 0;
	
	
	double imageStartTime = 0f;
	float actualStartTime = 0f;

	private byte[] textureBuffer;
	
	public float debug_time;
//    private bool textureSizeChanged = false;
	
#if LADYBUG_VERSION_110	
	bool isHighBitDepth;
#endif
	
	//bit of spped up
	public bool is_use_thread = false;
	
	//Oculus Rift
	public RenderingMode render_mode = RenderingMode.Spherical;
	//public float fov = 110;
	public float fov = 78;
	
	//flags
	public bool streamReady	 = false;
	private bool image_updated = true;
	private bool keepstreaming = false;
//	private bool frame_stopping = false;
	
	public bool foreground = true;
	public Vector3 offset = new Vector3 (0.0f, 0.0f, 0.0f);


	
	//thraeding
	System.Object sync = new System.Object();
	Thread thread;
	
	// FrameFps
	public float FrameFps;
	int frame_numFrames = 0;
	float frame_lastsecond = 0;
	float frame_fps_updateInterval = 1.0f; 
	float frame_fps_timeUntilNextInterval; 
	
	// FPS calculater
	public float fps;
	float fps_updateInterval = 1.0f; 
	float fps_accumulated   = 0.0f;
	float fps_timeUntilNextInterval; 
	int fps_numFrames = 0;
	
	
	public float dRadius = 1.1f;
	
	const float CONVERT_RADIAN = 2.0f * Mathf.PI /360f;
	// Use this for initialization
	
	void Awake(){
		
		files = GameObject.Find ("FileManager").GetComponent<FileManager>();
		
		InitAudio ();
	}
	
	
	void Start () {
		
		if(render_mode == RenderingMode.Spherical)
		{
			textureWidth = SphereWidth;
	 		textureHeight = SphereHeight;
		}else if(render_mode == RenderingMode.Panoramic)
		{
			textureWidth = PanoWidth;
	 		textureHeight = PanoHeight;
		}
			
		CreateTextureAndPassToPlugin();	
		
		initStream();
		
		
		if(is_use_thread){
			keepstreaming = true;
			thread = new Thread(ThreadLadyBug);
			thread.Start();
		}
		
		fps_timeUntilNextInterval = fps_updateInterval;
		
		//debug
		debug_time = Time.timeSinceLevelLoad;
	}
	
	public void SetForeGround()
	{	
		PluginRender.SetStreamRenderStatus (true);
		foreground = true;
	}
	
	public void SetBackGround()
	{
		PluginRender.SetStreamRenderStatus (false);
		foreground = false;
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
		if(foreground)
		{
			if(!is_use_thread)
			{
        		processLadybugImages();
			}
			
			displayLadybugImages();
			
			ShowFPS();
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
	
	void ShowFrameFPS()
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
	
	
	private void initStream()
    {
		LadybugError error = LadybugError.LADYBUG_OK;
		
		streamNum = files.streams.Count;
		Debug.Log ("Ladybug : the number of streams : " + streamNum);
		
		if(streamId > streamNum - 1){
			streamId = 	streamNum - 1;
		}
		
		//Intialize Context
		error = Ladybug.CreateContext(out CurLadybugContext);
 		handleError(error, "Ladybug.CreateContext");
			
		error = Ladybug.CreateStreamContext(out CurStreamContext);
	 	handleError(error, "Ladybug.CreateStreamContext");
		
		error = Ladybug.GetStreamConfigFile(CurStreamContext, temp_config_file);
	 	handleError(error, "Ladybug.GetStreamConfigFile");

		LadybugStreamHeadInfo streamHeadInfo;
		error = Ladybug.GetStreamHeader(CurStreamContext, out streamHeadInfo, null);
	 	handleError(error, "Ladybug.GetStreamHeader");

		error = Ladybug.SetColorTileFormat(CurLadybugContext, streamHeadInfo.stippledFormat);
		handleError(error, "Ladybug.SetColorTileFormat");
			
		error = Ladybug.LoadConfig(CurLadybugContext, temp_config_file);
		handleError(error, "Ladybug.LoadConfig");
	 		
		//Set Frame Length for each stream
		numImages = new List<int>(streamNum);
		for(int i=0; i<streamNum; i++)
		{	
			//Debug.Log ("Configure Stream : " + i);
			error = Ladybug.InitializeStreamForReading(CurStreamContext, files.streams[i].stream_name, true);
		 	handleError(error, "Ladybug.InitializeStreamForReading");
			
			uint tmp_numImage;
	 		error = Ladybug.GetStreamNumOfImages(CurStreamContext, out tmp_numImage);
	 		handleError(error, "Ladybug.GetStreamNumOfImages");
			numImages.Add ((int)tmp_numImage);
		}
		//Set start and end of frames
		InitFrames();
		
		//Set Initial Stream 
		error = Ladybug.InitializeStreamForReading(CurStreamContext, files.streams[streamId].stream_name, true);
	 	handleError(error, "Ladybug.InitializeStreamForReading");			
		
		error = Ladybug.ReadImageFromStream(CurStreamContext, out curImage);
		handleError(error, "Ladybug.ReadImageFromStream");
			
	
	
		
	 	// bool res = commonInitialize();
		
		
    }
	
	void InitFrames()
	{
		for(int i=0; i<streamNum; i++)
		{	
			int start, end;
			if(files.streams[i].end_frame == 0){
				end = numImages[i];		
			}else if(endFrames[i] > numImages[i]){
				end = numImages[i];	
			}else{
				end = files.streams[i].end_frame;	
			}
			
			if(files.streams[i].start_frame > end){
				start = end;
			}else{
				start = files.streams[i].start_frame;	
			}
			startFrames.Add (start);
			endFrames.Add (end);
		}
		
		curFrame = startFrames[streamId];
	}
	
	void InitAudio()
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
		
		Ladybug.Set3dMapSphereSize(CurLadybugContext, dRadius);
		
        return (error == 0);
	}
	
	public void ResetStream()
	{
		Debug.Log ("Reset stream");
		curFrame = startFrames[streamId];
		
		image_updated = false;
	}
	
	public void GoToStream(int index)
	{
		Debug.Log ("GotoStream");

		streamId = index;
		LadybugError error = LadybugError.LADYBUG_OK;

		error = Ladybug.InitializeStreamForReading(CurStreamContext, files.streams[streamId].stream_name, true);
		handleError(error, "Ladybug.InitializeStreamForReading (GoToStream)");
			
	 	error = Ladybug.ReadImageFromStream(CurStreamContext, out curImage);
		handleError(error, "Ladybug.ReadImageFromStream (GoToStream)");	
		
		curFrame = startFrames[streamId];
		
		image_updated = false;
	
	}
	
	
	public void GoToStreamBackground(int index)
	{
		
		Debug.Log ("GotoStream  background");
		GoToStream(index);
		
		processLadybugImages();
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

	void cleanup(){
		LadybugError error;
            
		streamReady = false;
		image_updated = false;
		keepstreaming = false;
	
		lock(sync){
			if (CurStreamContext != 0)
        	{
				error = Ladybug.StopStream(CurStreamContext);
				handleError(error, "Ladybug.StopStream");
        	   
       	   		error = Ladybug.DestroyStreamContext(ref CurStreamContext);
		   		handleError(error, "Ladybug.DestroyStreamContext");
			}
			
	
			if (CurLadybugContext != 0)
    		{
				error = Ladybug.DestroyContext(ref CurLadybugContext);
				handleError(error, "Ladybug.DestroyContext");
				
			}
		}
		

	}
	
    [MethodImpl(MethodImplOptions.Synchronized)]
    unsafe private void processLadybugImages()
    {
#if	DEBUG_TIME
		Debug.Log(" p1 : " +  ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif
		//Debug.Log ("ProcssLadybugImage");
		//Debug.Log ("processLadybugImages");
		if (image_updated) // if the previously processed image is not displayed, do nothing.
			return;
		
		LadybugError error;
		
		error = Ladybug.GoToImage(CurStreamContext, (uint)curFrame);
		handleError(error, "Ladybug.GoToImage");
		
	//	Debug.Log (curFrame + "/" + numImages);
		//Console.WriteLine("process_and_display seqnum=" + curImage.uiSeqNum.ToString()
        //                + " seqid=" + curImage.imageInfo.ulSequenceId);
		
#if	DEBUG_TIME
		Debug.Log(" p2 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif
		
	
		fixed (byte* texBufPtr = &textureBuffer[0])
        {
			// get image
			error = Ladybug.ReadImageFromStream(CurStreamContext, out curImage);
	 		handleError(error, "Ladybug.ReadImageFromStream");
			
            // this is a trick to make a pointer of arrays.
            byte** texBufPtrArray = stackalloc byte*[6];
            for (int i = 0; i < 6; i++)
            {
#if LADYBUG_VERSION_110
				texBufPtrArray[i] = texBufPtr + (isHighBitDepth ? i * 2 : i) * curImage.uiRows * curImage.uiCols * 4;
#else                 
				texBufPtrArray[i] = texBufPtr +  i * curImage.uiRows * curImage.uiCols * 4;
#endif
            }
			
#if	DEBUG_TIME
		Debug.Log(" p3 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif
			
#if LADYBUG_VERSION_110
			error = Ladybug.ConvertImage(CurLadybugContext, ref curImage, texBufPtrArray, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU));  
#else
			LadybugImageInfo imageInfo;
			error = Ladybug.ConvertToMultipleBGRU32(CurLadybugContext, ref curImage, texBufPtrArray, out imageInfo);		
#endif
			handleError(error, "Ladybug.ConvertImage");
	
#if	DEBUG_TIME
		Debug.Log(" p4 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif
			
            if (error == LadybugError.LADYBUG_OK)
            {
#if LADYBUG_VERSION_110				
				error = Ladybug.UpdateTextures(CurLadybugContext, Ladybug.LADYBUG_NUM_CAMERAS, texBufPtrArray, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU));
#else
				error = Ladybug.UpdateTextures(CurLadybugContext, Ladybug.LADYBUG_NUM_CAMERAS, texBufPtrArray);
				
#endif              
                handleError(error, "Ladybug.UpdateTextures");
            }
			
#if	DEBUG_TIME
		Debug.Log(" p5 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif
            
        }
		if(error == LadybugError.LADYBUG_OK)
		{
        	image_updated = true;		
		}
	}
		
	
	unsafe public void displayLadybugImages()
	{
		
 		if (image_updated)
        {
			//Debug.Log ("display_dll");
				
			LadybugError error = LadybugError.LADYBUG_OK;
			//lock(sync){
            //spherical view
            if(render_mode == RenderingMode.Spherical)
			{
				Quaternion CameraOrientation = Quaternion.identity;
            	//ksk has to replace
				//OVRDevice.GetPredictedOrientation(0, ref CameraOrientation);                    
               
                Vector3 rotation = CameraOrientation.eulerAngles;
               
			    float rotX = -(rotation.x - offset.x)   * CONVERT_RADIAN ;
	            float rotY =  (rotation.y + offset.y)   * CONVERT_RADIAN ;
                float rotZ =  (rotation.z + offset.z)   * CONVERT_RADIAN ;
              
#if	DEBUG_TIME
		Debug.Log(" d1 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif			
				
                error = Ladybug.SetSphericalViewParams(CurLadybugContext, fov, rotZ, rotX, rotY, 0f, 0f, 0f );
                handleError(error, "Ladybug.SetSphericalViewParams");
				
#if	DEBUG_TIME
		Debug.Log(" d2 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif					
				
                error = Ladybug.SetOffScreenImageSize(CurLadybugContext, LadybugOutputImage.LADYBUG_SPHERICAL, (uint)textureWidth, (uint)textureHeight );
                handleError(error, "Ladybug.SetOffScreenImageSize");
				
				
#if	DEBUG_TIME
		Debug.Log(" d3 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif					
				
#if LADYBUG_VERSION_110				
				error = Ladybug.RenderOffScreenImage(CurLadybugContext, LadybugOutputImage.LADYBUG_SPHERICAL, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGR16 : LadybugPixelFormat.LADYBUG_BGR), out processImage);
#else
				error = Ladybug.RenderOffScreenImage(CurLadybugContext, LadybugOutputImage.LADYBUG_SPHERICAL, out processImage);
#endif  	
				handleError(error, "Ladybug.RenderOffScreenImage");
              
				
#if	DEBUG_TIME
		Debug.Log(" d4 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
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
				PluginRender.UpdateTextureStream (processImage.pData, false);
			}else{
				Debug.Log ("RenderOffScreen Error");	
			}
		
			
			
#if	DEBUG_TIME
		Debug.Log(" d5 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif						
			//in first frame reset time
			if(curFrame == startFrames[streamId])
			{
				imageStartTime = (double)curImage.imageInfo.ulTimeSeconds + (double)curImage.imageInfo.ulTimeMicroSeconds / 1000000;
				actualStartTime = Time.timeSinceLevelLoad;
				
				//for frame fps
				frame_lastsecond = Time.timeSinceLevelLoad;
				
				//Debug.Log("frame 1: "+ imageStartTime);
			}
		
			//frame adjusting
			double curImageTime = (double)curImage.imageInfo.ulTimeSeconds + (double)curImage.imageInfo.ulTimeMicroSeconds / 1000000;
			float imageElapsed = (float)(curImageTime - imageStartTime);
			float actualElapsed;
			
			//Debug.Log("frame " + curFrame +": " +  curImageTime);
			
			if(files.streams[streamId].with_audio_time)
			{
				actualElapsed =  AudioCtl.GetCurAudioTime();
				//roll back by audio
				if(AudioCtl.IsNearEnd())
				{
					curFrame = startFrames[streamId]; 
				}
			}else{
				actualElapsed =  Time.timeSinceLevelLoad - actualStartTime;
			}
			
		
			if(imageElapsed > actualElapsed + files.streams[streamId].audio_latency)
			{
				//frame_stopping = true;
			}else
			{
				curFrame++;	
				ShowFrameFPS();
			}
			
			
#if	DEBUG_TIME
		Debug.Log(" d6 : " + ( Time.timeSinceLevelLoad - debug_time ) );
		debug_time = Time.timeSinceLevelLoad;
#endif					
			//roll back
			if(curFrame > endFrames[streamId] - 1){
				curFrame = startFrames[streamId]; 
			}
			
			image_updated = false;
			
		}
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
	
	public void IncAudioLatency()
	{
		files.streams[streamId].audio_latency = files.streams[streamId].audio_latency += 0.01f;
		
		Debug.Log ("audio latency :" + files.streams[streamId].audio_latency);
	}
	
	public void DecAudioLatency()
	{
		files.streams[streamId].audio_latency = files.streams[streamId].audio_latency -= 0.01f;
		Debug.Log ("audio latency :" + files.streams[streamId].audio_latency);
	}
	
	
	private void CreateTextureAndPassToPlugin()
	{
		// Create a texture
		texture = new Texture2D (textureWidth, textureHeight);
		texture.Apply();
		GetComponent<Renderer>().material.mainTexture = texture;
		
		// Pass texture pointer to the plugin
		PluginRender.SetTexturePtrFromUnity (texture.GetNativeTexturePtr(), textureWidth, textureHeight, (int)ScreenMode.Stream);
	}
	
	
	void ThreadLadyBug(){
		while(keepstreaming){
			if(foreground)
			{	
				Thread.Sleep(0);
				lock(sync){
					processLadybugImages();
				}
			}
		}
	}
	
	
}
