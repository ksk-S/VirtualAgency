using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FileManagingSpace;

using CommonUtil;

//managing audio control. to be instantiated two; one for live, another for record.
public class AudioManager : MonoBehaviour { 
	
	FileManager files;
	
	public GameObject AudioClipsObj;
	public GameObject LiveClipsObj;
	public GameObject StreamClipsObj;
	
	public GameObject SingleAudioObj;
	public GameObject[] MultiAudioObj = new GameObject[8];
	
	public MultiAudioSourceBuffer multiAudioBuffer;
	SingleAudioSourceBuffer singleAudioBuffer;
	
/*	public AudioSource audio_source;
	public AudioSource[] multi_audio_source = new AudioSource[8];
*/
	
	public List<string> audioFileNames = new List<string>();

	public List<InterfaceAudioClip> liveclips; 
	public List<InterfaceAudioClip> audioclips;
	
	public InterfaceAudioClip CurAudioClip ;
	
	public InterfaceAudioSourceBuffer CurBuffer;

	private Status cur_live_status = Status.LIVE;
    public int cur_live_index = 0;
    public int pre_live_index = 0;

    public int cur_audio_index = 0;
	public int pre_audio_index = -1;


	public bool audioReady = false;
	public bool is_transition = false;

	public float offset_time = 0.0f;

	public float multi_radius = 3.0f;

	public bool is_streaming_audio = true;

    public bool is_repeat_audio_at_end_of_video = true;
	void Awake () {
		InitializeAudioClipObj();
		InitializeSingleAudioObj();
		InitializeMultiAudioObj();
	
		files = GameObject.Find ("FileManager").GetComponent<FileManager>();
		
	
		for(int j=0; j< files.streams.Count; j++){
			audioclips.Add (null);
		}

        for (int j = 0; j < files.live_audios.Count; j++)
        {
            liveclips.Add(null);
        }

    }
    // Use this for initialization
    IEnumerator Start () {

		Debug.Log ("Create Audio Clips");

        foreach (LiveData elem in files.live_audios)
        {
            CreateLiveClip(files.live_audios.IndexOf(elem), elem.is_multi);

        }

        foreach (StreamData elem in files.streams)
		{
			CreateClip(files.streams.IndexOf(elem), elem.is_multi);
		}
			
		Debug.Log ("Audio Loading...");
		if(!is_streaming_audio)
		{
            foreach (LiveData elem in files.live_audios)
            {
                yield return StartCoroutine(LoadClip(liveclips[files.live_audios.IndexOf(elem)], elem.live_audio, elem.is_ogg));
            }

			foreach (StreamData elem in files.streams)
			{
                if (elem.is_external_audio)
                {
                    yield return StartCoroutine(LoadClip(audioclips[files.streams.IndexOf(elem)], elem.stream_audio, elem.is_ogg));
                }
			}	    

			if(cur_audio_index >= audioclips.Count){
				cur_audio_index = audioclips.Count - 1;
			}
			/*
			bool is_playable = true;
			if(!liveclip.isReadyToPlay())
			{
				is_playable = false;
			}
			foreach(InterfaceAudioClip clip in audioclips)
			{		
				if(!clip.isReadyToPlay())
				{
					is_playable = false;
				}
			}
		
			if (is_playable) {
				Debug.Log ("Audio Ready");
				audioReady = true;
			} else {	
				yield return null;
			}
			*/
			audioReady = true;
		}else{


			yield return StartCoroutine(LoadClip(liveclips[0], files.live_audios[0].live_audio, files.live_audios[0].is_ogg));
			yield return StartCoroutine(LoadAudioStream(cur_audio_index));

			audioReady = true;
		}
		Debug.Log ("Audio Ready");
		SetClipBack (Status.LIVE, 0);


		if(CurAudioClip != null)
			CurBuffer = CurAudioClip.audioSourceBuffer;


	}

