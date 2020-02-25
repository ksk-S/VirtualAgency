using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;


public class HeadTrackingRecorder : MonoBehaviour {

	SRController SRCtl;
	//OVRCameraRig OculusCamRig;
	//OVRDevice OculusDeviceCtl;
	LadybugPluginInterface ladybugCtl;


	[System.Serializable]
	public struct HeadTrackData
	{
		public float time;
		public int frame;
		public Vector3 rotation;
	}
	

	public List<HeadTrackData> head_track_record_list; 
	public List<HeadTrackData> head_track_replay_list; 

	public bool IsDetectHeadMotionMode = false;
	public bool IsHeadRecording = false;
	public bool IsHeadReplaying = false;

	public bool IsHeadDoubbledSpeed = false;

	int frameId = 0;
	int head_replay_index = 0;
	float record_start_time = 0.0f;
	float replay_start_time = 0.0f;

	Vector3 pre_rotation;
	public float minimal_secs_since_last_switch = 5.0f;
	public float additonal_random_sec_range = 5.0f;

	public float actual_waiting_secs = 0.0f;

	public float angle_threshold = 0.25f;
	float time_last_switch;

	public bool switching_timing = false;

	//get average head speed
	public static int num_average = 3;
	Queue<float> angleQueue = new Queue<float>(num_average);
	Queue<Vector2> orientationQueue = new Queue<Vector2>(num_average);

	public bool IsCalcHeadSpeed = false;
	Vector3 pre_rotation_headspeed_detection;
	public float angle_threshold_headspeed_detection = 0.150f;
	public float angle_threshold_headspeed_detection2 = 0.0f;

	// Use this for initialization
	void Start () {
		
		head_track_record_list = new List<HeadTrackData>();
		head_track_replay_list = new List<HeadTrackData>();

		//OculusCamRig = GameObject.Find ("OVRCameraRig").GetComponent<OVRCameraRig>();


		SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();
		ladybugCtl = SRCtl.ladybugCtl;

		if(!SRCtl.useDomeVideo){
			if(SRCtl.useLiveLadybug)
			{
				ladybugCtl = GameObject.Find ("SphericalScreen").GetComponent("LadybugPluginCamera") as LadybugPluginInterface;
			}else
			{
				ladybugCtl = GameObject.Find ("SphericalScreen").GetComponent("LadybugPluginStream") as LadybugPluginInterface;
			}
		}

	}



	// Update is called once per frame
	void Update () {
	
		if(IsDetectHeadMotionMode)
		{
			DetectHeadMotion();

		}

		if(IsHeadRecording)
		{
			UpdateRecordHead();

		}
			
		if(IsHeadReplaying)
		{
			UpdateReplayHead();
		}

		if (IsHeadDoubbledSpeed) {

			HeadMovementExaggerate();
		}

		if (Input.GetKeyDown(KeyCode.R) )
		{

			if(IsHeadRecording)
			{
				StopHeadRecording();
			}else{
				StartHeadRecording();
			}
		}
		
		if (Input.GetKeyDown(KeyCode.P) )
		{
			if(IsHeadReplaying)
			{
				StopHeadReplay();
			}else{
				StartHeadReplay();

			}
		}

		if(IsCalcHeadSpeed)
		{
            //OVRPose pose = OVRManager.display.GetHeadPose(0.0); 
            //Vector3 rotation = pose.orientation.eulerAngles;
            Vector3 rotation = new Vector3();


            float angle =  Vector2.Angle(new Vector2(pre_rotation_headspeed_detection.x, pre_rotation_headspeed_detection.y), new Vector2(rotation.x, rotation.y));
			//float angle =  Vector3.Angle(pre_rotation_headspeed_detection, rotation);
			if(angle < 300){
				angleQueue.Enqueue(angle);
				orientationQueue.Enqueue(new Vector2(rotation.x, rotation.y));
				//Debug.Log (angle);

				if (angleQueue.Count >= num_average)
				{
					angleQueue.Dequeue();
					orientationQueue.Dequeue ();
				}

				pre_rotation_headspeed_detection = rotation;
			}

		}

	}

	public void StartDetectHeadMotion(float waiting_time)
	{
		IsDetectHeadMotionMode = true;
		switching_timing = false;

		time_last_switch = UnityEngine.Time.timeSinceLevelLoad;

		actual_waiting_secs = minimal_secs_since_last_switch + UnityEngine.Random.Range (0.0f, additonal_random_sec_range);

        //OVRPose pose = OVRManager.display.GetHeadPose(0.0); 
        //Quaternion rotation = pose.orientation;
        Quaternion rotation = new Quaternion();

        pre_rotation = rotation.eulerAngles;
	}	

	void DetectHeadMotion()
	{
        //OVRPose pose = OVRManager.display.GetHeadPose(0.0); 
        //Vector3 rotation = pose.orientation.eulerAngles;
        Vector3 rotation = new Vector3();


        if ( UnityEngine.Time.timeSinceLevelLoad - time_last_switch < actual_waiting_secs)
		{
			pre_rotation = rotation;
			return;
		}
		//Debug.Log ("after wating =" + actual_waiting_secs);
	
		float angele =  Vector3.Angle(pre_rotation, rotation);
		//Debug.Log (angele + " " + angle_threshold);

		//Debug.Log (rotation.x +"," + pre_rotation.x);
		if( (rotation.y < 20 || rotation.y > 340) || (angele < 10.0f && angele > angle_threshold) )
		{

			switching_timing = true;
			IsDetectHeadMotionMode = false;

			Debug.Log ("head motion detected!!  angle=" + angele);
		}
		pre_rotation = rotation;

	}

