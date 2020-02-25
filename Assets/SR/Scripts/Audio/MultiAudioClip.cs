using UnityEngine;
using System.Collections;

//stored panoramic audio sources
public class MultiAudioClip : InterfaceAudioClip
{
	string[] stream_audioFileNames = new string[8];
	
	AudioClip[] audioclips = new AudioClip[8];
	
	//AudioSource[] audioSources = new AudioSource[8]; 
	
	void Start()
	{
		is_multiple = true;	
	}
	void Update ()
	{
		isReadyToPlay ();
	}


	public override void UnloadClip()
	{
	
		for(int i=0; i<8; i++)
		{
			Destroy (audioclips[i]);
			audioclips[i] = null;
		}
	}

	public override bool isReadyToPlay()
	{
		bool isReady = true;
		for (int i=0;  i<8; i++)
		{
			if(audioclips[i] == null)
			{
				isReady = false;
			}else if(audioclips[i].loadState != AudioDataLoadState.Loaded)
			{
				isReady = false;
			}
		}

		is_ready_to_play = isReady;
		return is_ready_to_play;
	}
	
	public override void SetAudioFile(string filename, bool is_ogg)
	{
		for (int i=0;  i<8; i++)
		{
			if(is_ogg){
				stream_audioFileNames[i] = filename + "-" + (i+1) + ".ogg";
				StartCoroutine(coLoadFile(stream_audioFileNames[i], i, AudioType.OGGVORBIS));
			}else{
				stream_audioFileNames[i] = filename + "-" + (i+1) + ".wav";
				StartCoroutine(coLoadFile(stream_audioFileNames[i], i, AudioType.WAV));
			}

		}
	}
	
	public override void SetAudioSource(InterfaceAudioSourceBuffer buffer)
	{
		audioSourceBuffer = buffer;
	}
	
	IEnumerator coLoadFile(string fileName, int index, AudioType audiotype){
        WWW www = new WWW("file:///" + fileName);
        //Debug.Log("loading " + fileName);

        AudioClip myAudioClip = www.GetAudioClip();
        
        while (myAudioClip.loadState != AudioDataLoadState.Loaded) {
           //Debug.Log(myAudioClip.loadState);
            yield return www;
        }
        //Debug.Log ("Load multi audio file :" +  index + " : " + fileName );
        audioclips[index] = www.GetAudioClip(true, false, audiotype); // 3D sound, sreaming

        //Debug.Log("loaded " + fileName);

		www.Dispose();
		if(audioclips[index].loadState != AudioDataLoadState.Loaded)
		{
			Debug.Log("Can't load audio file (multi):" + fileName);
		}else{
	        string[] parts = fileName.Split('\\');
    	    audioclips[index].name = parts[parts.Length - 1];
		}
    }
	
	
	public override void SetClip()
	{
		audioSourceBuffer.SetClip (audioclips);
		
	}
	
	public override void SetClipBack()
	{
		audioSourceBuffer.SetClipBack (audioclips);
		
	}
		
	public override void Play()
	{
		audioSourceBuffer.PlayFront();
		/*
		for(int i=0; i<8; i++)
		{
			audioSources[i].Play ();
		}
		*/
		//audioSources[0].Play ();
		
		Debug.Log ("Audio Multi : play");
	}
	
	public override void Stop()
	{
		audioSourceBuffer.StopFront();
		
		Debug.Log ("Audio Multi : multi stop");
	}
	
	public override void Reset()
	{
		audioSourceBuffer.ResetFront(0.0f);
		/*
		for(int i=0; i<8; i++)
		{
			audioSources[i].time = 0;
		}
		*/
	}
	
	public override float GetTime()
	{
		return audioSourceBuffer.GetTime();
		//return audioSources[0].time;
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