	void InitializeAudioClipObj()
	{
		AudioClipsObj = new GameObject();
		AudioClipsObj.transform.position = new Vector3(0,0,0);
		AudioClipsObj.transform.parent = transform;
		AudioClipsObj.name = "AudioClips";
		
		LiveClipsObj = new GameObject();
		LiveClipsObj.transform.position = new Vector3(0,0,0);
		LiveClipsObj.transform.parent = AudioClipsObj.transform;
		LiveClipsObj.name = "Live";

		StreamClipsObj = new GameObject();
		StreamClipsObj.transform.position = new Vector3(0,0,0);
		StreamClipsObj.transform.parent = AudioClipsObj.transform;
		StreamClipsObj.name = "Stream";
	}
	
	void InitializeSingleAudioObj()
	{
		
		
		SingleAudioObj = new GameObject();
		SingleAudioObj.transform.position = new Vector3(0,0,0);
		SingleAudioObj.transform.parent = transform;
		SingleAudioObj.name = "SingleAudioSource";
		
		singleAudioBuffer = gameObject.AddComponent<SingleAudioSourceBuffer>() as SingleAudioSourceBuffer;
		singleAudioBuffer.audio_source1 = SingleAudioObj.AddComponent<AudioSource>();
		singleAudioBuffer.audio_source2 = SingleAudioObj.AddComponent<AudioSource>();
	}
	
	void InitializeMultiAudioObj()
	{
		multiAudioBuffer = gameObject.AddComponent<MultiAudioSourceBuffer>() as MultiAudioSourceBuffer;
		
		for(int i=0; i<8; i++)
		{
			float angle = 45 * i;
			float x = multi_radius * Mathf.Sin (angle *  0.0174532925f);
			float z = multi_radius * Mathf.Cos (angle *  0.0174532925f);
			MultiAudioObj[i] = new GameObject();
			MultiAudioObj[i].transform.position = new Vector3(x,0,z);
			MultiAudioObj[i].transform.parent = transform;
			MultiAudioObj[i].name = "MultiAudioSource_" + i;
		
			multiAudioBuffer.audio_sources1[i] =  MultiAudioObj[i].AddComponent<AudioSource>();
			multiAudioBuffer.audio_sources2[i] =  MultiAudioObj[i].AddComponent<AudioSource>();

			multiAudioBuffer.audio_sources1[i].playOnAwake = false;
			multiAudioBuffer.audio_sources2[i].playOnAwake = false;
			//Linear 
			//multiAudioBuffer.audio_sources1[i].rolloffMode = AudioRolloffMode.Linear;
			//multiAudioBuffer.audio_sources2[i].rolloffMode = AudioRolloffMode.Linear;
		}
	}


	public void RotateMultiAudioSource(float angle)
	{
		for(int i=0; i<8; i++)
		{
			MultiAudioObj[i].transform.RotateAround(transform.position, Vector3.up, angle);
		}
	}


	public void IncRadius()
	{
		multi_radius += 0.2f;

		SetRadius ();
	}
	
	public void DecRadius()
	{
		if (multi_radius > 0.4f)
		{
			multi_radius -= 0.2f;
		}
		SetRadius ();
	}

	void SetRadius()
	{
		for(int i=0; i<8; i++)
		{
			Vector3 norm = MultiAudioObj[i].transform.position;
			norm.Normalize();
			norm.x = norm.x * multi_radius;
			norm.y = norm.y * multi_radius;
			norm.z = norm.z *multi_radius;

			MultiAudioObj[i].transform.position = norm;
		}
	}

	void  CreateLiveClip(int index, bool is_multi)
	{
		if (is_multi) {
			liveclips[index] = LiveClipsObj.AddComponent <MultiAudioClip>() as InterfaceAudioClip;
			liveclips[index].SetAudioSource(multiAudioBuffer as InterfaceAudioSourceBuffer);
		} else {
			liveclips[index] = LiveClipsObj.AddComponent<SingleAudioClip>() as InterfaceAudioClip;
			liveclips[index].SetAudioSource(singleAudioBuffer as InterfaceAudioSourceBuffer);
		}
	}

