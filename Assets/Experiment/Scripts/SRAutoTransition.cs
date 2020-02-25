    using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using System.Text;

namespace FileManagingSpace
{

	public class SRAutoTransition : MonoBehaviour {

		SRController SRCtl;

		public bool isRunning = false;

        int index = 0;
        public float trans_sec = 15f;
        public float change_sec = 2f;

        void Awake(){
			
			SRCtl = GameObject.Find ("SRController").GetComponent<SRController>();

		}
	    // Use this for initialization
		void Start () {

            // Leap.Unity.InputModule.LeapInputModule inputmodule = GameObject.Find("LeapEventSystem").GetComponent<Leap.Unity.InputModule.LeapInputModule>();

            //TODO: ksk
            /*
            for (int i = 0; i < inputmodule.pointerRenderers.Count; i++)
            {
                inputmodule.pointerRenderers[i].enabled = false;
            }
            */


            SRCtl.mergeSec = change_sec;

            isRunning = true;
            StartCoroutine("StartDemo");
            
        }

		public IEnumerator  StartDemo(){

            //yield break;
            while (isRunning)
            {
                yield return new WaitForSeconds(trans_sec);

                index++;
                if(index > 4) { index = 0; }

                SRCtl.MergeStreamWithId(index);
			}

		}

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Hoge");
                if (isRunning)
                {

                    GameObject.Find("AVProContainer").transform.localEulerAngles = new Vector3(0f, 0f, 180f);

                    isRunning = false;
                    StopCoroutine("StartDemo");
                    SRCtl.GoToStream(5);
                }
                else
                {
                    GameObject.Find("AVProContainer").transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                    index = 0;
                    SRCtl.GoToStream(index);
                    isRunning = true;
                    StartCoroutine("StartDemo");

                }

            }

        }
    }


}
