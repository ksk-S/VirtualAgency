using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DurationJudgement
{

    [System.Serializable]
    public class DataManager 
    {
        
        public static int num_sessions = Enum.GetNames(typeof(AbstractMenuManager.ExpSession)).Length;

        public int nonce = 20000;

        public int numPractice = 10;
        public int numTraining = 10;
        public int numTrial = 20;
        public int numTargetAction = 10;

        public int num_blocks = 8;

        public System.Random rnd;


        string out_filename;


        public enum PerspectiveType
        {
            SELF, OTHER
        }

        public enum ReplayType
        {
            LIVE, REPLAY
        }

        public enum ActionyType
        {
            ACTION, NO_ACTION
        }

        public enum RandomType
        {
            NORMAL, RANDOM
        }

        public enum FrameType
        {
            OVER, NEAR
        }


        public long[] DelayArray = { 0, 250, 500 };

        public long[] DurationArray = { 1800, 2000, 2200 };

        public long[] TrainingDurations = { 1600, 1800, 2000, 2200, 2400 };

        [System.Serializable]
        public struct Result
        {
            public int sessionId;
            public int index;
            public int blockId;
            public int trialId;
            public ReplayType replayType;
            public ActionyType actionType;
            public PerspectiveType perspectiveType;
            public RandomType randomType;
            public FrameType frameType;
            public int delayId;
            public int targetActionId;
            public int durationId;
            public int actual_duration;
            public int repoted_duration;
            public float control;
            public float authorship;
            public float averageDelay;
            public float averageFPS;

        }

        //public List<List<Result>>[] ResultLists = new List<List<Result>>[num_sessions];
        
        public struct ResultListStruct{
            public List<Result> Practice;
            public List<Result> Training;
            public List<List<Result>> Main;
        }

        public ResultListStruct ResultLists;


        #region constructor
        public DataManager()
        {
            
        }
        #endregion


        // Use this for initialization
        public void CreateBlockConditions()
        {

            ResultLists.Main = new List<List<Result>>();
            for (int perspectiveId = 0; perspectiveId < 2; perspectiveId++)
            {
                for (int replayId = 0; replayId < 2; replayId++)
                {
                    for (int delayId = 0; delayId < DelayArray.Length; delayId++)
                    {
                        List<Result> blockedResult = new List<Result>();
                        int index = 0;
                        for (int targetId = 0; targetId < numTargetAction; targetId++)
                        {
                            for (int durationId = 0; durationId < DurationArray.Length; durationId++)
                            {
                                Result param = new Result();
                                param.sessionId = (int)AbstractMenuManager.ExpSession.MAIN;
                                param.index = index++;
                                param.perspectiveType = (PerspectiveType)perspectiveId;//  (PerspectiveType)0; //
                                param.replayType =  (ReplayType)replayId;// (ReplayType)1;  
                                param.delayId = delayId;
                                param.durationId = durationId;
                                param.targetActionId = targetId;
                                param.randomType = RandomType.NORMAL;
                                param.frameType = FrameType.OVER;
                                param.control = -1;
                                param.authorship = -1;

                                blockedResult.Add(param);
                            }
                        }
                        Shuffle(blockedResult);

                        int num_one_block = numTargetAction * DurationArray.Length / 3;
                        //Debug.Log("num trial per block" + num_one_block);
                        //Debug.Log("num trials in one condition" + blockedResult.Count());

                        for (int i =0; i<3; i++)
                        {
                            List<Result> SubList = blockedResult.GetRange(i * num_one_block, num_one_block);
                            ResultLists.Main.Add(SubList);
                            //Debug.Log((i * num_one_block) + " " + num_one_block + " " + SubList.Count());
                        }


                    }
                }
            }
            Shuffle(ResultLists.Main);

            Debug.Log("Blocked param List num_block: " + ResultLists.Main.Count() + "  num_trials:" + ResultLists.Main[0].Count());

        }

        // Use this for initialization
        public void CreateBlockNoActionConditions()
        {
            ResultLists.Main = new List<List<Result>>();
            for (int perspectiveId = 0; perspectiveId < 2; perspectiveId++)
            {
                for (int replayId = 0; replayId < 2; replayId++)
                {
                    for (int actionId = 0; actionId < 2; actionId++)
                    {
                        List<Result> blockedResult = new List<Result>();
                        int index = 0;
                        for (int targetId = 0; targetId < numTargetAction; targetId++)
                        {
                            for (int durationId = 0; durationId < DurationArray.Length; durationId++)
                            {
                                Result param = new Result();
                                param.sessionId = (int)AbstractMenuManager.ExpSession.MAIN;
                                param.index = index++;
                                param.perspectiveType = (PerspectiveType)perspectiveId;//  (PerspectiveType)0; //
                                param.replayType = (ReplayType)replayId;// (ActionType)1;  
                                param.actionType = (ActionyType)actionId;
                                param.randomType = RandomType.NORMAL;
                                param.frameType = FrameType.OVER;
                                param.durationId = durationId;
                                param.targetActionId = targetId;
                                param.control = -1;
                                param.authorship = -1;

                                blockedResult.Add(param);
                            }
                        }
                        Shuffle(blockedResult);

                        int num_one_block = numTargetAction * DurationArray.Length / 3;
                        //Debug.Log("num trial per block" + num_one_block);
                        //Debug.Log("num trials in one condition" + blockedResult.Count());

                        for (int i = 0; i < 3; i++)
                        {
                            List<Result> SubList = blockedResult.GetRange(i * num_one_block, num_one_block);
                            ResultLists.Main.Add(SubList);
                            //Debug.Log((i * num_one_block) + " " + num_one_block + " " + SubList.Count());
                        }


                    }
                }
            }
            Shuffle(ResultLists.Main);

            Debug.Log("Blocked param List num_block: " + ResultLists.Main.Count() + "  num_trials:" + ResultLists.Main[0].Count());

        }

        // Use this for initialization
        public void CreateSpacialRandomConditions()
        {
            ResultLists.Main = new List<List<Result>>();

            for (int replayId = 0; replayId < 2; replayId++)
            {
                for (int randomId = 0; randomId < 2; randomId++)
                {
                    List<Result> blockedResult = new List<Result>();
                    int index = 0;
                    for (int targetId = 0; targetId < numTargetAction; targetId++)
                    {
                        for (int durationId = 0; durationId < DurationArray.Length; durationId++)
                        {
                            Result param = new Result();
                            param.sessionId = (int)AbstractMenuManager.ExpSession.MAIN;
                            param.index = index++;
                            param.perspectiveType = PerspectiveType.SELF;
                            param.actionType = ActionyType.ACTION;
                            param.frameType = FrameType.OVER;
                            param.replayType = (ReplayType)replayId;
                            param.randomType = (RandomType)randomId;
                            param.durationId = durationId;
                            param.targetActionId = targetId;
                            param.control = -1;
                            param.authorship = -1;

                            blockedResult.Add(param);
                        }
                    }
                    Shuffle(blockedResult);

                    int num_one_block = numTargetAction * DurationArray.Length / 3;
                    //Debug.Log("num trial per block" + num_one_block);
                    //Debug.Log("num trials in one condition" + blockedResult.Count());

                    for (int i = 0; i < 3; i++)
                    {
                        List<Result> SubList = blockedResult.GetRange(i * num_one_block, num_one_block);
                        ResultLists.Main.Add(SubList);
                        //Debug.Log((i * num_one_block) + " " + num_one_block + " " + SubList.Count());
                    }


                }
            }
            Shuffle(ResultLists.Main);

            for (int i = 0; i < ResultLists.Main.Count; i++)
            {
                Debug.Log(ResultLists.Main[i][0].replayType + " " + ResultLists.Main[i][0].randomType);
            }
            Debug.Log("Blocked param List num_block: " + ResultLists.Main.Count() + "  num_trials:" + ResultLists.Main[0].Count());

        }

        // Use this for initialization
        public void CreateFrameConditions()
        {
            ResultLists.Main = new List<List<Result>>();

            for (int replayId = 0; replayId < 2; replayId++)
            {
                for (int frameId = 0; frameId < 2; frameId++)
                {
                    List<Result> blockedResult = new List<Result>();
                    int index = 0;
                    for (int targetId = 0; targetId < numTargetAction; targetId++)
                    {
                        for (int durationId = 0; durationId < DurationArray.Length; durationId++)
                        {
                            Result param = new Result();
                            param.sessionId = (int)AbstractMenuManager.ExpSession.MAIN;
                            param.index = index++;
                            param.perspectiveType = PerspectiveType.SELF;
                            param.actionType = ActionyType.ACTION;
                            param.frameType = (FrameType)frameId;
                            param.replayType = (ReplayType)replayId;
                            param.randomType = RandomType.NORMAL;
                            param.durationId = durationId;
                            param.targetActionId = targetId;
                            param.control = -1;
                            param.authorship = -1;

                            blockedResult.Add(param);
                        }
                    }
                    Shuffle(blockedResult);

                    int num_one_block = numTargetAction * DurationArray.Length / 3;
                    //Debug.Log("num trial per block" + num_one_block);
                    //Debug.Log("num trials in one condition" + blockedResult.Count());

                    for (int i = 0; i < 3; i++)
                    {
                        List<Result> SubList = blockedResult.GetRange(i * num_one_block, num_one_block);
                        ResultLists.Main.Add(SubList);
                        //Debug.Log((i * num_one_block) + " " + num_one_block + " " + SubList.Count());
                    }


                }
            }
            Shuffle(ResultLists.Main);

            for (int i = 0; i < ResultLists.Main.Count; i++)
            {
                Debug.Log(ResultLists.Main[i][0].replayType + " " + ResultLists.Main[i][0].frameType);
            }

            Debug.Log("Blocked param List num_block: " + ResultLists.Main.Count() + "  num_trials:" + ResultLists.Main[0].Count());

        }

        public void CreateRandomConditions()
        {

            Debug.Log("numTrial:" + numTrial);

            List<Result> TempList = new List<Result>();
            for (int perspectiveId = 0; perspectiveId < 2; perspectiveId++)
            {
                for (int actionId = 0; actionId < 2; actionId++)
                {
                    for (int delayId = 0; delayId < DelayArray.Length; delayId++)
                    {
                        for (int targetId = 0; targetId < numTargetAction; targetId++)
                        {
                            for (int durationId = 0; durationId < DurationArray.Length; durationId++)
                            {
                                for (int trialId = 0; trialId < numTrial; trialId++)
                                {
                                    Result param = new Result();
                                    param.sessionId = (int)AbstractMenuManager.ExpSession.MAIN;
                                    param.index = trialId;
                                    param.perspectiveType = (PerspectiveType)perspectiveId;//  (PerspectiveType)0; //
                                    param.replayType = (ReplayType)actionId; // (ActionType)1;  //
                                    param.delayId = delayId;
                                    param.durationId = durationId;
                                    param.targetActionId = targetId;
                                    param.delayId = delayId;
                                    param.control = -1;
                                    param.authorship = -1;

                                    TempList.Add(param);
                                }
                            }
                        }
                    }
                }
            }

            Shuffle(TempList);

            int num_all = TempList.Count();
            int num_trial_per_block = (int)Mathf.Ceil((float)num_all / (float)num_blocks);
            Debug.Log("all num list : " + TempList.Count() + " num_block:" + num_blocks + " num_trials:" + num_trial_per_block);

            ResultLists.Main = new List<List<Result>>();
            for (int blockId = 0; blockId < num_blocks; blockId++)
            {
                List<Result> blockedResult = new List<Result>();
                for (int trialId = 0; trialId < num_trial_per_block; trialId++)
                {
                    int index = trialId + blockId * num_trial_per_block;
                    if (index < num_all)
                    {
                        Result param = TempList[index];
                        param.index = index;
                        param.trialId = trialId;
                        param.blockId = blockId;
                        blockedResult.Add(param);
                    }
                }
                ResultLists.Main.Add(blockedResult);

                Debug.Log("Block: " + blockId + " Num Trial: " + ResultLists.Main[blockId].Count);
            }

        }

        public void CreatePracticeConditions() { 

            //Practice
            ResultLists.Practice = new List<Result>();
            for (int index = 0; index < numPractice; index++)
            {
                for (int targetId = 0; targetId < numTargetAction; targetId++)
                {
                    for (int perspectiveId = 0; perspectiveId < 2; perspectiveId++)
                    {
                        Result param = new Result();
                        param.index = index;
                        param.blockId = 0;
                        param.perspectiveType = (PerspectiveType)perspectiveId;
                        param.targetActionId = targetId;
                        param.replayType = ReplayType.LIVE; 
                        param.delayId = 0;
                        ResultLists.Practice.Add(param);
                    }
                }
            }

            Shuffle(ResultLists.Practice);

        }

        public void CreatePracticeNoActionConditions()
        {

            //Practice
            ResultLists.Practice = new List<Result>();
            for (int actionId = 0; actionId < numPractice; actionId++)
            {
                for (int targetId = 0; targetId < numTargetAction; targetId++)
                {
                    for (int perspectiveId = 0; perspectiveId < 2; perspectiveId++)
                    {
                            Result param = new Result();
                            param.index = actionId;
                            param.blockId = 0;
                            param.actionType = (ActionyType)actionId;
                            param.perspectiveType = (PerspectiveType)perspectiveId;
                            param.targetActionId = targetId;
                            param.randomType = RandomType.NORMAL;
                            param.frameType = FrameType.OVER;
                            param.replayType = ReplayType.LIVE;
                            param.delayId = 0;
                            ResultLists.Practice.Add(param);
                    }
                }
            }

            Shuffle(ResultLists.Practice);

        }

        public void CreatePracticeSpacialRandomConditions()
        {

            //Practice
            ResultLists.Practice = new List<Result>();
            for (int index = 0; index < numPractice; index++)
            {
                for (int targetId = 0; targetId < numTargetAction; targetId++)
                {
                    for (int randomId = 0; randomId < 2; randomId++)
                    {
                        Result param = new Result();
                        param.index = index;
                        param.blockId = 0;
                        param.targetActionId = targetId;
                        param.randomType = (RandomType)randomId;
                        param.actionType = ActionyType.ACTION;
                        param.perspectiveType = PerspectiveType.SELF;
                        param.frameType = FrameType.OVER;
                        param.replayType = ReplayType.LIVE;
                        param.delayId = 0;
                        ResultLists.Practice.Add(param);
                    }
                }
            }

            Shuffle(ResultLists.Practice);

        }
        public void CreatePracticeFrameShiftConditions()
        {

            //Practice
            ResultLists.Practice = new List<Result>();
            for (int index = 0; index < numPractice; index++)
            {
                for (int targetId = 0; targetId < numTargetAction; targetId++)
                {
                    for (int frameId = 0; frameId < 2; frameId++)
                    {
                        Result param = new Result();
                        param.index = index;
                        param.blockId = 0;
                        param.targetActionId = targetId;
                        param.frameType = (FrameType)frameId;
                        param.perspectiveType = PerspectiveType.SELF;
                        param.randomType = RandomType.NORMAL;
                        param.actionType = ActionyType.ACTION;
                        param.replayType = ReplayType.LIVE;
                        param.delayId = 0;
                        ResultLists.Practice.Add(param);
                    }
                }
            }

            Shuffle(ResultLists.Practice);

        }



        public void SetFilename(string data_dir, string dataname, int SubId)
        {

            string dir = data_dir + "/" + SubId + "/";
            Directory.CreateDirectory(dir);

            DateTime dt = DateTime.Now;
            out_filename = string.Format(dir + dataname + "_{0}-{1}-{2}-{3}-{4}-{5}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

        public void SaveHeaderToFile(int HandModel)
        {

            string text = "#";
            text += "numBlocks:" + num_blocks + ";";
            text += "numTrials:" + numTrial + ";";
            text += "Hand:" + HandModel + ";";
            text += "delays:";
            for (int i = 0; i < DelayArray.Length; i++)
            {
                text += DelayArray[i] + ",";
            }
            text += "durations:";
            for (int i = 0; i < DurationArray.Length; i++)
            {
                text += DurationArray[i] + ",";
            }

            text += Environment.NewLine;
            text += "#sessionId, blockId, trialId, ActionType, PerspectiveType, DelayId, MovementType, RandomType, FrameType, targetActionId, durationId, actual_duration, reported_duraton, control, authorship, averageDelay, averageFPS";
            text += Environment.NewLine;
            File.AppendAllText(out_filename, text);
        }


        public string GetTextFromParam(DataManager.Result p)
        {
            string text = "";
            text += p.sessionId + ", ";
            text += p.blockId + ", ";
            text += p.trialId + ", ";
            text += p.replayType + ", ";
            text += p.perspectiveType + ", ";
            text += p.delayId + ", ";
            text += p.actionType + ", ";
            text += p.randomType + ", ";
            text += p.frameType + ", ";
            text += p.targetActionId + ", ";
            text += p.durationId + ", ";
            text += p.actual_duration + ", ";
            text += p.repoted_duration + ", ";
            text += p.control + ", ";
            text += p.authorship + ", ";
            text += p.averageDelay + ", ";
            text += p.averageFPS + "";
            text += Environment.NewLine;
            return text;
        }
        public void SaveDataToFile(DataManager.Result p)
        {
            File.AppendAllText(out_filename, GetTextFromParam(p));
        }



        public void SetRandomSeed(int SubId)
        {
            rnd = new System.Random(SubId + nonce);
        }



        public void Shuffle<T>(List<T> list)
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

        /*
        public void Shuffle<T>(List<T> list)
        {
            //var rnd = new System.Random(Guid.NewGuid().GetHashCode());
            Enumerable.Range(1, list.Count).Reverse().ToList().ForEach(i => Swap(list, rnd.Next(i), i - 1));
        }

        public void Swap<T>(List<T> list, int index1, int index2)
        {
            var a = list[index1];
            list[index1] = list[index2];
            list[index2] = a;
        }
        */

    }
}