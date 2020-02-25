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

public class LadybugPluginStream : LadybugPluginInterface {
	
	public FileManager files;
    public FileManagingSpace.StreamData curStreamData;

	private bool keepstreaming = false;
	    
	public List<int> numImages;
	public List<int> startFrames;
	public List<int> endFrames;

	private bool FrameUpdated = false;

	public int streamId = 0;


	override public void Awake(){
		
		files = GameObject.Find ("FileManager").GetComponent<FileManager>();

		Init();
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
		
		initStream();
		
		
		if(is_use_thread){
			keepstreaming = true;
			thread = new Thread(ThreadLadyBug);
			thread.Start();
		}

		frame_fps_timeUntilNextInterval = frame_fps_updateInterval;
		fps_timeUntilNextInterval = fps_updateInterval;
		
		//debug
		debug_time_display = Time.timeSinceLevelLoad;
		debug_time_process = Time.timeSinceLevelLoad;
	}
	

	
	private void initStream()
    {

		LadybugError error = LadybugError.LADYBUG_OK;
		
		streamNum = files.streams.Count;
		//Debug.Log ("Ladybug : the number of streams : " + streamNum);
		
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
			

		error = Ladybug.SetFalloffCorrectionFlag(CurLadybugContext, true);
		handleError(error, "Ladybug.SetFalloffCorrectionFlag");

		error = Ladybug.SetColorProcessingMethod(CurLadybugContext, LadybugColorProcessingMethod.LADYBUG_HQLINEAR_GPU );
		handleError(error, "Ladybug.ladybugSetColorProcessingMethod");
		
		//LadybugColorProcessingMethod method;
		//error = Ladybug.GetColorProcessingMethod(CurLadybugContext, out method );
		//handleError(error, "Ladybug.ladybugGetColorProcessingMethod");
		//Debug.Log ("method :" + method);
		
		//	error  = Ladybug.SetDynamicStitching(CurStreamContext, false, true, NULL);
	//	handleError(error, "Ladybug.SetDynamicStitching");

		// if deleted, stream does not play, I don't know why
	 	bool res = commonInitialize();
		
		Debug.Log (res);
    }
	
	private void InitFrames()
	{
		for(int i=0; i<streamNum; i++)
		{	
			int start, end;
            if (files.streams[i].is_reversed)
            {
				if (files.streams[i].start_frame == 0)
                {
                    start = numImages[i] - 1;
                }
				else if (files.streams[i].start_frame > numImages[i])
                {
                    start = numImages[i] - 1;
                }
                else
                {
					start = files.streams[i].start_frame;
                    Debug.Log(i + " " + start);
                }

				if (files.streams[i].end_frame > start)
                {
                    end = start;
                }
                else
                {
					end = files.streams[i].end_frame;
                }
            }
            else
            {
				if (files.streams[i].end_frame == 0)
                {
                    end = numImages[i] - 1;
                }
				else if (files.streams[i].end_frame > numImages[i])
                {
                    end = numImages[i] - 1;
                }
                else
                {
					end = files.streams[i].end_frame;
                }

				if (files.streams[i].start_frame > end)
                {
                    start = end;
                }
                else
                {
					start = files.streams[i].start_frame;
                }
            }

			startFrames.Add (start);
			endFrames.Add (end);
		}
		
		curFrame = startFrames[streamId];
		curStreamData = files.streams[streamId];
	}

	override public void ChangeStreamStartFrame(int frame)
	{

		if (frame < numImages [streamId] && frame < endFrames [streamId]) {
			startFrames [streamId] = frame;
			files.streams[streamId].start_frame  = frame;
			Debug.Log ("change start frame " + frame);
		} else {
			startFrames [streamId] = endFrames [streamId];
			files.streams[streamId].start_frame  = frame;
		}
	}
	
	override public void ResetStream()
	{
		is_reach_the_end_flag = false;
		curFrame = startFrames[streamId];

		LadybugError error = LadybugError.LADYBUG_OK;
		error = Ladybug.GoToImage (CurStreamContext, (uint)curFrame);
		handleError (error, "Ladybug.GoToImage");
		
		error = Ladybug.ReadImageFromStream (CurStreamContext, out curImage);
		handleError (error, "Ladybug.ReadImageFromStream");

		if(isDynamicStitch)
		{
			LadybugPoint3d pPoint;
			pPoint.fX = 0; pPoint.fY = 0; pPoint.fZ = 0; pPoint.fTheta = 0; pPoint.fPhi = 0; pPoint.fCylAngle = 0; pPoint.fCylHeight = 0; 
			error  = Ladybug.SetDynamicStitching(CurLadybugContext, true, false, ref pPoint);
			handleError(error, "Ladybug.SetDynamicStitching");
		}else
		{
			error  = Ladybug.Set3dMapSphereSize(CurLadybugContext, dRadius);
			handleError(error, "Ladybug.Set3dMapSphereSize");
		}

		imageStartTime = (double)curImage.imageInfo.ulTimeSeconds + (double)curImage.imageInfo.ulTimeMicroSeconds / 1000000;
		actualStartTime = Time.timeSinceLevelLoad;
		
		image_updated = false;
        FrameUpdated = true;
	}

	override public void SetForeGround()
	{	
		
		is_reach_the_end_flag = false;
		MovingFrame();	
		UpdateImage ();
		displayLadybugImages(false);

		PluginRender.SetStreamRenderStatus (true);
		foreground = true;
	}
	
