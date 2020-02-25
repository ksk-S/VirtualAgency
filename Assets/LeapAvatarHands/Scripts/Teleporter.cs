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
    public class Teleporter : PointedGestureAction
    {
        public bool doRotation = true;      //for room scale you will probably want to turn this off, because rotation will be driven by the rotation of the VR headset
        
        /// <summary>
        /// Do the action that this class is responsible for
        /// In this case, teleport.
        /// </summary>
        protected override void DoAction()
        {
            //rotate
            if (doRotation)
                transform.rotation = Quaternion.LookRotation(hit.point - transform.position, Vector3.up);

            //teleport
            transform.position = hit.point;
            firing = false;
        }
    }
}
