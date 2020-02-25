/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Leap;
using System.Threading;

using System.Linq;

public enum RecorderState
{
    Idling = 0,
    Recording = 1,
}

public enum ReplayState
{
    Idling  = 0,
    Playing = 1,
    Delay   = 2,
    Pause   = 3
}

public static class ObjectExtension
{
    public static T DeepClone<T>(this T src)
    {
        using (var memoryStream = new System.IO.MemoryStream())
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, src);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return (T)binaryFormatter.Deserialize(memoryStream);
        }
    }
}

namespace Leap.Unity
{
    [System.Serializable]
    public class LeapRecorder
    {

        #region Recording Parameters

        public float speed = 1.0f;
        public bool loop = true;
        public RecorderState r_state = RecorderState.Idling;
        public ReplayState p_state = ReplayState.Idling;

        public List<Frame> frameRecordList;
        public List<Frame> frameReplayList;


        public int frame_index_;

        private long real_start_time;
        private long frame_start_time;

        private List<byte[]> frames_;
        private Frame current_frame_ = new Frame();
        #endregion

        #region Delay Parameters
        public long delay_millisec = 300;

        public bool ShowDelaySec = true;
        public float meanDelaySec = 0.0f;
        List<long> delaySecList = new List<long>();
        #endregion

        bool FirstPauseFlag = false;
        Frame PausedFrame;

        #region Deligate
        public delegate Frame UpdateFrame(Frame frame);

        public UpdateFrame updateFrame;
        #endregion

        System.Random rnd;



        #region constructor
        public LeapRecorder()
        {
            ResetRecording();
            ResetReplay();

            rnd = new System.Random(12);

            updateFrame = updateIdeling;
        }
        #endregion

        #region Record Control


        public void ClearRecording()
        {
            frameRecordList = new List<Frame>();
        }

        public void ResetRecording()
        {
            StopRecording();
            frameRecordList = new List<Frame>();

        }

        public void StartRecording()
        {
            r_state = RecorderState.Recording;

            if (p_state == ReplayState.Playing)
            {
                updateFrame -= updatePlayingOnLivePos;

            }
            if (p_state == ReplayState.Delay)
            {
                updateFrame -= updateDelay; ;
            }

            updateFrame += updateRecording;

            if(p_state == ReplayState.Playing)
            {
                updateFrame += updatePlayingOnLivePos;

            }
            if (p_state == ReplayState.Delay)
            {
                updateFrame += updateDelay; ;
            }

        }

        public void StopRecording()
        {
            r_state = RecorderState.Idling;

            updateFrame -= updateRecording;
        }

        public void SetDefault()
        {
            speed = 1.0f;
            loop = true;
        }

        public string FinishAndSaveRecording()
        {
            return "hoge";
            /*
            string path = SaveToNewFile();
            SetRecordFrameToReplay();
            StartReplay();
            return path;
            */
        }

        #endregion

        #region Replay Control
        public void StartReplayWithDelay(float msec)
        {
            CoroutineHandler.StartStaticCoroutine(CoStartReplayWithDelay(msec));
        }
        IEnumerator CoStartReplayWithDelay(float msec)
        {
            Debug.Log("start delayed replay " + msec);
            yield return new WaitForSeconds(msec / 1000f);

            StartReplay();
        }

        public void StartReplaywithPause(long msec)
        {
            p_state = ReplayState.Playing;
            updateFrame -= updateDelay;
            updateFrame -= updatePause;
            updateFrame -= updatePlaying;
            updateFrame -= updatePlayingOnLivePosOnCapsuleHand;
            updateFrame += updatePlayingOnLivePos;  // need to change if it is working

            real_start_time = (long)(Time.time * 1000 + msec);
            frame_start_time = frameReplayList[0].Timestamp;

        }

        public void StartReplay()
        {
            p_state = ReplayState.Playing;
            updateFrame -= updateDelay;
            updateFrame -= updatePause;
            updateFrame -= updatePlayingOnLivePos;
            updateFrame -= updatePlayingOnLivePosOnCapsuleHand;
            updateFrame += updatePlaying;

            real_start_time = (long)(Time.time * 1000);
            frame_start_time = frameReplayList[0].Timestamp;
            //Debug.Log("index:" + frame_index_ + " num frames:" + frameReplayList.Count);
        }


