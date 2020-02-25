using UnityEngine;
using System.Collections;

namespace Heartbeat
{
	public class FlashProperty : MonoBehaviour {
     	public FeedbackType feedback_type = FeedbackType.Sync;
		public bool ChangeColor = true;
		public Color target_color = new Color(1.0f, 0.0f, 0.0f, 1);
     	public Color original_color = new Color(1.0f, 1.0f, 1.0f, 1);
		public bool ChangeSize  = true;
		public float ratio  = 1.15f;
		 
    	public float delay = 0;
    	 
     
	     // Use this for initialization
    	 void Start () {
     	}
     
	     // Update is called once per frame
    	 void Update () {
     	
	     }
	}
}
