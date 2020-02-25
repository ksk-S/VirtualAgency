using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;


public class StopWatchTimer : MonoBehaviour {

	public Stopwatch stopWatch = new Stopwatch();
	public bool isStart = false;

	void Start()
	{

		StartWatch ();
	}
		
	public void StartWatch()
	{

		stopWatch.Start ();
		isStart = true;
	}

	public void Reset()
	{
		stopWatch.Reset ();
		stopWatch.Start ();
	}

	public long GetElapsedMilliseconds()
	{
		return stopWatch.ElapsedMilliseconds;
	}

	public string getTime()
	{
		return stopWatch.Elapsed.TotalSeconds.ToString();
	}



}

