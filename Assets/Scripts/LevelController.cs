using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelController : MonoBehaviour {
	public enum ACTOR_NAMES{
		BLOND_BOMB,
		GREEN_GOON,
		BLUE_BLOKE,
		RED_RUBE,
		PURPLE_PERSON,
		ANY
	};

	string[] actor_names = new string[6]{
		"Blondie",
		"Green Goon",
		"Axecop",
		"Red Rube",
		"Purple Person",
		"anybody",
	};

	public enum ACTOR_STATES{
		NORMAL,
		SPOOKED,
		PANICKED,
		FAINTED
	}

	string[] actor_states = new string[4]{
		"normal",
		"spooked",
		"panicked",
		"unconscious"
	};

	public List<Camera> cameras = new List<Camera>();


	public class Situation
	{
		public Situation()
		{
			actorNames = new List<ACTOR_NAMES>();
			actorStates = new List<ACTOR_STATES>();
		}
		public int cameraNum;
		public List<ACTOR_NAMES> actorNames;
		public List<ACTOR_STATES> actorStates;

	};

	string cameraNames = "Camera #";

	public List<Situation> tasks;
	public List<ACTOR_NAMES> activeActors;
	public List<bool> taskIsDone;
	public int numTasks;
	public float levelTimeLimit = 15.0f;
	public Map map;

	public bool checkTasks = false;
	public bool lostGame = false;
	public TimeUI uiUpdater;
	public TaskUIScript uiTaskUpdater;
	public Text cameraUIText;

	float levelTimeLimitTimer;
	bool timerFinished = true;
	bool initializeLevel = false;

	int previousCameraIndex = -1;
	int currCameraIndex = -1;

	bool CheckConditiosnEASY(Situation s, int task)
	{

		    if(tasks[task].cameraNum != -1 && s.cameraNum != tasks [task].cameraNum) {
			return false;
			
		}
		int matches = 0;
		for (int i = 0; i < s.actorNames.Count; ++i) {
			for(int j = 0; j < tasks[task].actorNames.Count; ++j)
			{
				if((s.actorNames[i] == tasks[task].actorNames[j] || tasks[task].actorNames[j] == ACTOR_NAMES.ANY) &&
				   s.actorStates[i] == tasks[task].actorStates[j])
				{
					Debug.Log(s.actorNames[i] + " : " + tasks[task].actorNames[j]);
					Debug.Log(s.actorStates[i] + " : " + tasks[task].actorStates[j]);
					matches++;
				}
			}
		}
		if (matches != tasks [task].actorNames.Count) {
			return false;
		}
		return true;
	}

	public Situation BuildSituation(int cameraNumber)
	{
		Camera cam = cameras [cameraNumber];

		Situation sit = new Situation ();

		Plane[] planes = GeometryUtility.CalculateFrustumPlanes (cam);

		GameObject[] actors = GameObject.FindGameObjectsWithTag ("Actor");
		foreach (GameObject g in actors) {
			Actors actor = g.GetComponent<Actors>();
			if(actor == null)
			{
				continue;
			}
			if(GeometryUtility.TestPlanesAABB(planes,
			                                  actor.GetComponent<CapsuleCollider>().bounds))
			{

					//Debug.Log("I SEE A GUY");
					sit.actorNames.Add(actor.actorName);
					sit.actorStates.Add (actor.GetActorFearState());
			}
		}
		sit.cameraNum = cameraNumber;
		return sit;
	}

	public string ParseSituation(Situation s, bool done)
	{
		string sitString = done ? "[X]" : "[]";
		sitString += "Bring " + GetName(s.actorNames [0]) + " looking " + GetName(s.actorStates [0]);
		/*
		for (int i = 1; i < s.actorNames.Count; ++i) {
			sitString += ", and " + GetName(s.actorNames[i]) + " looking " + GetName(s.actorStates[i]);
		}*/
		sitString += " in front of " + cameraNames + (s.cameraNum+1) + "\n";
		return sitString;

	}

	public string GetName(ACTOR_NAMES name)
	{
		if (name == ACTOR_NAMES.BLOND_BOMB)
			return actor_names [0];
		if (name == ACTOR_NAMES.GREEN_GOON)
			return actor_names [1];
		if (name == ACTOR_NAMES.BLUE_BLOKE)
			return actor_names [2];
		if (name == ACTOR_NAMES.RED_RUBE)
			return actor_names [3];
		if (name == ACTOR_NAMES.PURPLE_PERSON)
			return actor_names [4];
		return actor_names [5];
	}

	public string GetName(ACTOR_STATES state)
	{
		if (state == ACTOR_STATES.SPOOKED)
			return actor_states [1];
		if (state == ACTOR_STATES.PANICKED)
			return actor_states [2];
		if (state == ACTOR_STATES.FAINTED)
			return actor_states [3];

		return actor_states [0];
		
	}

	static public ACTOR_NAMES ConvertIntToName(int name)
	{
		if (name == 0)
			return ACTOR_NAMES.BLOND_BOMB;
		if (name == 1)
			return ACTOR_NAMES.GREEN_GOON;
		if (name == 2)
			return ACTOR_NAMES.BLUE_BLOKE;
		if (name == 3)
			return ACTOR_NAMES.RED_RUBE;
		if (name == 4)
			return ACTOR_NAMES.PURPLE_PERSON;
		return ACTOR_NAMES.ANY;
	}

	static public ACTOR_STATES ConvertIntToState(int state)
	{
		if (state == 1)
			return ACTOR_STATES.SPOOKED;
		if (state == 2)
			return ACTOR_STATES.PANICKED;
		return ACTOR_STATES.NORMAL;
	}

	public void ChangeMiniCamera(int newCameraIndex)
	{
		if(newCameraIndex != currCameraIndex)
		{
			previousCameraIndex = currCameraIndex;
			currCameraIndex = newCameraIndex;

			if(newCameraIndex >= cameras.Count)
			{
				Debug.LogError("Collideer is no tmapped correctly to camera");
			}
			else
			{
				cameraUIText.text = "Camera #" + (currCameraIndex + 1).ToString();
				cameras[previousCameraIndex].enabled = false;
				cameras[currCameraIndex].enabled = true;
			}

		}
	}

	void RandomizeTasks()
	{
		tasks = new List<Situation> ();
		taskIsDone = new List<bool>();
		Map map = GameObject.FindObjectOfType<Map> ();
		for (int i=0; i < numTasks; ++i) {
			Situation task = new Situation ();
			task.actorNames.Add( activeActors[Random.Range (0,activeActors.Count)]);
			task.actorStates.Add( ConvertIntToState(Random.Range (0, actor_states.Length)));
			task.cameraNum = Random.Range (0, cameras.Count);
			Debug.Log(ParseSituation(task,false));
			tasks.Add(task);
			taskIsDone.Add (false);
		}
	}

	public void InitializeLevel()
	{
		initializeLevel = true;
	}

	private void InitializeUIElements()
	{
		GameObject uielement = GameObject.FindGameObjectWithTag("UIMiniCamera");

		if(uielement == null)
		{
			Debug.LogError("MiniCamera UI not found");
			return;
		}

		cameraUIText = uielement.GetComponentInChildren<Text>();
		
		if(cameraUIText == null)
		{
			Debug.LogError("MiniCamera Text not found");
		}
	}

	private void ResetLevel()
	{
		map.ResetLevel ();
	}

	private void ResetMiniCamera()
	{
		previousCameraIndex = currCameraIndex = 0;
		cameraUIText.text = "Camera #" + (currCameraIndex + 1).ToString();
		cameras[currCameraIndex].enabled = true;
	}

	private void StartNewLevel()
	{
		activeActors = new List<ACTOR_NAMES> ();
		map.PreparePrefabs();
		RandomizeTasks();
		uiUpdater.ResetGauge();
<<<<<<< HEAD
		ResetMiniCamera();
=======
	
		levelTimeLimitTimer = 0.0f;
>>>>>>> abb00f203113f32991635a7be774a031c92a1106
		timerFinished = false;
		lostGame = false;
		Debug.Log ("Starting Timer");
		checkTasks = true;
		levelTimeLimitTimer = 0.0f;
	}

	// Use this for initialization
	void Start () {
		//RandomizeTasks ();
		checkTasks = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(initializeLevel)
		{
			initializeLevel = false;
			InitializeUIElements();
			ResetLevel();
			StartNewLevel();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			ResetLevel();
			StartNewLevel();
		}
		
		if (checkTasks && !timerFinished) {
			int numTasksDone = 0;
			string UIText =  string.Empty;
			for (int i = 0; i < tasks.Count; ++i) {
				Situation s = BuildSituation (tasks [i].cameraNum);
				if (!taskIsDone[i]) {					 
					if (CheckConditiosnEASY (s, i)) {
						taskIsDone [i] = true;
						Debug.Log (ParseSituation (s,taskIsDone[i]));
						Debug.Log ("Is done!");
					}
				} else {
					numTasksDone ++;
				}
				UIText += ParseSituation (tasks[i],taskIsDone [i]);
			}
			uiTaskUpdater.SetCurrentTask(UIText);
			if (numTasksDone == tasks.Count) {
				Debug.Log ("YOU WIN");
				uiTaskUpdater.WinGameText();
				checkTasks = false;
			}
		}

		if(!timerFinished)
		{
			levelTimeLimitTimer += Time.deltaTime;
			if(levelTimeLimitTimer > levelTimeLimit)
			{
				timerFinished = true;
				Debug.Log("Time is finished");
				if(checkTasks)
				{
					lostGame = true;
					uiTaskUpdater.LoseGameText();
				}
			}

			if(uiUpdater != null)
				uiUpdater.UpdateUI(1.0f - levelTimeLimitTimer/levelTimeLimit);
			else Debug.Log ("UI Updater not found");
			
		}



	}
}
