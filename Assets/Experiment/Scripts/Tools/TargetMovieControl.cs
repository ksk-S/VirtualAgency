using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class TargetMovieControl : MonoBehaviour {

    public int num_images = 5;

    VideoPlayer videoPlayer;
    RawImage rawImage;

    RawImage videoImage;

    public List<VideoClip> videoClips = new List<VideoClip>();
    public List<Texture> images = new List<Texture>();


    private void Awake()
    {
        videoPlayer = GameObject.Find("VideoPanel").GetComponent<VideoPlayer>();
        videoImage = GameObject.Find("VideoPanel").GetComponent<RawImage>();
        rawImage = GameObject.Find("ImagePanel").GetComponent<RawImage>();
    }



    // Use this for initialization
    void Start () {

        for(int i=1; i<= num_images; i++)
        {
            videoClips.Add(Resources.Load<VideoClip>("TargetMovements/Movies/L" + i));
            images.Add(Resources.Load<Texture>("TargetMovements/Images/L" + i));


        }

        UnityEngine.Debug.Log("Loaded Image/Video: num video" + videoClips.Count);
        videoImage.enabled = false;
        rawImage.enabled = false;
    }



    public void PlayImage(int index)
    {
        rawImage.texture = images[index];
        ShowImagePanel();
    }


    public void PlayVideo(int index)
    {
        videoPlayer.Stop();

        videoPlayer.clip = videoClips[index];;
        ShowVideoPanel();

        videoPlayer.Play();
    }

    void ShowImagePanel()
    {
        videoImage.enabled = false;
        rawImage.enabled = true;
    }

    void ShowVideoPanel()
    {
        videoImage.enabled = true;
        rawImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
