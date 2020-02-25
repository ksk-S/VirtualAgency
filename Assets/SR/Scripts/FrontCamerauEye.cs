using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using uEye;
using CommonUtil;
using RenderingAPI;
using System;

public class FrontCamerauEye : FrontCameraInterface
{
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

    public bool IsEnabled = true;

	public static int textureWidth = 840;
	public static int textureHeight = 800;		

	public bool is_delayed = false;
	public int delayed_frame = 10;
	private Queue<byte[]> delayedImages = new Queue<byte[]>();

    //camera params
	public double exposure = 14f;
    public int gain = 0;
    public int saturation = 120;
    public int gamma = 112;
//    public uint temperature = 3280;
	public uint temperature = 4480;


    //max/min
	const double EXPOSURE_MAX = 72f;
	const double EXPOSURE_MIN = 0.01f;
	
	const int GAIN_MAX = 100;
	const int GAIN_MIN = 0;
	
	const int SATURATION_MAX = 200;
	const int SATURATION_MIN = 0;
	
	const int GAMMA_MAX = 1000;
	const int GAMMA_MIN = 1;	
	
	const uint TEMP_MAX = 10000;
	const uint TEMP_MIN = 450;
	
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
	public static System.Object sync = new System.Object();

	bool foreground = false;
	
	//alternative images
	bool is_alternative_image = false;
	// string binary_image_file = "Images/altImagePNG.bytes";

   // float accumulated_time = 0.0f;

	public bool isNOTusingNativePlugin = true;
	// bool flag1 = true;

	public override void Start () {	
		
		if(InitCamera() && IsEnabled)
		{
			Debug.Log ("uEye camera Ready");
			InitTexture(); 
			
		}else{
			Debug.Log ("uEye camera not found");
			is_alternative_image = true;

			GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>("Images/altImage");
		}
		
		fps_timeUntilNextInterval = fps_updateInterval;
	}
	
	public override void SetForeGround()
	{
        /*
		PluginRender.SetLiveRenderStatus (true);
        */
		foreground = true;


	}

    public override void SetBackGround()
	{
	//	PluginRender.SetLiveRenderStatus (false);
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

        SetInitParameters();

		m_Camera.PixelFormat.Set (uEye.Defines.ColorMode.RGBA8Packed);

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
		
			Debug.Log ("MemId:" + MemId + " W:" + cameraWidth + " H:" + cameraHeight + " bpp:" + bpp + " pitch:" + pitch);
			
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

        //event is registered
        m_Camera.EventFrame += onFrameEvent;

		return true;
	}


    void SetInitParameters()
    {
        m_Camera.Timing.Exposure.Set(exposure);

        m_Camera.Gain.Hardware.Scaled.SetMaster(gain);
		m_Camera.Saturation.Set(saturation, saturation);
        m_Camera.Color.Temperature.Set(temperature); 
		m_Camera.Gamma.Software.Set(gamma);
	
    }

	void InitTexture()
	{
//		texture = new Texture2D (textureWidth, textureHeight);
		texture = new Texture2D (textureWidth, textureHeight, TextureFormat.RGBA32, false);

		texture.Apply();
		GetComponent<Renderer>().material.mainTexture = texture;


		image = new byte[textureWidth * textureHeight * 4];
	}

	
	void OnApplicationQuit()
	{
		Debug.Log ("uEye onApplicationQuit");
		cleanup();
	}

	void OnDestroy()
	{
		Debug.Log ("uEye OnDestroy");
		cleanup();
	}

	void cleanup()
	{
		SetBackGround();
		if(m_Camera.IsOpened)
		{
			m_Camera.Memory.Free(MemId);
			m_Camera.Acquisition.Stop();
			m_Camera.Exit();
		}
	}



    private void onFrameEvent(object sender, EventArgs e)
    {
        if (foreground)
        {
			UpdateImage();
		}
    }


	public override void Update(){

		if (foreground && !is_alternative_image) {
			lock(sync){
				texture.LoadRawTextureData (image);
				texture.Apply();
			}
		}	

	}

	unsafe void UpdateImage()
	{
        lock(sync){
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

			}
        }
        ShowFPSAlt();
	}

	
	public override void IncExposure()
	{
		m_Camera.Timing.Exposure.Get(out exposure);
		if(exposure < EXPOSURE_MAX) exposure += 0.1f;
		m_Camera.Timing.Exposure.Set(exposure); 
		
		Debug.Log ("exposure " + exposure);
	}
    public override void DecExposure()
	{
		m_Camera.Timing.Exposure.Get(out exposure);
		if(exposure > EXPOSURE_MIN) exposure -= 0.1f;
		m_Camera.Timing.Exposure.Set(exposure);
		
		Debug.Log ("exposure " + exposure);
	}

    public override void IncGain()
	{
		m_Camera.Gain.Hardware.Scaled.GetMaster (out gain);
		if(gain < GAIN_MAX) gain++;
		m_Camera.Gain.Hardware.Scaled.SetMaster(gain);
		Debug.Log("gain " + gain);
	}

    public override void DecGain()
	{
		m_Camera.Gain.Hardware.Scaled.GetMaster (out gain);
		if(gain > GAIN_MIN) gain--;
		m_Camera.Gain.Hardware.Scaled.SetMaster(gain);
		Debug.Log("gain " + gain);
	}

    public override void IncSaturation()
	{
		int u,v;
		m_Camera.Saturation.Get(out u, out v);
		saturation = u;
		if(saturation < SATURATION_MAX) saturation++;
		m_Camera.Saturation.Set(saturation, saturation);
		Debug.Log ("saturation " + saturation);
	}
    public override void DecSaturation()
	{
		int u,v;
		m_Camera.Saturation.Get(out u, out v);
		saturation = u;
		if(saturation > SATURATION_MIN) saturation--;
		m_Camera.Saturation.Set(saturation, saturation);
		Debug.Log ("saturation " + saturation);
	}

    public override void IncGamma()
	{
		m_Camera.Gamma.Software.Get(out gamma); 
		if(gamma < GAMMA_MAX) gamma++;
		m_Camera.Gamma.Software.Set(gamma); 
		Debug.Log ("gamma " + gamma);
	}
    public override void DecGamma()
	{
		m_Camera.Gamma.Software.Get(out gamma); 
		if(gamma > GAMMA_MIN) gamma--;
		m_Camera.Gamma.Software.Set(gamma);
		Debug.Log ("gamma " + gamma);
	}

    public override void IncTemp()
	{
		m_Camera.Color.Temperature.Get(out temperature); 
		if(temperature < TEMP_MAX) temperature += 20;
		m_Camera.Color.Temperature.Set(temperature);		
		Debug.Log ("temperature " + temperature);
	}
    public override void DecTemp()
	{
		
		m_Camera.Color.Temperature.Get(out temperature); 
		if(temperature > TEMP_MIN) temperature -= 20;
		m_Camera.Color.Temperature.Set(temperature); 
		Debug.Log ("temperature " + temperature);
	}

    public override void DecAOI()
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
    public override void IncAOI()
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

	
	
}