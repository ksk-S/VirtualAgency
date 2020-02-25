using UnityEngine;
using System.Collections;

public class LeapHandModifier : MonoBehaviour {

    GameObject LeapHandController;
    // GameObject LeapSpace;
    // GameObject HandSwither;


    public KeyCode IncVolumeKey = KeyCode.F2;
    public KeyCode DecVolumeKey = KeyCode.F1;
    public KeyCode IncSizeKey = KeyCode.F4;
    public KeyCode DecSizeKey = KeyCode.F3;
    public KeyCode SetHandOppositePerspectiveKey = KeyCode.L;
    public KeyCode SetHandTransparentKey = KeyCode.K;

    public float size = 1.0f;
    public float sickness = 0.0f;

    public GameObject[] HandModels;
    public Material[] SkinMaterials;

    public GameObject PhysicalHands;

    public bool isOpposite = false;
    public bool transparent = false;

    CycleHandPairs cycler;

    // Use this for initialization
    void Start () {
        sickness = 0;
        SetVolume(sickness);

        size = 1.0f;
        SetSize(size);

        LeapHandController = GameObject.Find("LeapHandController");
        // LeapSpace = GameObject.Find("LeapSpace");
        // HandSwither = GameObject.Find("HandSwither");
        cycler = GetComponent<CycleHandPairs>();

        //PhysicalHands = GameObject.Find("PhysicsModels");

    }

    public void ResetVolume()
    {

        sickness = 0.0f;
        SetVolume(sickness);

    }

    public void IncVolume()
    {
        sickness += 0.001f;
        SetVolume(sickness);

    }


    public void IncVolume(float tic)
    {
        sickness += tic;
        SetVolume(sickness);

    }

    public void DecVolume()
    {

        if(sickness > -0.006f)
        {
            sickness -= 0.001f;
        }
        SetVolume(sickness);

    }

    public void DecVolume(float tic)
    {

        if (sickness > -0.006f)
        {
            sickness -= tic;
        }
        SetVolume(sickness);

    }
    void SetVolume(float sickness)
    {
        for (int i = 0; i < SkinMaterials.Length; i++)
        {
            SkinMaterials[i].SetFloat("_Amount", sickness);
        }
    }

    public void IncSize()
    {
        size += 0.01f;
        SetSize(size);
    }
    public void DecSize()
    {
        if (size > 0.011f)
        {
            size -= 0.01f;
        }
        SetSize(size);
    }
    public void IncSize(float tic)
    {
        if (size > 0f)
        {
            size += tic;
        }
        SetSize(size);
    }
    public void DecSize(float tic)
    {
        if (size > 0f)
        {
            size -= tic;
        }
        SetSize(size);
    }

    void SetSize(float sze)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name != "CapsuleHand_L" && child.gameObject.name != "CapsuleHand_R")
            {
                child.transform.localScale = new Vector3(size, size, size);
            }
        }
        Leap.Unity.RigidFinger.finger_scale = size;

        //Transform[] trans = PhysicalHands.GetComponentsInChildren<Transform>();
        foreach (Transform child in PhysicalHands.transform)
        {
            child.transform.localScale = new Vector3(size, size, size);
        }


        /*
        Transform [] trans = PhysicalHands.GetComponentsInChildren< Transform > ();
        foreach (Transform child in trans)
        {
           child.transform.localScale = new Vector3(size, size, size);
        }
        */
    }


    public void SetHandOppositePerspective()
    {

      //  LeapHandController.transform.SetParent(HandSwither.transform);
        LeapHandController.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
        LeapHandController.transform.localPosition = new Vector3(0f, 0f, 0.8f);
    }
    public void SetHandOwnPerspective()
    {
      //  LeapHandController.transform.SetParent(LeapSpace.transform);
        LeapHandController.transform.localEulerAngles = new Vector3(270f, 180f, 0f);
        LeapHandController.transform.localPosition = new Vector3(0f, 0f, 0f);

    }

    public void SetHandTransparent()
    {
        cycler.disableAllGroups();
    }

    public void UnsetHandTransparent()
    {
        cycler.CurrentGroup = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(IncVolumeKey))
        {
            IncVolume();
        }

        if (Input.GetKey(DecVolumeKey))
        {
            DecVolume();
        }

        if (Input.GetKey(IncSizeKey))
        {
            IncSize();
        }

        if (Input.GetKey(DecSizeKey))
        {
            DecSize();
        }

        if (Input.GetKeyDown(SetHandOppositePerspectiveKey))
        {
            if (!isOpposite)
            {
                SetHandOppositePerspective();
            }
            else
            {
                SetHandOwnPerspective();
            }
            isOpposite = !isOpposite;
        }

        if (Input.GetKeyDown(SetHandTransparentKey))
        {
            if (!transparent)
            {
                SetHandTransparent();
               //LeapHandController.SetActive(false);
            }
            else
            {
                UnsetHandTransparent();
                //LeapHandController.SetActive(true);
            }
            transparent = !transparent;
        }

    }

}
