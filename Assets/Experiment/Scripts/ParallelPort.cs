using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;

public class ParallelPort : MonoBehaviour {

    public const short FILE_ATTRIBUTE_NORMAL = 0x80;
    public const short INVALID_HANDLE_VALUE = -1;
    public const uint GENERIC_READ = 0x80000000;
    public const uint GENERIC_WRITE = 0x40000000;
    public const uint CREATE_NEW = 1;
    public const uint CREATE_ALWAYS = 2;
    public const uint OPEN_EXISTING = 3;

    public bool isReady = false;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess,
         uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
         uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    IntPtr ptr;

    // Use this for initialization
    void Start () {
        ptr = CreateFile("LPT1", GENERIC_WRITE, 0,
                    IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

        /* Is bad handle? INVALID_HANDLE_VALUE */
        if (ptr.ToInt32() == -1)
        {
            Debug.Log("Parallel Setup Error");
            /* ask the framework to marshall the win32 error code to an exception */
            // Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }else
        {
            isReady = true;
        }
    }

    public void SendText(string receiptText)
    {
        if (isReady)
        {
            FileStream lpt = new FileStream(ptr, FileAccess.ReadWrite);
            Byte[] buffer = new Byte[2048];
            //Check to see if your printer support ASCII encoding or Unicode.
            //If unicode is supported, use the following:
            //buffer = System.Text.Encoding.Unicode.GetBytes(Temp);
            buffer = System.Text.Encoding.ASCII.GetBytes(receiptText);
            lpt.Write(buffer, 0, buffer.Length);
            lpt.Close();
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