	override public void SetBackGround()
	{
		PluginRender.SetStreamRenderStatus (false);
		foreground = false;
	}


	override  public void GoToStream(int index)
    {
        Debug.Log("GotoStream");

        streamId = index;

        LadybugError error = LadybugError.LADYBUG_OK;
		error = Ladybug.InitializeStreamForReading(CurStreamContext, files.streams[streamId].stream_name, true);
        handleError(error, "Ladybug.InitializeStreamForReading (GoToStream)");

        error = Ladybug.ReadImageFromStream(CurStreamContext, out curImage);
        handleError(error, "Ladybug.ReadImageFromStream (GoToStream)");

        curFrame = startFrames[streamId];
		curStreamData = files.streams[streamId];

		ResetStream ();
    }
	
	
	override  public void GoToStreamBackground(int index)
	{
		
		Debug.Log ("GotoStream  background");
		GoToStream(index);
		
		processLadybugImages();
	}
	

	override public void cleanup(){
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

	// Update is called once per frame
	override public void Update () {
		
		if(foreground)
		{
			MovingFrame();	
			if(!is_use_thread)
			{
				UpdateImage ();
			}
			displayLadybugImages(false);

		}
	}

	override public void UpdateImage()
	{
		if (FrameUpdated) 
		{
//			Debug.Log ("processLadybugImages curFrame" + curFrame);

			processLadybugImages();
			
			FrameUpdated = false;
		}
	}


	private void MovingFrame()
	{
        if (FrameUpdated) return;
		// only called at the first frame
		if(curFrame == startFrames[streamId])
        //  if(FrameUpdated && is_started)
        {
         /*

			//for frame fps
			//frame_lastsecond = Time.timeSinceLevelLoad;
            //Debug.Log("frame 1: " + imageStartTime + " " + (imageStartTime - 1392036812.69753f) );
            */
  		}

		//frame adjusting
		double curImageTime = (double)curImage.imageInfo.ulTimeSeconds + (double)curImage.imageInfo.ulTimeMicroSeconds / 1000000;
        float imageElapsed = (float)(curImageTime - imageStartTime) ;
		float actualElapsed;

		//Debug.Log("frame: " + curFrame +"  curImageTime: " +  curImageTime + " startTime:" + imageStartTime + " elapsed" + imageElapsed);

		//check audio sync
        if (curStreamData.with_audio_time)
		{
			actualElapsed = ( AudioCtl.GetCurAudioTime() - AudioCtl.offset_time ) ;//+ curStreamData.audio_latency; 
		}else{
			actualElapsed = Time.timeSinceLevelLoad - actualStartTime;
		}
		//change speed
        actualElapsed *= curStreamData.play_speed;

		//Debug.Log ("audio time: " + AudioCtl.GetCurAudioTime ()+ " actual eplased" + actualElapsed + " image time" + imageElapsed);

		//change reverse
		int reverse_coeff = curStreamData.is_reversed ? -1 : 1;
		imageElapsed *= reverse_coeff;

        //Debug.Log(curFrame + ": " + actualElapsed + " " + imageElapsed + " " + curImageTime);
        if (imageElapsed > actualElapsed )
		{
			//Debug.Log ("too early " + imageElapsed + " " + actualElapsed);
			return;
		}else
		{ 
			int counter = 0;
			do{	
				counter++;
				curFrame += reverse_coeff;
				//added for faster movie
				LadybugError error = LadybugError.LADYBUG_OK;
				error = Ladybug.GoToImage (CurStreamContext, (uint)curFrame);
				handleError (error, "Ladybug.GoToImage");

				error = Ladybug.ReadImageFromStream (CurStreamContext, out curImage);
				handleError (error, "Ladybug.ReadImageFromStream");

				curImageTime = (double)curImage.imageInfo.ulTimeSeconds + (double)curImage.imageInfo.ulTimeMicroSeconds / 1000000;
				imageElapsed = (float)(reverse_coeff*(curImageTime - imageStartTime)) ;

				//if(counter == 1) Debug.Log (curFrame + " " +  actualElapsed );
				//if(counter > 1) Debug.Log ("too late " + counter +" " + imageElapsed + " " + actualElapsed);
			}while(imageElapsed < actualElapsed - 0.0165f && counter < 30);

			// end of the frame conditions
			if (curStreamData.is_reversed)
            {
				if (curFrame < endFrames[streamId] + 1)
				{
					if(is_repeat_after_end)
					{
						ResetStream();
						AudioCtl.ReplayAudio();
					}else{
						is_reach_the_end_flag = true;
					}
				}
			}else{
				if (curFrame > endFrames[streamId] - 1)
				{
					if(is_repeat_after_end)
					{
						ResetStream();
						AudioCtl.ReplayAudio();
					}else{
						is_reach_the_end_flag = true;
					}
				}
			}
			
			FrameUpdated = true;
			ShowFrameFPS ();
		}


	}
	
	override public void IncAudioLatency()
	{
        curStreamData.audio_latency  += 0.01f;

        Debug.Log("audio latency :" + curStreamData.audio_latency);
	}
	
	override public void DecAudioLatency()
	{
        curStreamData.audio_latency -= 0.01f;
        Debug.Log("audio latency :" + curStreamData.audio_latency);
	}


	override public void ThreadLadyBug(){
		Debug.Log ("Thread Start");
		while(keepstreaming){
			Thread.Sleep(0);
			if(foreground)
			{	
				UpdateImage();
			}
		}
		Debug.Log ("Thread Finished");
	}
	
	
}
