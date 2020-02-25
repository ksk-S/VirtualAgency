using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAvatarRotation : MonoBehaviour {

    Vector3 init_pos;

	// Use this for initialization
	void Start () {
        init_pos = transform.localPosition;

        StartCoroutine("RecoverPositions");
    }

    IEnumerator RecoverPositions()
    {
        while (true)
        {
            transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            transform.localPosition = init_pos;

            yield return null;
        }
    }
}
