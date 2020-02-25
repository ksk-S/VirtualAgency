using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{

	public class FileStorage : MonoBehaviour {
		public List<StreamData> streams = new List<StreamData>();
		public LiveData live_audio;

		void Awake()
  		{
    	}

	}
}

