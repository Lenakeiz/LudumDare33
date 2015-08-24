using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarScript : MonoBehaviour {

	private float currVelocity;

	public Image image;
	public Text percentage;
	public Text actorName;
	public Text panicStatus;

	public void SetName(string _name)
	{
		actorName.text = _name;
	}

	public void SetAmount(float targetValue, string _status)
	{
		float value = Mathf.SmoothDamp(image.fillAmount, targetValue, ref currVelocity, Time.deltaTime);
		image.fillAmount = value;
		percentage.text = (int)(value * 100) + "%";
		panicStatus.text =  _status;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
