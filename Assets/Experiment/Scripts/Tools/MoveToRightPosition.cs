using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToRightPosition : MonoBehaviour {

    public Vector3 position;

	// Use this for initialization
	void Start () {
        transform.localPosition = position;

    }
	
	// Update is called once per frame
	void Update () {
		
	}


}
