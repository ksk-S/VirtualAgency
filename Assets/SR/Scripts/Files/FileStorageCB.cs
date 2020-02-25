using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{
   
	public class FileStorageCB : FileStorage {

		void Awake()
  		{

			live_audio = new LiveData ("Assets\\Resources\\sounds\\Multi\\live\\Audio Track", true, true);


			streams.Add(new StreamData("Assets\\Resources\\Videos\\closed_0-162_10000kbps-0000_2048_1.ogv",
			                           "", 0, 0, true, false, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.86f, true, true, false));

			streams.Add(new StreamData("Assets\\Resources\\Videos\\cclosed_0-162_30000kbps-0000_5400_10kbps_2048.ogv",
			                           "", 0, 0, true, false, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.86f, true, true, false));

			streams.Add(new StreamData("Assets\\Resources\\Videos\\closed_0-162_30000kbps-0000_5400_10kbps_5400.ogv",
			                           "", 0, 0, true, false, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.86f, true, true, false));

			streams.Add(new StreamData("Assets\\Resources\\Videos\\closed_0-162_30000kbps-0000_5400_20kbps_5400.ogv",
			                           "", 0, 0, true, false, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.86f, true, true, false));

			streams.Add(new StreamData("Assets\\Resources\\Videos\\closed_0-162_30000kbps-0000_5400_30kbps_5400.ogv",
			                           "", 0, 0, true, false, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.86f, true, true, false));
		

    	}
	}
}

