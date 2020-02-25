using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{
   
	public class FileStorageTimeLapse : FileStorage {

		void Awake()
  		{

			live_audio = new LiveData ("Assets\\Resources\\sounds\\Multi\\live\\Audio Track", true, true);

			/*
			//distance
			streams.Add(new StreamData("Assets\\Resources\\Videos\\time\\ladybug_panoramic_96-1571_20000kbps-0000_normal.ogv",
			                           "C:\\Streams\\20141021Distance\\Ladybug-Stream-000000.pgr", 96, 0, true, false, 1.0f,
			                           "Assets\\Resources\\Videos\\time\\sounds\\normal\\Audio Track", -1.15f , true, true));
			//-1.15 for streat -1.2 for video

			streams.Add(new StreamData("Assets\\Resources\\Videos\\time\\ladybug_panoramic_96-1571_20000kbps-0000_fast.ogv",
			                           "C:\\Streams\\20141021Distance\\Ladybug-Stream-000000.pgr", 96, 0, true, false, 1.1f,
			                           "Assets\\Resources\\Videos\\time\\sounds\\fast\\Audio Track", -1.15f, true, true));
			//-1.15 for streat -0.2 for video

			streams.Add(new StreamData("Assets\\Resources\\Videos\\time\\ladybug_panoramic_96-1571_20000kbps-0000_slow.ogv",
			                           "C:\\Streams\\20141021Distance\\Ladybug-Stream-000000.pgr", 96, 0, true, false, 0.905f,
			                           "Assets\\Resources\\Videos\\time\\sounds\\slow\\Audio Track", -1.15f, true, true));
			//-1.15 for streat -2.0 for video

			//test for sub block
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141021Distance\\300-799\\ladybug_20141215_150825_000004-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141021Distance\\300-799\\sounds\\normal\\Audio Track", 0.0f , true, true));

			*/

			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141021Garden\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20141021Garden\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true, false));
		/*
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141021Garden\\2000-3000\\ladybug_2000-3000-000000.pgr", 0, 0, true, false, 1.1f,
			                           "C:\\Streams\\20141021Garden\\2000-3000\\sounds\\fast\\Audio Track", 0.0f, true, true));

			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141021Garden\\3000-4000\\ladybug_3000-4000-000000.pgr", 0, 0, true, false, 0.905f,
			                           "C:\\Streams\\20141021Garden\\3000-4000\\sounds\\slow\\Audio Track", 0.0f, true, true));
		
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141021Garden\\4000-5000\\ladybug_4000-5000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141021Garden\\4000-5000\\sounds\\normal\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141021Garden\\5000-6000\\ladybug_5000-6000-000000.pgr", 0, 0, true, false, 1.1f,
			                           "C:\\Streams\\20141021Garden\\5000-6000\\sounds\\fast\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141021Garden\\6000-7000\\ladybug_6000-7000-000000.pgr", 0, 0, true, false, 0.905f,
			                           "C:\\Streams\\20141021Garden\\6000-7000\\sounds\\slow\\Audio Track", 0.0f, true, true));


			//market
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true));

			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\2000-3000\\ladybug_2000-3000-000000.pgr", 0, 0, true, false, 1.1f,
			                           "C:\\Streams\\20141027Market\\2000-3000\\sounds\\fast\\Audio Track", 0.0f, true, true));

			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\3000-4000\\ladybug_3000-4000-000000.pgr", 0, 0, true, false, 0.905f,
			                           "C:\\Streams\\20141027Market\\3000-4000\\sounds\\slow\\Audio Track", 0.0f, true, true));


			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\4000-5000\\ladybug_4000-5000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\4000-5000\\sounds\\normal\\Audio Track", 0.0f, true, true));

			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\5000-6000\\ladybug_5000-6000-000000.pgr", 0, 0, true, false, 1.1f,
			                           "C:\\Streams\\20141027Market\\5000-6000\\sounds\\fast\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\6000-7000\\ladybug_6000-7000-000000.pgr", 0, 0, true, false, 0.905f,
			                           "C:\\Streams\\20141027Market\\6000-7000\\sounds\\slow\\Audio Track", 0.0f, true, true));


			//highway
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Highway\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Highway\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Highway\\2000-3000\\ladybug_2000-3000-000000.pgr", 0, 0, true, false, 1.1f,
			                           "C:\\Streams\\20141027Highway\\2000-3000\\sounds\\fast\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Highway\\3000-4000\\ladybug_3000-4000-000000.pgr", 0, 0, true, false, 0.905f,
			                           "C:\\Streams\\20141027Highway\\3000-4000\\sounds\\slow\\Audio Track", 0.0f, true, true));
			
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Highway\\4000-5000\\ladybug_4000-5000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Highway\\4000-5000\\sounds\\normal\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Highway\\5000-6000\\ladybug_5000-6000-000000.pgr", 0, 0, true, false, 1.1f,
			                           "C:\\Streams\\20141027Highway\\5000-6000\\sounds\\fast\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Highway\\6000-7000\\ladybug_6000-7000-000000.pgr", 0, 0, true, false, 0.905f,
			                           "C:\\Streams\\20141027Highway\\6000-7000\\sounds\\slow\\Audio Track", 0.0f, true, true));
			*/



    	}
	}
}

