using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using uEye;
using RenderingAPI;
using System;
using System.Diagnostics; 


public class FrontCameraInterface : MonoBehaviour {
	
	//public GameObject AudioObject;
	public AudioManager audioCtl;
	public float alpha_param = 1.0f;
	//fps
	public float fps;
	protected float fps_updateInterval = 1.0f; 
	protected float fps_accumulated   = 0.0f;
	protected float fps_timeUntilNextInterval; 
	protected int fps_numFrames = 0;

    //fps alt
    protected Stopwatch sw;

    protected float sw_pretime = 0.0f;
    protected float sw_updateInterval = 0.5f;
    protected float sw_accumulated = 0.0f;
    protected float sw_timeUntilNextInterval; 
    protected int sw_numFrames = 0;

	
	public static bool flag2 = true;

    public virtual void Awake()
    {
		//audioCtl = GameObject.Find ("AudioObject").GetComponent<AudioManager>();
	
         sw = new Stopwatch();
         sw.Start(); 
         sw_pretime = sw.ElapsedMilliseconds;
    }


    public virtual void Start()
    {
    }


    public virtual void SetForeGround()
	{
	}

    public virtual void SetBackGround()
	{
	}
	
	
	public virtual void Update()
    {
		
	}
	public virtual void SetAlpha(float alpha)
	{
		alpha_param = alpha;
		GetComponent<Renderer>().material.SetFloat ("_Param", alpha_param);
       
	}


	public virtual void SetXYLimit(float x, float y)
	{
		GetComponent<Renderer>().material.SetFloat ("_XRatio", x);
		GetComponent<Renderer>().material.SetFloat ("_YRatio", y);

	}

    public virtual void IncExposure(){}
    public virtual void DecExposure(){}
    public virtual void IncGain(){}
    public virtual void DecGain() { }
    public virtual void IncSaturation() { }
    public virtual void DecSaturation() { }
    public virtual void IncGamma() { }
    public virtual void DecGamma() { }
    public virtual void IncTemp() { }
    public virtual void DecTemp() { }
    public virtual void DecAOI() { }
    public virtual void IncAOI() { }
  
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

    protected void ShowFPSAlt()
    {
        
        float delta = (sw.ElapsedMilliseconds - sw_pretime) / 1000;
        sw_timeUntilNextInterval -= delta;
        sw_accumulated += delta;
        ++sw_numFrames;

        if (sw_timeUntilNextInterval < 0.0)
        {
            fps =  sw_numFrames / sw_accumulated;
            sw_timeUntilNextInterval = sw_updateInterval;
            sw_accumulated = 0.0f;
            sw_numFrames = 0;
        }

        sw_pretime = sw.ElapsedMilliseconds;
    }
}