using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwaysFacingAtPosition : MonoBehaviour {
    public Camera cam;
    public Transform TargetPos;
    public float distance = 0.5f;

    public bool isFixedPosition = false;
    public Vector3 initialHandPos = new Vector3();

    public enum FrameType
    {
        OVER, NEAR
    }

    public FrameType frameLocation = FrameType.OVER;

    // Use this for initialization
    void Start () {
		
	}

    public void SetInitialHandPosition()
    {
        Vector3 handPos = TargetPos.position;
        handPos.x += distance;
        initialHandPos = handPos;
        transform.localPosition = handPos; 

    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform);

        if (!TargetPos.gameObject.activeInHierarchy)
        {
            GameObject palm = GameObject.Find("RigidRoundHand_L(Clone)/palm");
            if (palm != null)
            {
                TargetPos = palm.GetComponent<Transform>();

                Debug.Log("alternative hand found");
            }
            else
            {
                GameObject palm2 = GameObject.Find("RigidRoundHand_L/palm");
                if (palm2 != null)
                {
                    TargetPos = palm2.GetComponent<Transform>();
                    Debug.Log("alternative hand found2");
                }
            }
        }

        if (frameLocation == FrameType.OVER)
        {
            transform.localPosition = TargetPos.position;
        }
        else
        {
            if (isFixedPosition)
            {
                Vector3 pos = TargetPos.position;
                pos.x += distance;
                transform.localPosition = pos;
            }
            else
            {
               // transform.localPosition = initialHandPos;
            }
        }
    }
}
