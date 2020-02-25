using UnityEngine;
using System.Collections;

using Leap;


namespace Leap.Unity
{
    public class RecordingControls : MonoBehaviour
    {
        public KeyCode startRecord = KeyCode.R;
        public KeyCode stopRecord = KeyCode.S;
        public KeyCode startReplay = KeyCode.P;
        public KeyCode startReplayOnLivePos = KeyCode.I;
        public KeyCode startReplayOnLivePosCapsuleHand = KeyCode.U;
        public KeyCode stopReplay = KeyCode.O;
        public KeyCode SaveToFile = KeyCode.T;
        public KeyCode LoadFromFile = KeyCode.L;
        public KeyCode resetRecording = KeyCode.X;
        public KeyCode delayToggle = KeyCode.Y;
        public KeyCode pauseToggle = KeyCode.Z;
        public KeyCode startRandom = KeyCode.C;
        public KeyCode stopRandom = KeyCode.V;

        LeapRecorder leapRecorder;
        CapsuleHand capsuleHand;

        void Start()
        {
            Transform[] trs = GameObject.Find("LeapHandController").GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trs)
            {
                if (t.name == "CapsuleHand_L")
                {
                    capsuleHand =  t.gameObject.GetComponent<CapsuleHand>();
                }
            }
            Debug.Log(capsuleHand);

            if (this.name == "LeapHandController")
            {
                leapRecorder = GetComponent<HandModelManager>().recorder_;
            }
            else { 

                leapRecorder = GetComponent<LeapAvatarHands.IKOrionLeapHandController>().recorder_;
            }
        }

        void Update()
        {

            if (Input.GetKeyDown(startRecord))
            {
                leapRecorder.StartRecording();
            }

            if (Input.GetKeyDown(stopRecord))
            {
                leapRecorder.StopRecording();
                Debug.Log("Record :" + " num frames:" + leapRecorder.frameRecordList.Count);

                leapRecorder.SetRecordFrameToReplay();
                Debug.Log("Replay index:" + leapRecorder.frame_index_ + " num frames:" + leapRecorder.frameRecordList.Count);


            }

            if (Input.GetKeyDown(startReplay))
            {
                Debug.Log("Replay start");
                leapRecorder.StartReplay();
            }

            if (Input.GetKeyDown(startReplayOnLivePos))
            {
                Debug.Log("Replay on Live Pos start");
                leapRecorder.StartReplayOnLivePos();
            }

            if (Input.GetKeyDown(startReplayOnLivePosCapsuleHand))
            {
                Debug.Log("Replay on Live Pos start");
                leapRecorder.StartReplayOnLivePosCapsuleHand();
            }

            if (Input.GetKeyDown(stopReplay))
            {
                Debug.Log("Replay stop");
                leapRecorder.StopReplay();
            }

            if (Input.GetKeyDown(SaveToFile))
            {
                leapRecorder.SaveRecordListToFile("testRecord.txt");
            }


            if (Input.GetKeyDown(LoadFromFile))
            {
                leapRecorder.LoadReplayListFromFile("testRecord.txt");
            }


            if (Input.GetKeyDown(resetRecording))
                leapRecorder.ResetRecording();

            if (Input.GetKeyDown(delayToggle))
            {
                if (leapRecorder.p_state == ReplayState.Delay)
                {
                    leapRecorder.StopDelay();
                }
                else
                {
                    leapRecorder.StartDelay();
                }
            }
            if (Input.GetKeyDown(pauseToggle))
            {
                if (leapRecorder.p_state == ReplayState.Pause)
                {
                    leapRecorder.UnPause();
                }
                else
                {
                    leapRecorder.Pause();
                }

            }

            if (Input.GetKeyDown(startRandom))
            {
                if (!capsuleHand.randomHelper.isRandomised)
                {
                    capsuleHand.StartRandomise();
                }
            }

            if (Input.GetKeyDown(stopRandom))
            {
                capsuleHand.StopRandomise();


            }

        }
    }
}