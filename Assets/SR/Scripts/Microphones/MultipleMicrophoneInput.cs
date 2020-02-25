using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]

public class MultipleMicrophoneInput : MonoBehaviour 
{
	//A boolean that flags whether there's a connected microphone
//	private bool micConnected = false;

	//The maximum and minimum available recording frequencies
	private int[] minFreqs;
	private int[] maxFreqs;

	//A handle to the attached AudioSource
	private AudioSource goAudioSource;

	//The current microphone
	//	private int currentMic = 0;

	//The selected microphone
	//private int selectedMic = 0;

	//An integer that stores the number of connected microphones
	private	int numMics;

	//A boolean that flags whether the audio capture is active or not
	//private bool recActive = false;

	//Use this for initialization
	void Start() 
	{
		//An integer that stores the number of connected microphones
		numMics = Microphone.devices.Length;
		Debug.Log ("num mics : " + numMics);
		//Check if there is at least one microphone connected
		if(numMics <= 0)
		{
			//Throw a warning message at the console if there isn't
			Debug.LogWarning("No microphone connected!");
		}
		else //At least one microphone is present
		{
			//Set 'micConnected' to true
//			micConnected = true;

			//Initialize the minFreqs and maxFreqs array to hold the same number of integers as there are microphones
			minFreqs = new int[numMics];
			maxFreqs = new int[numMics];

			//Get the recording capabilities of each microphone
			for(int i=0; i < numMics; i++)
			{
				Microphone.GetDeviceCaps(Microphone.devices[i], out minFreqs[i], out maxFreqs[i]);

				//According to the documentation, if both minimum and maximum frequencies are zero, the microphone supports any recording frequency...
				if(minFreqs[i] == 0 && maxFreqs[i] == 0)
				{
					//...meaning 44100 Hz can be used as the recording sampling rate for the current microphone
					maxFreqs[i] = 44100;
				}
			}

			//Get the attached AudioSource component
			goAudioSource = this.GetComponent<AudioSource>();
			
			goAudioSource.clip = Microphone.Start(null, true, 1000, 44100);
		    while(!(Microphone.GetPosition(null) > 0)) {
    		}    
			goAudioSource.Play();
    		goAudioSource.mute=false;
		}
		
	}

}