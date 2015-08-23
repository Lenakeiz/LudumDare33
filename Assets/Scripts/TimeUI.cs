using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeUI : MonoBehaviour {

	Image image;
	public void ResetGauge()
	{
		image.fillAmount = 1.0f;
	}

	public void UpdateUI(float amount)
	{
		image.fillAmount = Mathf.Clamp(amount,0.0f,1.0f);
	}

	// Use this for initialization
	void Awake () {
		image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
