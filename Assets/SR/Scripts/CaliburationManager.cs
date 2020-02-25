using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using CommonUtil;
using RenderingAPI;

public class CaliburationManager : MonoBehaviour {

	protected SRController SRCtl;

    protected GameObject WebCamScreen;
    protected GameObject BlackBackground;
    protected GameObject LadybugScreen;

    protected GameObject DomeContainer;
    protected GameObject AVProContainer;


    protected Camera AltCamera;

	[HideInInspector]
	public LadybugPluginInterface ladybugCtl {get { return SRCtl.ladybugCtl; }}
    
	[HideInInspector]
	public DomeVideoInterface curVideoCtl{get { return SRCtl.curVideoCtl; }}

	[HideInInspector]
	public FrontCameraInterface frontCamCtl {get { return SRCtl.frontCamCtl; }}

	public bool useDomeVideo { get { return SRCtl.useDomeVideo; } }
	public bool useDomeAVPro { get { return SRCtl.useDomeAVPro; } }
	public Status live_status { get { return SRCtl.live_status; } }


	public CalibrationMode caliburationMode = CalibrationMode.None;

	public float cariburation_duplication_ratio = 0.5f;
	public float cariburation_alpha_ratio = 0.5f;

	public float cariburation_framed_ratioX = 0.8f;
	public float cariburation_framed_ratioY = 0.8f;

    public HybridMode hybridMode = HybridMode.None;
    public float hybrid_horizontal_pos = 0.5f;
    public float hybrid_vertical_pos = 0.5f;


	public Shader alphaShader;
    public Shader verticalShader; 
	public Shader horizontalShader;
    public Shader frameShader;
    public Shader frameblackShader;

    public bool isExperiementMode = false;

	protected virtual void Awake ()
	{	
		SRCtl = gameObject.GetComponent<SRController> ();

		WebCamScreen = GameObject.Find ("FrontCamScreen") as GameObject;
		BlackBackground = GameObject.Find ("BlackBackground") as GameObject;
		LadybugScreen =GameObject.Find ("SphericalScreen") as GameObject;

		DomeContainer = GameObject.Find ("DomeContainer") as GameObject;

		AVProContainer = GameObject.Find ("AVProContainer") as GameObject;;

        AltCamera = GameObject.Find("AltCamera").GetComponent<Camera>();
        AltCamera.enabled = false;
    }

    protected virtual void Start()
    {
    }

	// Update is called once per frame
    protected void Update()
    {
		if (!isExperiementMode)
		{
			ManageKeyboard ();
		}
		// ManageHybridMode();
	}


    public virtual void SetHorizontalSplitScreenes()
	{
		WebCamScreen.GetComponent<Renderer>().material.shader = horizontalShader;
		BlackBackground.GetComponent<Renderer>().material.shader = horizontalShader;

        AltCamera.enabled = true;
        
        SetParamOnSplitScreen();
	}

    public virtual void SetVerticalSplitScreenes()
    {
		WebCamScreen.GetComponent<Renderer>().material.shader = verticalShader;
		BlackBackground.GetComponent<Renderer>().material.shader = verticalShader;

        AltCamera.enabled = true;

        SetParamOnSplitScreen();
    }

    public virtual void RevertSplitScreenes()
    {
        WebCamScreen.GetComponent<Renderer>().material.shader = alphaShader;
        BlackBackground.GetComponent<Renderer>().material.shader = alphaShader;

        BlackBackground.GetComponent<Renderer>().material.SetFloat("_Param", 1.0f);
    }



    protected void SetFramedScreenes()
	{

		WebCamScreen.GetComponent<Renderer>().material.shader = frameShader;
		BlackBackground.GetComponent<Renderer>().material.shader = frameShader;

        AltCamera.enabled = true;

        SetParamOnSplitScreen();
	}

    protected void SetFramedblackScreenes()
	{

        WebCamScreen.GetComponent<Renderer>().material.shader = frameblackShader;
        BlackBackground.GetComponent<Renderer>().material.shader = frameblackShader;

        AltCamera.enabled = true;

        SetParamOnSplitScreen();

   }


    protected void SetRenderingBothScreen()
   {

        //record start
        if (useDomeVideo){
           curVideoCtl.SetForeGround ();

           if(useDomeAVPro)
           {
               AVProContainer.SetActive (true);
           }else{
               DomeContainer.SetActive (true);
           }
       }else{
           ladybugCtl.SetForeGround();

           LadybugScreen.GetComponent<Renderer>().enabled = true;
       }

       //live
       frontCamCtl.SetForeGround();
       WebCamScreen.GetComponent<Renderer>().enabled = true;

        frontCamCtl.SetAlpha (1.0f);
		if(useDomeVideo){
			curVideoCtl.SetAlpha (1.0f );
		}
	}

