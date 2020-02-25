using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileManagingSpace
{
   
	public class FileStorageAll : FileStorage {

	
		void Awake()
  		{
			live_audio = new LiveData ("Assets\\Resources\\sounds\\Multi\\live\\Audio Track", true, true);

			//keisuke2
			streams.Add(new StreamData("",
			                           "I:\\Streams\\20141110b\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "I:\\Streams\\20141110b\\sounds\\Audio Track", 0.66f, true, true, false));
			//keisuke3
			streams.Add(new StreamData("",
			                           "I:\\Streams\\20141110c\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "I:\\Streams\\20141110c\\sounds\\Audio Track", 0.66f, true, true, false));

			//campus Garden
			streams.Add(new StreamData("",
			                           "I:\\Streams\\20141021Garden\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "I:\\Streams\\20141021Garden\\sounds\\Audio Track", 0.86f, true, true, false));
			
			//campus Market
			streams.Add(new StreamData("",
			                           "I:\\Streams\\20141027Market\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "I:\\Streams\\20141027Market\\sounds\\Audio Track", -1.35f, true, true, false));
			//campus Highway
			streams.Add(new StreamData("",
			                           "I:\\Streams\\20141027Highway\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "I:\\Streams\\20141027Highway\\sounds\\Audio Track", -0.55f, true, true, false));

			//hill desktip
			streams.Add(new StreamData("", "I:\\Streams\\DitchlingBeacon\\Ladybug-Stream-000000.pgr", 0, 0, true, false, false, false, 1.0f,
			                           "I:\\Streams\\DitchlingBeacon\\soundsOgg\\Audio TrackSub", 0.86f, true, true, false));

			//change blindness
			streams.Add(new StreamData("Assets\\Resources\\Videos\\ChangeBlindness_B_4000kbps-0000.ogv",
			                           "C:\\Streams\\20140228CB2\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20140228CB2\\sounds\\Audio Track", 0.86f, true, false, false));
			
			streams.Add(new StreamData("Assets\\Resources\\Videos\\ChangeBlindness_A_4000kbps-0000.ogv",
			                           "C:\\Streams\\20140228CB1\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20140228CB1\\sounds\\Audio Track", 0.2f, true, false, false));
			
			streams.Add(new StreamData("Assets\\Resources\\Videos\\ChangeBlindness_C_4000kbps-0000.ogv",
			                           "C:\\Streams\\20140228CB3\\Ladybug-Stream-000000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Streams\\20140228CB3\\sounds\\Audio Track", 0.2f, true, false, false));

			streams.Add(new StreamData("Assets\\Resources\\Videos\\Anil_4000kbps-0000.ogv",
			                           "C:\\Data\\PGR\\20131108\\Ladybug-Stream-003000.pgr", 0, 0, true, false, true, false, 1.0f,
			                           "C:\\Data\\Audio\\20131108\\Audio Track", 0.86f, true, false, false));
			/*

			//hill laptop

			streams.Add(new StreamData("", "C:\\Streams\\20140429DitchlingBeacon\\Ladybug-Stream-000000.pgr", 0, 0, false, false, 1.0f,
			                           "C:\\Streams\\20140429DitchlingBeacon\\soundsOgg\\Audio TrackSub", 0.86f, true, true));
	
		


			//campus 1
			streams.Add(new StreamData("Assets\\Resources\\Videos\\Campus_A_4000kbps-0000.ogv",
			                           "C:\\Streams\\20140321CampusAudio\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20140321CampusAudio\\sounds\\Audio Track", 0.86f, true, true));

			//campus 2
            streams.Add(new StreamData("", "C:\\Streams\\20140321CampusAudio2\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20140321CampusAudio2\\sounds\\Audio Track", 0.86f, true, false));


			//keisuke1
			streams.Add(new StreamData("",
			                           "I:\\Streams\\20141111a\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "I:\\Streams\\20141111a\\sounds\\Audio Track", 0.66f, true, true));

			//keisuke2
			streams.Add(new StreamData("",
			                           "I:\\Streams\\20141110a\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "I:\\Streams\\20141110a\\sounds\\Audio Track", 0.66f, true, true));


			//keisuke3
			streams.Add(new StreamData("",
			                           "I:\\Streams\\20141110c\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "I:\\Streams\\20141110c\\sounds\\Audio Track", 0.66f, true, true));




			//campus 1 slow
			streams.Add(new StreamData("",
									   "C:\\Streams\\20140321CampusAudio\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.5f,
			                           "C:\\Streams\\20140321CampusAudio\\soundsFast\\Audio Track", 0.86f, true, true));

			//campus 1 fast
			streams.Add(new StreamData("",
			                           "C:\\Streams\\20140321CampusAudio\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 0.666f,
			                           "C:\\Streams\\20140321CampusAudio\\soundsSlow\\Audio Track", 0.86f, true, true));

			
			//Chi2-222 A
			streams.Add(new StreamData("Assets\\Resources\\Videos\\ladybug_panoramic_2048x1024_4000kbps-0000.ogv",
			                           "C:\\Streams\\20140204\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20140204\\sounds\\Audio Track", 0.86f, true, false));
		

			//Anil
			streams.Add(new StreamData("Assets\\Resources\\Videos\\Anil_4000kbps-0000.ogv",
			                           "C:\\Data\\PGR\\20131108\\Ladybug-Stream-003000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Data\\Audio\\20131108\\Audio Track", 0.86f, true, false));


			*/
			/*
			//keisuke
            streams.Add(new StreamData("",
            							"C:\\Streams\\20140313b\\Ladybug-Stream-000000.pgr", 0, 0, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.2f, true, false));

            //inverse keisuke
            streams.Add(new StreamData("",
            							"C:\\Streams\\20140228InersePerson\\Ladybug-Stream-000000.pgr", 700, 0, false, true, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.2f, true, false));

            //normal keisuke
            streams.Add(new StreamData("",
            						"C:\\Streams\\20140228InersePerson\\Ladybug-Stream-000000.pgr", 250, 0, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.2f, true, false));

            //outside window inverse
            streams.Add(new StreamData("",
            						   "C:\\Streams\\20140210\\Ladybug-Stream-001000.pgr", 0, 0, false, true, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.2f, true, false));

            streams.Add(new StreamData("",
            						"C:\\Streams\\20140210\\Ladybug-Stream-002000.pgr", 0, 0, false, true, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.2f, true, false));

            streams.Add(new StreamData("",
            						"C:\\Streams\\20140210\\Ladybug-Stream-003000.pgr", 0, 0, false, true, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.2f, true, false));

            //on my seat
            streams.Add(new StreamData("",
            						"C:\\Streams\\20140205\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Streams\\20140205\\sounds\\Audio Track", 0.86f, true, false));

            streams.Add(new StreamData("",
            						"C:\\Streams\\20140204B\\Ladybug-Stream-000000.pgr", 0, 0, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.20f, true, false));
            


			//Anil Guitar
            streams.Add(new StreamData("",
            							"C:\\Data\\PGR\\20131108b\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Data\\Audio\\20131108b\\Audio Track", 0.86f, true, false));
			
			//David
            streams.Add(new StreamData("",
            							"C:\\Data\\PGR\\20131101b\\Ladybug-Stream-001000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Data\\Audio\\20131101b\\Audio Track", 0.86f, true, false));


            //streams.Add(new StreamData("",
            							"C:\\Data\\PGR\\20131101\\Ladybug-Stream-000000.pgr", 0, 0, true, false, 1.0f,
			//						   "C:\\Data\\Audio\\20131101\\Audio Track", 0.88f, true));
			
			//Keisuke
            streams.Add(new StreamData("",
            							"C:\\Data\\PGR\\20131105\\Ladybug-Stream-001000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Data\\Audio\\20131105\\Audio Track", 0.86f, true, false));
			//Cariburation laptop
            streams.Add(new StreamData("",
            							"C:\\Data\\PGR\\20131031\\Ladybug-Stream-003000.pgr", 0, 0, true, false, 1.0f,
			                           "C:\\Data\\Audio\\20131031\\Audio Track", 0.86f, true, false));
		
			
			//RIKEN Office
            streams.Add(new StreamData("", "Assets\\Resources\\Videos\\Campus_2048x1024_10000bps.ogv",
            							"Assets\\Resources\\streams\\default-000000.pgr", 0, 0, true, false, 1.0f,
			 					   "Assets\\Resources\\sounds\\Multi\\live\\Audio Track",
			                           "Assets\\Resources\\sounds\\Multi\\live\\Audio Track", 0.20f, true, false));
			 					 //  "Assets\\Resources\\sounds\\Multi\\sample1\\Audio Track", 0.20f, true));
								//	"Assets\\Resources\\sounds\\Multi\\michael\\Audio Track", 0.20f, true));
			
			//Car Drive
            streams.Add(new StreamData("", "Assets\\Resources\\streams\\ladybug_FullJPEG_00000071-000000.pgr", 0, 0, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\live_dummy_sussex.wav", 0.20f, false, false));
									//"Assets\\Resources\\sounds\\michael binaural.wav", 0.20f, false));
			
			//RIKEN Experiment Room Scene 1
            streams.Add(new StreamData("", "C:\\Data\\PGR\\20101002\\exp1Ladybug-Stream-000000.pgr", 0, 0, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\live_dummy_sussex.wav", 0.20f, false, false));
								   //"C:\\Data\\PGR\\20101002\\exp1Ladybug-Stream-000000-2010_10_2_15_18_50_124.wav", 0.20f, false));
			//RIKEN Experiment Room Scene 2
            streams.Add(new StreamData("", "C:\\Data\\PGR\\20101006\\exp5Ladybug-Stream-000000.pgr", 0, 0, false, false, 1.0f,
									//"C:\\Data\\PGR\\20101006\\exp5Ladybug-Stream-000000-2010_10_6_16_42_35_364.wav", 0.20f, false));
			//RIKEN Experiment Room Emma
            streams.Add(new StreamData("", "C:\\Data\\PGR\\20100810\\a\\Ladybug-Stream-000000.pgr", 0, 0, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\live_dummy_sussex.wav", 0.20f, false, false));
			//6
            streams.Add(new StreamData("", "C:\\Data\\PGR\\20100917\\Ladybug-Stream-000000-000000.pgr", 0, 0, false, false, 1.0f,
			                           "Assets\\Resources\\sounds\\live_dummy_sussex.wav", 0.20f, false, false));
			*/


    	}
		

	}
}