	void  CreateClip(int index, bool is_multi)
	{
		if(is_multi)
		{
			audioclips[index] = StreamClipsObj.AddComponent<MultiAudioClip>() as InterfaceAudioClip;
			audioclips[index].SetAudioSource(multiAudioBuffer as InterfaceAudioSourceBuffer);
		}else
		{
			audioclips[index]  = StreamClipsObj.AddComponent<SingleAudioClip>() as InterfaceAudioClip;
			audioclips[index].SetAudioSource(singleAudioBuffer as InterfaceAudioSourceBuffer);
		}

	}

	public IEnumerator LoadAudioStream(int streamId)
	{
		StreamData elem = files.streams [streamId];
		yield return StartCoroutine(LoadClip(audioclips[streamId], elem.stream_audio, elem.is_ogg));
	}
	
	IEnumerator LoadClip(InterfaceAudioClip clip, string file,  bool is_ogg)
	{
		clip.SetAudioFile(file, is_ogg);
		yield return 0;
	}


	void Update () {

		
	}

	public void StopAll()
	{
		if(CurAudioClip != null)
		{
		CurAudioClip.audioSourceBuffer.StopFront();
		CurAudioClip.audioSourceBuffer.StopBack();
		}
	}

    public void ReplayAudio()
    {
        
		if(CurAudioClip != null)
		{
        	CurAudioClip.audioSourceBuffer.StopFront();
			CurAudioClip.audioSourceBuffer.ResetFront(offset_time);
        	CurAudioClip.audioSourceBuffer.PlayFront();
		}
		Debug.Log ("Replay Audio");
        //CrossFade( cur_live_status,  cur_audio_index);

        /*
        audioclips[(int)cur_live_status][cur_audio_index].audioSourceBuffer.StopFront ();
        audioclips[(int)cur_live_status][cur_audio_index].audioSourceBuffer.ResetFront ();
        audioclips[(int)cur_live_status][cur_audio_index].audioSourceBuffer.PlayFront ();
        */
    }
	
	/*
	public  void SetClip(Status status, int index)
	{
		live_status = status;
		audio_index = index;

		if (index < audioclips[(int)live_status].Count)
		{
			if(audioclips[(int)live_status][index].isReadyToPlay()){
				audioclips[(int)live_status][index].SetClip();
			}else{
				Debug.Log ("not yet ready for SetClip");	
			}
		}else{
			Debug.Log ("An audio file has not yet set");	
		}
	}
	*/
	
	
	public  void SetClipBack(Status status, int index)
	{
		if (status == Status.LIVE) {
            if (index < liveclips.Count)
            {
                if (liveclips[index].isReadyToPlay())
                {
                    liveclips[index].SetClipBack();
                    CurAudioClip = liveclips[index];
                }
                else
                {
                    Debug.Log("not yet ready for SetClip for live");
                }
            }
		} else {
			if (index < audioclips.Count) {
				if (audioclips [index].isReadyToPlay ()) {
					audioclips [index].SetClipBack ();	
					CurAudioClip = audioclips [index];
				} else {
					Debug.Log ("not yet ready for SetClip for replay");
                    //try read live
                    liveclips[0].SetClipBack();
                    CurAudioClip = liveclips[0];
                }
			} else {
					Debug.Log ("An audio file has not yet set");	
			}
		}
	}
		



		
	public IEnumerator CrossFade(Status status, int index, bool isReset=false, bool isPause=false)
	{
        Debug.Log("CrossFade " + status.ToString() + " i:" + index);
		/*
		while (is_transition) {
			yield return 0;
		}
		*/

		Status pre_live_status = cur_live_status;
		cur_live_status = status;

        string debug_text = "status:" + pre_live_status + " >> " + cur_live_status;
        if (pre_live_status == Status.STREAM) {
			pre_audio_index = cur_audio_index;
            debug_text += " pre_stream_index:" + pre_audio_index;
        }
        else { 
            pre_live_index = cur_live_index;
            debug_text += " pre_live_index:" + pre_audio_index;
        }

        if(cur_live_status == Status.STREAM)
        {
            cur_audio_index = index;
            debug_text += " cur_stream_index:" + cur_audio_index;
        }
        else
        {
            cur_live_index = index;
            debug_text += " cur_live_index:" + cur_audio_index;
        }

        Debug.Log(debug_text);
        
        
        InterfaceAudioSourceBuffer preBuffer = CurBuffer;
		InterfaceAudioSourceBuffer newBuffer = CurBuffer;

		if (pre_live_status == Status.LIVE) {
			preBuffer = liveclips[pre_live_index].audioSourceBuffer;
		} else {
			preBuffer = audioclips[pre_audio_index].audioSourceBuffer;
		}
        
		if(cur_live_status == Status.LIVE) 
		{
            SetClipBack(cur_live_status, cur_live_index);

            newBuffer = liveclips[cur_live_index].audioSourceBuffer;
			offset_time = 0.0f;

		}else if(cur_live_status == Status.STREAM) 
		{
            SetClipBack(cur_live_status, cur_audio_index);

            newBuffer = audioclips[cur_audio_index].audioSourceBuffer;
				
			offset_time = (files.streams[index].start_frame / 16.0f  - files.streams[index].audio_latency) / files.streams[index].play_speed;
			Debug.Log ("audio offset time : " + offset_time);
		}

	
		yield return StartCoroutine(CoCrossFade(0.5f,  preBuffer,  newBuffer, isReset, isPause));
		Debug.Log ("finsih cross fading");
		if (is_streaming_audio) {
			if (cur_live_status == Status.STREAM) {
				if (pre_audio_index != cur_audio_index)
				{
					audioclips [pre_audio_index].UnloadClip ();
					Debug.Log ("UnloadClilp " + pre_audio_index);
					Resources.UnloadUnusedAssets();
					Debug.Log ("Release memory");
				}
			}
		}
	}
	
