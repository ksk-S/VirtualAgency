using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{
   
	public class FileStorageAVProTest  : FileStorage {

	
		void Awake()
		{   

			live_audio = new LiveData (Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", true, true);


			//C2-222
			streams.Add(new StreamData("C:\\Streams\\20140204\\me.mov",
			                           "C:\\Streams\\20140204\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20140204\\sounds\\Audio Track", 0.0f, true, true, false));

			//driving
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\driving.mov",
			                           Application.dataPath + "\\Resources\\streams\\ladybug_FullJPEG_00000071-000000.pgr", 0, 0, true, false, false, false, 1.0f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0.0f, true, true, false));

			//market s
			streams.Add(new StreamData("C:\\Streams\\20141027Market\\1000-2000\\Sussex_S.mov",
			                           "C:\\Streams\\20141027Market\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true, false));

			//ditchling beacon 
			streams.Add(new StreamData("C:\\Streams\\DitchlingBeaconMini\\Downs.mov",
			                           "C:\\Streams\\DitchlingBeaconMini\\ladybug_20150324_190712_000000-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\DitchlingBeaconMini\\soundsL\\Audio Track", 0f, true, true, false));

			//time lapse
			streams.Add(new StreamData("C:\\Streams\\DitchlingBeaconTimeLapse\\Downs.mov",
			                           "C:\\Streams\\DitchlingBeaconTimeLapse\\ladybug_20150324_191652_000002-000000.pgr", 0, 0, true, false, false, false, 40.0f,
			                           "C:\\Streams\\DitchlingBeaconMini\\soundsL\\Audio Track", 0f, true, true, false));

		



    	}

	}
}

