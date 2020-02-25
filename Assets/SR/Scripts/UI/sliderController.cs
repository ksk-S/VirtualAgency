using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class sliderController : MonoBehaviour {

	enum Status{
		None, Answer, Feedback
	}

	Status status = Status.None;

	Text question;
	Slider slider;
	Text textValue;
	Text fbText;
	Text nextText;

	Text LabelLeft;
	Text LabelRight;
	Text LabelCentre;


	GameObject fbHandle;

	bool isSliderEnabled = false;

	public float sliderMaxValueTime = 3.0f;
	public float sliderMaxValueDuration = 600f;
	public float sliderMaxValueQuestions = 100.0f;


	public float sliderValue = 1.5f;
	public float sensitivity = 0.1f;

	public float feedbackvalue = 2.5f;

	public bool isRunning = false;

	// Use this for initialization
	void Start () {
		question = GameObject.Find ("Question").GetComponent<Text> ();
		slider = GameObject.Find ("Slider").GetComponent<Slider> ();
		textValue = GameObject.Find ("ScaleValue").GetComponent<Text> ();
		fbHandle = GameObject.Find ("FeedbackHandle");
		fbText = GameObject.Find ("FeedbackText").GetComponent<Text> ();
		nextText = GameObject.Find ("ClicktoContinueText").GetComponent<Text> ();

		LabelLeft = GameObject.Find ("LabelLeft").GetComponent<Text> ();
		LabelRight = GameObject.Find ("LabelRight").GetComponent<Text> ();
		LabelCentre = GameObject.Find ("LabelCentre").GetComponent<Text> ();

		slider.maxValue = sliderMaxValueTime;

		HideSlider ();
		DisableSlider ();
		HideFeedback ();
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 pos = Input.mousePosition;
		//float value = (float)pos.x / (float)Screen.width *2;

		if(isSliderEnabled){
			sliderValue = Input.GetAxis("Mouse X") * sensitivity * slider.maxValue;

			slider.value += sliderValue; 

			if(slider.maxValue == sliderMaxValueDuration){
				textValue.text = Mathf.Floor(slider.value / 60).ToString ("0") +"m" + (slider.value % 60).ToString ("00")+"s";
			}else{
				textValue.text = slider.value.ToString("0.0") + " s";
			}
		
		}
		KeyControl ();
	}


	public void RandomizeHandle()
	{
		float random = Random.Range (0f, slider.maxValue);
		slider.value = random; 
		textValue.text = slider.value.ToString("0.0") + " s";
	}

	public void SetHandleCentre()
	{
		slider.value = slider.maxValue / 2;
		textValue.text = slider.value.ToString("0.0") + " s";
	}

	public float GetSliderValue()
	{
		return slider.value;

	}

	public void EnableSlider()
	{
		isSliderEnabled = true;
	}


	public void DisableSlider()
	{
		isSliderEnabled = false;
	}


	public void HideSlider()
	{
		question.enabled = false;
		slider.gameObject.SetActive (false);
		textValue.enabled = false;

		LabelLeft.enabled = false;
		LabelRight.enabled = false;
		LabelCentre.enabled = false;
	}

	public void ShowSliderForTimePerception(){

		LabelLeft.enabled = false;
		LabelRight.enabled = false;
		LabelCentre.enabled = false;

		question.enabled = true;
		slider.gameObject.SetActive (true);
		textValue.enabled = true;

		question.text = "For how long was the red patch presented?";
		slider.maxValue = sliderMaxValueTime;
	}

	public void ShowSliderForBlockDurationEstimation(string message){
		
		LabelLeft.enabled = false;
		LabelRight.enabled = false;
		LabelCentre.enabled = false;
		
		question.enabled = true;
		slider.gameObject.SetActive (true);
		textValue.enabled = true;
		
		question.text = message;
		slider.maxValue = sliderMaxValueDuration;
	}


	public void ShowSliderForQuestionnaire(string message){

		LabelLeft.enabled = true;
		LabelRight.enabled = true;
		LabelCentre.enabled = true;

		question.enabled = true;
		slider.gameObject.SetActive (true);
		textValue.enabled = false;

		char delimiter ='\n';
		string[] words = message.Split(delimiter);

		Debug.Log (words.Length);
		question.text = words[0];
		LabelLeft.text = words[1];
		LabelRight.text = words[2];
		LabelCentre.text = words[3];


		slider.maxValue = sliderMaxValueQuestions;
		SetHandleCentre ();
	}

	public void ShowProductionFeedback(float correct, float answer){

		textValue.text = "";

		question.text = "Your produced interval was " + answer.ToString("0.00") + " s";

		slider.value = answer;

		fbHandle.SetActive(true);
		RectTransform t = GameObject.Find ("Handle Slide Area").GetComponent<RectTransform>();

 		//Debug.Log (t.rect.xMin + " " + t.rect.xMax + " " + fbHandle.GetComponent<RectTransform> ().rect.width);
		
		Vector3 position = fbHandle.transform.localPosition;
		float xPos = Mathf.Lerp (t.rect.xMin, t.rect.xMax, (correct / slider.maxValue));
		position.x = xPos;// - (int)(fbHandle.GetComponent<RectTransform> ().rect.width / 2);//+ 3;
		fbHandle.transform.localPosition = position;
		
		fbText.text = "Your target interval was " + correct.ToString("0.00") + " s";
		nextText.text = "Click to continue";
	}

	public void ShowFeedback(float feedback)
	{
		fbHandle.SetActive(true);

		RectTransform t = GameObject.Find ("Handle Slide Area").GetComponent<RectTransform>();

		//Debug.Log (t.rect.xMin + " " + t.rect.xMax + " " + fbHandle.GetComponent<RectTransform> ().rect.width);

		Vector3 position = fbHandle.transform.localPosition;
		float xPos = Mathf.Lerp (t.rect.xMin, t.rect.xMax, (feedback / slider.maxValue));
		position.x = xPos - (int)(fbHandle.GetComponent<RectTransform> ().rect.width / 2)  + 3;
		fbHandle.transform.localPosition = position;

		fbText.text = "Correct interval was " + feedback.ToString("0.0") + " s";
		nextText.text = "Click to continue";
	}

	public void HideFeedback()
	{
		fbHandle.SetActive(false);
		fbText.text = "";
		nextText.text = "";
	}


	void KeyControl(){
		if(isRunning)
		{
			if(Input.GetMouseButtonDown(0))
			{
				Debug.Log ("mouset click");
				switch(status)
				{
				case Status.None:
					ShowSliderForTimePerception();
					EnableSlider ();
					Debug.Log("Slider");

					status = Status.Answer;
					break;

				case Status.Answer:
					Debug.Log ("Answer " + sliderValue);
					DisableSlider();
					ShowFeedback (feedbackvalue);
					Debug.Log("Feedback");

					status = Status.Feedback;
					break;
			
				case Status.Feedback:
					HideSlider ();
					HideFeedback ();
					Debug.Log("Feedback");
					status = Status.None;
					break;
				}
			}
		}
	}



}
