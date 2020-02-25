using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using uEye;
using CommonUtil;
using RenderingAPI;
using System;

public class uEyeCamera : MonoBehaviour {
	
	public GameObject AudioObject;
	public AudioPlay audioCtl;
	
	//uEye
	uEye.Camera m_Camera = new uEye.Camera();
	uEye.Defines.Status statusRet;

	//public string parameter_file = "Assets\\Resources\\uEyeParameter\\hego";
	
	//** could not read from file, only able to load from camera inside with() ** 
		public string parameter_file = "Assets\\Resources\\uEyeParameter\\uEyeCameraParamBinning.ini";
		
	int MemId;
	int cameraWidth;
	int cameraHeight;
	int bpp;
	int pitch;

	//static int textureWidth = 1280;
	//static int textureHeight = 1024;		
//	public static int textureWidth= 1024;
//	public static int textureHeight  = 720;	

	public static int textureWidth = 800;
	public static int textureHeight = 800;		
	
//	int textureWidth = 640;
//	int textureHeight = 512;
	
	public bool is_delayed = false;
	public int delayed_frame = 10;
	private Queue<byte[]> delayedImages = new Queue<byte[]>();


	public double expsoure = 13.6f;
	const double EXPOSURE_MAX = 24f;
	const double EXPOSURE_MIN = 0.01f;
	
	public int gain = 0;
	const int GAIN_MAX = 100;
	const int GAIN_MIN = 0;
	
	public int saturation = 100;
	const int SATURATION_MAX = 200;
	const int SATURATION_MIN = 0;
	
	public int gamma = 100;
	const int GAMMA_MAX = 1000;
	const int GAMMA_MIN = 1;	
	
	public uint temperature = 4300;
	const uint TEMP_MAX = 10000;
	const uint TEMP_MIN = 2450;
	
	public int width = textureWidth;
	const int WIDTH_MAX = 1280;
	const int WIDTH_MIN = 2;
	
	public int height = textureHeight;
	const int HEIGHT_MAX = 1024;
	const int HEIGHT_MIN = 2;

	
	//texture
	public Texture2D texture;
	private Color32[] color32;
	byte[] image;


	//threading
	public System.Object sync = new System.Object();
	Thread thread;
	
	bool foreground = false;
	
	//alternative images
	bool is_alternative_image = false;
	public TextAsset alt_image;
	// string binary_image_file = "Images/altImagePNG.bytes";
	
	//fps
	public float fps;
	float fps_updateInterval = 1.0f; 
	float fps_accumulated   = 0.0f;
	float fps_timeUntilNextInterval; 
	int fps_numFrames = 0;
	
	void Awake(){
		AudioObject = GameObject.Find ("AudioObject") as GameObject;
		audioCtl = AudioObject.GetComponent("AudioPlay") as AudioPlay;	
	}
	
	
	void Start () {	
		
		if(InitCamera())
		{
			Debug.Log ("uEye camera Ready");
			InitTexture(); 
			
		}else{
			Debug.Log ("uEye camera not found");
			is_alternative_image = true;
			InitAlternativeImage();
		}
		
		fps_timeUntilNextInterval = fps_updateInterval;
		
		//Render.SetTexturePtrFromUnity (texture.GetNativeTexturePtr(), textureWidth, textureHeight);
		
		///thread = new Thread(ThreadEyeCamera);
		//thread.Start();
		//yield return StartCoroutine("CallPluginAtEndOfFrames");
	}
	
	public void SetForeGround()
	{
		PluginRender.SetLiveRenderStatus (true);
		foreground = true;
	}
	
	public void SetBackGround()
	{
		PluginRender.SetLiveRenderStatus (false);
		foreground = false;
	}
	
	
	
	bool InitCamera()
	{
		
		statusRet = m_Camera.Init ();
		if (statusRet != uEye.Defines.Status.SUCCESS)
        {
           Debug.Log("uEye: Camera initializing failed");
			return false;
        }
	
		statusRet = m_Camera.Parameter.Load();
		//statusRet = m_Camera.Parameter.Load(parameter_file);
		if (statusRet != uEye.Defines.Status.SUCCESS)
        {
           Debug.Log("uEye: Load parameter failed");
			return false;
        }
		
		m_Camera.Memory.Allocate(out MemId, true);
		if (statusRet != uEye.Defines.Status.SUCCESS)
        {
           Debug.Log("uEye: Allocate Memory failed");
			return false;
        }
		
		if (statusRet == uEye.Defines.Status.SUCCESS)
        {
			m_Camera.Acquisition.Capture(uEye.Defines.DeviceParameter.DontWait);
			m_Camera.Memory.Inquire (MemId, out cameraWidth, out cameraHeight, out bpp, out pitch);
		
			Debug.Log (MemId + " " + cameraWidth + " " + cameraHeight + " " + bpp + " " + pitch);
			
			textureWidth = cameraWidth;
			textureHeight  = cameraHeight;
		}else{
			return false;
			/*
			m_Camera.Acquisition.Capture(uEye.Defines.DeviceParameter.DontWait);
			m_Camera.Memory.Inquire (MemId, out cameraWidth, out cameraHeight, out bpp, out pitch);
		
			textureWidth = 512;
			textureHeight  = 512;
			*/
		}
		
		int posX=0, posY=0, w=0, h=0;
		m_Camera.Size.AOI.Get(out posX, out posY, out w, out h);
		Debug.Log ("uEye pos " + posX + " " + posY + " " + w + " " + h);
		//m_Camera.Size.AOI.Set(1024, 768, 128, 152);
		
		return true;
	}
	
	
	void InitTexture()
	{
		texture = new Texture2D (textureWidth, textureHeight);
		texture.Apply();
		GetComponent<Renderer>().material.mainTexture = texture;
		
		PluginRender.SetTexturePtrFromUnity (texture.GetNativeTexturePtr(), textureWidth, textureHeight, (int)ScreenMode.Live);
		
		image = new byte[textureWidth * textureHeight * 4];
	}
	