    protected void SetRenderingSingleScreen()
	{

        //Rendering Single Screen
        if (live_status == Status.LIVE)
		{
			// stream stop
			if(useDomeVideo){
				if(useDomeAVPro)
				{
					AVProContainer.SetActive (false);
				}else{
					DomeContainer.SetActive (false);
				}
				curVideoCtl.SetAlpha (0.0f );
				curVideoCtl.SetBackGround();
			}else{
				LadybugScreen.GetComponent<Renderer>().enabled = false;
				ladybugCtl.SetBackGround();
			}

			frontCamCtl.SetAlpha (1.0f);

		}else if(live_status == Status.STREAM){


			WebCamScreen.GetComponent<Renderer>().enabled = false;
			frontCamCtl.SetAlpha (0.0f);
			frontCamCtl.SetBackGround();


			if(useDomeVideo){
				curVideoCtl.SetAlpha (1.0f );
			}

		}
	}

    protected void SetTransparentScreen()
	{
		frontCamCtl.SetAlpha (cariburation_alpha_ratio);
		
		if(useDomeVideo){
			curVideoCtl.SetAlpha (1.0f-cariburation_alpha_ratio );
		}else{
			ladybugCtl.SetAlpha (1.0f-cariburation_alpha_ratio );
		}
	}


    protected void SetParamOnSplitScreen()
    {

		if (caliburationMode == CalibrationMode.Frame || caliburationMode == CalibrationMode.FramedBlack) {

			frontCamCtl.SetXYLimit (cariburation_framed_ratioX, cariburation_framed_ratioY);
			BlackBackground.GetComponent<Renderer>().material.SetFloat ("_XRatio", cariburation_framed_ratioX);
			BlackBackground.GetComponent<Renderer>().material.SetFloat ("_YRatio", cariburation_framed_ratioY);

		} else {

			frontCamCtl.SetAlpha (cariburation_duplication_ratio);
			BlackBackground.GetComponent<Renderer>().material.SetFloat ("_Param", cariburation_duplication_ratio);

		}

	}

    protected void GotoCaliburationHorizontal()
	{
        caliburationMode = CalibrationMode.Horizontal;

		SetRenderingBothScreen();
		SetHorizontalSplitScreenes();

	}

    protected void GotoCaliburationVertical()
    {
        caliburationMode = CalibrationMode.Vertical;

		SetRenderingBothScreen();
		SetVerticalSplitScreenes();
        
    }

    protected void GotoCaliburationFrame()
	{
		caliburationMode = CalibrationMode.Frame;

		SetRenderingBothScreen();
		SetFramedScreenes();

	}

    protected void GotoCaliburationFrameBlack()
	{
		caliburationMode = CalibrationMode.FramedBlack;

		SetRenderingBothScreen();
		frontCamCtl.SetBackGround();

		SetFramedblackScreenes();
		
	}

    protected void GotoCaliburationOverlay()
	{
		caliburationMode = CalibrationMode.Overlay;
	
		RevertSplitScreenes();

		SetRenderingBothScreen();

        SetTransparentScreen ();

	}

    protected void GotoCaliburationNone()
	{
		caliburationMode = CalibrationMode.None;

        AltCamera.enabled = false;

        RevertSplitScreenes();

		SetRenderingSingleScreen();
	}

    //for Framed experiment
	public void SetFrameMode(float screenratioX, float screenratioY){

		cariburation_framed_ratioX = screenratioX;
		cariburation_framed_ratioY = screenratioY;

		SetParamOnSplitScreen();

		GotoCaliburationFrame ();
		isExperiementMode = true;
	}

    //for Framed experiment
    public void SetFrameBackMode(float screenratioX, float screenratioY){
        cariburation_framed_ratioX = screenratioX;
		cariburation_framed_ratioY = screenratioY;
		
		SetParamOnSplitScreen();

		GotoCaliburationFrameBlack ();
		isExperiementMode = true;
	}

    //for Framed experiment
    public void UnsetFrameMode(){
		GotoCaliburationNone ();
		isExperiementMode = false;
	}


