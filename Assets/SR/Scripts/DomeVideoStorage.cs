using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FileManagingSpace;
using System.IO;

public class DomeVideoStorage : MonoBehaviour {
	
	public FileManager files;
	public bool isReady = false;
	public List<MovieTexture> videos = new List<MovieTexture>();
	public int videoNum;
    public int liveNum;

	public List<VideoImageElement> ContentContainer =  new List<VideoImageElement>();

	public AVProWindowsMediaMovie AVProCtl; 

	void Awake () {
		files = GameObject.Find ("FileManager").GetComponent<FileManager>();

	}
	void Start() {
		videoNum = files.streams.Count;

        liveNum = files.live_audios.Count;

		Debug.Log ("videoNum=" + videoNum + " liveNum" + liveNum);
	}

	IEnumerator LoadVideos(){
		foreach(StreamData elem in files.streams)
		{
			yield return StartCoroutine(LoadVideo(elem.video_name));
		}
		isReady = true;
		Debug.Log ("Finish All Video Loading");
		//videoNum = videos.Count;

	}

	IEnumerator LoadVideo(string fileName){
		WWW www = new WWW("file://" + fileName);

		MovieTexture movie =  www.GetMovieTexture();
		while (!movie.isReadyToPlay)
			yield return www;

		string[] parts = fileName.Split('\\');
		movie.name = parts[parts.Length - 1];
		movie.loop = false;
		videos.Add (movie);

		Debug.Log (movie.name);
	}



	IEnumerator LoadAVProVideos(){
        
		foreach(StreamData elem in files.streams)
		{
            if (elem.is_image)
            {
                yield return StartCoroutine(LoadImage(elem.video_name));
            }
            else
            {
                yield return StartCoroutine(LoadAVProVideo(elem.video_name, elem.is_embeded_audio, elem.start_frame));
            }
		}
		isReady = true;
		Debug.Log ("Finish All AVPro Video Loading");
	}
    IEnumerator LoadImage(string fileName)
    {
        Debug.Log(fileName);
        VideoImageElement elem = gameObject.AddComponent<VideoImageElement>();
        elem.isImage = true;


        WWW www = new WWW("file://" + fileName);
        elem.imageTexture = www.texture;
        yield return www;

        ContentContainer.Add(elem);
        yield return 0;
    }
    IEnumerator LoadAVProVideo(string fileName, bool embeded, int start_frame){
        
        VideoImageElement elem = gameObject.AddComponent<VideoImageElement>();
        elem.isImage = false;
        elem.isEmbededAudio = embeded;
        elem.AVProVideo = gameObject.AddComponent<AVProWindowsMediaMovie>();

        ContentContainer.Add (elem);
        elem.AVProVideo._folder = Path.GetDirectoryName(fileName);
        elem.AVProVideo._filename = Path.GetFileName(fileName);;
        elem.AVProVideo._playOnStart = false;
        elem.AVProVideo._loop = true;
        elem.AVProVideo._start_frame = (uint)start_frame;
        elem.AVProVideo.SetPositionFrame((uint)start_frame);
        
        elem.AVProVideo.LoadMovie(false);
		yield return 0;
	}
}
