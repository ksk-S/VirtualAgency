using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour {
    public enum Status
    {
        IDELING = 0, FADEIN, FADEOUT
    }
    public Status status = Status.IDELING;
    public float duration = 0.4f;
    float time = 0f;

    Image CanvasBlackScreenImage;

    // Use this for initialization
    void Awake () {
        CanvasBlackScreenImage = GetComponent<Image>();
    }
	
    public void FadeIn()
    {
        if (CanvasBlackScreenImage.enabled) return;

        time = 0f;
        status = Status.FADEIN;
        CanvasBlackScreenImage.enabled = true;
        CanvasBlackScreenImage.color = new Color(0f, 0f, 0f, 0f);

    }
    public void FadeOut()
    {
        if (!CanvasBlackScreenImage.enabled) return;

        time = 0f;
        status = Status.FADEOUT;
        CanvasBlackScreenImage.color = new Color(0f, 0f, 0f, 1.0f);
    }

    // Update is called once per frame
    void Update () {
	
        switch(status)
        {
            case Status.FADEIN:
                time += Time.deltaTime;
                CanvasBlackScreenImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(0.0f, 1.0f, (time / duration)));
                if (time > duration)
                {
                    status = Status.IDELING;

                }
                break;
            case Status.FADEOUT:
                time += Time.deltaTime;
                CanvasBlackScreenImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(1.0f, 0.0f, (time / duration)));
                if (time > duration)
                {
                    status = Status.IDELING;

                    CanvasBlackScreenImage.enabled = false;
                }
                break;
            
        }
	}
}
