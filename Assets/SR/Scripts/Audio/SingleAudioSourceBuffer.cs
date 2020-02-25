using UnityEngine;
using System.Collections;

public class SingleAudioSourceBuffer : InterfaceAudioSourceBuffer
{
	public AudioSource audio_source1;
	public AudioSource audio_source2;

	
	public override void UnloadClipBack()
	{
		if(flag)
		{
			Destroy (audio_source2.clip);
			audio_source2.clip = null;
		}else{
			Destroy (audio_source1.clip);
			audio_source1.clip = null;
		}
	}

	public override void UnloadClipFront()
	{
		if(flag)
		{
			Destroy (audio_source1.clip);
			audio_source1.clip = null;
		}else{
			Destroy (audio_source2.clip);
			audio_source2.clip = null;
		}
	}

	public override void SetClip(AudioClip clip)
	{
		if(flag)
		{
			audio_source1.clip = clip;
		}else{
			audio_source2.clip = clip;
		}
	}
	
	public override void SetClip(AudioClip[] clip)
	{
	}
	
	public override void SetClipBack(AudioClip clip)
	{
		if(flag)
		{
			audio_source2.clip = clip;
		}else{
			audio_source1.clip = clip;
		}
	}
	
	public override void SetClipBack(AudioClip[] clip)
	{
	}
	
	
	public override void PlayFront()
	{
		if(flag)
		{
			audio_source1.Play();
		}else{
			audio_source2.Play();
		}	
	}
	public override void PlayBack()
	{
		if(flag)
		{
			audio_source2.Play();
		}else{
			audio_source1.Play();
		}	
	}

	public override void StopFront()
	{
		if(flag)
		{
			audio_source1.Stop();
		}else{
			
			audio_source2.Stop();
		}	
	}
	public override void StopBack()
	{
		if(flag)
		{
			audio_source2.Stop();
		}else{
			
			audio_source1.Stop();
		}	
	}

	public override void PauseFront()
	{
		if(flag)
		{
			audio_source1.Pause();
		}else{
			
			audio_source2.Pause();
		}	
	}
	public override void PauseBack()
	{
		if(flag)
		{
			audio_source2.Pause();
		}else{
			
			audio_source1.Pause();
		}	
	}

	public override void ResetFront(float t)
	{		
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_source1.time = t;
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_source2.time = t;
			}
		}
		
	}
	public override void ResetBack(float t)
	{
		if(flag)
		{
			for(int i=0; i<8; i++)
			{
				audio_source2.time = t;
			}
		}else{
			for(int i=0; i<8; i++)
			{
				audio_source1.time = t;
			}
		}			
	}
		
	
	public override float GetVolumeFront()
	{
		if(flag)
		{
			return audio_source1.volume;
		}else{
			return audio_source2.volume;
		}	
	}	
	public override float GetVolumeBack()
	{
		if(flag)
		{
			return audio_source2.volume;
		}else{
			return audio_source1.volume;
		}	
	}

	public override void SetVolumeFront(float volume)
	{
		if(flag)
		{
			audio_source1.volume = volume;
		}else{
			audio_source2.volume = volume;
		}	
	}
	
	public override void SetVolumeBack(float volume)
	{
		if(flag)
		{
			audio_source2.volume = volume;
		}else{
			audio_source1.volume = volume;
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
			audio_source1.Play();
		}else{
			audio_source2.Play();
		}	
	}
	
	public override void Stop()
	{
		if(flag)
		{
			audio_source1.Stop();
		}else{
			
			audio_source2.Stop();
		}	
	}
	*/
	


	
	public override string GetClipNameFront()
	{
		
		if(flag)
		{
			if(audio_source1.clip == null)
			{
				return "not set";	
			}
			return audio_source1.clip.name;
		}else
		{
			if(audio_source2.clip == null)
			{
				return "not set";	
			}
			return audio_source2.clip.name;
		}
	}
	public override string GetClipNameBack()
	{
		if(flag)
		{
			if(audio_source2.clip == null)
			{
				return "not set";	
			}
			return audio_source2.clip.name;
		}else
		{
			if(audio_source1.clip == null)
			{
				return "not set";	
			}
			return audio_source1.clip.name;
		}
	}
	
	
	public override bool IsPlayFront()
	{
		if(flag)
		{
			return audio_source1.isPlaying;
		}else{
			return audio_source2.isPlaying;
		}
	}
	public override bool IsPlayBack()
	{
		if(flag)
		{
			return audio_source2.isPlaying;
		}else{
			return audio_source1.isPlaying;
		}
	}

	public override void SetTime(float time){
		
		audio_source1.time = time;
		audio_source2.time = time;
	}

	public override float GetTime()
	{
		if(flag)
		{
			return audio_source1.time;
		}else{
			return audio_source2.time;
		}
	}
	
	public override float GetLength()
	{
		if(flag)
		{
			if(audio_source1.clip == null)
			{
				return 10000000000f;
			}else{
				return 	audio_source1.clip.length;
			}
		}else{
			if(audio_source2.clip == null)
			{
				return 10000000000f;
			}else{
				return 	audio_source2.clip.length;
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

