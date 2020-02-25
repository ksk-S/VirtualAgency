using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Ports;

public class Arduino : MonoBehaviour
{
    public string portName = "COM5";

    INIFile iniFileCtl;
    string config_file = "config.ini";


    private static SerialPort s_serial;
	public bool isAvailable = false;

    void Awake()
    {
        string initfile = Application.dataPath + "/../" + config_file;
        iniFileCtl = new INIFile(initfile);

        LoadStringFromFile(ref portName, "Arduino", "PortName");

    }
    // Use this for initialization
    void Start()
    {
        s_serial = new SerialPort(portName, 9600);
		try{
        	s_serial.Open();
		}catch  
		{  
			Debug.Log ("no serial port for arduino : port name" + portName);
			return;
		}
        Debug.Log("Serial Port for arduino was succesfully opened: port name" + portName);

        isAvailable = true;
    }

    public void Vibrate()
    {
		if (isAvailable) {
			s_serial.Write ("S");
		}
    }
    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            print("space key was pressed");
            Vibrate();
        }
        

    }
    void LoadStringFromFile(ref string var, string group, string var_name)
    {
        try
        {
            var = iniFileCtl[group, var_name].Trim();
        }
        catch
        {
            Debug.Log(var_name + " is not boolean in the Init file ");
        }

    }
}