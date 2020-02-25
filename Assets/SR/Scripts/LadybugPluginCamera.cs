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

public class LadybugPluginCamera : LadybugPluginInterface {
	
	
	private bool keepgrabbing = false;
	
	override public void Awake(){
		
		Init ();
	}
	
	override public void Start () {
		
		if(render_mode == RenderingMode.Spherical)
		{
			textureWidth = SphereWidth;
	 		textureHeight = SphereHeight;
		}else if(render_mode == RenderingMode.Panoramic)
		{
			textureWidth = PanoWidth;
	 		textureHeight = PanoHeight;
		}
			
		CreateTextureAndPassToPlugin((int)ScreenMode.Stream);	
		initCamera();
		
		if(is_use_thread){
			keepgrabbing = true;
			thread = new Thread(ThreadLadyBug);
			thread.Start();
		}
		
		fps_timeUntilNextInterval = fps_updateInterval;
		
		//debug
		debug_time_display = Time.timeSinceLevelLoad;
		debug_time_process = Time.timeSinceLevelLoad;

	}
	
	override public void ResetStream()
	{
	}
	override  public void GoToStream(int index)
	{
	}
	override  public void GoToStreamBackground(int index)
	{
	}
		
	
	

	
	private void initCamera()
    {
		
		LadybugError error = LadybugError.LADYBUG_OK;
		//CameraGUIError guierror;
		
		//Intialize Context
		error = Ladybug.CreateContext(out CurLadybugContext);
 		handleError(error, "Ladybug.CreateContext");
			
		error = Ladybug.InitializeFromIndex(CurLadybugContext, 0);
        handleError(error, "Ladybug.InitializeFromSer, ialNumber");

        LadybugCameraInfo camInfo = new LadybugCameraInfo();
        error = Ladybug.GetCameraInfo(CurLadybugContext, ref camInfo);
        handleError(error, "Ladybug.GetCameraInfo");
		Debug.Log (camInfo.pszModelName);
		
		error = Ladybug.LoadConfig(CurLadybugContext, null);
        handleError(error, "Ladybug.LoadConfig");
		
        error = Ladybug.Start(CurLadybugContext, LadybugDataFormat.LADYBUG_DATAFORMAT_ANY);
        handleError(error, "Ladybug.Start");
		
		int retry = 5;
        do
        {
            lock (sync)
            {
                error = Ladybug.GrabImage(CurLadybugContext, out curImage);
				handleError(error, "GrabImage");
                retry--;
            }
        } while (error != LadybugError.LADYBUG_OK && retry > 0);
		
		
	 	//bool res = commonInitialize();
    }
	
	

	override public void cleanup(){
		LadybugError error;
            
		streamReady = false;
		image_updated = false;
	
	
		
		if(is_use_thread){
			lock(sync){		
			    keepgrabbing = false;
                int cnt = 0;
             	while (thread.IsAlive)
             	{
                	 Thread.Sleep(10);
                 	cnt++;
             	}
				error = Ladybug.Stop(CurLadybugContext);
            	handleError(error, "Ladybug.Stop");
			
			}
		
			if( thread != null)
			{
				Debug.Log ("thread abort");
				thread.Abort(); 
  			}
		}

		if (CurLadybugContext != 0)
        {
        	error = Ladybug.DestroyContext(ref CurLadybugContext);
			handleError(error, "Ladybug.DestroyContext");
			
			CurLadybugContext = 0;
		}

	}


	// Update is called once per frame
	override public void Update () {
		
		if(foreground)
		{
			if(!is_use_thread)
			{
				UpdateImage();
			}
			
			processLadybugImages();
			if (image_updated)
			{
				displayLadybugImages(false);
			}
		}
	}


	
	override public void UpdateImage()
	{
		LadybugError error = Ladybug.GrabImage(CurLadybugContext, out curImage);
		handleError(error, "Ladybug.GrabImage");

		//ShowFrameFPS ();
	}


	override public void ThreadLadyBug(){
		UnityEngine.Debug.Log("grabLoop started");
		while(keepgrabbing){
			if(foreground)
			{	
				Thread.Sleep(0);
				UpdateImage ();
				
			}
		}
		UnityEngine.Debug.Log("grabLoop done");
	}
	
	
}
