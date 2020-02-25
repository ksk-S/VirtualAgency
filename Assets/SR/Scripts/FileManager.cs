using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using System.Text;

namespace FileManagingSpace
{


    [System.Serializable]
    public class LiveData
    {
        public string live_audio;
        public bool is_multi;
        public bool is_ogg;

        public LiveData(string la_name, bool multi, bool ogg)
        {
            live_audio = la_name;
            is_multi = multi;
            is_ogg = ogg;
        }

        public LiveData()
        {
        }
    }


    [System.Serializable]
    public class StreamData
    {
        public string video_name;
        public string stream_name;
        public int start_frame;
        public int end_frame;
        public bool is_external_audio;
        public bool is_embeded_audio;

        public bool with_audio_time; //video is time-locked to the audio time

        public bool is_reversed;
        public float play_speed;

        public string stream_audio;
        public float audio_latency;

        public bool is_multi;
        public bool is_ogg;
        public bool is_image;

        public StreamData(string dome_name, string v_name, int start, int end, bool audio, bool embeded, bool audio_time, bool reverse, float speed, string sa_name, float latency, bool multi, bool ogg, bool im)
        {
            video_name = dome_name;
            stream_name = v_name;
            start_frame = start;
            end_frame = end;

            is_external_audio = audio;
            is_embeded_audio = embeded;

            with_audio_time = audio_time;
            is_reversed = reverse;
            play_speed = speed;

            stream_audio = sa_name;

            audio_latency = latency;
            is_multi = multi;
            is_ogg = ogg;
            is_image = im;

        }
        public StreamData()
        {
        }
    }

    /*
    public enum VideoSet{
        None, All, TimeLapse, TimeLapse2, TimeLapse3, TimeLapse4, Change, Demo, Minimal, Science, AVPro, EncodeTest, DeepDream
    }
*/

    public class FileManager : MonoBehaviour
    {

        INIFile iniFileCtl;
        public string config_file = "config.ini";

        public string JsonDirectory = "/JSON/Files/";
        public string JsonFilename = "deepdream.json";



        public bool useDomeVideo = false;

        //public VideoSet videoSets = VideoSet.All;

        public List<StreamData> streams = new List<StreamData>();
        //		public LiveData live_audio;
        public List<LiveData> live_audios = new List<LiveData>();

        void Awake()
        {
            /*
            FileStorage storage = gameObject.AddComponent<FileStorageAll>();

            switch(videoSets){
            case VideoSet.All: storage = gameObject.AddComponent<FileStorageAll>(); break;
            case VideoSet.TimeLapse: storage = gameObject.AddComponent<FileStorageTimeLapse>(); break;
            case VideoSet.TimeLapse2: storage = gameObject.AddComponent<FileStorageTimeLapse2>(); break;
            case VideoSet.TimeLapse3: storage = gameObject.AddComponent<FileStorageTimeLapse3>(); break;
            case VideoSet.TimeLapse4: storage = gameObject.AddComponent<FileStorageTimeLapse4>(); break;
            case VideoSet.Change: storage = gameObject.AddComponent<FileStorageCB>(); break;
            case VideoSet.Minimal: storage = gameObject.AddComponent<FileStorageMinimal>(); break;
            case VideoSet.Demo: storage = gameObject.AddComponent<FileStorageDemo>(); break;
            case VideoSet.Science: storage = gameObject.AddComponent<FileStorageScienceMuseum>(); break;
            case VideoSet.AVPro: storage = gameObject.AddComponent<FileStorageAVProTest>(); break;
            case VideoSet.EncodeTest: storage = gameObject.AddComponent<FileStorageVideoEncodingTest>(); break;
            case VideoSet.DeepDream: storage = gameObject.AddComponent<FileStorageDeepDream>(); break;
            default : break;
            }
            streams = storage.streams;
            live_audio = storage.live_audio;
            */
            string initfile = Application.dataPath + "/../" + config_file;
            iniFileCtl = new INIFile(initfile);

            LoadStringFromFile(ref JsonFilename, "SR", "VideoFileName");


            Debug.Log("===JSON FILE READ START===");
            string filename = Application.dataPath + "/../" + JsonDirectory + JsonFilename;
            Debug.Log(filename);

            FileInfo fi = new FileInfo(filename);
            string jsonText = "";
            try
            {
                using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.ASCII))
                {
                    jsonText = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.Log("error in reading file : " + e.Message);
            }



            Dictionary<string, object> json = Json.Deserialize(jsonText) as Dictionary<string, object>;