    public virtual void ControlCaliburationUp()
	{
	
		if (caliburationMode == CalibrationMode.Horizontal || caliburationMode == CalibrationMode.Vertical )
        {
            cariburation_duplication_ratio += 0.01f;
            if (cariburation_duplication_ratio > 1.0f) cariburation_duplication_ratio = 1.0f;
			SetParamOnSplitScreen();
        }

		if (caliburationMode == CalibrationMode.Frame || caliburationMode == CalibrationMode.FramedBlack) {

			cariburation_framed_ratioX += 0.01f;
			cariburation_framed_ratioY += 0.01f;
			if (cariburation_framed_ratioX > 1.0f) cariburation_framed_ratioX = 1.0f;
			if (cariburation_framed_ratioY > 1.0f) cariburation_framed_ratioY = 1.0f;
			SetParamOnSplitScreen();
		}

		if(caliburationMode == CalibrationMode.Overlay )
		{
			cariburation_alpha_ratio += 0.01f;
			if(cariburation_alpha_ratio > 1.0f )cariburation_alpha_ratio = 1.0f;
			SetTransparentScreen();
		}
	}
    protected virtual void ControlCaliburationDown()
	{
		if (caliburationMode == CalibrationMode.Horizontal || caliburationMode == CalibrationMode.Vertical )
		{
			cariburation_duplication_ratio -= 0.01f;
			if(cariburation_duplication_ratio < 0.0f) cariburation_duplication_ratio = 0.0f;
			SetParamOnSplitScreen();
		}

		if (caliburationMode == CalibrationMode.Frame || caliburationMode == CalibrationMode.FramedBlack) {
			
			cariburation_framed_ratioX -= 0.01f;
			cariburation_framed_ratioY -= 0.01f;
			if (cariburation_framed_ratioX < 0.5f) cariburation_framed_ratioX = 0.5f;
			if (cariburation_framed_ratioY < 0.5f) cariburation_framed_ratioY = 0.5f;
			SetParamOnSplitScreen();
		}

		if(caliburationMode == CalibrationMode.Overlay )
		{
			cariburation_alpha_ratio -= 0.01f;
			if(cariburation_alpha_ratio < 0.0f )cariburation_alpha_ratio = 0.0f;
			SetTransparentScreen();
		}
	}


    protected void GamePadCaliburation()
	{
	
		//Cariburation mode
		float rawVertical2 = Input.GetAxisRaw("VerticalRStick");
		if (caliburationMode != CalibrationMode.None)
		{
			if (rawVertical2 > 0)
			{
				ControlCaliburationUp();
			}
			else if (rawVertical2 < 0)
			{
				ControlCaliburationDown();
			}	
		}else if(hybridMode == HybridMode.Horizontal)
		{
			hybrid_horizontal_pos += rawVertical2 * 0.01f;
			if(hybrid_horizontal_pos < 0.0f){
				hybrid_horizontal_pos = 1.0f;
			}
			if(hybrid_horizontal_pos > 1.0f){
				hybrid_horizontal_pos = 0.0f;
			}
		}else if (hybridMode == HybridMode.Vertical)
		{
			hybrid_vertical_pos += rawVertical2 * 0.01f;
			if(hybrid_vertical_pos < 0.0f){
				hybrid_vertical_pos = 1.0f;
			}
			if(hybrid_vertical_pos > 1.0f){
				hybrid_vertical_pos = 0.0f;
			}
		}

		
	}

    protected void SetCameraDepth(Camera cam, int depth)
	{
		cam.GetComponent<Camera>().depth = depth;
	}


    protected void SetCameraSolidColor(Camera cam)
	{
		cam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
		cam.GetComponent<Camera>().backgroundColor = Color.black;
	}

    protected void SetCameraDontClear(Camera cam)
	{
		cam.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
	}

    protected void SetCameraDepthOnly(Camera cam)
	{
		cam.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
	}


    protected void ManageKeyboard()
    {

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (caliburationMode == CalibrationMode.Overlay)
            {
                GotoCaliburationNone();
            }
            else
            {
                GotoCaliburationOverlay();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {

            if (caliburationMode == CalibrationMode.None)
            {
                GotoCaliburationHorizontal();
            }
            else if (caliburationMode == CalibrationMode.Horizontal)
            {
                GotoCaliburationVertical();
            }
            else if (caliburationMode == CalibrationMode.Vertical)
            {
                GotoCaliburationNone();
            }

            Debug.Log(caliburationMode);
        }

        if (Input.GetKeyDown(KeyCode.Z)){

            if (caliburationMode == CalibrationMode.None)
            {

                GotoCaliburationFrame();
            }
            else if (caliburationMode == CalibrationMode.Frame)
            {
                GotoCaliburationFrameBlack();
            }
            else if (caliburationMode == CalibrationMode.FramedBlack)
            {
                GotoCaliburationNone();
            }
        }


        if (caliburationMode != CalibrationMode.None)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                ControlCaliburationUp();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                ControlCaliburationDown();
            }
        }

        GamePadCaliburation();
    }


}
