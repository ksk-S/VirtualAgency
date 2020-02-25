using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowOVRvisionStarter : MonoBehaviour {
    public GameObject OvrvisionProCamera;

    SRController srCtl;

    // Update is called once per frame
    IEnumerator Start()
    {
        srCtl = GetComponent<SRController>();

        while(!srCtl.isReady)
        {
            yield return null;
        }
        OvrvisionProCamera.SetActive(true);
    }
}
