using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedFrameControl : MonoBehaviour
{
    List<MeshRenderer> cross_rederers = new List<MeshRenderer>();
    List<MeshRenderer> frame_rederers = new List<MeshRenderer>();

    public AwaysFacingAtPosition facingControl;

    // Use this for initialization

    void Awake()
    {
        MeshRenderer[] renderers = new MeshRenderer[6];
        renderers = this.transform.GetComponentsInChildren<MeshRenderer>();

        facingControl = GetComponent<AwaysFacingAtPosition>();
        Debug.Log("Facing" + facingControl);
        foreach (MeshRenderer renderer in renderers)
        {
            if(renderer.gameObject.name == "RedCrossHorizontal" || renderer.gameObject.name == "RedCrossVertical")
            {
                cross_rederers.Add(renderer);
            }
            else {
                frame_rederers.Add(renderer);
            }
        }

    }

    void Start()
    {
        HideCross();
        HideFrame();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetFacingControl()
    {
        facingControl = GetComponent<AwaysFacingAtPosition>();
    }

    public void MakeFrameOver()
    {
        facingControl.frameLocation = AwaysFacingAtPosition.FrameType.OVER;
    }

    public void MakeFrameNear()
    {
        facingControl.frameLocation = AwaysFacingAtPosition.FrameType.NEAR;
        facingControl.SetInitialHandPosition();
    }


    public void ShowCross()
    {
        foreach (MeshRenderer render in cross_rederers)
        {
            render.enabled = true;
        }
    }
    public void HideCross()
    {
        foreach (MeshRenderer render in cross_rederers)
        {
            render.enabled = false;
        }
    }
    public void ShowFrame()
    {
        foreach (MeshRenderer render in frame_rederers)
        {
            render.enabled = true;
        }

    }
    public void HideFrame()
    {
        foreach (MeshRenderer render in frame_rederers)
        {
            render.enabled = false;
        }
    }

}