        public void StartReplayOnLivePos()
        {
            p_state = ReplayState.Playing;
            updateFrame -= updateDelay;
            updateFrame -= updatePause;
            updateFrame -= updatePlaying;
            updateFrame -= updatePlayingOnLivePosOnCapsuleHand;
            updateFrame += updatePlayingOnLivePos;

            real_start_time = (long)(Time.time * 1000);
            frame_start_time = frameReplayList[0].Timestamp;
            //Debug.Log("index:" + frame_index_ + " num frames:" + frameReplayList.Count);
        }

        public void StartReplayOnLivePosCapsuleHand()
        {
            p_state = ReplayState.Playing;
            updateFrame -= updateDelay;
            updateFrame -= updatePause;
            updateFrame -= updatePlaying;
            updateFrame -= updatePlayingOnLivePos;
            updateFrame += updatePlayingOnLivePosOnCapsuleHand;

            real_start_time = (long)(Time.time * 1000);
            frame_start_time = frameReplayList[0].Timestamp;
            //Debug.Log("index:" + frame_index_ + " num frames:" + frameReplayList.Count);
        }

        public void StartReplayOnLivePosCapsuleHandwithPause(long msec)
        {
            p_state = ReplayState.Playing;
            updateFrame -= updateDelay;
            updateFrame -= updatePause;
            updateFrame -= updatePlaying;
            updateFrame -= updatePlayingOnLivePos;
            updateFrame += updatePlayingOnLivePosOnCapsuleHand;

            real_start_time = (long)(Time.time * 1000 + msec);
            frame_start_time = frameReplayList[0].Timestamp;
        }


        public void StopReplay()
        {
            PauseReplay();

            frame_index_ = 0;
        }

        public void PauseReplay()
        {
            p_state = ReplayState.Idling;
            updateFrame -= updatePlaying;
            updateFrame -= updatePlayingOnLivePos;
            updateFrame -= updatePlayingOnLivePosOnCapsuleHand;


        }

        public void ResetReplay()
        {
            frameReplayList = new List<Frame>();
            frame_index_ = 0;
        }

        public void SetRecordFrameToReplay()
        {
            frameReplayList = frameRecordList;
        }

        public void SetRecordFrame(List<Frame> input_frames)
        {
            frameReplayList = input_frames;

        }

        #endregion

        #region Delay Control

        public void StartDelay()
        {
            frame_index_ = 0;
            real_start_time = (long)(Time.time * 1000);
            ResetReplay();

            delaySecList = new List<long>();
            meanDelaySec = 0;

            p_state = ReplayState.Delay;
            updateFrame -= updatePlaying;
            updateFrame -= updatePlayingOnLivePos;
            updateFrame -= updatePlayingOnLivePosOnCapsuleHand;
            updateFrame -= updatePause;
            updateFrame += updateDelay;


        }
        public void StopDelay()
        {
            p_state = ReplayState.Idling;
            r_state = RecorderState.Idling;

            updateFrame -= updateDelay;
        }


        public void StartDelayingWithFrame(long delay)
        {
            delay_millisec = delay;
            StartDelay();
        }


        public void SetDelayMillisec(long frame)
        {
            delay_millisec = frame;
        }

        public long GetDelayMillisec()
        {
            return delay_millisec;
        }

        #endregion
        
        #region Pause Control
        public void Pause()
        {
            FirstPauseFlag = true;
            p_state = ReplayState.Pause;

            updateFrame = null;
            updateFrame += updatePause;

        }
        public void UnPause()
        {
            p_state = ReplayState.Idling;
            updateFrame -= updatePause;
        }
        #endregion
        
        #region Frame Processing

        public float GetProgress()
        {
            return (float)frame_index_ / frameReplayList.Count;
        }

        public int GetIndex()
        {
            return frame_index_;
        }


        public void SetIndex(int new_index)
        {
            if (new_index >= frameReplayList.Count)
            {
                frame_index_ = frameReplayList.Count - 1;
            }
            else {
                frame_index_ = new_index;
            }
        }

        public void AddFrame(Frame frame)
        {
            frameRecordList.Add(frame.ShallowCopy());
        }

        public void AddFrameDelay(Frame frame)
        {
            frameReplayList.Add(frame.ShallowCopy());

        }

        public Frame GetCurrentFrame()
        {
            return current_frame_;
        }


