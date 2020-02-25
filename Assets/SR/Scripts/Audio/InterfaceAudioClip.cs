using UnityEngine;
using System.Collections;

public abstract class InterfaceAudioClip : MonoBehaviour
{
	public InterfaceAudioSourceBuffer audioSourceBuffer;
	
	public bool is_multiple;
	public bool is_ready_to_play = false;

	// Use this for initialization
	void Start ()
	{
	}
	// Update is called once per frame
	void Update ()
	{
	}

	
	
	public abstract void UnloadClip();
	public abstract bool isReadyToPlay();
	
	public abstract void SetAudioFile(string filename, bool is_ogg);
	
	public abstract void SetAudioSource(InterfaceAudioSourceBuffer buffer);
	
	public abstract void SetClip();
	public abstract void SetClipBack();
	
	public abstract void Play();
	public abstract void Stop();
	public abstract void Reset();
	
	public abstract float GetTime();
	public abstract float GetLength();
	public abstract bool IsNearEnd();

}

