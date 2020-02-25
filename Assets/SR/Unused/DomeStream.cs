// Drawing Stream Image but using SetPixcel. too slow

using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
//using System.Drawing;
using System.Text;
//using System.Windows.Forms;
//using System.Drawing.Imaging;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;


using LadybugAPI19;

public class DomeStream : MonoBehaviour {
	public string path = "Assets\\Ladybug\\PGR\\ladybug_FullJPEG_00000071-000000.pgr";
	//public string path = "C:\\Data\\PGR\\20101006\\exp1Ladybug-Stream-000000.pgr";
	//public string path = "C:\\Data\\PGR\\20101002\\exp1Ladybug-Stream-000000.pgr";
	public uint numImages = 0;
	public int curFrame = 0;
	
	Texture2D texture;
	
//	static int textureWidth = 2048;/
//	static int textureHeight = 1024;
//	static int textureWidth = 1024;
//	static int textureHeight = 512;
	static int textureWidth = 1572;
	static int textureHeight = 786;

	private Color32[] color32 = new Color32[textureWidth * textureHeight];
	
	private int ladybugContext;
    private int streamContext;
    private int guiContext;
	
	// textures
    private byte[] textureBuffer;
    private bool textureSizeChanged = false;
	
	//flags
	private bool image_updated = false;
	private bool initialized = false;
	private bool keepstreaming = false;
	
	private LadybugImage curImage;
	
	const string temp_config_file = "config.cal";
	
	//thraeding
	System.Object sync = new System.Object();
	Thread thread;
	
	// fps
	public GameObject FPSObj;
	//GUIText FPSGuiText;
	public float fps;
	public float fps_updateInterval = 1.0f; 
	float fps_accumulated   = 0.0f;
	float fps_timeUntilNextInterval; 
	int fps_numFrames = 0;
	
	// Use this for initialization
	
	void Awake(){
		}

	void Start(){
		texture = new Texture2D (textureWidth, textureHeight);
		GetComponent<Renderer>().material.mainTexture = texture;
	
		initStream();
		
		keepstreaming = true;
		thread = new Thread(ThreadLadyBug);
		thread.Start();
		
		fps_timeUntilNextInterval = fps_updateInterval;
		//FPSGuiText = FPSObj.AddComponent ("GUIText") as GUIText;
	}
	
	/*
	IEnumerator Start () {
		initStream();
		
		fps_timeUntilNextInterval = fps_updateInterval;
		FPSGuiText = FPSObj.AddComponent ("GUIText") as GUIText;
		
		CreateTextureAndPassToPlugin();		
		yield return StartCoroutine("CallPluginAtEndOfFrames");
		
	}
	*/
	
	// Update is called once per frame
	void Update () {
        //processAndDisplay();
	 	display3();
		ShowFPS();
	}
	
	void ShowFPS(){
		fps_timeUntilNextInterval -= Time.deltaTime;
		fps_accumulated += Time.timeScale / Time.deltaTime;
		++fps_numFrames;

		if( fps_timeUntilNextInterval <= 0.0 )
		{
			fps = fps_accumulated / fps_numFrames;
			//string format = System.String.Format( "FPS: {0:F2}", fps );
			//FPSGuiText.text = format;
 
			fps_timeUntilNextInterval = fps_updateInterval;
			fps_accumulated = 0.0F;
			fps_numFrames = 0;
		}
	}
	
