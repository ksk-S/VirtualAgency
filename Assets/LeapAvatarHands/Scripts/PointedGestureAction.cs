/**
Teleportation functions for use with Leap Motion gestures
Set the rayTransform to be your "pointing at" transform (usually the camera, so you teleport to wherever
you're looking). Then set up the mask to only work on your Floor objects (however you've tagged those).

Set up a Leap gesture or other trigger to fire the TeleportTo() method, in the manner of your choosing.

Author: Ivan Bindoff
*/

using UnityEngine;
using System.Collections;
using Leap.Unity;

namespace LeapAvatarHands
{
    public class PointedGestureAction : MonoBehaviour
    {
        public Transform rayTransform;      //the transform from which the ray is cast forwards (typically camera)
        public LayerMask rayMask;           //the layer that the ray can collide with (typically set to floor layer)
        protected bool canFire = false;        //can we fire right now? i.e. is the rayTransform currently looking at a surface that matches the rayMask.
        public bool requiresLayerMatch = true;  //do we need to be looking/pointing at an appropriate layer to be able to fire the action?
        protected RaycastHit hit;                     //what the ray hit
        protected bool firing = false;
        
        public GameObject indicatorPrefab;  //the prefab that is drawn where you are about to teleport
        protected GameObject indicator;     //the instance of that prefab. Keeps track of it to prevent having excessive Instantiate calls.

        void Awake()
        {
            if (rayTransform == null)
            {
                Debug.Log("Teleporter::Awake::No rayTransform set in the inspector. You SHOULD set this manually. Defaulting to camera!");
                rayTransform = Camera.main.transform;
            }
            if (indicatorPrefab == null)
                Debug.Log("Teleporter::Awake::No teleport indicator prefab assigned in inspector. Player won't be able to see where they're going to teleport to.");
        }
        
        void Update()
        {
            if(rayTransform != null)
            {
                //cast a ray from the ray transform (typically camera)

                if (Physics.Raycast(rayTransform.transform.position, rayTransform.forward, out hit, 100f, rayMask))
                {
                    //if we hit something we are allowed to fire the action
                    canFire = true;
                    if (indicator == null && indicatorPrefab != null)
                    {
                        indicator = GameObject.Instantiate<GameObject>(indicatorPrefab);
                    }
                    if(indicator != null)
                        indicator.transform.position = hit.point;
                }
                else
                {
                    if (requiresLayerMatch)
                        canFire = false;
                    else
                        canFire = true; //when we don't require a layer match, we can fire whenever we please.

                    if(indicator != null)
                        Destroy(indicator);
                }
            }
        }

        /// <summary>
        /// Fires the action
        /// </summary>
        public virtual void Fire()
        {
            if(canFire)
            {
                firing = true;
                
                Debug.Log("teleported");
            }
        }


        /// <summary>
        /// Teleportation actually happens in LateUpdate() to help ensure nothing else can interfere 
        /// with the position during the frame processing procedure.
        /// </summary>
        public void LateUpdate()
        {
            if(firing)
            {
                DoAction();
            }
        }

        /// <summary>
        /// Do the action that this class is responsible for
        /// </summary>
        protected virtual void DoAction()
        {
            Debug.Log("PointedGestureAction::DoAction::Doing an empty action. You must extend the PointedGestureAction class and override the DoAction() method to achieve your desired result. See Teleporter and Shooter classes for examples.");
            firing = false;
        }
    }
}
