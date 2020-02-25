using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FileManagingSpace;

public class DomeVideoTest : MonoBehaviour {
	

	public MovieTexture cur_video;

	public float fps;
	float fps_updateInterval = 1.0f; 
	float fps_accumulated   = 0.0f;
	float fps_timeUntilNextInterval; 
	int   fps_numFrames = 0;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material.mainTexture = cur_video;
		cur_video.loop = true;
		cur_video.Play();
	}
	
	// Update is called once per frame
	void Update () {
		ShowFPS();
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
}
