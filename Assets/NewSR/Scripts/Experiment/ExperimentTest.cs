using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentTest : MonoBehaviour {

    public string config_file = "configSR.ini";

	INIFile iniFileCtl;
    EyeHeadRecorder recorder;

    public int SubId;
    public string result_dir = "ResultsFOVE/";

    void Awake()
    {
        recorder = GameObject.Find("Fove Interface").GetComponent<EyeHeadRecorder>();

        LoadConfigFile();
    }

    void LoadConfigFile()
    {
        string initfile = Application.dataPath + "/../" + config_file;
        Debug.Log(initfile);
        iniFileCtl = new INIFile(initfile);

        try
        {
            SubId = int.Parse(iniFileCtl["Experiment", "SubId"]);
        }
        catch
        {
            Debug.Log("SubId is not numerical in the Init file ");
        }

        try
        {
            result_dir = iniFileCtl["Experiment", "result_dir"].Trim();
        }
        catch
        {
            Debug.Log("result_dir is not string in the Init file ");
        }
    }

	// Use this for initialization
	void Start () {

	}


    IEnumerator StartRecording()
    {
        string dir = result_dir + "/" + SubId + "/";
        recorder.SetFilename(dir, "test.txt");
        recorder.SaveHeader();
        recorder.StartRecording();

        yield return new WaitForSeconds(5.0f);

        recorder.StopRecording();
        recorder.SaveData();
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine("StartRecording");
        }

	}
}
