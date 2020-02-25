using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAxis : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log(Input.GetKeyDown(KeyCode.Return) + " "+ Input.GetAxis("Mouse ScrollWheel V"));
    }
}
