using UnityEngine;
using System.Collections;

public abstract class InterfaceAudioSourceBuffer : MonoBehaviour
{

	public bool flag = false;

	public abstract void UnloadClipFront();
	public abstract void UnloadClipBack();

	public abstract void SetClip(AudioClip[] clips);
	public abstract void SetClip(AudioClip clip);
	
	public abstract void SetClipBack(AudioClip[] clips);
	public abstract void SetClipBack(AudioClip clip);
	
	public abstract void PlayFront();
	public abstract void PlayBack();

	public abstract void StopFront();
	public abstract void StopBack();

	public abstract void PauseFront();
	public abstract void PauseBack();
	
	public abstract float GetVolumeFront();
	public abstract float GetVolumeBack();

	public abstract void SetVolumeFront(float volume);
	public abstract void SetVolumeBack(float volume);

	public abstract string GetClipNameFront();
	public abstract string GetClipNameBack();
	
	public abstract bool IsPlayFront();
	public abstract bool IsPlayBack();
	
	public abstract void SwapBuffer();

	public abstract void ResetFront(float t);
	public abstract void ResetBack(float t);

	public abstract void SetTime(float time);

	/*
	public abstract void Play();	
	public abstract void Stop();
	*/
	
	//kokora hen atode naosu video to awaseru hituyo ari
	public abstract float GetTime();
	public abstract float GetLength();
	public abstract bool IsNearEnd();
}

