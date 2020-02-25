using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeepDreamSubRating : MonoBehaviour {

    SRController srCtl;
    public GameObject AVProObject;
    Text instruction;

    void Awake()
    {
        srCtl = GameObject.Find("SRController").GetComponent<SRController>();
        instruction = GameObject.Find("InstructionText2").GetComponent<Text>();
    }

    // Use this for initialization
    IEnumerator Start () {

        while (!srCtl.isReady)
        {
            yield return null;
        }

        instruction.text =  Resources.Load<TextAsset>("Texts/DeepDreamRating/instruction").text; 
        srCtl.GoToInstructionScreen();

        DomeVideoAVPro videoCtl = srCtl.curVideoCtl as DomeVideoAVPro;
        videoCtl.curVideo.loopEvent += onFinish;

    }

    public void onFinish(object sender, System.EventArgs e)
    {
        Debug.Log ("AVPro video onFnish");

        //finishEvent.Invoke(this, System.EventArgs.Empty);
        StartCoroutine(srCtl.ToLive());

        instruction.text = Resources.Load<TextAsset>("Texts/DeepDreamRating/end").text;
        srCtl.GoToInstructionScreen();
    }

    void StartExperiment()
    {
        StartCoroutine(srCtl.ToRecord()); 
    }

    // Update is called once per frame
    void Update () {

        if (Input.anyKeyDown)
        {
            StartExperiment();

        }

	}
}
