using UnityEngine;
using System.Collections;

//stored single audio sources
public class SingleAudioClip : InterfaceAudioClip
{
	public AudioClip audioclip;
	public string audioFileName;
	
	//AudioSource audioSource;
	
	void Start()
	{
		is_multiple = false;	
	}
	
	void Update ()
	{
		isReadyToPlay ();
	}

	public override void UnloadClip()
	{

		Destroy (audioclip);
		audioclip = null;

	}
	
	public override bool isReadyToPlay()
	{
		if(audioclip == null)
		{
			is_ready_to_play =  false;
		}else
		{
			is_ready_to_play = audioclip.isReadyToPlay;
		}
		return is_ready_to_play;
	}
	
	public override void SetAudioFile(string filename, bool is_ogg)
	{
		audioFileName = filename;

		if(is_ogg){
			StartCoroutine(coLoadFile(audioFileName, AudioType.OGGVORBIS));
		}else{
			StartCoroutine(coLoadFile(audioFileName, AudioType.WAV));
		}
	}
	
	public override void SetAudioSource(InterfaceAudioSourceBuffer buffer)
	{
		audioSourceBuffer = buffer;
	}
	
	IEnumerator coLoadFile(string fileName, AudioType audiotype){
        WWW www = new WWW("file://" + fileName);
		
        AudioClip myAudioClip = www.GetAudioClip();
        while (!myAudioClip.isReadyToPlay)
        yield return www;	
		
		//Debug.Log ("audio file loaded " + fileName );
		audioclip = www.GetAudioClip(false, true, audiotype); // 2D sound, sreaming
		if(!audioclip.isReadyToPlay)
		{
			Debug.Log("Can't load audio file:" + fileName);
		}else{
	        string[] parts = fileName.Split('\\');
    	    audioclip.name = parts[parts.Length - 1];
		}
    }
	
	public override void SetClip()
	{
		audioSourceBuffer.SetClip (audioclip);
		/*
		if(isReadyToPlay())
		{
			audioSource.clip = 	audioclip;
		}else{
			Debug.Log ("Clip not ready");	
		}
		*/
	}
	
	public override void SetClipBack()
	{
		audioSourceBuffer.SetClipBack (audioclip);
	}
	
	public override void Play()
	{
		Debug.Log ("Audio Single : Play");
		audioSourceBuffer.PlayFront ();
	}
	
	public override void Stop()
	{
		Debug.Log ("Audio Single : Stop");
		audioSourceBuffer.StopFront ();
	}
	
	public override void Reset()
	{
		audioSourceBuffer.ResetFront(0.0f);
	}
	
	public override float GetTime()
	{
		return audioSourceBuffer.GetTime();
	}
	
	public override float GetLength()
	{
		return audioSourceBuffer.GetLength();
	}
	
		
	public override bool IsNearEnd()
	{
		if(audioSourceBuffer.GetTime() > audioSourceBuffer.GetLength() - 1.0)
		{
			return true;
		}else{
			return false;	
		}
	}
	
}

