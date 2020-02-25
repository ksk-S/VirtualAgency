    using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using System.Text;

namespace FileManagingSpace
{

	public class WelcomeSRDemo : MonoBehaviour {

		SRController SRCtl;

		public bool isDemo = false;
		public bool isRunning = false;

        public bool isStartingFromLive = false;

		public string JsonDirectory = "/JSON/Scenes/";
		public string JsonFilename = "Welcome1.json";

        [System.Serializable]
        public class Live
        {
            public int audioId;
            public float duration;
        }

        [System.Serializable]
        public class Scene
		{
			public int videoId;
			public float duration;
		}

        public List<Live> lives = new List<Live>();
		public List<Scene> scenes = new List<Scene>();

        INIFile iniFileCtl;
        string config_file = "config.ini";

        void Awake(){
			
			SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();

		}
	// Use this for initialization
		void Start () {

            string initfile = Application.dataPath + "/../" + config_file;
            iniFileCtl = new INIFile(initfile);

            LoadStringFromFile(ref JsonFilename, "SR", "ScenarioFileName");

            Debug.Log ("===DEMO JSON FILE READ START===");
			string filename = Application.dataPath + "/../" + JsonDirectory + JsonFilename;
			Debug.Log (filename);
		
			FileInfo fi = new FileInfo(filename);
			string jsonText = "";
			try {
				using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.ASCII)){
					jsonText = sr.ReadToEnd();
				}
			} catch (Exception e){
				Debug.Log ("error in reading file : " + e.Message);
			}
			//Debug.Log (jsonText);
			Dictionary<string, object> json = Json.Deserialize (jsonText) as Dictionary<string, object>;
			
			foreach( KeyValuePair <string, object> entry in json )
            {
                if (entry.Key == "lives")
                {
                    foreach (object entry2 in (List<object>)entry.Value)
                    {
                        Dictionary<string, object> elem = (Dictionary<string, object>)entry2;
                        Live live = new Live();
                        live.audioId = Convert.ToInt32(elem["audioId"]);
                        live.duration = Convert.ToSingle(elem["duration"]);

                        lives.Add(live);
                    }
                }

                if (entry.Key == "scenes")
				{
					foreach( object entry2 in (List<object>)entry.Value )
					{
						Dictionary<string, object> elem = (Dictionary<string, object>)entry2;
						Scene scene  = new Scene();
						scene.videoId  = Convert.ToInt32(elem["videoId"]);
						scene.duration = Convert.ToSingle(elem["duration"]);

						scenes.Add (scene);
					}
				}
			}
		
		
			Debug.Log ("===JSON FILE READ END===");
 		
		}

		public IEnumerator  StartDemo(){
			Debug.Log ("Demo Start2");

            //yield break;
            while (isRunning)
            {
                if (isStartingFromLive) { 
                    SRCtl.GoToLiveAudio(lives[0].audioId);
                    for (int i = 0; i < lives.Count; i++)
                    {
                        float start_time = UnityEngine.Time.timeSinceLevelLoad;

                        Live live = lives[i];
                        SRCtl.GoToLiveAudio(live.audioId);
                        while ( (UnityEngine.Time.timeSinceLevelLoad - start_time) < live.duration)
                        {
                           // Debug.Log((UnityEngine.Time.timeSinceLevelLoad - start_time));
                            yield return null;
                        }
                    }

         
                }
                SRCtl.GoToStream(scenes[0].videoId);
                SRCtl.curVideoCtl.Reset();
                StartCoroutine(SRCtl.ToRecord());
                SRCtl.curLiveId = 0;

                for (int i=0; i< scenes.Count; i++)
				{
					Scene scene = scenes[i];
					Debug.Log ("Video: " + scene.videoId + " Time:" + scene.duration);
					if(i != 0){
                        //SRCtl.GoToStream(scene.videoId);
                        SRCtl.MergeStreamWithId(scene.videoId);

                    }
					Debug.Log (SRCtl.curVideoCtl.GetElapsedTime() );
                    float s_time = UnityEngine.Time.timeSinceLevelLoad;
                    while ((UnityEngine.Time.timeSinceLevelLoad - s_time) < scene.duration)
                    {
//                        while ( SRCtl.curVideoCtl.GetElapsedTime() < scene.duration){
						yield return null;
					}
				}

                if (isStartingFromLive)
                {
                    StartCoroutine(SRCtl.ToLive());
                }
                else
                {
                    SRCtl.MergeStreamWithId(0);
                }
				isRunning = false;
			}

		}

		// Update is called once per frame
		void Update () {

			if( Input.GetKeyDown(KeyCode.Return)){

				if(isDemo && isRunning){
					isRunning = false;
					StartCoroutine(SRCtl.ToLive()); 
				}else if(isDemo && !isRunning){
					isRunning = true;
					StartCoroutine("StartDemo");
				}
			}


		/*
		if ( (Input.GetKey (KeyCode.L) || Input.GetKey(KeyCode.JoystickButton8))&& !isDemo  ) {
			StopCoroutine("DemoSession");
			StopCoroutine("LiveToRecord");
			StopCoroutine("RecordToLiveAfterFinish");
			StopCoroutine("SwitchToRecord");

			Debug.Log ("Demo Finish");
			isDemo = false;
		}

		if ( (Input.GetKey (KeyCode.O) || Input.GetKey (KeyCode.JoystickButton9))&& !isDemo  ) {
			StopCoroutine("DemoSession");
			StopCoroutine("LiveToRecord");
			StopCoroutine("RecordToLiveAfterFinish");
			StopCoroutine("SwitchToRecord");
			isDemo = true;

			Debug.Log ("Demo Start");
			StartCoroutine("DemoSession");
		}
		*/




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
    }


}
