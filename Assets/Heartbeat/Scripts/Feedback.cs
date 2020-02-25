using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace Heartbeat
{
    public class Feedback : MonoBehaviour
    {
    	Status status;
		//--------------------------------------
		//Parameters
		//--------------------------------------
		public int system_delay = 400 + 70 + 15;
		//--------------------------------------
		//Status
		//--------------------------------------
		public FeedbackType type = FeedbackType.Sync;
		public int precede_time;
		public double noise;
		public float freq_change_rate;
        public bool feedback_flag = false;

		public int actual_beat_count = 0;
		public double feedback_bpm = 0;
		public double feedback_strength = 0;
		public int freq_change_beat_msec = 1000;
		public bool auto_update;
		

		//--------------------------------------
		//Variables
		//--------------------------------------
		long feedback_pre_time = 0;
        int callback_timer_status = 0;
//		int nn_callback_timer_status = 0;
		
//		long alt_estimated_beat_time1 = 0;
//        long alt_estimated_beat_time2 = 0;
		
        Queue<double> feedback_beat_events = new Queue<double>(HeartBeat.max_beat_events);

	    //-------------------------
        //Timer
        //-------------------------
		
		bool timer1_flag = false;
		bool timer2_flag = false;
		bool timer_change_freq_flag = false;
		
		float time_next_beat1;
		float time_next_beat2;
		float time_change_freq;
//		float time_nn_beat1;
//		float time_nn_beat2;
		
		
		System.Timers.Timer timer_change_freq;
/*
        System.Timers.Timer timer_next_beat1;
        System.Timers.Timer timer_next_beat2;
//		System.Timers.Timer timer_nn_beat1 = new System.Timers.Timer();
//      System.Timers.Timer timer_nn_beat2 = new System.Timers.Timer();
*/
		
		//normal random 
        static System.Random Rn = new System.Random(System.Environment.TickCount);
        static double Alpha, Beta, BoxMuller1, BoxMuller2;
        static bool Flag = false;
      
		public delegate void BeatOccurHandler();
		public event BeatOccurHandler BeatOccur;
		
        public Feedback()
        {
        }
		
		public static Feedback CreateComponent (GameObject where, FeedbackType feedbackType, bool auto1, int precede_time1, float noise1, float freq_change_rate1, ref Status status1)
		{
    		Feedback fb = where.AddComponent<Feedback>();
			fb.type = feedbackType;
    		fb.auto_update = auto1;
			fb.precede_time = precede_time1;
			fb.noise = noise1;
			fb.freq_change_rate = freq_change_rate1;
			fb.status = status1;
    		return fb;
  		}
		
		public void Start()
		{
			BeatOccur += Dummy;
			if (auto_update) {
				timer_change_freq_flag = true;
				
				timer_change_freq = new System.Timers.Timer();
				timer_change_freq.Interval = 1000;
				timer_change_freq.AutoReset = true;
				timer_change_freq.Elapsed += new ElapsedEventHandler(FreqChangeCallback);

				timer_change_freq.Start ();	
			}
		}
		
		void Update()
		{
			if(timer1_flag)
			{
				time_next_beat1 -= UnityEngine.Time.deltaTime * 1000;
   				if(time_next_beat1 < 0){
    		    	NextBeatCallback();
					timer1_flag = false;
				}
			}

			if(timer2_flag)
			{
				time_next_beat2 -= UnityEngine.Time.deltaTime * 1000;
   				if(time_next_beat2 < 0){
    		    	NextBeatCallback();
					timer2_flag = false;
				}
			}

			if(timer_change_freq_flag)
			{
				CalledOnBeat("freq");
				timer_change_freq_flag = false;
			}

		}
		
		public void CallNextBeat(int next_beat_time)
		{
			int jitter = 0;
			if(noise > 0)
			{
				jitter = (int)NormalDistribution(0, noise);
			}
			
			next_beat_time -= (precede_time +  system_delay + jitter);
			if (next_beat_time > 0)
            {
                switch(callback_timer_status)
                {
                        case 0:
							time_next_beat1 = next_beat_time;
							timer1_flag = true;
						
//                            timer_next_beat1.Interval = next_beat_time ;
//                            timer_next_beat1.Start();
                            callback_timer_status = 1;
                            break;
                        case 1:
							time_next_beat2 = next_beat_time;
							timer2_flag = true;
//                            timer_next_beat2.Interval = next_beat_time ;
//                            timer_next_beat2.Start();
                            callback_timer_status = 0;
                            break;
                    }
            }
			//CallNNBeat(next_beat_time);
		}

		
		void Dummy()
		{
		}
		
		
		public void SetBeatDiff(int msec)
		{
			freq_change_beat_msec = msec;
		}

        public void FreqChangeCallback(object sender, EventArgs e)
		//public void FreqChangeCallback()
        {
			timer_change_freq_flag = true;

			time_change_freq = freq_change_beat_msec * freq_change_rate;
			timer_change_freq.Interval = time_change_freq;
		}
		
		
		 //public void NextBeatCallback(object sender, EventArgs e)
		public void NextBeatCallback()
        {
			CalledOnBeat("next");
        }
		
		/*
		 public void NNBeatCallback(object sender, EventArgs e)
        {
			CalledOnBeat("nn");
        }
        */
		
        public void CalledOnBeat(string state)
        {
			long diff = status.sw.ElapsedMilliseconds - feedback_pre_time;
		
			//if(state != "freq" && precede_time == 0){
			//	UnityEngine.Debug.Log("Beat " + (diff) + " " + state);
			//}
			
			if(diff < 150){
				return;
			}
			actual_beat_count++;
			feedback_pre_time = status.sw.ElapsedMilliseconds;
			feedback_flag = true;
			
			BeatOccur();
			
			//calc bpm
			feedback_beat_events.Enqueue(diff);
            if (feedback_beat_events.Count() >= HeartBeat.max_beat_events)
            {
                feedback_beat_events.Dequeue();
            }
            double avg_sec_per_beat = feedback_beat_events.Average();
            feedback_bpm = 60 / (avg_sec_per_beat / 1000);

        }
		
		 public static double NormalDistribution(double mu, double sigma)
        {
            if (Flag)
            {

                Alpha = Rn.NextDouble();
                Beta = Rn.NextDouble() * Math.PI * 2;
                BoxMuller1 = Math.Sqrt(-2 * Math.Log(Alpha));
                BoxMuller2 = Math.Sin(Beta);
            }
            else
                BoxMuller2 = Math.Cos(Beta);
            Flag = !Flag;
            return sigma * (BoxMuller1 * BoxMuller2) + mu;
        }
		
    }
}