        public Frame NextFrame()
        {
            //current_frame_ = new Frame();

            if (frameReplayList.Count > 0)
            {
                if (frame_index_ >= frameReplayList.Count && loop)
                {
                    frame_index_ -= frameReplayList.Count;
                }
                else if (frame_index_ < 0 && loop)
                {
                    frame_index_ += frameReplayList.Count;
                }


                if (frame_index_ < frameReplayList.Count && frame_index_ >= 0)
                {
                    current_frame_ = frameReplayList[(int)frame_index_];

                    long real_time = (long)(Time.time * 1000) - real_start_time;
                    long frame_time = (current_frame_.Timestamp - frame_start_time) / 1000;
                    if (real_time > frame_time)
                    {
                        while (real_time > frame_time)
                        {
                            frame_index_++;
                            if (frame_index_ >= frameReplayList.Count)
                            {
                                if (loop)
                                {
                                    frame_index_ = 0;
                                    real_start_time = (long)(Time.time * 1000);
                                }
                                else
                                {
                                    StopReplay();
                                }
                            }

                            current_frame_ = frameReplayList[(int)frame_index_];
                            frame_time = (current_frame_.Timestamp - frame_start_time) / 1000;
                            real_time = (long)(Time.time * 1000) - real_start_time;
                        };
                    }
                    //Debug.Log("index " + frame_index_ + "real " + real_time + " frame " + frame_time);
                    //Debug.Log("normal: " + frame_index_ + " " + current_frame_.Id);
                }
                else {
                    StopReplay();
                }
            }
            // Debug.Log(frame_index_ + " " + current_frame_.Id);
            return current_frame_;
        }

        public Frame NextFrameDelay()
        {
            // current_frame_ = new Frame();
            if (frameReplayList.Count > 0)
            {
                while (frameReplayList[frameReplayList.Count - 1].Timestamp - frameReplayList[0].Timestamp > delay_millisec * 1000)
                {
                    frameReplayList.RemoveAt(0);
                }
                current_frame_ = frameReplayList[0];
            }
            return current_frame_;
        }

        public List<Frame> GetFrames()
        {
            return frameRecordList;
        }

        public int GetFramesCount()
        {
            return frameRecordList.Count;
        }
        #endregion

        #region Update Frames 
        public Frame updateIdeling(Frame frame)
        {
            return frame;
        }

        public Frame updateRecording(Frame frame)
        {
            AddFrame(frame);
            return frame;
        }

        public Frame updatePlaying(Frame frame)
        {
            return NextFrame();
        }

        public Frame updatePlayingOnLivePos(Frame cur_frame)
        {
            Frame replay_frame = NextFrame();

            Vector LeftPalm = new Vector();
            Vector RightPalm = new Vector();

           // Arm LeftArm = new Arm();
           // Arm RightArm = new Arm();

            foreach (Hand hand in cur_frame.Hands)
            {
                if (hand.IsLeft)
                {
                    LeftPalm = hand.PalmPosition;
                    //LeftArm = hand.Arm.DeepClone();

                }
                else
                {
                    RightPalm = hand.PalmPosition;
                    //RightArm = hand.Arm.DeepClone();
                }


            }

            for (int i = 0; i < replay_frame.Hands.Count; i++)
            {
                if (replay_frame.Hands[i].IsLeft)
                {
                    replay_frame.Hands[i].PalmPosition = LeftPalm;
                    //replay_frame.Hands[i].Arm = LeftArm;

                }
                else
                {
                    replay_frame.Hands[i].PalmPosition = RightPalm;
                    //replay_frame.Hands[i].Arm = RightArm;
                }
            }

            return replay_frame;

        }

        public Frame updatePlayingOnLivePosOnCapsuleHand(Frame cur_frame)
        {
            Frame replay_frame = NextFrame();

            Vector LeftPalm = new Vector();
            Arm LeftArm = new Arm();

            foreach (Hand hand in cur_frame.Hands)
            {
                if (hand.IsLeft)
                {
                    LeftPalm = hand.PalmPosition;
                    LeftArm = hand.Arm.DeepClone();
                }
            }

            for (int i = 0; i < replay_frame.Hands.Count; i++)
            {
                if (replay_frame.Hands[i].IsLeft)
                {
                    Vector diff = replay_frame.Hands[i].PalmPosition - LeftPalm;
                    replay_frame.Hands[i].PalmPosition = LeftPalm;

                    for(int fingerId=0; fingerId < 5; fingerId++)
                    {
                        for(int boneId=0; boneId < 4; boneId++)
                        {
                            Bone b = replay_frame.Hands[i].Fingers[fingerId].Bone((Bone.BoneType)boneId);
                            b.NextJoint -= diff;

                        }
                    }

                    replay_frame.Hands[i].Arm = LeftArm;

                }
            }

            return NextFrame();
        }

