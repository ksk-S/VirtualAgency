using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
A little example script that shoots a projectile. Triggered by a Leap gesture.

    Author: Ivan Bindoff
    */

namespace LeapAvatarHands
{

    public class Shooter : PointedGestureAction
    {
        public GameObject bulletPrefab;
        public float launchForce;
        
        /// <summary>
        /// Do the action. In this case, shoot a projectile from the rayTransform.
        /// </summary>
        protected override void DoAction()
        {
            Debug.Log("shooting");
            GameObject bullet = GameObject.Instantiate<GameObject>(bulletPrefab);
            bullet.transform.position = rayTransform.position + (rayTransform.forward*0.1f) + (rayTransform.up*0.1f);
            bullet.transform.rotation = rayTransform.rotation;
            bullet.transform.Rotate(90f, 0f, 0f);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(rayTransform.forward*launchForce, ForceMode.Impulse);
            else
                Debug.LogError("Shooter::DoAction::Bullet prefab doesn't have a rigidbody component. Physics won't apply to it...");
            firing = false; //it's fired, so stop firing now.
        }

    }
}
