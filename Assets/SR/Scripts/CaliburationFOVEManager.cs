using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonUtil;
using RenderingAPI;

public class CaliburationFOVEManager : CaliburationManager
{

    GameObject CalibrationScreen;

    Vector3 HorizontalInitPos = new Vector3(30f, 0, 30f);
    Vector3 VerticalInitPos   = new Vector3(0f, 35f, 30f);

    float HorizontalMax = 60f;
    float HorizontalMin = -60f;

    float VerticalMax = 70f;
    float VerticalMin = -70f;

    protected override void Awake()
    {
        base.Awake();

        CalibrationScreen = GameObject.Find("CalibrationScreen");
        CalibrationScreen.SetActive(false);
    }

    public override void SetHorizontalSplitScreenes()
	{
        CalibrationScreen.SetActive(true);
        CalibrationScreen.transform.localPosition = HorizontalInitPos;
	}

    public override void SetVerticalSplitScreenes()
    {

        CalibrationScreen.SetActive(true);
        CalibrationScreen.transform.localPosition = VerticalInitPos;
    }

    public override void RevertSplitScreenes()
    {

        CalibrationScreen.SetActive(false);
    }

    public override void ControlCaliburationUp()
    {

        if (caliburationMode == CalibrationMode.Horizontal){
            
            float x = CalibrationScreen.transform.localPosition.x;
            if (x < HorizontalMax)
            {
                CalibrationScreen.transform.localPosition = new Vector3(x + 1.0f, 0, 30f);

            }
        }
        if (caliburationMode == CalibrationMode.Vertical)
        {
            float y = CalibrationScreen.transform.localPosition.y;
            if (y < VerticalMax)
            {
                CalibrationScreen.transform.localPosition = new Vector3(0, y+1.0f, 30f);

            }
        }

        if (caliburationMode == CalibrationMode.Overlay)
        {
            cariburation_alpha_ratio += 0.01f;
            if (cariburation_alpha_ratio > 1.0f) cariburation_alpha_ratio = 1.0f;
            SetTransparentScreen();
        }
    }
    protected override void ControlCaliburationDown()
    {
        if (caliburationMode == CalibrationMode.Horizontal)
        {
            float x = CalibrationScreen.transform.localPosition.x;
            if (x > HorizontalMin)
            {
                CalibrationScreen.transform.localPosition = new Vector3(x - 1.0f, 0, 30f);

            }
        }


        if (caliburationMode == CalibrationMode.Vertical)
        {
            float y = CalibrationScreen.transform.localPosition.y;
            if (y > VerticalMin)
            {
                CalibrationScreen.transform.localPosition = new Vector3(0, y - 1.0f, 30f);

            }
        }

        if (caliburationMode == CalibrationMode.Overlay)
        {
            cariburation_alpha_ratio -= 0.01f;
            if (cariburation_alpha_ratio < 0.0f) cariburation_alpha_ratio = 0.0f;
            SetTransparentScreen();
        }
    }



}
