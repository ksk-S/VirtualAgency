using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fader : MonoBehaviour {
    public enum Status
    {
        IDELING = 0, FADEIN, FADEOUT
    }
    public Status status = Status.IDELING;
    public float duration = 0.4f;
    float time = 0f;

    CanvasGroup canvasGroup;

    // Use this for initialization
    void Start () {
        canvasGroup = GetComponent<CanvasGroup>();

     
    }
	
    public void FadeIn()
    {
        if (gameObject.activeSelf) return;

        time = 0f;
        status = Status.FADEIN;

        gameObject.SetActive(true);
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>(); 
        }
       
        canvasGroup.alpha = 0.0f;
        
    }
    public void FadeOut()
    {
        if (!gameObject.activeSelf) return;

        time = 0f;
        status = Status.FADEOUT;

    }

    // Update is called once per frame
    void Update () {
	
        switch(status)
        {
            case Status.FADEOUT:
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, (time/ duration));
                if (time > duration)
                {
                    gameObject.SetActive(false);
                    status = Status.IDELING;
                }
                break;
            case Status.FADEIN:
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, (time / duration));
                if (time > duration)
                {
                    status = Status.IDELING;
                }
                break;
        }
	}
}
