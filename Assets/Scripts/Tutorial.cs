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

	private GameObject ballon;
//	private Text currText;
	private GameObject movieScreen;
//	private RawImage currMovieTexture;
	private bool startTrigger = false;
	private bool startTutorial = false;

	public float lerpTimer = 0.0f;
	public float speed = 0.01f;

	private bool locker = false;
	// Use this for initialization

	public void Next()
	{
		locker = true;
		if(TutorialStructure.Count != 0 && currCounter < TutorialStructure.Count)
		{
			ballon.GetComponentInChildren<Text>().text = TutorialStructure[currCounter].Text;
			MovieTexture currMovie = movieScreen.GetComponentInChildren<RawImage>().texture as MovieTexture;
			if(currMovie != null)
			{
				if(currMovie.isPlaying)
				{
					currMovie.Stop();
					movieScreen.GetComponentInChildren<RawImage>().color = new Color(0.0f,0.0f,0.0f);
				}
			}	
			if(TutorialStructure[currCounter].Movie != null)
			{							
				movieScreen.GetComponentInChildren<RawImage>().texture = TutorialStructure[currCounter].Movie;
				movieScreen.GetComponentInChildren<RawImage>().color = new Color(1.0f,1.0f,1.0f);
				TutorialStructure[currCounter].Movie.loop = true;
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
			MovieTexture currMovie = movieScreen.GetComponentInChildren<RawImage>().texture as MovieTexture;
			if(currMovie != null)
			{
				if(currMovie.isPlaying)
				{
					currMovie.Stop();
				}
			}	
			currMovie = null;
			lerpTimer = 0.0f;
			director.SetActive(false);
			currCounter = 0;
			cam.transform.position = initialPos.position;
			cam.transform.rotation = initialPos.rotation;
			GameObject.FindGameObjectWithTag("UI").GetComponent<ShowPanels>().ShowMenu();
			ballon.SetActive(false);
			movieScreen.SetActive(false);
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
		ballon = gameObject.transform.FindChild("Ballon").gameObject;
		if(ballon == null)
		{
			Debug.LogError("Text for tutorial not found");
		}

		movieScreen = gameObject.transform.FindChild("TutorialVideo").gameObject;
		if(movieScreen == null)
		{
			Debug.LogError("MovieScreen not found");
		}
	}

	IEnumerator MoveCamera()
	{

		float initialAngle = initialPos.transform.eulerAngles.x;
		float finalAngle = finalPos.transform.eulerAngles.x;
		while (lerpTimer < 1)
		{
			lerpTimer += Time.deltaTime * speed;
			cam.transform.position = Vector3.Lerp(initialPos.transform.position, finalPos.transform.position, lerpTimer);

			float calculatedAngle = Mathf.LerpAngle(initialAngle,finalAngle,lerpTimer);
			cam.transform.eulerAngles = new Vector3(calculatedAngle, 0 ,0);
			yield return null;
		}

		cam.transform.position = finalPos.transform.position;
		cam.transform.eulerAngles = new Vector3(finalPos.transform.eulerAngles.x, 0 ,0);
			 
		yield return null;

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
			StartCoroutine(MoveCamera());
			//At the end of the coroutine activate the panel Text and movie

		}
		else if(startTutorial)
		{
			startTutorial = false;
			ballon.SetActive(true);
			movieScreen.SetActive(true);

			cam.GetComponent<LevelController>().PlaySound(3,0.0f);
			Next();

		}

		if(locker)
			locker = false;


	}
}
