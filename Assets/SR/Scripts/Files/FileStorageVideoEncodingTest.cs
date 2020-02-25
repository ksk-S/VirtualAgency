using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{
   
	public class FileStorageVideoEncodingTest : FileStorage {

		void Awake()
  		{
			//Debug.Log (Application.dataPath);

			live_audio = new LiveData (Application.dataPath + "/Resources/sounds/Multi/live/Audio Track", true, true);


			streams.Add(new StreamData("C:\\Streams\\TimePerception\\Test\\test_hap.mov",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "/Resources/sounds/Multi/live/Audio Track", 0f, true, true, false));


			streams.Add(new StreamData("C:\\Streams\\TimePerception\\Test\\test_hap_alpha.mov",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "/Resources/sounds/Multi/live/Audio Track", 0f, true, true, false));
			
			streams.Add(new StreamData("C:\\Streams\\TimePerception\\Test\\test_hap_q.mov",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "/Resources/sounds/Multi/live/Audio Track", 0f, true, true, false));


			
			streams.Add(new StreamData("C:\\Streams\\TimePerception\\Test\\test_mpeg4.mov",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "/Resources/sounds/Multi/live/Audio Track", 0f, true, true, false));

			streams.Add(new StreamData("C:\\Streams\\TimePerception\\Test\\test_h263.mov",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "/Resources/sounds/Multi/live/Audio Track", 0f, true, true, false));

			streams.Add(new StreamData("C:\\Streams\\TimePerception\\Test\\test_premier_mpeg4.mov",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "/Resources/sounds/Multi/live/Audio Track", 0f, true, true, false));

			streams.Add(new StreamData("C:\\Streams\\TimePerception\\Test\\test_animation.mov",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "/Resources/sounds/Multi/live/Audio Track", 0f, true, true, false));

    	}
	}
}