	void InitAlternativeImage()
	{
		InitTexture();
		image = alt_image.bytes;
		//alt_image = Resources.Load (binary_image_file) as TextAsset;
		//	Debug.Log ("Alt : " + alt_image.bytes.Length);
		//	alt_image.bytes.CopyTo (image, alt_image.bytes.Length);
	}
	
	void OnApplicationQuit()
	{
		cleanup();
	}
	
	void cleanup()
	{
		m_Camera.Memory.Free(MemId);
		m_Camera.Acquisition.Stop();
		m_Camera.Exit();
	}
	
	
	void Update(){
		if(foreground)
		{
			UpdateImage();
		}
		ShowFPS();
		
	}
	
	unsafe void UpdateImage()
	{
		if(!is_alternative_image)
		{
			m_Camera.Memory.GetActive(out MemId);
			m_Camera.Memory.CopyToArray(MemId, out image);

			if(is_delayed)
			{
				byte[] copyImage = new byte[textureWidth * textureHeight * 4];
				Buffer.BlockCopy(image, 0, copyImage, 0, textureWidth * textureHeight * 4);
				delayedImages.Enqueue(copyImage);

				if(	delayedImages.Count > delayed_frame)
				{
					image = delayedImages.Dequeue();
				}
			}

			PluginRender.UpdateTextureLive ( image );
		}else{
			PluginRender.UpdateTextureLive ( image );
		}
	}

	
	public void IncExposure()
	{
		m_Camera.Timing.Exposure.Get(out expsoure);
		if(expsoure < EXPOSURE_MAX) expsoure += 0.1f;
		m_Camera.Timing.Exposure.Set(expsoure); 
		
		Debug.Log ("expsoure " + expsoure);
	}
	public void DecExposure()
	{
		m_Camera.Timing.Exposure.Get(out expsoure);
		if(expsoure > EXPOSURE_MIN) expsoure -= 0.1f;
		m_Camera.Timing.Exposure.Set(expsoure);
		
		Debug.Log ("expsoure " + expsoure);
	}
		
	public void IncGain()
	{
		m_Camera.Gain.Hardware.Scaled.GetMaster (out gain);
		if(gain < GAIN_MAX) gain++;
		m_Camera.Gain.Hardware.Scaled.SetMaster(gain);
		Debug.Log("gain " + gain);
	}
	
	public void DecGain()
	{
		m_Camera.Gain.Hardware.Scaled.GetMaster (out gain);
		if(gain > GAIN_MIN) gain--;
		m_Camera.Gain.Hardware.Scaled.SetMaster(gain);
		Debug.Log("gain " + gain);
	}
	
	public void IncSaturation()
	{
		int u,v;
		m_Camera.Saturation.Get(out u, out v);
		saturation = u;
		if(saturation < SATURATION_MAX) saturation++;
		m_Camera.Saturation.Set(saturation, saturation);
		Debug.Log ("saturation " + saturation);
	}
	public void DecSaturation()
	{
		int u,v;
		m_Camera.Saturation.Get(out u, out v);
		saturation = u;
		if(saturation > SATURATION_MIN) saturation--;
		m_Camera.Saturation.Set(saturation, saturation);
		Debug.Log ("saturation " + saturation);
	}
		
	public void IncGamma()
	{
		m_Camera.Gamma.Software.Get(out gamma); 
		if(gamma < GAMMA_MAX) gamma++;
		m_Camera.Gamma.Software.Set(gamma); 
		Debug.Log ("gamma " + gamma);
	}
	public void DecGamma()
	{
		m_Camera.Gamma.Software.Get(out gamma); 
		if(gamma > GAMMA_MIN) gamma--;
		m_Camera.Gamma.Software.Set(gamma);
		Debug.Log ("gamma " + gamma);
	}
	
	public void IncTemp()
	{
		m_Camera.Color.Temperature.Get(out temperature); 
		if(temperature < TEMP_MAX) temperature += 20;
		m_Camera.Color.Temperature.Set(temperature);		
		Debug.Log ("temperature " + temperature);
	}
	public void DecTemp()
	{
		
		m_Camera.Color.Temperature.Get(out temperature); 
		if(temperature > TEMP_MIN) temperature -= 20;
		m_Camera.Color.Temperature.Set(temperature); 
		Debug.Log ("temperature " + temperature);
	}
	
	public void DecAOI()
	{
		return;
        /*
		//it might be dangerous to change the size in real time
		
		int posX=0, posY=0;
		
		if(height > HEIGHT_MIN) height -= 2;
		width = (int)((float)height * 1.25f);
		
	//	image = new byte[width * height * 4];
		
		m_Camera.Size.AOI.Set(posX,  posY, width, height);
		Debug.Log ("AOI :" + posX + ","+  posY + ","+ width+ ","+ height);
        */
	}
	public void IncAOI()
	{
		return;
		/*
		int posX=0, posY=0;
		if(height < HEIGHT_MAX) height += 2;
		width = (int)((float)height * 1.25f);
		
	//	image = new byte[width * height * 4];
	
		
		m_Camera.Size.AOI.Set(posX,  posY, width, height);
		Debug.Log ("AOI " + posX + ","+  posY + ","+ width+ ","+ height);
	*/

    }
	
/*
uEye.SizeAOI.Set(int s32PosX, int s32PosY, int s32Width, int s32Height)
	 */



	
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
	
	void ThreadEyeCamera(){
		Thread.Sleep(0);
		while(true){
			lock(sync){
				UpdateImage();
				//ShowFPS();
			}
		}
	}
	
	
}