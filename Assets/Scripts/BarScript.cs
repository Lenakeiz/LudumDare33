using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarScript : MonoBehaviour {

	private float currVelocity;

	public Image image;

	public void SetAmount(float targetValue)
	{
		float value = Mathf.SmoothDamp(image.fillAmount, targetValue, ref currVelocity, Time.deltaTime);
		image.fillAmount = value;
		Text _text =  GetComponentInChildren<Text>();
		_text.text = (int)(value * 100) + "%";
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
