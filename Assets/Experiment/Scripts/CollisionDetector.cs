using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour {


    StimulusTest stimulator;

    Renderer visualStimulusRenderer;

    public bool IsActive = false;
    // Use this for initialization

    int counter = 0;

    public float timeout = 0.25f;

    private void Awake()
    {
        
        visualStimulusRenderer = GameObject.Find("MyFingerStimulusSkin").GetComponent<Renderer>();

        stimulator = GameObject.Find("TestStimulus").GetComponent<StimulusTest>();


    }

    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision other)
    {
        if (IsActive)
        {
            Debug.Log("visual stimulus was sent : count " + (++counter));
            
            stimulator.SendParallelSignal();
            stimulator.count_stimuli_catch++;

            Debug.Log("Hand Collision with Collider:" + stimulator.count_stimuli_catch + " time:" + stimulator.sw.ElapsedMilliseconds);
            IsActive = false;

            StartCoroutine(EraseStimulus());
        }
    }

    IEnumerator EraseStimulus()
    {
        yield return new WaitForSeconds(timeout);
        if (!IsActive)
        {
            visualStimulusRenderer.enabled = false;
        }
    }

}
