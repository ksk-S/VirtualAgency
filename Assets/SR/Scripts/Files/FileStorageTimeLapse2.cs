using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{
   
	public class FileStorageTimeLapse2 : FileStorage {

		void Awake()
  		{

			live_audio = new LiveData (Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", true, true);


			//market
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\market_10K_normal_x1.00.ogv",
			                           "", 0, 0, true, false, false, false, 1.0f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));
			
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\market_10K_slow_x1.50.ogv",
			                           "", 0, 0, true, false, false, false, 1.50f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));
			
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\market_10K_fast_x0.66.ogv",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));
			
			//campus Garden
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\garden_10K_normal_x1.00.ogv",
			                           "", 0, 0, true, false, false, false, 1.0f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));
			
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\garden_10K_slow_x1.50.ogv",
			                           "", 0, 0, true, false, false, false, 1.50f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));
			
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\garden_10K_fast_x0.66.ogv",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));

			//roadway
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\roadway_10K_normal_x1.00.ogv",
			                           "", 0, 0, true, false, false, false, 1.0f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));
			
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\roadway_10K_slow_x1.50.ogv",
			                           "", 0, 0, true, false, false, false, 1.50f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));
			
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\Time\\roadway_10K_fast_x0.66.ogv",
			                           "", 0, 0, true, false, false, false, 0.50f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0f, true, true, false));
			



    	}
	}
}

