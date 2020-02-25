using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class ActiveSession : InteractiveBehaviour {

	Experiment expCtl;

    GameObject SliderPanel;
    GameObject InstructionPanel;
    GameObject MenuPanel;

    Button TrainingSessionButton;
    Button FirstSessionButton;
    Button SecondSessionButton;

    Image screen;
	Text message;
	Text bottomMessage;
	Text sliderValue;
	Text counterText;
	Slider slider;
	Image instructionPanel;
	Text instructionText;

	List<Experiment.Result> ResultList;
	
	//bool isSliderEnabled = false;

	//int counter = 1;



    public void TrainingStart()
    {
    }

    public void ExperimentStart(int sessionId)
    {
        StartCoroutine(CoExperimentStart(sessionId));
    }

    public IEnumerator CoExperimentStart(int sessionId)
    {
        yield return StartCoroutine ("WaitForMouseDown");
        /*
		yield return StartCoroutine ("WaitForMouseDown");

		
		instructionPanel.enabled = false;
		instructionText.enabled = false;

		//sliderContainer.SetActive(false);
		screen.enabled = true;
		bottomMessage.text = " click to start ";
		message.text = "";

		yield return StartCoroutine ("WaitForMouseDown");
	
		counter = 1;

		int max = ResultList.Count;
		//debug
		if(expCtl.debugMode) max = expCtl.debug_mode_num_trials;
		for(int i=0; i< max; i++){

			Experiment.Result param = ResultList[i];
         
			screen.enabled = true;
			if(counter % expCtl.paramNoise.trials_to_break == 0){
				message.text = "Take a break\n click to continue";
				yield return StartCoroutine ("WaitForMouseDown");
			}

			message.text = "+";
			yield return new WaitForSeconds(expCtl.paramNoise.rest_sec);

			screen.enabled = false;

            string filename;
            if (expCtl.noiseEstimationStatus == Experiment.NoiseEstimationStatus.Practice)
            {
                filename = expCtl.NoiseEstimationNaturalImageDir + param.img.filename;
            }
            else
            {
                filename = expCtl.NoiseEstimationObjectImageDir + param.noise_image_name;
            }
			
			//Show scrumbled image
			ImageCtl.LoadImage (filename);
			ImageCtl.ShowScrumbled(expCtl.paramNoise.starndard_noise);
			param.standard_noise = expCtl.paramNoise.starndard_noise;

			isSliderEnabled = false;
			sliderContainer.SetActive(false);

			message.text = "";
			bottomMessage.text = "Reference image";

			yield return new WaitForSeconds(expCtl.paramNoise.standard_sec);

			bottomMessage.text = "";
			screen.enabled = true;
			message.text = "+";

			yield return new WaitForSeconds(expCtl.paramNoise.rest_sec);
			//Show noised image

			screen.enabled = false;
			message.text = "";
			bottomMessage.text = "Test image";
			ImageCtl.ShowNoise(expCtl.paramNoise.noises[param.noise_index]);

			yield return new WaitForSeconds(expCtl.paramNoise.test_sec);

			ImageCtl.ShowNothing();

			bottomMessage.text = "";
			message.text = "Estimate the noise level of the second image compared to the first one";
			slider.value = 100;
			isSliderEnabled = true;
			sliderContainer.SetActive(true);

			yield return StartCoroutine ("WaitForMouseDown");

			//save data
			param.noise_estimation = slider.value;

            if (expCtl.noiseEstimationStatus == Experiment.NoiseEstimationStatus.Practice)
            {
                SavePracticeDataToFile(param);
            }
            else { 
                SaveDataToFile(param);
            }
            counter++;

  
		}
		screen.enabled = true;
		message.text = "Finished\n click to continue";
		yield return StartCoroutine ("WaitForMouseDown");


        switch (expCtl.SessionId)
        {
            case 0:
                expCtl.status.ActiveOnly = true; break;

            case 1:
                expCtl.status.ActiveVR = true; break;

            case 2:
                expCtl.status.ActiveNoVR = true; break;

        }
          */

    }

        string GetTextFromParam(Experiment.Result p){
	
		string text = "";
		text += p.index + ", ";
	
		text += Environment.NewLine;
		return text;
	}

	void SaveHeaderToFile()
	{
		string text = "#";
        /*
		text += "noise_level:";
		for(int i=0; i < expCtl.paramNoise.noises.Length; i++){
			text += expCtl.paramNoise.noises[i] + ",";
		}
		text += ";";
		text += "stardard_noise:" + expCtl.paramNoise.starndard_noise + ";";
		text += Environment.NewLine;

		text += "# familyId, couplingId, objectId, presentId, standard_noise, noise_index, noise_estimation";
        */
		text += Environment.NewLine;
		File.AppendAllText(out_filename, text);
	}

	void SaveDataToFile(Experiment.Result p)
	{
		
		File.AppendAllText(out_filename, GetTextFromParam (p));
	}

    void SaveHeaderToFilePractice()
    {
        string text = "#";
        text += "noise_level:";
        /*
        for (int i = 0; i < expCtl.paramNoise.noises.Length; i++)
        {
            text += expCtl.paramNoise.noises[i] + ",";
        }
        text += ";";
        text += "stardard_noise:" + expCtl.paramNoise.starndard_noise + ";";
        text += Environment.NewLine;

        text += "# emotionId, subCategoryId, imageId, standard_noise, noise_index, noise_estimation";
        text += Environment.NewLine;
        */
        File.AppendAllText(out_filename, text);
    }

    string GetTextFromParamPractice(Experiment.Result p)
    {

        string text = "";
        text += p.index + ", ";
        text += Environment.NewLine;
        return text;
    }

    void SavePracticeDataToFile(Experiment.Result p)
    {

        File.AppendAllText(out_filename, GetTextFromParamPractice(p));
    }


    // Update is called once per frame
    protected override void DoUpdate ()
	{
        /*
        if (expCtl.debugMode)
        {
            counterText.text = counter + " / " + ResultList.Count;  
        }

        if (isSliderEnabled){

			slider.value += Input.GetAxis("Mouse X") * expCtl.mouse_sensitivity * slider.maxValue; 
			
			sliderValue.text = slider.value.ToString ();
		}
        */
	}

	void Swap<T>(List<T> list, int index1, int index2) {
		var a = list[index1];
		list[index1] = list[index2];
		list[index2] = a;
	}
	
	void Shuffle<T>(List<T> list) {
		var rnd = new System.Random();
		Enumerable.Range(1, list.Count).Reverse().ToList().ForEach(i => Swap(list, rnd.Next(i), i - 1));
	}
}