        public Frame updateDelay(Frame frame)
        {
            AddFrameDelay(frame);

            if (ShowDelaySec)
            {
                delaySecList.Add(frame.Timestamp - GetCurrentFrame().Timestamp);
                if (delaySecList.Count > 10)
                {
                    meanDelaySec = (float)delaySecList.Average() / 1000f;
                    delaySecList.RemoveAt(0);
                }
            }
            return NextFrameDelay();
        }

        public Frame updatePause(Frame frame)
        {
            if (FirstPauseFlag)
            {
                PausedFrame = frame.ShallowCopy();
                FirstPauseFlag = false;
            }
            return PausedFrame;
        }

        public Frame updateFixedFrame(Frame frame)
        {
            if (p_state == ReplayState.Idling)
            {
                return frame;
            }
            else
            {
                return current_frame_;
            }
        }
        #endregion

        #region Save and Load


        public void SaveIndexFingerListToFile(string path)
        {
            SaveIndexFingerListToFile(frameRecordList, path);
        }

        public void SaveRecordListToFile(string path)
        {
            SaveFramesToFile(frameRecordList, path);

        }
        public void LoadReplayListFromFile(string path)
        {
            frameReplayList = LoadFramesFromFile(path);

            Debug.Log(" Load from File : " + frameReplayList.Count);
        }



        public void SaveFramesToFile(List<Frame> frames, string path)
        {

            if (File.Exists(@path))
            {
                File.Delete(@path);
            }

            using (StreamWriter sw = new StreamWriter(path))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                binaryFormatter.Serialize(sw.BaseStream, frames);

            }
        }


        public void SaveIndexFingerListToFile(List<Frame> frames, string path)
        {

            if (File.Exists(@path))
            {
                File.Delete(@path);
            }

            string text = "id,timestamp,index_x,index_y,index_z,pinky_x,pinky_y,pinky_z" + Environment.NewLine;
            
            foreach (Frame frame in frames)
            {
                text += frame.Id +","+ frame.Timestamp + ",";
                foreach (Hand hand in frame.Hands)
                {
                    if (hand.IsLeft)
                    {
                        foreach (Finger finger in hand.Fingers)
                        {
                            if(finger.Type == Finger.FingerType.TYPE_INDEX)
                            {
                                Leap.Vector pos = finger.TipPosition;
                                text += pos.x +","+pos.y+","+pos.z +",";
                            }
                        }
                        foreach (Finger finger in hand.Fingers)
                        {
                            if (finger.Type == Finger.FingerType.TYPE_PINKY)
                            {
                                Leap.Vector pos = finger.TipPosition;
                                text += pos.x + "," + pos.y + "," + pos.z;
                            }
                        }

                    }
                }

                text += Environment.NewLine;
            }

            File.AppendAllText(path, text);
        }


        public List<Frame> LoadFramesFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("Tracking File does not exist");
                Debug.Log("path:" + path);

                return null;
            }

            using (StreamReader sr = new StreamReader(path))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                
                return (List<Frame>)binaryFormatter.Deserialize(sr.BaseStream);
            }
        }
        #endregion

        /*
        #region randomise space
        public Frame RandomiseFrame(Frame frame)
        {
            Frame randomised = frame.ShallowCopy();

            foreach (Hand hand in frame.Hands)
            {
                if (hand.IsLeft)
                {
                    Vector3 palmPos = hand.PalmPosition.ToVector3();
                    
                    foreach (Finger finger in hand.Fingers)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            int key = getFingerJointIndex((int)finger.Type, j);
                          //  finger.Bone((Leap.Bone.BoneType)j).NextJoint ;

                        }
                    }
                }
            }
            return frame;
        }

        private int getFingerJointIndex(int fingerIndex, int jointIndex)
        {
            return fingerIndex * 4 + jointIndex;
        }

        private void Shuffle<T>(List<T> list)
        {

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion
        */

    }





}
