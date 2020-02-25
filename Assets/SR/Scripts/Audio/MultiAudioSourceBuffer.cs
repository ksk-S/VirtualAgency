using UnityEngine;
using System.Collections;

public class MultiAudioSourceBuffer : InterfaceAudioSourceBuffer
{
	public AudioSource[] audio_sources1 = new AudioSource[8];
	public AudioSource[] audio_sources2 = new AudioSource[8];
	
	int[] randomize_array = {0, 4, 7, 3, 6, 2, 5, 1};


	public override void UnloadClipBack()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				Destroy (audio_sources2[i].clip);
				audio_sources2[i].clip = null;
			}

		}else{
			for(int i=0; i<8; i++)
			{
				Destroy (audio_sources1[i].clip);
				audio_sources1[i].clip = null;
			}
		}
	}
	
	public override void UnloadClipFront()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				Destroy (audio_sources1[i].clip);
				audio_sources1[i].clip = null;
			}
			
		}else{
			for(int i=0; i<8; i++)
			{
				Destroy (audio_sources2[i].clip);
				audio_sources2[i].clip = null;
			}
		}
	}

	public override void SetClip(AudioClip clip)
	{
	}
	
	public override void SetClip(AudioClip[] clip)
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].clip = clip[i];
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].clip = clip[i];
			}
		}

	}
	
	public override void SetClipBack(AudioClip clip)
	{
	}
	
	public override void SetClipBack(AudioClip[] clip)
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].clip = clip[i];
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].clip = clip[i];
			}
		}
	}
	
	
	public override void PlayFront()
	{
		if(flag)
		{	
			for(int i=0; i<8; i++)
			{
				audio_sources1[randomize_array[i]].Play();
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources2[randomize_array[i]].Play();
			}
		}	
	}
	public override void PlayBack()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources2[randomize_array[i]].Play();
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources1[randomize_array[i]].Play();
			}
		}	
	}
	public override void StopFront()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].Stop();
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].Stop();
			}
		}	
	}
	public override void StopBack()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].Stop();
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].Stop();
			}
		}				
	}


	public override void PauseFront()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].Pause();
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].Pause();
			}
		}	
	}
	public override void PauseBack()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].Pause();
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].Pause();
			}
		}	
	}

	public override void ResetFront(float t)
	{		
		if(flag)
		{

			for(int i=0; i<8; i++)
			{
				audio_sources1[i].time = t;
			}
		}else{

			for(int i=0; i<8; i++)
			{
				audio_sources2[i].time = t;
			}
		}
		
	}
	public override void ResetBack(float t)
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].time = t;
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].time = t;
			}
		}			
	}
	
	
	public override float GetVolumeFront()
	{
		if(flag)
		{
			return audio_sources1[0].volume;
		}else{
			return audio_sources2[0].volume;
		}	
	}	
	public override float GetVolumeBack()
	{
		if(flag)
		{
			return audio_sources2[0].volume;
		}else{
			return audio_sources1[0].volume;
		}	
	}

	public override void SetVolumeFront(float volume)
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].volume = volume;
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].volume = volume;
			}
		}	
	}
	
	public override void SetVolumeBack(float volume)
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].volume = volume;
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].volume = volume;
			}
		}
	}
	
		
	public override string GetClipNameFront()
	{
		if(flag)
		{
			if(audio_sources1[0].clip == null)
			{
				return "not set";	
			}
			return audio_sources1[0].clip.name;
		}else
		{
			if(audio_sources2[0].clip == null)
			{
				return "not set";	
			}
			return audio_sources2[0].clip.name;
		}
	}
	public override string GetClipNameBack()
	{
		if(flag)
		{
			if(audio_sources2[0].clip == null)
			{
				return "not set";	
			}	
			return audio_sources2[0].clip.name;
		}else
		{
			if(audio_sources1[0].clip == null)
			{
				return "not set";	
			}	
			return audio_sources1[0].clip.name;
		}
	}
	
	
	
	public override bool IsPlayFront()
	{
		if(flag)
		{
			return audio_sources1[0].isPlaying;

		}else{
			return audio_sources2[0].isPlaying;
		}
	}
	public override bool IsPlayBack()
	{
		if(flag)
		{
			return audio_sources2[0].isPlaying;
		}else{
			return audio_sources1[0].isPlaying;
		}
	}
	
	
	
	public override void SwapBuffer()
	{
		flag  = !flag; 	
	}
	
	/*
	public override void Play()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].Play();
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].Play();
			}
		}	
	}
	
	public override void Stop()
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_sources1[i].Stop();
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_sources2[i].Stop();
			}
		}	
	}
	*/

	public override void SetTime(float time){
	
		for(int i=0; i<8; i++)
		{
			audio_sources1[i].time = time;
		}
	
		for(int i=0; i<8; i++)
		{
			audio_sources2[i].time = time;
		}
	
	}

	public override float GetTime()
	{
		if(flag)
		{
			return 	audio_sources1[0].time;
		}else{
			return 	audio_sources2[0].time;
		}
	}
	public override float GetLength()
	{
		if(flag)
		{
			if(audio_sources1[0].clip == null)
			{
				return 100000000000f;
			}else
			{
				return 	audio_sources1[0].clip.length;
			}
		}else{
			if(audio_sources2[0].clip == null)
			{
				return 100000000000f;
			}else{
				return 	audio_sources2[0].clip.length;
			}
		}
	}

	public override bool IsNearEnd()
	{
		
		if (flag) {
			if (GetTime () > GetLength () - 1.0) {
				return true;
			} else {
				return false;	
			}			
		}else{
			if (GetTime () > GetLength () - 1.0) {
				return true;
			} else {
				return false;	
			}	
		}
	}


}

