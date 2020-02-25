using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

using Leap;

namespace DurationJudgement
{
    [System.Serializable]
    public class TrackingManager
    {

        public string result_dir = "Results/";
        public string data_dir;

        public string defaultMovementDir = "DefaultMovements/";
        protected List<List<Frame>>[,] RecordedFrames = new List<List<Frame>>[2, 20];

        public int num_samples = 2; 

        public void SetDirForMain(int subId, DurationJudgement.AbstractMenuManager.ExpSession sessionId, int blockId)
        {
            DateTime dt = DateTime.Now;
            string now = string.Format("-{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            //data_dir = result_dir + "Tracking/" + subId.ToString() + "/" + sessionId.ToString() + "-" + blockId + now + "/";

            data_dir = result_dir + "/" + subId.ToString() + "/TRACKING-block" + blockId + now + "/";

            Directory.CreateDirectory(data_dir + "Raw/");
            Directory.CreateDirectory(data_dir + "Finger/");
        }

        public void SetDirForPractice(int subId)
        {
            DateTime dt = DateTime.Now;
            string now = string.Format("-{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            data_dir  = result_dir + "Tracking/" + subId.ToString() + "/Practice" +  now + "/";

            Directory.CreateDirectory(data_dir);

            Directory.CreateDirectory(data_dir + DataManager.PerspectiveType.SELF.ToString() + "/");
            Directory.CreateDirectory(data_dir + DataManager.PerspectiveType.OTHER.ToString() + "/");
        }

        public void SaveTrackingPractice(Leap.Unity.LeapRecorder leapRecorder, DataManager.PerspectiveType perspectiveType, int targetActionId, int index)
        {
            CoroutineHandler.StartStaticCoroutine(SaveTrackingPracticeCoroutine(leapRecorder, perspectiveType, targetActionId, index));
        }

        public void SaveTrackingMain(Leap.Unity.LeapRecorder leapRecorder, int trialId, DataManager.PerspectiveType perspectiveType, DataManager.ReplayType replayType, DataManager.ActionyType actionType, int targetActionId)
        {
            CoroutineHandler.StartStaticCoroutine(SaveTrackingMainCoroutine(leapRecorder, trialId, perspectiveType, replayType, actionType, targetActionId));
        }

        IEnumerator SaveTrackingPracticeCoroutine(Leap.Unity.LeapRecorder leapRecorder, DataManager.PerspectiveType perspectiveType, int targetActionId, int index)
        {
            string trackingDir = data_dir + "/" + perspectiveType.ToString() + "/action" + targetActionId + "/";
            Directory.CreateDirectory(trackingDir);

            string filename = trackingDir + "track" + index + ".txt";

            leapRecorder.SaveRecordListToFile(filename);

            leapRecorder.ResetRecording();

            yield return null;
        }

        IEnumerator SaveTrackingMainCoroutine(Leap.Unity.LeapRecorder leapRecorder, int trialId, DataManager.PerspectiveType perspectiveType, DataManager.ReplayType replayType, DataManager.ActionyType actionType, int targetActionId)
        {
            string filename = data_dir + "Raw/" +"trial" + trialId + "-raw-" + actionType.ToString() + "-" + replayType.ToString() + "-v" + targetActionId + ".txt";
            leapRecorder.SaveRecordListToFile(filename);

            string filename2 = data_dir + "Finger/" + "trial" + trialId + "-finger-" + actionType.ToString() + "-" + replayType.ToString() + "-v" + targetActionId + ".txt";
            leapRecorder.SaveIndexFingerListToFile(filename2);

            leapRecorder.ResetRecording();

            yield return null;
        }


        public void LoadRecordedMovemnts(Leap.Unity.LeapRecorder leapRecorder, int num_videos)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < num_videos; j++)
                {
                    RecordedFrames[i, j] = new List<List<Frame>>();
                    for (int k = 0; k < num_samples; k++)
                    {
                        string dir = defaultMovementDir + ((DataManager.PerspectiveType)i).ToString() + "/" + "action" + j + "/";
                        string filename = dir + "track" + k + ".txt";
                        List<Frame> frames = leapRecorder.LoadFramesFromFile(filename);
                        RecordedFrames[i, j].Add(frames);
                    }
                    UnityEngine.Debug.Log("Load Default Movement: pers=" + i + " action" + j + " " + RecordedFrames[i, j].Count);
                }
            }
        }

        public void SetDefaultMovement(Leap.Unity.LeapRecorder leapRecorder, int perspectiveType, int targetActionId, int index)
        {
            UnityEngine.Debug.Log("Set Default Action:" + perspectiveType + " " + targetActionId + " " + index);
            leapRecorder.ResetReplay();
            leapRecorder.SetRecordFrame(RecordedFrames[perspectiveType, targetActionId][index]);
        }


    }
}