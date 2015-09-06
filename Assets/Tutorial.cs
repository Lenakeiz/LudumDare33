using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialInfo
{
	public string Text;
	public MovieTexture Movie;
}

public class Tutorial : MonoBehaviour {

	public List<TutorialInfo> TutorialStructure;

	public GameObject director;

	private GameObject cam;
	public Transform initialPos;
	public Transform finalPos;

	private int currCounter = 0;

	private Text currText;
	private RawImage currMovieTexture;
	private bool startTrigger = false;
	private bool startTutorial = false;

	private bool locker = false;
	// Use this for initialization

	public void Next()
	{
		locker = true;
		if(TutorialStructure.Count != 0 && currCounter < TutorialStructure.Count)
		{
			currText.text = TutorialStructure[currCounter].Text;
			
			if(TutorialStructure[currCounter].Movie != null)
			{
				MovieTexture currMovie = currMovieTexture.texture as MovieTexture;
				if(currMovie != null)
				{
					if(currMovie.isPlaying)
					{
						currMovie.Stop();
					}
				}				
				currMovieTexture.texture = TutorialStructure[currCounter].Movie;
				TutorialStructure[currCounter].Movie.Play();				
			}

			if(currCounter == 0)
			{
				//Show next button here
			}
			currCounter++;
		}
		else
		{
			//Go back to main menu
			director.SetActive(false);
			currCounter = 0;
			cam.transform.position = initialPos.position;
			cam.transform.rotation = initialPos.rotation;
			GameObject.FindGameObjectWithTag("UI").GetComponent<ShowPanels>().ShowMenu();
			gameObject.SetActive(false);
		}
	}

	public void SetInitializeTrigger()
	{
		startTrigger = true;
		director.SetActive(true);
	}

	public void InitializeTutorial()
	{
		GameObject ballon = gameObject.transform.FindChild("Ballon").gameObject;
		if(ballon == null)
		{
			Debug.LogError("Text for tutorial not found");
		}
		else
		{
			currText = ballon.GetComponentInChildren<Text>();
		}

		GameObject movieScreen = gameObject.transform.FindChild("TutorialVideo").gameObject;
		if(movieScreen == null)
		{
			Debug.LogError("MovieScreen not found");
		}
		else
		{
			currMovieTexture = movieScreen.GetComponentInChildren<RawImage>();
		}

		startTutorial = true;
	}

	void Awake() {
		cam = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
		if(startTrigger)
		{
			startTrigger = false;
			InitializeTutorial();

			//StartCoroutine to move camera towards director;

			//At the end of the coroutine activate the panel Text and movie

		}
		else if(startTutorial)
		{
			startTutorial = false;
			cam.transform.position = finalPos.position;
			cam.transform.rotation = finalPos.rotation;

			Next();

		}

		if(locker)
			locker = false;


	}
}
