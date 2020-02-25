using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;
using System.IO;
using Heartbeat;

public class HeartBeatECG : HeartBeat
	//class HeartbeatDetection : ScriptableObject
    {
	    //-------------------------
        //Heartbeat Detection (not used now)
		//-------------------------
        public static int beat_thresh = 20;
		public static int num_ecg_data = 10;
		public static int num_moving_average = 5;
        public static int beat_count_sup_msec = 200;
	
		
		//-------------------------
		//Variables
		//-------------------------
     	double pretime = 0;
		
		Queue<double> EcgData = new Queue<double>(num_ecg_data);
		Queue<double> EcgMovingAvg = new Queue<double>(num_moving_average);
	
        public HeartBeatECG()
		{
        }
	
			
		public void ProcessQRS()
		{
			GenerateBeat();
		}
     
        // used to receive the data
        public void Process(double ecg)
        {
            //Detect Peak and store in BeatEventHist
			
       		if(DetectPeak(ecg))
			{
            	GenerateBeat();
			}
			
        }

        private bool DetectPeak(double ecg)
        {
			bool is_beat = false;
			//UnityEngine.Debug.Log("sw " + status.sw.ElapsedMilliseconds);

			//store raw data 10
			EcgData.Enqueue(ecg);
			if (EcgData.Count() >= num_ecg_data)
        	{
				EcgData.Dequeue();
			}
			//store moving average 5
			double moving_average = EcgData.Average();
			//UnityEngine.Debug.Log(" moving average" + moving_average);
			EcgMovingAvg.Enqueue(moving_average);
			if (EcgMovingAvg.Count() >= num_moving_average)
        	{
				EcgMovingAvg.Dequeue();
			}
			
			//gradient
			double[] MovingAvgArray = EcgMovingAvg.ToArray();
			double y1 = MovingAvgArray[0];
			double y2 = MovingAvgArray[EcgMovingAvg.Count()/2];
			double y3 = MovingAvgArray[EcgMovingAvg.Count()-1];
			double gradient = (y1 -4*y2 +3*y3)/2;
			
		
			//if(EcgGradLater.Average() < 0&& EcgGradFormer.Average()  > 0){
			//	UnityEngine.Debug.Log(ecg + " " + moving_average + " " +gradient + " " + EcgGradFormer.Average() + " " +  EcgGradLater.Average());
			//}
			
			if( (status.sw.ElapsedMilliseconds - pretime) >  beat_count_sup_msec )
			{
	            //peaks are detected when the current signal is larger than the average of past data, and sufficient time passed.
				if( gradient > beat_thresh)
					{
						UnityEngine.Debug.Log("Detect Peak " + (status.sw.ElapsedMilliseconds - pretime));
						pretime =  status.sw.ElapsedMilliseconds;
  
						is_beat = true;
						//UnityEngine.Debug.Log("count " + EcgHistFormer.Count());
						
					}
			}
			
			
			if(is_beat) 
			{
				return true;
			}else{	
				return false;
			}		
          
        }
		
		


    }

