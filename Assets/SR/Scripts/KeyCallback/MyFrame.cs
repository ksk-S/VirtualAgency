using UnityEngine;
using System.Collections;

public class MyFrame
{
	private static int lastID = 0;

	private int ID {get; set;}
	private float timestamp { get; set; }
	
	public MyFrame ()
	{
		this.ID = ++lastID;
		this.timestamp = UnityEngine.Time.realtimeSinceStartup;
	}
}
