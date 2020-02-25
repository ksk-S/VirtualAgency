using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;
using System.IO;
using Heartbeat;

public class HeartBeat : MonoBehaviour
	//class HeartbeatDetection : ScriptableObject
    {
		public static int max_beat_events = 10;
	
        public Status status;
   		public Stopwatch sw = new Stopwatch();
	
		//-------------------------
		//Status
		//-------------------------
	    public bool raw_beat_flag = false;
		public double raw_beat_bpm = 0;
		public int avg_beat_diff = 0;
		
		public int beat_count = 0;		
		public long previous_beat_time = 0;
	
		public bool use_correct_feedback = true;
		public bool use_delayed_feedback = true;
		public bool use_fast_feedback = true;
		public bool use_slow_feedback = true;
		public bool use_same_feedback = true;
		public bool use_systol_feedback = true;
		public bool use_diastole_feedback = true;
		
		[HideInInspector]
		public Feedback feedback_correct;
		[HideInInspector]
		public Feedback feedback_delayed;
		[HideInInspector]
		public Feedback feedback_fast;
		[HideInInspector]
		public Feedback feedback_slow;	
		[HideInInspector]
		public Feedback feedback_same;	

		[HideInInspector]
		public Feedback feedback_systol;	
		[HideInInspector]
		public Feedback feedback_diastole;	
	
		Queue<double> BeatEventHist = new Queue<double>(max_beat_events);
      
	
	    //dummy beats
        double pre_dummy_time = 0;
	
		//normal random 
        protected static System.Random Rn = new System.Random(System.Environment.TickCount);
        protected static double Alpha, Beta, BoxMuller1, BoxMuller2;
        protected static bool Flag = false;
      
		public bool isDummyMode = false;
		public int dummyInterval = 600;


		public delegate void RawBeatOccurHandler();
		public event RawBeatOccurHandler RawBeatOccur;

		public bool healthy = false;

		public HeartBeat()
		{
         
        }
	
		public void Awake()
		{
			status = new Status(ref sw);
			
			GameObject feedbackObj = GameObject.Find ("Feedbacks");

			if(use_correct_feedback){
				feedback_correct 	= Feedback.CreateComponent(feedbackObj, FeedbackType.Sync, false,     0,  0f,   1f, ref status);
			}

			if (use_delayed_feedback) {
				feedback_delayed = Feedback.CreateComponent (feedbackObj, FeedbackType.Delay, false, -350, 50f, 1f, ref status);
			}

			if( use_fast_feedback){
				feedback_fast    	= Feedback.CreateComponent(feedbackObj, FeedbackType.Fast, true,      0, 50f, 0.7f, ref status);
			}

			if( use_slow_feedback){
				feedback_slow    	= Feedback.CreateComponent(feedbackObj, FeedbackType.Slow, true,      0, 50f, 1.3f, ref status);
			}
			
			if( use_same_feedback){
				feedback_same    	= Feedback.CreateComponent(feedbackObj, FeedbackType.Same, true,      0,  0f, 1.0f, ref status);
			}

			if( use_systol_feedback){
				feedback_systol     = Feedback.CreateComponent(feedbackObj, FeedbackType.Systole, false,  0,  0f,   1f, ref status);
			}

			if( use_diastole_feedback){
				feedback_diastole   = Feedback.CreateComponent(feedbackObj, FeedbackType.Diastole, false, -350,  0f,   1f, ref status);
			}

			RawBeatOccur += Dummy;
		}

	
		void Dummy()
		{
		}

		public virtual void  Start()
		{
		}
	
		public virtual void  Update()
		{
		}
		
		public void GenerateBeat()
        {
			long diff = status.sw.ElapsedMilliseconds - previous_beat_time;
			if( diff < 200)
			{
				return;
			}
		    beat_count++;


			RawBeatOccur();

            //calc beat period
            previous_beat_time = status.sw.ElapsedMilliseconds;
			
			//create flash strength
			raw_beat_flag = true;
			
            //store beat periods
            BeatEventHist.Enqueue(diff);
            if (BeatEventHist.Count() >= max_beat_events)
            {
                BeatEventHist.Dequeue();
            }
			
		
			//UnityEngine.Debug.Log ("Generate Beat");
			//UnityEngine.Debug.Log ("hist " + BeatEventHist.Count ());
            //calc bpm and show
            avg_beat_diff = (int)BeatEventHist.Average();
			//UnityEngine.Debug.Log ("aveg " + BeatEventHist.Average () + " "  + avg_beat_diff);

            raw_beat_bpm = (double)( 60.0 / ( (double)avg_beat_diff / 1000.0) );
          
			
			
			if(use_correct_feedback)
			{
				feedback_correct.CallNextBeat(avg_beat_diff);
			}
			
			if(use_delayed_feedback)
			{
				feedback_delayed.CallNextBeat(avg_beat_diff);	
			}
			
			if(use_slow_feedback)
			{
				feedback_slow.SetBeatDiff(avg_beat_diff);
			}
			
			if(use_fast_feedback)
			{
				feedback_fast.SetBeatDiff(avg_beat_diff);
			}
			
			if(use_same_feedback)
			{
				feedback_same.SetBeatDiff(avg_beat_diff);
			}
		
			if (use_systol_feedback) {
				feedback_systol.CallNextBeat (avg_beat_diff);

			}
			if (use_diastole_feedback) {
				feedback_diastole.CallNextBeat (avg_beat_diff);
			}
		}  
	
		public void MakeDummyBeat(double dummy_diff){
			if (status.sw.ElapsedMilliseconds > pre_dummy_time + dummy_diff)
            {
                pre_dummy_time = status.sw.ElapsedMilliseconds;
				
                GenerateBeat();
            }
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

