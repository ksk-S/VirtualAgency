﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwaysFacing : MonoBehaviour {
    public Camera cam;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(cam.transform);
	}
}
