/**
Positions and rotates the avatar such that it will appropriately correspond to the position of the VR headset.

Author: Ivan Bindoff
*/

using UnityEngine;
using System.Collections;
using Leap.Unity;

namespace LeapAvatarHands
{

    public class RoomScaleVRAvatar : MonoBehaviour
    {
        [Tooltip("The root transform in the hierarchy that this script lives in.")]
        public Transform rootTransform;     //the root transform in the hierarchy that this script lives in
        [Tooltip("The neck transform to synchronize VR headset rotation with. Must be set!")]
        public Transform neckTransform;     //the neck transform to match rotation with
        [Tooltip("The hip transform of the avatar. (Only required for experimental spine bending feature)")]
        public Transform hipTransform;          //the hip transform, we need to stop doing spine bend here.
        [Tooltip("The transform of the camera/HMD we're using to drive the avatar. Defaults to main camera if unset.")]
        public Transform targetTransform;   //i.e. the camera being driven by a VR headset
        [Tooltip("Minimum y rotation the neck can have before it'll start turning the body to compensate")]
        public float minYrotation = -30;          //the minimum y rotation the neck can have before it'll start turning the body to compensate
        [Tooltip("Maximum y rotation the neck can have before it'll start turning the body to compensate")]
        public float maxYrotation = 30;          //the maximum y rotation the neck can have before it'll start turning the body to compensate
        [Tooltip("Speed with which it will try to compensate body rotation")]
        public float compensationSpeed = 150f;   //the speed with which it will try to compensate
        protected float targetYrot = 0f;        //the y rotation we're aiming for
        [Tooltip("Transform that represents the ideal mounting position i.e. where the camera should sit relative to the avatar's head. You must create and set this!")]
        public Transform idealMountingPosition; //the ideal mounting position for the camera to sit on the avatar.

        [HideInInspector]
        [Tooltip("Experimental spine bending feature is not working correctly at this time.")]
        public bool doSpineBending = false;     //enable the experimental lean-over spine bending feature.
        [Tooltip("How far the Y needs to deviate between ideal mount position and camera on the Y axis before deciding to start bending.")]
        public float maxYdifferenceBeforeBend = 0.1f;     //the maximum difference between ideal mount position and camera on the Y axis before we decide to bend the avatar
        public Vector3 maxBendIncrement = new Vector3(30f, 30f, 30f);  //the maximum bend rotation increment you can try per IK pass for each rotation axis
        protected Vector3 solvedBendRotation = Vector3.zero;    //the amount of rotation the IK solver has figured to apply to each spine transform to get us where we want to be

        [Tooltip("Can we move this frame? External scripts can toggle this if they need to take over positioning for a moment.")]
        public bool canMove = true;             //can we move this frame? External scripts can toggle this if they need to take over positioning for a moment.

        [Tooltip("Allow recenter the camera back to ideal mount position with a button press?")]
        public bool buttonRecentersCamera = true;
        [Tooltip("Set the button that recenters the camera")]
        public KeyCode recenterButton = KeyCode.Space;

        [Tooltip("Should we recenter camera when user puts their HMD on?")]
        public bool recenterOnUserPresence = true;

        protected bool _lastUserPresence = false;
        
        

        public void StopMovementForOneFrame()
        {
            canMove = false;
        }

        protected void Awake()
        {
            if (neckTransform == null)
            {
                Debug.LogWarning(gameObject.name + "::MatchRotation::Awake::Neck transform has not been explicitly set, defaulting to transform. This is ok only if this script is placed on the neck transform.");
                neckTransform = transform;
            }
            if(rootTransform == null)
            {
                rootTransform = neckTransform.root;
            }
            if(targetTransform == null)
            {
                Debug.LogWarning(gameObject.name + "::MatchRotation::Awake:: No targetTransform set. Defaulting to Camera.main");
                targetTransform = Camera.main.transform;
            }
            if (idealMountingPosition == null)
                Debug.LogError(gameObject.name + "::MatchRotation::Awake:: No idealMountingPosition set. You need to create a transform as a child of the avatar's head that is positioned where the VR camera should ideally be mounted, and set this in the inspector.");
            else
            {
                RecenterCamera();
            }
            if (hipTransform == null && doSpineBending)
                Debug.LogWarning(gameObject.name + "::MatchRotation::Awake:: Hip transform isn't set, this is ok as long as your hip transform has 'Hips' in the name. May cause issues with spine bending algorithm otherwise.");

            if (targetTransform == null)
                targetTransform = Camera.main.transform;
        }

        void Update()
        {
            if(doSpineBending)
                DoSpineBend();

            DoNeckRotation();

            DoBodyRotation();

            DoRepositioning();

            RecenterOnUserPresence();
        }

        