	public bool IsHeadMoving()
	{
		float sum = 0f;
		foreach( float angle in angleQueue )
		{
			sum += angle;
		}
		float angle_mean = sum / angleQueue.Count;

		//Vector3 rotation = OVRManager.display.GetHeadPose(0.0).orientation.eulerAngles;
		//float angle_long =  Vector2.Angle(orientationQueue.Peek (), new Vector2 (rotation.x, rotation.y));
		//Debug.Log (angle_mean + " " + angle_long);

		//Debug.Log ("sum : " + sum / angleQueue.Count);
		if ( angle_mean  > angle_threshold_headspeed_detection ){ //&& angle_long > angle_threshold_headspeed_detection2) {
			return true;
		} else {
			return false;
		}
	}

	void UpdateRecordHead()
	{
		HeadTrackData data = new HeadTrackData();

        //OVRPose pose = OVRManager.display.GetHeadPose(0.0); 
        //Quaternion rotation = pose.orientation;
        Quaternion rotation = new Quaternion();

                data.rotation = rotation.eulerAngles;

		data.time = UnityEngine.Time.timeSinceLevelLoad - record_start_time;
		data.frame = frameId++;
		
		head_track_record_list.Add (data);
	}

	void UpdateReplayHead()
	{
		if(head_track_replay_list.Count <= head_replay_index){
			//OculusCamRig.transform.localRotation = Quaternion.identity;
			//OculusCamCtl.SetOrientationOffset(Quaternion.identity);
			StopHeadReplay();
			return;
		}
		HeadTrackData data = head_track_replay_list[head_replay_index];
		float tracking_time = data.time;
		
		float elapsed_time = UnityEngine.Time.timeSinceLevelLoad - replay_start_time;
		if(tracking_time >  elapsed_time){
			return;
		}else{
			
			int counter = 0;
			do{
				data = head_track_replay_list[head_replay_index];
				tracking_time = data.time;

				SRCtl.OVRStreamCameraRight.transform.eulerAngles = data.rotation;
				SRCtl.OVRStreamCameraLeft.transform.eulerAngles = data.rotation;
				SRCtl.OVRCentreAnchor.transform.eulerAngles = data.rotation;

				if (!SRCtl.useDomeVideo) {
					ladybugCtl.cur_head_history = data.rotation;
				}
				head_replay_index++;
				counter++;
			}while(tracking_time < elapsed_time - 0.01666f && counter < 30);
//			Debug.Log (counter);
		}

	}
	public void ClearHeadRecordingData()
	{
		head_track_record_list.Clear ();
	}
	
	public void StartHeadRecording()
	{
//		Debug.Log ("Start Recording");
		IsHeadRecording = true;
		record_start_time = UnityEngine.Time.timeSinceLevelLoad;
		frameId = 0;
		head_track_record_list.Clear ();

		UpdateRecordHead ();
	}
	public void StopHeadRecording()
	{
		
//		Debug.Log ("Stop Recording");
		IsHeadRecording = false;
		//save to file?

		head_track_replay_list = new List<HeadTrackData>(head_track_record_list);
		head_replay_index = 0;
	}
	
	public void StartHeadReplay()
	{
		//Debug.Log ("Start Replay");
		//OculusCamRig.IsRotationUpdate = false;
	
		IsHeadReplaying = true;
		replay_start_time = UnityEngine.Time.timeSinceLevelLoad;

		head_replay_index = 0;
		if (SRCtl.useDomeVideo) {
		} else {
			ladybugCtl.is_head_replay = true;
		}

		UpdateReplayHead ();
	}
	public void StopHeadReplay()
	{
//		Debug.Log ("Stop Replay");
//		OculusCamRig.IsRotationUpdate = true;
//		OculusCamCtl.EnableOrientation = true;
		IsHeadReplaying = false;
		if (SRCtl.useDomeVideo) {
		} else {
			ladybugCtl.is_head_replay = false;
		}

	}

	public void SetTrajectoryForReplay(List<HeadTrackData> data)
	{
		head_track_replay_list = data;
	}

	public List<HeadTrackData> GetRecordedTrajectory()
	{
		return head_track_record_list;
	}

	public void HeadMovementExaggerate(){
        //	OculusCamRig.IsRotationUpdate = false;

        //OVRPose pose = OVRManager.display.GetHeadPose(0.0); 
        //Quaternion rotation = pose.orientation;
        //Vector3 vec = rotation.eulerAngles;
        Vector3 vec = new Vector3();

		if(vec.y > 0f && vec.y < 90f )
		{
			vec.y *= 2.0f;
		}else if (vec.y > 270f && vec.y < 360f )
		{
			vec.y = -360f + vec.y * 2.0f;
		}else{
			vec.y = 180f;
		}
		//Debug.Log (vec.x );

		SRCtl.OVRStreamCameraRight.transform.eulerAngles = vec;
		SRCtl.OVRStreamCameraLeft.transform.eulerAngles = vec;
		SRCtl.OVRCentreAnchor.transform.eulerAngles = vec;

	}

}