            foreach (KeyValuePair<string, object> entry in json)
            {
                //Debug.Log (entry.Key + " : " + entry.Value);
                if (entry.Key == "live")
                {
                    foreach (object entry2 in (List<object>)entry.Value)
                    {

                        Dictionary<string, object> elem = (Dictionary<string, object>)entry2;

                        LiveData live_audio = new LiveData();
                        live_audio.live_audio = (string)elem["filename"];
                        live_audio.is_multi = (bool)elem["is_multi"];
                        live_audio.is_ogg = (bool)elem["is_ogg"];
                        live_audios.Add(live_audio);
                    }
                }
                if (entry.Key == "files")
                {
                    foreach (object entry2 in (List<object>)entry.Value)
                    {

                        Dictionary<string, object> elem = (Dictionary<string, object>)entry2;

                        Debug.Log(elem["video_name"]);
                        StreamData stream = new StreamData();
                        stream.video_name = (string)elem["video_name"];
                        stream.stream_name = (string)elem["stream_name"];
                        stream.start_frame = Convert.ToInt32(elem["start_frame"]);
                        stream.end_frame = Convert.ToInt32(elem["end_frame"]);
                        stream.is_external_audio = (bool)elem["is_external_audio"];
                        stream.is_embeded_audio = (bool)elem["is_embeded_audio"];
                        stream.with_audio_time = (bool)elem["with_audio_time"];
                        stream.is_reversed = (bool)elem["is_reversed"];
                        stream.play_speed = Convert.ToSingle(elem["play_speed"]);
                        stream.stream_audio = (string)elem["stream_audio"];
                        stream.audio_latency = Convert.ToSingle(elem["audio_latency"]);
                        stream.is_multi = (bool)elem["is_multi"];
                        stream.is_ogg = (bool)elem["is_ogg"];
                        stream.is_image = (bool)elem["is_image"];
                        streams.Add(stream);
                    }
                }
                /*
                if(a.Key == "file")
                {
                    foreach( KeyValuePair <string, object> b in (Dictionary<string, object>)a.Value )
                    {
                        Debug.Log (" " + b.Key + " : " + b.Value);
                    }
                }
                */
            }


            Debug.Log("===JSON FILE READ END===");


            Debug.Log("The number of streams : " + streams.Count);
            FileCheck();
            Debug.Log("The number of existing streams : " + streams.Count);

        }


        void LoadStringFromFile(ref string var, string group, string var_name)
        {
            try
            {
                var = iniFileCtl[group, var_name].Trim();
            }
            catch
            {
                Debug.Log(var_name + " is not boolean in the Init file ");
            }

        }

        void FileCheck()
        {

            for (int i = live_audios.Count - 1; i >= 0; i--)
            {
                string suffix;
                if (live_audios[i].is_ogg)
                {
                    suffix = ".ogg";
                }
                else
                {
                    suffix = ".wav";
                }

                if (live_audios[i].is_multi)
                {
                    for (int j = 1; j <= 8; j++)
                    {
                        string m_live_audio_name = live_audios[i].live_audio + "-" + j + suffix;

                        if (!File.Exists(m_live_audio_name))
                        {
                            Debug.Log("Audio (Live) File Does Not Exist: " + m_live_audio_name);
                        }
                    }
                }
                else
                {
                    if (!File.Exists(live_audios[i].live_audio))
                    {
                        Debug.Log("Audio (Live) File Does Not Exist: " + live_audios[i].live_audio);
                    }
                }
            }

            for (int i = streams.Count - 1; i >= 0; i--)
            {

                bool absent = false;
                string video_name = streams[i].video_name;
                string stream_name = streams[i].stream_name;
                string stream_audio_name = streams[i].stream_audio;

                if (useDomeVideo)
                {
                    if (!File.Exists(video_name))
                    {
                        Debug.Log("Video File Does Not Exist: [" + i + "] " + video_name);
                        absent = true;
                    }
                }
                else
                {
                    if (!File.Exists(stream_name))
                    {
                        Debug.Log("Stream File Does Not Exist: [" + i + "] " + stream_name);
                        absent = true;
                    }
                }

                if (streams[i].is_external_audio)
                {

                    bool is_multi = streams[i].is_multi;
                    bool is_ogg = streams[i].is_ogg;

                    string suffix;
                    if (is_ogg)
                    {
                        suffix = ".ogg";
                    }
                    else
                    {
                        suffix = ".wav";
                    }
                    if (absent)
                    {
                        Debug.Log("stream audio is not vailid!");
                    }
                    if (is_multi)
                    {
                        for (int j = 1; j <= 8; j++)
                        {
                            string m_stream_audio_name = stream_audio_name + "-" + j + suffix;


                            if (!File.Exists(m_stream_audio_name))
                            {
                                Debug.Log("Audio (Stream) File Does Not Exist: [" + i + "." + j + "] " + m_stream_audio_name);
                                absent = true;
                            }
                        }

                    }
                    else
                    {
                        if (!File.Exists(stream_audio_name))
                        {
                            Debug.Log("Audio (Stream) File Does Not Exist: [" + i + "] " + stream_audio_name);
                            absent = true;
                        }
                    }
                }


                if (absent)
                {
                    Debug.Log("Stream #" + i + "is Removed from the List");
                    streams.Remove(streams[i]);
                }
            }
        }

    }
}

