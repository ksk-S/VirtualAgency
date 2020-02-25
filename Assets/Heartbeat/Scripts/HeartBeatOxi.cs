using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using Heartbeat;

public class HeartBeatOxi : HeartBeat
{	
	[System.Serializable]
	public enum DetectionMethod
	{
		Status, Slope
	}

	//for data recording
	[System.Serializable]
	public class PulseData
	{
		public long time;
		public int raw;
		public int beat_stauts;
		public int beat_raw;
		public int healthy;
	}
	public string OxiPortName = "COM1";


	public DetectionMethod method = DetectionMethod.Status;

	StopWatchTimer timer;
	public bool is_save_data = true;
	public string data_dir = "PulseData";
	public string out_filename = "pulse.csv";
	public int SaveBufferSize =  200;
	List<PulseData> pulseSaveDataList = new List<PulseData>();
	


	//-------------------------
	// Parameters
	//-------------------------
	const int STATUS_BYTE = 0;
	const int PLETH_MSB_BYTE = 1;
	const int PLETH_LSB_BYTE = 2;
	const int FLAT_BYTE = 3;
	const int CHECKSUM_BYTE = 4;
 		
	SerialPort serial;
	System.Threading.Thread main_thread;
	System.Threading.Thread calib_thread;
//	public bool heartbeat_flag = false;
	object sync = new object ();
   
	public object sync_data = new object ();

	//-------------------------
	//Variables
	//-------------------------
	bool oxi_previous_beat = false;

	int data_receive_count = 0;
	int data_corret_count = 0;

	

	//visulization
	public bool is_draw_graph = true;
	public int pulseHistorySize = 1000;
	public Queue<PulseData> pulseHistory = new Queue<PulseData>();

	
	//caliburation
	public bool isCalibrated = false;
	List<List<int>> calib_raw_history = new List<List<int>>();


	public int CalibWindow = 125;
	public int CalibRepeat = 3;
	public int beat_detection_threshold = 5000;
	Queue<bool> oxi_previous_beat_raw_queue = new Queue<bool>();
	//raw_history
	Queue<int> raw_history = new Queue<int>();
	int raw_history_max = 3;

	public bool ReCaliburation = false;

	public string config_file = "config.ini";


	bool isRunning = false;

