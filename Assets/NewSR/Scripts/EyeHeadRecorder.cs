using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtil;
using System;
using System.IO;

public class EyeHeadRecorder : MonoBehaviour {

    public FoveInterfaceBase foveInterface;
    public bool isRecording = false;

    public string out_filename;

    [System.Serializable]
    public struct EyeTrackData
    {
        public float time;
        public int frame;
        public Vector3 EyePosition;
        public Vector3 HeadPosition;
        public Vector3 HeadRotation;
        public Vector3 ScreenPosition;
        public Fove.Managed.EFVR_Eye EyeClosed;
    }
    public List<EyeTrackData> eye_track_record_list; 



	// Use this for initialization
	void Start () {
		
	}


    public void SetFilename(string dir, string filename)
    {

        Directory.CreateDirectory(dir);

        DateTime dt = DateTime.Now;
        string fn = string.Format(filename + "-{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        out_filename = dir + "/" + fn;
    }

    public void StartRecording()
    {
        isRecording = true;
    }
    public void StopRecording()
    {
        isRecording = false;
    }

    public void Reset()
    {
        eye_track_record_list.Clear();
    }

    public void SaveHeader()
    {

        string text = "#EyePosX, EyePosY, EyePosZ, HeadRotX, HeadRotY, HeadRotZ, HeadPosX, HeadPosY, HeadPosZ, ScreenX, ScreenY, ScreenZ, EyeClosed";
        text += System.Environment.NewLine;
        File.AppendAllText(out_filename, text);

    }

    public void SaveData()
    {
        string text = "";

        for(int i=0; i < eye_track_record_list.Count; i++ )
        {
            EyeTrackData eyedata = eye_track_record_list[i];
            text += eyedata.EyePosition.x + ", " + eyedata.EyePosition.y + ", " + eyedata.EyePosition.z+ ", ";
            text += eyedata.HeadRotation.x + ", " + eyedata.HeadRotation.y + ", " + eyedata.HeadRotation.z + ", ";
            text += eyedata.HeadPosition.x + ", " + eyedata.HeadPosition.y + ", " + eyedata.HeadPosition.z + ", ";
            text += eyedata.ScreenPosition.x + ", " + eyedata.ScreenPosition.y + ", " + eyedata.ScreenPosition.z + ", ";
            text += eyedata.EyeClosed;
            text += System.Environment.NewLine;
        }
        File.AppendAllText(out_filename, text);
    }
	
	// Update is called once per frame
	void Update () {
        if (isRecording) { 

            Vector3 screenPos;

            FoveInterfaceBase.EyeRays rays = foveInterface.GetGazeRays();
            RaycastHit hit;
            Physics.Raycast(rays.left, out hit, Mathf.Infinity);
            if (hit.point != Vector3.zero) // Vector3 is non-nullable; comparing to null is always false
            {
                screenPos = hit.point;
            }
            else
            {
                screenPos = rays.left.GetPoint(3.0f);
            }

            EyeTrackData eyedata = new EyeTrackData();

            eyedata.EyePosition = foveInterface.GetLeftEyeVector();
            eyedata.HeadRotation = foveInterface.GetHMDRotation().eulerAngles;
            eyedata.HeadPosition = foveInterface.GetHMDPosition();
            eyedata.EyeClosed = foveInterface.CheckEyesClosed();
            eyedata.ScreenPosition = screenPos;

            eyedata.time = Time.realtimeSinceStartup;
            eye_track_record_list.Add(eyedata);

            //Fove.Managed.SFVR_GazeConvergenceData con;
            //FoveInterfaceBase.GetFVRHeadset().GetGazeConvergence(out con);
            //Debug.Log(con.distance);
        }



	}
}
