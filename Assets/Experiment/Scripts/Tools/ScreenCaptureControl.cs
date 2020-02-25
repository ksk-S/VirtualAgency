using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCaptureControl : MonoBehaviour
{
    public static string dir = "ScreenshotOutput/";
    
    public static int frameRate = 25;
    public static int sizeMultiplier = 1;

    private static string whole_dir = "";
    private static int startFrameCount = 0;
    private static bool isRecording = false;

    void Start()
    {

        System.IO.Directory.CreateDirectory(dir);
    }

    public static void StartCapture(int videoId)
    {

        Time.captureFramerate = frameRate;
        whole_dir = dir + "/" + "video" + (videoId + 1);
        System.IO.Directory.CreateDirectory(whole_dir);

        startFrameCount = Time.frameCount;

        isRecording = true;
    }


    public static void StopCapture()
    {

        Time.captureFramerate = 0;
        isRecording = false;
    }
    void Update()
    {
        if (isRecording)
        {
            // name is "realFolder/shot 0005.png"
            var name = string.Format("{0}/shot{1:D04}.png", whole_dir, Time.frameCount - startFrameCount);

            ScreenCapture.CaptureScreenshot(name, sizeMultiplier);
        }
    }
}