using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FileManagingSpace;

public class DomeVideoInterface : MonoBehaviour {

	public DomeVideoStorage storage; 
	public bool foreground = false;
	public int streamId = 0;

	public bool isLoaded = false;
	public float alpha_param = 0.5f;

	public float duration = 0.0f;
	public float[] elapsedVideoTime;

	protected bool loop = true;

	public virtual bool isLoop
	{
		set{ loop = true;}
		get{ return loop;}
	}

	public virtual bool isPlaying {
		get{ return false;}
	}

	//Audio 
	protected AudioManager AudioCtl;


	public virtual void Awake () {

		AudioCtl = GameObject.Find ("AudioObject").GetComponent("AudioManager") as AudioManager;
		storage = GameObject.Find ("DomeVideoStorage").GetComponent<DomeVideoStorage>();
	}
	// Use this for initialization
	void Start () {
	}

	public virtual IEnumerator WaitUntilVideoLoad(){
		yield return 0;
	}
	public virtual void SetVideo(int id){
	}

	public virtual void SetForeGround(bool PlayVideoOnStart=true)
	{	
	}
	
	public virtual void SetBackGround(bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
	}

	public virtual void GoToStream(int index, bool PlayVideoOnStart=true, bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
	}

	public virtual void GoToStreamBackground(int index, bool ResetVideoOnStop=true, bool PauseVideoOnBackground=true)
	{
	}

	public virtual void SetAlpha(float alpha)
	{
		alpha_param = alpha;
		GetComponent<Renderer>().material.SetFloat ("_Param", alpha_param);
	}

	public virtual void Reset () {
	}

	public virtual void SetPositionFrame (uint frame) {
	}
	
	public virtual uint GetPositionFrame () {
		return 0;
	}


	public virtual void Reset (int id) {
	}

	public virtual void Seek(float second){
	}
	public virtual void SetPlaybackRate(float speed){
	}
	public virtual float GetPlaybackRate(){ return 1.0f;}

	public virtual void Play(){
	}
	public virtual void Stop(){
	}


	public virtual float GetDuration () {
		return 0f;
	}

	public virtual uint GetDurationFrame () {
		return 0;
	}

	public virtual float GetElapsedTime()
	{
		return 0f;
	}

	// Update is called once per frame
	public virtual void Update () {


	}
}