	private void initStream()
    {
	
		LadybugError error = Ladybug.CreateContext(out ladybugContext);
	 	handleError(error);

		error = Ladybug.CreateStreamContext(out streamContext);
	 	handleError(error);

	 	error = Ladybug.InitializeStreamForReading(streamContext, path, true);
	 	handleError(error);
		
	 	error = Ladybug.GetStreamConfigFile(streamContext, temp_config_file);
	 	handleError(error);

		LadybugStreamHeadInfo streamHeadInfo;
		error = Ladybug.GetStreamHeader(streamContext, out streamHeadInfo, null);
	 	handleError(error);

	 	error = Ladybug.SetColorTileFormat(ladybugContext, streamHeadInfo.stippledFormat);
	 	handleError(error);

	 	error = Ladybug.LoadConfig(ladybugContext, temp_config_file);
		handleError(error);
	
	 	error = Ladybug.ReadImageFromStream(streamContext, out curImage);
	 	handleError(error);
		
	
	 	error = Ladybug.GetStreamNumOfImages(streamContext, out numImages);
	 	handleError(error);
		
	 	//bool res = commonInitialize();
		
	 	processAndDisplay();
			
    }
	

	
	public bool commonInitialize()
	{
		
        LadybugColorProcessingMethod colorProc = LadybugColorProcessingMethod.LADYBUG_DOWNSAMPLE4; // fast
        //LadybugColorProcessingMethod colorProc = LadybugColorProcessingMethod.LADYBUG_HQLINEAR; // slow but hiqh quality
        

		LadybugError error = Ladybug.SetColorProcessingMethod(ladybugContext, colorProc);
        handleError(error);
		
        textureBuffer = new byte[Ladybug.LADYBUG_NUM_CAMERAS * curImage.uiRows * curImage.uiCols * 4];
	
        initializeAlphaMasks();

        error = Ladybug.ConfigureOutputImages(ladybugContext, (0x1 << 12) );//LadybugOutputImage.LADYBUG_PANORAMIC
        
        handleError(error);

        //opengl.bind();
        //error = Ladybug.SetDisplayWindow(ladybugContext);
        //handleError(error);
        //opengl.unbind();

        image_updated = false;

        if (error == 0)
        {
            initialized = true;
        }
        return (error == 0);
   		
	}

	private bool initializeAlphaMasks()
    {
		LadybugError error;

		if (ladybugContext == 0) return false;

		LadybugColorProcessingMethod curMethod;
		Ladybug.GetColorProcessingMethod(ladybugContext, out curMethod);

		uint texture_width = curImage.uiCols;
		uint texture_height = curImage.uiRows;
		if (curMethod == LadybugColorProcessingMethod.LADYBUG_DOWNSAMPLE4 || curMethod == LadybugColorProcessingMethod.LADYBUG_MONO)
		{
			texture_width /= 2;
			texture_height /= 2;
		}

		error = Ladybug.LoadAlphaMasks(ladybugContext, texture_width, texture_height);
		if (error != LadybugError.LADYBUG_OK)
		{
		 	error = Ladybug.InitializeAlphaMasks(ladybugContext, texture_width, texture_height, false);
			handleError(error);
		}

            return (error == 0);
	}
	
	void OnDisable()
	{
		
		// cleanup();
	}
	
	void OnApplicationQuit()
	{
		
   	    cleanup();
	}
		
	void OnDestroy()
	{
		
		cleanup();
	}

	void cleanup(){
		LadybugError error;
            
		initialized = false;
		image_updated = false;
		keepstreaming = false;
	
		lock(sync){
			if (streamContext != 0)
        	{
            	error = Ladybug.StopStream(streamContext);
            	handleError(error);

	            error = Ladybug.DestroyStreamContext(ref streamContext);
    	        handleError(error);
       	 	}

	        if (ladybugContext != 0)
    	    {
        	    error = Ladybug.DestroyContext(ref ladybugContext);
            	handleError(error);
        	}
		}

	}
	
