using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioPlay : MonoBehaviour {
	public AudioSource live_audio_source;
	public string live_audioFileName;
	
	public AudioSource stream_audio_source;
	public List<string> stream_audioFileNames = new List<string>();
	public List<AudioClip> audioclips = new List<AudioClip>();
	
	public int audio_index = 0;
	public bool audioReady = false;
	
	void Awake () {
		
		live_audio_source = gameObject.AddComponent<AudioSource>();
		stream_audio_source = gameObject.AddComponent<AudioSource>();
		
		//live_audio_source.loop = true;
		//stream_audio_source.loop = true;
		
		live_audioFileName = "Assets\\Resources\\sounds\\live_background_noise.wav";
		
		stream_audioFileNames.Add("Assets\\Resources\\sounds\\live_background_noise.wav");
		stream_audioFileNames.Add("Assets\\Resources\\sounds\\default-000000.wav");
		stream_audioFileNames.Add("C:\\Data\\PGR\\20101002\\exp1Ladybug-Stream-000000-2010_10_2_15_18_50_124.wav");
		stream_audioFileNames.Add("C:\\Data\\PGR\\20101006\\exp5Ladybug-Stream-000000-2010_10_6_16_42_35_364.wav");
		stream_audioFileNames.Add("Assets\\Resources\\sounds\\default-000000.wav");
		stream_audioFileNames.Add("C:\\Data\\PGR\\20100917\\Ladybug-Stream-000000-000000-2010_9_17_14_3_24_791.wav");
		//stream_audioFileNames.Add("C:\\Data\\PGR\\20101002\\exp1Ladybug-Stream-000000-2010_10_2_15_18_50_124.wav");
		
		StartCoroutine(coLiveLoadFile(live_audioFileName));
		
		foreach(string fileName in stream_audioFileNames)
		{
			StartCoroutine(coLoadFile(fileName));
		}

		if(audio_index >= stream_audioFileNames.Count){
			audio_index = stream_audioFileNames.Count - 1;
		}
		
		
	}
	// Use this for initialization
	void Start () {
		
	}
	
	void Update () {
		
		if(!audioReady){
			bool is_playable = true;
			if (audioclips.Count == 0)
			{
				is_playable = false;
			}else{
				foreach(AudioClip clip in audioclips)
				{
					if(!clip.isReadyToPlay)
					{
						is_playable = false;
					}
				}
			}
			if(live_audio_source.clip == null || !live_audio_source.clip.isReadyToPlay)
			{
				is_playable = false;
			}
				
			if(is_playable)
			{
				SetClip(audio_index);
				audioReady = true;
			}
		}
		
		//** atode motto naosu
		if(audioReady)
		{
			if(live_audio_source.time > live_audio_source.clip.length - 0.2)
			{
				live_audio_source.Stop ();
				live_audio_source.time = 0;
				live_audio_source.Play ();
			}
			if(stream_audio_source.time > stream_audio_source.clip.length - 0.2)
			{
				stream_audio_source.Stop ();
				stream_audio_source.time = 0;
				stream_audio_source.Play ();
			}
		}
		
	}
	
	IEnumerator coLiveLoadFile(string fileName){
        WWW www = new WWW("file://" + fileName);
		
        AudioClip myAudioClip = www.GetAudioClip();
        while (!myAudioClip.isReadyToPlay)
        yield return www;	
		
        AudioClip clip = www.GetAudioClip(true, true, AudioType.WAV); // 3D sound, sreaming
		if(!clip.isReadyToPlay)
		{
			Debug.Log("Can't load audio file:" + fileName);
		}else{
	        string[] parts = fileName.Split('\\');
    	    clip.name = parts[parts.Length - 1];
			live_audio_source.clip = clip;
		}
    }
	
	IEnumerator coLoadFile(string fileName){
        WWW www = new WWW("file://" + fileName);
		
        AudioClip myAudioClip = www.GetAudioClip();
        while (!myAudioClip.isReadyToPlay)
        yield return www;
		
        AudioClip clip = www.GetAudioClip(true, true, AudioType.WAV); // 3D sound, sreaming
		if(!clip.isReadyToPlay)
		{
			Debug.Log("Can't load audio file:" + fileName);
		}else{
	        string[] parts = fileName.Split('\\');
    	    clip.name = parts[parts.Length - 1];
        	audioclips.Add(clip);
		}
    }
	
	
	public void SetClip(int index)
	{
		if (index < audioclips.Count)
		{
			if(audioclips[index].isReadyToPlay){
				stream_audio_source.clip = audioclips[index];
			}else{
				Debug.Log ("not yet ready for SetClip");	
			}
		}else{
			Debug.Log ("An audio file has not yet set");	
		}
	}
	
	public void SetAudiobyId(int index)
	{
		SetClip(index);
	}
	
	public void PlayAudiobyId(int index)
	{
		SetClip(index);
		PlayStream();
	}
	
	public void PlayLiveDelayed(float delay)
	{
		live_audio_source.PlayDelayed(delay);
	}
	
	public void PlayLive()
	{
		if(live_audio_source.clip == null || !live_audio_source.clip.isReadyToPlay)
		{
			Debug.Log ("Live Audio has not yet ready");
		}else{
			live_audio_source.Play ();	
		}
	}
	public void StopLive()
	{
		live_audio_source.Stop ();
	}
	
	
	public void PlayStream()
	{
		stream_audio_source.Play ();
	}
	
	public void StopStream()
	{
		stream_audio_source.Stop ();
	}
	
	
	// Update is called once per frame
	
}
