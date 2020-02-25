using UnityEngine;
using System.Collections;

public class PatchManager : MonoBehaviour {

	GameObject mask;

	public Vector2 min_range = new Vector2(10f, 0f);
	public Vector2 max_range = new Vector2(30f, 20f);
	public GameObject OVRCentreAnchor;

	void Awake() {
		GameObject TrackingSpaceStream = GameObject.Find ("OVRCameraRig").transform.Find ("TrackingSpace").gameObject;
		OVRCentreAnchor = TrackingSpaceStream.transform.Find ("CenterEyeAnchor").gameObject;
	}
	// Use this for initialization
	void Start () {
		mask = GameObject.Find ("Mask");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.H) ) {

			float x = UnityEngine.Random.Range (-max_range.x, max_range.x);
			float y = UnityEngine.Random.Range (-max_range.y, max_range.y);

			SetPatchRandomInSight (x, y);
		}

		if (Input.GetKeyDown (KeyCode.N) ) {
			SetPatchInCentreCameraRotation();
		}
	}

	public void MovePatchVertical(float vertical)
	{
		mask.transform.RotateAround(transform.position, Vector3.right, vertical);
	}
	public void MovePatchHorizontal(float horizontal)
	{
		mask.transform.RotateAround (transform.position, Vector3.up, horizontal);
	}

	
	public void SetPatchRandomInSight(float x, float y){
		SetPatchInCentreCameraRotation ();

		MovePatchVertical (y);
		MovePatchHorizontal (x);
		mask.GetComponent<Renderer>().enabled = true;
	}

	public void RemovePatch(){
		mask.GetComponent<Renderer>().enabled = false;
	}

	
	public void SetPatchInCentreCameraRotation(){


		Vector3 rotation =OVRCentreAnchor.transform.localRotation.eulerAngles;
		
		mask.transform.localPosition = new Vector3 (0, 0, 20);
		MovePatchVertical (rotation.x);
		MovePatchHorizontal (rotation.y);
	}


	public void SetPatchInCentreOculusSensor(){

		Vector3 rotation = GetHeadOrientation ();

		mask.transform.localPosition = new Vector3 (0, 0, 20);
		MovePatchVertical (rotation.x);
		MovePatchHorizontal (rotation.y);
	}

	public Vector3 GetHeadOrientation(){

        //OVRPose pose = OVRManager.display.GetHeadPose(0.0); 
        //Quaternion rotation = pose.orientation;
        //return rotation.eulerAngles;
        return new Vector3();
    }
}