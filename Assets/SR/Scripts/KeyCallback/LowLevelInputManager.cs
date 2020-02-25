using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

using ListExtensions;


public class LowLevelInputManager : ManagerSingleton<LowLevelInputManager>
{
	private const int WH_MOUSE_LL = 14;
	private const int WH_KEYBOARD_LL = 13;
	private const int WM_KEYDOWN = 0x0100;
	public const int WM_KEYUP = 0x0101;
	private const int WM_MOUSEDOWN = 0x201; 
	private const int WM_MOUSEUP = 0x202;

	private static LowLevelKeyboardProc _proc = HookCallback;
	private static IntPtr _hookID = IntPtr.Zero;

	public bool isHookActive {get; set;}
		
	private static List<KeyPress> keys;
//	private static List<MyFrame> frames;

	public static float lastKeyDown;
	public static float lastKeyUp;

	public static long lastKeyDownStopWatch;
	public static long lastKeyUpStopWatch;


	private static bool keyPressing = false;

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
	
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool UnhookWindowsHookEx(IntPtr hhk);
	
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
	
	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr GetModuleHandle(string lpModuleName);

	private static Stopwatch sw;
	//private static StopWatchTimer timer;

	// Use this for initialization
	void Start () 
	{

		//frames = new List<MyFrame> ();
		keys = new List<KeyPress> ();

		//timer = GameObject.Find("Time").GetComponent<StopWatchTimer>();	
		sw = new Stopwatch();
		sw.Start ();
	}

	public void ResetTimer(){
		sw.Reset ();
		sw.Start ();
	}

	// Update is called once per frame
	void Update () 
	{
		/*
		frames.Add (new MyFrame());

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			Application.Quit();
		}
		
		*/

		if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
		{
			if (Input.GetKeyDown (KeyCode.H) && !isHookActive) 
			{
				_hookID = SetHook(_proc);
				isHookActive = true;
				UnityEngine.Debug.Log("Installed Win32 API hook.");

			}
			if (Input.GetKeyDown (KeyCode.U) && isHookActive) 
			{
				UnhookWindowsHookEx (_hookID);
				isHookActive = false;
				UnityEngine.Debug.Log("Removed Win32 API hook.");

			}
		}
	}

	public void SetKeyboardHook ()
	{
		if (!isHookActive) 
		{
			keyPressing = false;
			_hookID = SetHook(_proc);
			isHookActive = true;
			UnityEngine.Debug.Log("Installed Win32 API hook.");
		}
	}

	public void UnsetKeyboardHook ()
	{
		if (isHookActive) 
		{
			UnhookWindowsHookEx (_hookID);
			isHookActive = false;
			UnityEngine.Debug.Log("Removed Win32 API hook.");
		}
	}

	public void SetMouseHook ()
	{
		if (!isHookActive) 
		{
			keyPressing = false;
			_hookID = SetMouseHook(_proc);
			isHookActive = true;
			UnityEngine.Debug.Log("Installed Win32 API mouse hook.");
		}
	}

	public void UnsetMouseHook ()
	{
		if (isHookActive) 
		{
			UnhookWindowsHookEx (_hookID);
			isHookActive = false;
			UnityEngine.Debug.Log("Removed Win32 API mouse hook.");
		}
	}
	protected override void OnApplicationQuit()
	{
		sw.Stop ();
		try 
		{
			//frames.SaveToCSV (Path.Combine (Application.dataPath, "frames.csv"));
			//keys.SaveToCSV (Path.Combine (Application.dataPath, "keys.csv"));
		}
		catch (IOException e)
		{
			UnityEngine.Debug.LogError(e.Message);
		}

		if (isHookActive) UnhookWindowsHookEx (_hookID);

		base.OnApplicationQuit();
	}
	/*
	public int CurrentFrameIdx ()
	{
		return frames.Count;
	}
	*/

	private static IntPtr SetHook(LowLevelKeyboardProc proc)
	{
		using (Process curProcess = Process.GetCurrentProcess())
		using (ProcessModule curModule = curProcess.MainModule)
		{
			return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
		}
	}

	private static IntPtr SetMouseHook(LowLevelKeyboardProc proc)
	{
		using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
		{
			return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
		}
	}
	
	private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
	
	private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
	{
		// UnityEngine.Debug.Log (string.Format("HookCallback(): {0} s since start-up.", UnityEngine.Time.realtimeSinceStartup));

		if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
		{
			int vkCode = Marshal.ReadInt32(lParam);	
			long now = sw.ElapsedMilliseconds;
			keys.Add(new KeyPress(vkCode, UnityEngine.Time.realtimeSinceStartup, now));

			if( keyPressing == false){
				lastKeyDown = UnityEngine.Time.realtimeSinceStartup;
				lastKeyDownStopWatch = now;

//				UnityEngine.Debug.Log ("key down" + lastKeyDownStopWatch);
			}
			keyPressing = true;
		}

		if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) 
		{
			lastKeyUp = UnityEngine.Time.realtimeSinceStartup;
			lastKeyUpStopWatch = sw.ElapsedMilliseconds;
			keyPressing = false;

			
//			UnityEngine.Debug.Log ("key up" + lastKeyUpStopWatch);
		}

		if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEDOWN) 
		{
			int vkCode = Marshal.ReadInt32(lParam);	
			long now = sw.ElapsedMilliseconds;
			keys.Add(new KeyPress(vkCode, UnityEngine.Time.realtimeSinceStartup, now));
			
			if( keyPressing == false){
				lastKeyDown = UnityEngine.Time.realtimeSinceStartup;
				lastKeyDownStopWatch = now;
				//				UnityEngine.Debug.Log ("key down" + lastKeyDownStopWatch);
			}
			keyPressing = true;

		}
		
		if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEUP) 
		{
			lastKeyUp = UnityEngine.Time.realtimeSinceStartup;
			lastKeyUpStopWatch = sw.ElapsedMilliseconds;
			keyPressing = false;
			//			UnityEngine.Debug.Log ("key up" + lastKeyUpStopWatch);
		}

		return CallNextHookEx(_hookID, nCode, wParam, lParam);
	}

	public long GetElapsedMilliseconds()
	{
		return sw.ElapsedMilliseconds;
	}
}