	IEnumerator CoCrossFade( float duration,  InterfaceAudioSourceBuffer curBuffer,  InterfaceAudioSourceBuffer newBuffer, bool isReset=false, bool isPause=false)
	{
		is_transition = true;
			
	//	Debug.Log("Curernt Front Clip : " + curBuffer.GetClipNameFront());
	//	Debug.Log("Next    Back Clip : " + newBuffer.GetClipNameBack());
		
		multiAudioBuffer.SwapBuffer();
	 	singleAudioBuffer.SwapBuffer();

		//Debug.Log (offset_time); // koko de reset sarete simatteru noka
		newBuffer.SetVolumeFront(0f);

		if (isReset) {
			newBuffer.ResetFront (offset_time);
		}

		//newBuffer.SetTime (5.0f);
		//Debug.Log (newBuffer.GetTime ());
		newBuffer.PlayFront();

		float volume = 0f;
		float currentTime = 0.0f;
		float waitTime = 0.02f;
		while(!(Mathf.Approximately(duration, currentTime)))
     	{
			currentTime += Time.fixedDeltaTime;
			volume = Mathf.Lerp( 0.0f, 1.0f, currentTime/duration );
		
			curBuffer.SetVolumeBack (1f - volume);
			newBuffer.SetVolumeFront (volume);
			yield return new WaitForSeconds(waitTime);
		}
		newBuffer.SetVolumeFront (1.0f);

		if(isPause)
		{
			Debug.Log ("Pause Audio");
			curBuffer.PauseBack ();
		}
		/*
		else{
			Debug.Log ("Stop Audio");
			curBuffer.StopBack ();
		}
		*/

		is_transition = false;
	}
	
	
	public  float GetCurAudioTime()
	{
		//has to support non-multi sounds later 
		return multiAudioBuffer.GetTime ();
		//return audioclips[cur_audio_index].GetTime();
	}
	
	public float GetCurAudioLength()
	{
		return multiAudioBuffer.GetLength ();
		//return audioclips[cur_audio_index].GetLength ();
	}
	
	public bool IsNearEnd()
	{	
		return multiAudioBuffer.IsNearEnd ();
//		return audioclips[cur_audio_index].IsNearEnd();
	}
	
}
