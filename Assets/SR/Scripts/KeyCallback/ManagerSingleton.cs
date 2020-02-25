using UnityEngine;
using System.Collections;

public class ManagerSingleton<T> : MonoBehaviour where T : Component
{
	protected static T instance;
	public static T Instance 
	{ 
		get { return instance; } 
	}
		
	// Update is called once per frame
	protected virtual void Awake () 
	{
		instance = this as T;

		Save(false);
	}

	protected virtual void OnApplicationQuit()
	{
		Save(true);

		instance = null;
	}
	
	public virtual void Save(bool append) {}
}