        void LateUpdate()
        {
            if(buttonRecentersCamera && Input.GetKeyDown(recenterButton))
            {
                RecenterCamera();   //particularly useful for if you launch editor while sitting down or headset on table, then stand up...
            }
        }

        /// <summary>
        /// figure out where the avatar needs to be positioned to make the camera in the right place
        /// </summary>
        protected void DoRepositioning()
        {
            Vector3 difference = idealMountingPosition.transform.position - targetTransform.position;

            //physically move the avatar to adjust for the difference
            if (canMove)
                rootTransform.position = new Vector3(rootTransform.position.x - difference.x, rootTransform.position.y, rootTransform.position.z - difference.z);
            else
                canMove = true;
        }

        /// <summary>
        /// do recentering on user presence detected
        /// </summary>
        protected void RecenterOnUserPresence()
        {
            if (recenterOnUserPresence)
            {
#if UNITY_2017_2_OR_NEWER
                var userPresence = UnityEngine.XR.XRDevice.userPresence == UnityEngine.XR.UserPresenceState.Present;
#else
                var userPresence = UnityEngine.VR.XRDevice.userPresence == UnityEngine.VR.UserPresenceState.Present;
#endif

                if (_lastUserPresence != userPresence)
                {
                    if (userPresence)
                    {
                        //Debug.Log("resetting on presence");
                        StartCoroutine(RecenterCamera());
                    }
                    _lastUserPresence = userPresence;
                }

            }
        }

        /// <summary>
        /// Turn neck transform to look in the same direction as the VR headset
        /// </summary>
        protected void DoNeckRotation()
        {
            if (targetTransform != null)
            {
                neckTransform.rotation = Quaternion.LookRotation(targetTransform.forward, Vector3.up);
            }
        }

        /// <summary>
        /// If the neck is too far turned, turn the body instead
        /// </summary>
        protected void DoBodyRotation()
        {
            float normalizedYangle = neckTransform.localRotation.eulerAngles.y;
            if (normalizedYangle > 180f)
                normalizedYangle -= 360f;
            if (normalizedYangle < -180f)
                normalizedYangle += 360f;

            if (normalizedYangle < minYrotation)
            {
                //Debug.Log("too small");
                targetYrot = targetTransform.rotation.eulerAngles.y;
                rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.Euler(0, targetYrot, 0), Time.deltaTime * compensationSpeed);
            }

            if (normalizedYangle > maxYrotation)
            {
                //Debug.Log("too big");
                targetYrot = targetTransform.rotation.eulerAngles.y;
                rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.Euler(0, targetYrot, 0), Time.deltaTime * compensationSpeed);
            }
        }

        /// <summary>
        /// Calculates then applies the spine bend factor required to get the camera to be at the right y height, if any.
        /// 
        /// NOTE: Currently not working as the hands get very stretched wrists. I think this is because the IK pass totally
        /// fails to account for the bent spine. Possibly it doesn't know the spine is bent when it occurs, or possibly it cannot
        /// account for the runtime alteration to the spine bones....
        /// </summary>
        protected void DoSpineBend()
        {
            //if the Y difference is too big then the player is presumably leaning over
            //so let's do a bit of fudgey on the spine to get the difference shrunk
            //if there's still a difference, it will keep adding bend factor each frame
            Vector3 difference = idealMountingPosition.transform.position - targetTransform.position;
            if (difference.y > maxYdifferenceBeforeBend)
                solvedBendRotation = new Vector3(solvedBendRotation.x + (Time.deltaTime * maxBendIncrement.x), 0, 0);
            else if (difference.y < 0 && solvedBendRotation.x > 0)   //if it's gone the other way, unbend
                solvedBendRotation = new Vector3(solvedBendRotation.x - (Time.deltaTime * maxBendIncrement.x), 0, 0);

            if (doSpineBending && solvedBendRotation != Vector3.zero)
                ApplyRotationToSpine(solvedBendRotation);
        }
        
        /// <summary>
        /// Applies the spine bending effect.
        /// </summary>
        /// <param name="chosenRotation"></param>
        protected void ApplyRotationToSpine(Vector3 chosenRotation)
        {
            //traverse all the way back down from this neck transform to hips transform
            Transform current = neckTransform.parent;
            while (current != neckTransform.root && !current.name.Contains("Hips") && current != hipTransform)
            {
                current.transform.localRotation = Quaternion.Euler(chosenRotation);
                current = current.parent;
            }
        }

        /// <summary>
        /// Resets the camera to be in the ideal mounting position.
        /// </summary>
        IEnumerator RecenterCamera()
        {
            yield return new WaitForSeconds(0.1f);
            targetTransform.position = idealMountingPosition.position;
            targetTransform.rotation = idealMountingPosition.rotation;
#if UNITY_2017_2_OR_NEWER
            UnityEngine.XR.InputTracking.Recenter();
#else
            UnityEngine.VR.InputTracking.Recenter();
#endif
        }
    }
}
