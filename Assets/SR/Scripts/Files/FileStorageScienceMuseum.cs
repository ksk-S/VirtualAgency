using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{
   
	public class FileStorageScienceMuseum  : FileStorage {

	
		void Awake()
		{   

			live_audio = new LiveData ("Assets\\Resources\\sounds\\Multi\\live\\Audio Track", true, true);

			//ditchling beacon 
			
			streams.Add(new StreamData("C:\\Streams\\DitchlingBeaconMini\\ladybug_panoramic_0-2707_30000kbps-0000.ogv",
			                           "C:\\Streams\\DitchlingBeaconMini\\ladybug_20150324_190712_000000-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\DitchlingBeaconMini\\soundsL\\Audio Track", 0f, true, true, false));

			//time lapse
			streams.Add(new StreamData("C:\\Streams\\DitchlingBeaconTimeLapse\\ladybug_panoramic_0-1387_30000kbps-0000.ogv",
			                           "C:\\Streams\\DitchlingBeaconTimeLapse\\ladybug_20150324_191652_000002-000000.pgr", 0, 0, true, false, false, false, 40.0f,
			                           "C:\\Streams\\DitchlingBeaconMini\\soundsL\\Audio Track", 0f, true, true, false));


			//Evelyn Musk
			streams.Add(new StreamData("C:\\Streams\\20150325ScienceMuseum\\Evelyn\\ladybug_panoramic_212-492_30000kbps-0000.ogv",
			                           "C:\\Streams\\20150325ScienceMuseum\\Evelyn\\Ladybug-Stream-000000.pgr", 212, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20150325ScienceMuseum\\Evelyn\\sounds\\Audio Track", 0.36f, true, true, false));
			//Balls
			streams.Add(new StreamData("C:\\Streams\\20150325ScienceMuseum\\Balls\\ladybug_panoramic_0-376_30000kbps-0000.ogv",
			                           "C:\\Streams\\20150325ScienceMuseum\\Balls\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20150325ScienceMuseum\\Balls\\sounds\\Audio Track", 0.36f, true, true, false));

			//Hilke
			streams.Add(new StreamData("C:\\Streams\\20150325ScienceMuseum\\Hielke\\ladybug_panoramic_0-700_30000kbps-0000.ogv",
			                           "C:\\Streams\\20150325ScienceMuseum\\Hielke\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20150325ScienceMuseum\\Hielke\\sounds\\Audio Track", 0.36f, true, true, false));

			//market
			streams.Add(new StreamData("C:\\Streams\\20141027Market\\1000-2000\\ladybug_panoramic_0-1000_30000kbps-0000.ogv",
			                           "C:\\Streams\\20141027Market\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true, false));

			//market
			streams.Add(new StreamData("C:\\Streams\\20141027Market\\1000-2000\\Sussex.ogv",
			                           "C:\\Streams\\20141027Market\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true, false));
			

			
		
				
			/*
			//Hielke
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20150325ScienceMuseum\\Talk\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20150325ScienceMuseum\\Talk\\sounds\\Audio Track", 0.4f, true, true));

			*/



			/*
			//ksk3
			streams.Add(new StreamData("C:\\Streams\\20150324Rehearsal\\ksk3\\ladybug_panoramic_0-92_30000kbps-0000.ogv",
			                           "C:\\Streams\\20150324Rehearsal\\ksk3\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20150324Rehearsal\\ksk3\\sounds\\Audio Track", -1.2f, true, true));

			//balls
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20150324Rehearsal\\balls\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20150324Rehearsal\\balls\\sounds\\Audio Track", -1.2f, true, true));

*/
			/*
			//ksk
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20150324Rehearsal\\ksk\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20150324Rehearsal\\ksk\\sounds\\Audio Track", -1.2f, true, true));
			//ksk2
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20150324Rehearsal\\ksk2\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20150324Rehearsal\\ksk2\\sounds\\Audio Track", -1.2f, true, true));
			//ksk3
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20150324Rehearsal\\ksk3\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20150324Rehearsal\\ksk3\\sounds\\Audio Track", -1.2f, true, true));
			//lift
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20150324Rehearsal\\lifting\\Ladybug-Stream-000000.pgr", 0, 0, false, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\1000-2000\\sounds\\normal\\Audio Track", -1.2f, true, true));
			*/

			/*
			//market
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true));


			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Highway\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 50, true, false, 1.0f,
			                           "C:\\Streams\\20141027Highway\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true));

			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141021Garden\\1000-2000\\ladybug_1000-2000-000000.pgr", 0, 50, true, false, 1.0f,
			                           "C:\\Streams\\20141021Garden\\1000-2000\\sounds\\normal\\Audio Track", 0.0f, true, true));



			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\2000-3000\\ladybug_2000-3000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\2000-3000\\sounds\\normal\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\3000-4000\\ladybug_3000-4000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\3000-4000\\sounds\\normal\\Audio Track", 0.0f, true, true));
			
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\4000-5000\\ladybug_4000-5000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\4000-5000\\sounds\\normal\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\5000-6000\\ladybug_5000-6000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\5000-6000\\sounds\\normal\\Audio Track", 0.0f, true, true));
			
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20141027Market\\6000-7000\\ladybug_6000-7000-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20141027Market\\6000-7000\\sounds\\normal\\Audio Track", 0.0f, true, true));

			*/

					

    	}

	}
}