	public override void Start ()
	{	
		DateTime dt = DateTime.Now;
		if (is_save_data)
		{
			Directory.CreateDirectory (data_dir);
			out_filename  = string.Format(data_dir + "\\pulseoxi-" + "{0}-{1}-{2}-{3}-{4}-{5}.csv", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
		}

		timer = GameObject.Find ("Time").GetComponent<StopWatchTimer> ();



		string initfile = Application.dataPath + "/../" + config_file;
		INIFile iniFileCtl = new INIFile (initfile);

		try{
			OxiPortName = iniFileCtl ["Heartbeat", "comPort"];
		}catch{
			UnityEngine.Debug.Log ("comPort can't be read from init file ");
		}
		UnityEngine.Debug.Log ("comport " + OxiPortName);	

		Connect ();

	}
	
	public override void Update ()
	{
		SaveData ();
		//HeartUpdate ();

		if(ReCaliburation){
			ReCalibrate();
			ReCaliburation = false;
		}
		if(isDummyMode){
			MakeDummyBeat(dummyInterval);
		}	
	}
	
	public void HeartUpdate ()
	{
	//	lock (sync) {
			//UnityEngine.Debug.Log("heart update " + heartbeat_flag);
		//	if (heartbeat_flag) {
				//GenerateBeat (); 
				//heartbeat_flag = false;
		//	}
	//	}
	}

	private void SaveData(){
		if (!is_save_data)
			return;


		if(pulseSaveDataList.Count > SaveBufferSize){
			string saved_contents = "";
			for(int i=0; i< pulseSaveDataList.Count; i++)
			{
				PulseData _data = pulseSaveDataList[i];
				saved_contents += _data.time + ", ";
				saved_contents += _data.raw + ", ";
				saved_contents += _data.beat_raw + ", ";
				saved_contents += _data.beat_stauts + ", ";
				saved_contents += _data.healthy + "";
				saved_contents += Environment.NewLine;
			}
			File.AppendAllText(out_filename, saved_contents);
			pulseSaveDataList.Clear ();
		}
	}

	public Queue<PulseData> getBuffer(){
		return pulseHistory;
	}


	public  bool Connect ()
	{
		serial = new SerialPort ();
		//serial.DataReceived += new SerialDataReceivedEventHandler(Oxi_DataReceived);
		//serial.ErrorReceived += new SerialErrorReceivedEventHandler(Oxi_ErrorReceived);

		if (serial.IsOpen == false) {
			serial.PortName = OxiPortName;
			serial.BaudRate = 9600;
			serial.DataBits = 8;
			serial.Parity = Parity.None;
			serial.StopBits = StopBits.One;
			serial.Handshake = Handshake.None;
//                serial.Handshake = Handshake.RequestToSend;
//                serial.Handshake = Handshake.XOnXOff;
						//serial.Encoding = Encoding.Unicode;
						//serial.ReadBufferSize = 16384;x

			try {
				UnityEngine.Debug.Log ("Trying to open Serial Port " + OxiPortName + " for a pulse oximeter");
				serial.Open ();
				isRunning = true;

				main_thread = new System.Threading.Thread (new ThreadStart (LoopDetectBeatThread));
				main_thread.Start ();

				calib_thread = new System.Threading.Thread (new ThreadStart (CaliburationThread));
				calib_thread.Start ();
       		
				return true;	
			} catch (Exception ex) {
				UnityEngine.Debug.Log (ex.Message);
				return false;
			
			}
		}
			return true;
	}

	public  bool Disconnect ()
	{
		if (serial.IsOpen == true) {
			try {
				serial.Close ();
				UnityEngine.Debug.Log ("Close Serial Port for Pulse Oxi");
			} catch (Exception ex) {
				UnityEngine.Debug.Log (ex.Message);
				return false;
			}
		}
		return true;
	}



	void CaliburationThread(){

		UnityEngine.Debug.Log ("==========> CALIB START  <===========");


		for(int winIdx = 0; winIdx < CalibRepeat; winIdx++){
			List<int> ListData = new List<int>();
			UnityEngine.Debug.Log ("  ========> CALIB WINDOW " +  (winIdx + 1) );
			for(int calIdx = 0; calIdx < CalibWindow; calIdx++){

				Byte[] data = new Byte[256];
				try {
					for (int i=0; i<5; i++) {
						data[i] = (byte)serial.ReadByte ();
					}
					//byte temp = (byte)serial.ReadByte ();
				} catch (Exception e) {
					UnityEngine.Debug.Log (data_receive_count + ": can't get packets :" + e.Message);
				}

				if(CheckPacket(data)){
					int raw = (int)data [PLETH_MSB_BYTE] * 256 + (int)data [PLETH_LSB_BYTE];
					ListData.Add (raw);
				}else{
					System.Threading.Thread.Sleep (20);
					try {
						serial.ReadByte ();
					} catch (Exception e) {
						UnityEngine.Debug.Log (data_receive_count + ": can't get packets :" + e.Message);
					}
				}
			}
			calib_raw_history.Add (ListData);
		}



		List<float> MaxList = new List<float> ();
		for(int i = 0; i < calib_raw_history.Count(); i++){
			List<int> DataList = calib_raw_history[i];
			List<int> DiffList = new List<int> ();
			for(int j = 0; j < DataList.Count()-(raw_history_max+1); j++){
				int sum = 0;
				for(int k = 0; k<raw_history_max; k++){
					sum += DataList[j];
				}
				float ave = (float)sum / raw_history_max;
				int cur = DataList[j + raw_history_max];
				DiffList.Add (cur - (int)ave);
			}
			UnityEngine.Debug.Log ("Diff max " + DiffList.Max ());
			MaxList.Add (DiffList.Max ());
		}
		beat_detection_threshold = (int) MaxList.Average ();
		UnityEngine.Debug.Log ("Calib Threash " + beat_detection_threshold);


		UnityEngine.Debug.Log ("==========> CALIB FINISH  <===========");

		lock (sync)
		{
			isCalibrated = true;
			Monitor.Pulse(sync);
		}
	}
	
	
	public void LoopDetectBeatThread ()
	{   
		UnityEngine.Debug.Log ("Pulse Oximeter Thread Start");
		while (isRunning) {

			lock (sync)
			{
				// wait for calibration to finish
				while(!isCalibrated)
				{
					UnityEngine.Debug.Log ("==========> CALIB MONITOR ON  <===========");
					Monitor.Wait(sync);
					UnityEngine.Debug.Log ("==========> CALIB MONITOR OFF <===========");
				}
			}

			data_receive_count++;

			Byte[] data = new Byte[256];
			try {
				for (int i=0; i<5; i++) {

					data[i] = (byte)serial.ReadByte ();
				}
				//byte temp = (byte)serial.ReadByte ();
			} catch (Exception e) {
				UnityEngine.Debug.Log (data_receive_count + ": can't get packets :" + e.Message);
			}
	
			if(CheckPacket(data)){

				System.Threading.Thread thread2 = new System.Threading.Thread (new ParameterizedThreadStart (ThreadDetectBeat));
				thread2.Start (data);
			
			}else{
				UnityEngine.Debug.Log (data_receive_count + ": Packet Error");

				System.Threading.Thread.Sleep (20);
				try {
					serial.ReadByte ();
				} catch (Exception e) {
					UnityEngine.Debug.Log (data_receive_count + ": can't get packets :" + e.Message);
				}

			}
			System.Threading.Thread.Sleep (0);

		}
	}
	
	private bool CheckPacket(Byte[] data)
	{

		System.Collections.BitArray status_bits = new System.Collections.BitArray(new byte[] {data[STATUS_BYTE]});
	
	//	if(status_bits[0]) UnityEngine.Debug.Log("Sync bit = "  + status_bits[0] + " " + data_receive_count);
		bool correct_packets = true;

		try {
			if (!status_bits [7]) {
				UnityEngine.Debug.Log("BIT7 is 0");
				correct_packets =  false;
			}
		} catch (Exception e) {
			correct_packets =  false;
			UnityEngine.Debug.Log ("can't finish check BIT7 :" + e.Message);
		}

		//Check Sum
		try {
			int sum = 0;
			for (int i=0; i<4; i++) {
				sum += data [i];
			}
			//UnityEngine.Debug.Log((int)data[4] + " " + (sum % 256));
			if ((int)data [4] != (sum % 256)) {
				UnityEngine.Debug.Log("check sum error");
				correct_packets = false;
			}
		} catch (Exception e) {
			correct_packets =  false;
			UnityEngine.Debug.Log ("can't finish check sum :" + e.Message);
		}

		return correct_packets;

	}


	public void ThreadDetectBeat (object o)
	{
		Byte[] data = (Byte[])o;

		System.Collections.BitArray bits;

		bits = new System.Collections.BitArray (new byte[] {data[STATUS_BYTE]});

		//UnityEngine.Debug.Log ("Save Pulse");
		//SavePulse (bits, data);

		bool flag_beat_raw = DetectBeatFromRaw (data);
		bool flag_beat_status = DetectBeatFromStatus (bits);

		//UnityEngine.Debug.Log ("Save Pulse " + flag_beat_raw + " " + flag_beat_status);
		StorePulse (data, flag_beat_raw, flag_beat_status);

		lock (sync) {
			if(method == DetectionMethod.Slope){
				if(flag_beat_raw){
					//heartbeat_flag = true;
					GenerateBeat();
				}
			}else if(method == DetectionMethod.Status){
				if(flag_beat_status){
					//heartbeat_flag = true;
					GenerateBeat();
				}
			}
		}
	}

	void StorePulse (Byte[] data, bool beat_raw, bool beat_status)
	{
		if (!is_save_data) return;

		PulseData pulseData = new PulseData ();
		//UnityEngine.Debug.Log (timer);
		if(timer.isStart){
			pulseData.time = timer.GetElapsedMilliseconds();
		}
		pulseData.raw = (int)data [PLETH_MSB_BYTE] * 256 + (int)data [PLETH_LSB_BYTE];
		pulseData.beat_raw = beat_raw ? 1 : 0;
		pulseData.beat_stauts = beat_status ? 1 : 0;
		pulseData.healthy = healthy ? 1 : 0;

		pulseSaveDataList.Add (pulseData);

		//UnityEngine.Debug.Log (pulseHistory.Count ());
		lock(sync_data){
			pulseHistory.Enqueue (pulseData);
			if(pulseHistory.Count > pulseHistorySize){
				pulseHistory.Dequeue();
			}
		}

	}

	bool DetectBeatFromRaw (Byte[] data)
	{

		int raw = (int)data [1] * 256 + (int)data [2];

		bool beat = false;
		//UnityEngine.Debug.Log (raw_history.Count);
		if(raw_history.Count != raw_history_max){
			// do nothing
		}else{
			int diff = raw - (int)raw_history.Average ();
			//UnityEngine.Debug.Log (diff);
			bool is_beat_near_past = false;
			foreach( bool flag in oxi_previous_beat_raw_queue )
			{
				if(flag) is_beat_near_past = true;
			}
		
			if(diff > beat_detection_threshold * 0.65 && !is_beat_near_past && oxi_previous_beat_raw_queue.Count == 10){
				//UnityEngine.Debug.Log (data_corret_count + " Beat!");

				beat = true;
				oxi_previous_beat_raw_queue.Enqueue (true);
			}else{
				oxi_previous_beat_raw_queue.Enqueue (false);
			}
		}

		if(oxi_previous_beat_raw_queue.Count > 10){
			oxi_previous_beat_raw_queue.Dequeue();
		}


		raw_history.Enqueue (raw);
		if(raw_history.Count > raw_history_max){
			raw_history.Dequeue();
		}

		if(beat) return true;
		else return false;

	}

	bool DetectBeatFromStatus (System.Collections.BitArray bits)
	{
		data_corret_count++;

		//UnityEngine.Debug.Log(data_receive_count + " " + data_corret_count);
		bool beat = false;

		//Sensor is not good
		if (bits [3] | bits [4] | bits [5] | bits [6]) {
			healthy = false;
		}else{
			healthy = true;
		}

		if ((bits [1] | bits [2]) ) {
			if(!oxi_previous_beat){
				beat = true;
				oxi_previous_beat = true;
			}
		}else{
			oxi_previous_beat = false;
		}

	//	UnityEngine.Debug.Log (pulseData.time + " " + pulseData.raw + " " + pulseData.beat );
		
		if (beat) {
			return true;
		} else {
			return false;
		}
	}

	public void ReCalibrate(){

		if(isCalibrated){
			isCalibrated = false;

			calib_thread = new System.Threading.Thread (new ThreadStart (CaliburationThread));
			calib_thread.Start ();
		}

	}

	void OnApplicationQuit ()
	{
		UnityEngine.Debug.Log ("ApplicationQuit in HeartbeatOxi");
		Disconnect ();

		isRunning = false;
		if (main_thread != null) {
			main_thread.Abort (); 
		}

		if (calib_thread != null) {
			calib_thread.Abort (); 
		}
	}

		

}