    [MethodImpl(MethodImplOptions.Synchronized)]
    unsafe private void processAndDisplay()
    {
		if (image_updated) // if the previously processed image is not displayed, do nothing.
			return;
		
		curFrame++;
		if(curFrame > numImages){ curFrame = 0; }
		Ladybug.GoToImage(streamContext, (uint)curFrame);
		
		//Console.WriteLine("process_and_display seqnum=" + curImage.uiSeqNum.ToString()
        //                + " seqid=" + curImage.imageInfo.ulSequenceId);

		LadybugError error;
		fixed (byte* texBufPtr = &textureBuffer[0])
        {
			// get image
			error = Ladybug.ReadImageFromStream(streamContext, out curImage);
	 		handleError(error);
			
			// Account for 16 bit data
            bool isHighBitDepth = dataFormat.isHighBitDepth(curImage.dataFormat);
			
			
			//============== need to each image grab?
			//==============
            // this is a trick to make a pointer of arrays.
            byte** texBufPtrArray = stackalloc byte*[6];
            for (int i = 0; i < 6; i++)
            {
                texBufPtrArray[i] = texBufPtr + i * curImage.uiRows * curImage.uiCols * 4;
            }

            if (textureSizeChanged)
            {
                textureSizeChanged = false;
                initializeAlphaMasks();
            }
			//=================
		   error = Ladybug.ConvertImage(ladybugContext, ref curImage, texBufPtrArray, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU));
         
			
            handleError(error);
			
            if (error == LadybugError.LADYBUG_OK)
            {
                
				error = Ladybug.UpdateTextures(ladybugContext, Ladybug.LADYBUG_NUM_CAMERAS, texBufPtrArray, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU));
                
				handleError(error);
				
            }
            
        }
        image_updated = true;		
		
    }
	
	//ugoku kedo fps=9
	unsafe public void display3()
	{
  	    if (initialized)
        {
			// Account for 16 bit data
            bool isHighBitDepth = dataFormat.isHighBitDepth(curImage.dataFormat);
			
			
			LadybugProcessedImage processImage;
			//LadybugError error;
			image_updated = false;
			//lock(sync){
			 Ladybug.SetOffScreenImageSize(ladybugContext, LadybugOutputImage.LADYBUG_PANORAMIC, (uint)textureWidth, (uint)textureHeight );
			 Ladybug.RenderOffScreenImage( ladybugContext, LadybugOutputImage.LADYBUG_PANORAMIC, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU), out processImage);
			//}
			//Debug.Log ((int)processImage.uiCols + " " +  (int)processImage.uiRows);
		
			byte* tmpBuffer = &processImage.pData[0];
    	    	for( var i = 0; i < textureWidth * textureHeight; i++, tmpBuffer = tmpBuffer + 3)
				//for( var i = 0; i < textureWidth * textureHeight; i++ )
			
				{
					color32[i].r = *(tmpBuffer +2);
					color32[i].g = *(tmpBuffer +1);
					color32[i].b = *(tmpBuffer ) ;
				/*
					color32[i].r = *(tmpBuffer + i * 3 +2);
					color32[i].g = *(tmpBuffer + i * 3 +1);
					color32[i].b = *(tmpBuffer + i * 3) ;
					*/
				}
			
			texture.SetPixels32(color32);
			texture.Apply ();
		}
	}
	//ugokanai dllgawade texture hari
	unsafe public void display2()
	{
		LadybugProcessedImage processImage;
		image_updated = false;
		
		// Account for 16 bit data
        bool isHighBitDepth = dataFormat.isHighBitDepth(curImage.dataFormat);
			
		
		Ladybug.SetOffScreenImageSize(ladybugContext, LadybugOutputImage.LADYBUG_PANORAMIC, (uint)textureWidth, (uint)textureHeight );
		
		Ladybug.RenderOffScreenImage(ladybugContext, LadybugOutputImage.LADYBUG_PANORAMIC, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU), out processImage);
			 	
		
		//int textureID = texture.GetNativeTextureID();
		//MyLadybug.RenderTextureUpdate(textureID, textureWidth,textureHeight, processImage.pData);
	}
	//ugokanai load image
	unsafe public void display()
	{
		LadybugProcessedImage processImage;
		image_updated = false;
		
		
		// Account for 16 bit data
        bool isHighBitDepth = dataFormat.isHighBitDepth(curImage.dataFormat);
		Ladybug.RenderOffScreenImage(ladybugContext, LadybugOutputImage.LADYBUG_PANORAMIC, (isHighBitDepth ? LadybugPixelFormat.LADYBUG_BGRU16 : LadybugPixelFormat.LADYBUG_BGRU), out processImage);
	
		Texture2D tex = new Texture2D ((int)processImage.uiRows, (int)processImage.uiCols);
		Debug.Log ((int)processImage.uiRows + " " +  (int)processImage.uiCols);
		
		if( processImage.pixelFormat == LadybugPixelFormat.LADYBUG_BGR)
		{
			Debug.Log ("BGR");
		}else if( processImage.pixelFormat == LadybugPixelFormat.LADYBUG_BGRU)
		{
			Debug.Log ("BGRU");
		}else{
			Debug.Log ("Other");
		}
		
		int length = (int)(processImage.uiRows * processImage.uiCols * 3);
		byte[] processImagearray = new byte[length];
		
		Marshal.Copy(new IntPtr(processImage.pData), processImagearray, 0, length);

		//Marshal.Copy(new IntPtr(&processImage.pData[0]), processImagearray, 0, length); 
		
		
		tex.LoadImage(processImagearray);
		
		//tex.LoadImage(LoadBytes("Assets/Ladybug/PGR/a3820d5e.jpg"));
		GetComponent<Renderer>().material.mainTexture = tex;

		
		
	}
	
	
	public void handleError(LadybugError errorCode)
	{
        if (errorCode != LadybugError.LADYBUG_OK)
        {
            //Console.Out.WriteLine(Ladybug.ErrorToString(errorCode));
            UnityEngine.Debug.Log(Ladybug.ErrorToString(errorCode));
        }

    }

    public void handleError(CameraGUIError errorCode)
    {
        if (errorCode != CameraGUIError.PGRCAMGUI_OK)
            UnityEngine.Debug.Log("CameraGUIError error code = " + errorCode.ToString());

    }	
	
	unsafe void BytePtrToArray(byte* bptr, ref byte[] barray ) {
		for ( int i = 0; i < barray.Length; i++ ) {
			barray[i] = *bptr;
			bptr += Marshal.SizeOf( typeof( byte ) ) ;
		}
	}
	
	
	unsafe void Copy(byte* bptr, ref byte[] barray)
	{
		int len = barray.Length;
		
		fixed (byte* pDst = barray)
		{
			byte* ps = bptr;
			byte* pd = pDst;
			for(int i=1; i< len/4; i++)
			{
				*((int*)pd) = *((int*)ps);
                pd += 4;
                ps += 4;
				
			}
			for (int i = 0; i < len % 4 ; i++)
            {
                *pd = *ps;
                pd++;
                ps++;
            }
		}
    }
	
	
	byte[] LoadBytes(string path) {
		FileStream fs = new FileStream(path, FileMode.Open);
		BinaryReader bin = new BinaryReader(fs);
		byte[] result = bin.ReadBytes((int)bin.BaseStream.Length);
		bin.Close();
		return result;
	}
	
	
	private void CreateTextureAndPassToPlugin()
	{
		// Create a texture
		texture = new Texture2D(256,256,TextureFormat.ARGB32,false);
		
	  	//texture = new Texture2D (textureWidth, textureHeight, TextureFormat.ARGB32,false);
		// Set point filtering just so we can see the pixels clearly
		// texture.filterMode = FilterMode.Point;
		texture.Apply();
		GetComponent<Renderer>().material.mainTexture = texture;

		// Pass texture pointer to the plugin
		//MyLadybug.SetTextureFromUnity (texture.GetNativeTexturePtr());
	}
	
	
	private IEnumerator CallPluginAtEndOfFrames ()
	{
		while (true) {
			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame();
			
			// Set time for the plugin
			//MyLadybug.SetTimeFromUnity (Time.timeSinceLevelLoad);
			//MyLadybug.SetContextFromUnity (ladybugContext);
			
			// Issue a plugin event with arbitrary integer identifier.
			// The plugin can distinguish between different
			// things it needs to do based on this ID.
			// For our simple plugin, it does not matter which ID we pass here.
			UnityEngine.GL.IssuePluginEvent(1);
		}
	}
	
	
	void ThreadLadyBug(){
		while(keepstreaming){
			Thread.Sleep(0);
			lock(sync){
				processAndDisplay();
			}
		}
	}
	
	
}
