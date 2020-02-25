using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{
   
	public class FileStorageMinimal : FileStorage {

	
		void Awake()
  		{
			live_audio = new LiveData ("Assets\\Resources\\sounds\\Multi\\live\\Audio Track", true, true);
			//keisuke2

			//driving
			streams.Add(new StreamData(Application.dataPath + "\\Resources\\Videos\\driving.mov",
			                           Application.dataPath + "\\Resources\\streams\\ladybug_FullJPEG_00000071-000000.pgr", 0, 0, true, false, false, false, 1.0f,
			                           Application.dataPath + "\\Resources\\sounds\\Multi\\live\\Audio Track", 0.0f, true, true, false));


    	}

	}
}

