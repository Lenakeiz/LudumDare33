using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskUIScript : MonoBehaviour {

	Text text;

	public void LoseGameText()
	{
		text.text = "CUT CUT CUT!!! What are you doing?!?";
	}

	public void WinGameText()
	{
		text.text = "GOOD JOB! You are truly a star";
	}

	public void SetCurrentTask(string situation)
	{
		text.text = situation;
	}

	// Use this for initialization
	void Awake () {
		text = GetComponentInChildren<Text>();
		text.text = "What I should let you do today?";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
