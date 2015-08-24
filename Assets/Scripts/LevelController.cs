using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class LevelController : MonoBehaviour {

	public enum GAME_STATE{
		PLAYING,
		WIN,
		LOSE,
		WAITING
	};

	public enum ACTOR_NAMES{
		BLOND_BOMB,
		GREEN_GOON,
		BLUE_BLOKE,
		RED_RUBE,
		PURPLE_PERSON,
		ANY
	};

	string[] actor_names = new string[6]{
		"Sporty Spice",
		"Mack Eroni",
		"Bobby Blue",
		"Miss Strict",
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

	public enum DIFFICULTY{
		EASY,
		MEDIUM,
		HARD,
	}

	public List<Camera> cameras = new List<Camera>();

	public AudioSource audioSource;
	public AudioMixerGroup directorChannel;
	public float minDelayTimeTalking = 0.0f;
	public float maxDelayTimeTalking = 3.0f;
	public List<AudioClip> clips;

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
	public bool gameOver = false;
	public TimeUI uiUpdater;
	public TaskUIScript uiTaskUpdater;
	public Text cameraUIText;
	public Text lvlCounterText;
	public int lvlCounter = 1;

	public GAME_STATE gameState;

	float levelTimeLimitTimer;
	bool timerFinished = true;
	bool initializeLevel = false;

	int previousCameraIndex = -1;
	int currCameraIndex = -1;

	public void PlaySound(int indexSound, float delay)
	{
		if(indexSound >= clips.Count)
		{
			Debug.LogError("Index for audio is wrong");
		}
		else
		{
			Debug.Log("Playing director voice: " + indexSound); 
			if(audioSource.isPlaying)
			{
				audioSource.Stop();
			}
			audioSource.clip = clips[indexSound];
			audioSource.PlayDelayed(delay);
		}
		
	}

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
				if(cam.transform.parent)
				{
					BoxCollider b=cam.transform.parent.GetComponentInChildren<BoxCollider>();
					if(b.bounds.Intersects(actor.GetComponent<CapsuleCollider>().bounds))
					{
						sit.actorNames.Add(actor.actorName);
						sit.actorStates.Add (actor.GetActorFearState());
					}
				}
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

	void RandomizeTasks(DIFFICULTY diff )
	{
		tasks.Clear();
		taskIsDone.Clear();
		Map map = GameObject.FindObjectOfType<Map> ();
		for (int i=0; i < numTasks; ++i) {
			Situation task = new Situation ();

			int complexity =1;
			if(diff== DIFFICULTY.MEDIUM)
			{
				complexity =2;
			}
			else if(diff == DIFFICULTY.HARD)
			{
				complexity =3;
			}
			for(int j=0; j< complexity; ++j)
			{
				task.actorNames.Add( activeActors[Random.Range (0,activeActors.Count)]);
				task.actorStates.Add( ConvertIntToState(Random.Range (0, actor_states.Length)));
			}
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

		uiUpdater = GameObject.FindGameObjectWithTag("TimerUI").GetComponent<TimeUI>();

		if(uiUpdater == null)
		{
			Debug.LogError("UI Game Timer not found");
		}

		uiTaskUpdater =  GameObject.FindGameObjectWithTag("Clapboard").GetComponent<TaskUIScript>();

		if(uiTaskUpdater == null)
		{
			Debug.LogError("UI Task Updater not found");
		}

		map = GameObject.FindGameObjectWithTag("MapTile").GetComponent<Map>();

		if(uiTaskUpdater == null)
		{
			Debug.LogError("Map not found");
		}

		lvlCounterText = GameObject.FindGameObjectWithTag("LvlCounterUI").GetComponent<Text>();

		if(lvlCounterText == null)
		{
			Debug.LogError("LvlCounterUINotFound");
		}

	}

	private void ResetLevel()
	{
		map.ResetLevel ();
		GameObject[] uiGauges = GameObject.FindGameObjectsWithTag("GaugeBar");
		if(uiGauges == null)
		{
			Debug.LogError("Cannot Find gauges for erasing");
		}
		else
		{
			if(uiGauges.Length != 0)
			{
				for (int i = 0; i < uiGauges.Length; i++) {
					GameObject.DestroyImmediate(uiGauges[i]);
				}
			}
		}
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
		RandomizeTasks(DIFFICULTY.EASY);
		uiUpdater.ResetGauge();

		lvlCounterText.text = "Lvl #"+lvlCounter.ToString();

		ResetMiniCamera();
	
		levelTimeLimitTimer = 0.0f;
		timerFinished = false;
		Debug.Log ("Starting Timer");
		checkTasks = true;
		levelTimeLimitTimer = 0.0f;

		gameState = GAME_STATE.PLAYING;

		PlaySound(0,0f);
	}

	IEnumerator PrepareNewLevel()
	{
		yield return new WaitForSeconds(7.0f);
		GameObject.Find ("Camera").GetComponent<GameOver>().Reset();
		ResetLevel();
		StartNewLevel();
	}

	// Use this for initialization
	void Start () {
		//RandomizeTasks ();
		checkTasks = false;
		tasks = new List<Situation>();
		audioSource = GetComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.outputAudioMixerGroup = directorChannel;
	}
	
	// Update is called once per frame
	void Update () {
		if(initializeLevel)
		{
			initializeLevel = false;
			lvlCounter = 1;
			InitializeUIElements();
			ResetLevel();
			StartNewLevel();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			gameObject.GetComponent<GameOver>().GameOverActivate(true);
			//ResetLevel();
			//StartNewLevel();
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
						PlaySound(Random.Range(1,4),0);
					}
				} else {
					numTasksDone ++;
				}
				UIText += ParseSituation (tasks[i],taskIsDone [i]);
			}
			uiTaskUpdater.SetCurrentTask(UIText);
			if (numTasksDone == tasks.Count) {
				PlaySound(7,0);
				Debug.Log ("YOU WIN");
				gameState = GAME_STATE.WIN;
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
					gameState = GAME_STATE.LOSE;
					PlaySound(Random.Range(4,7),0);
					uiTaskUpdater.LoseGameText();
				}
			}

			if(uiUpdater != null)
				uiUpdater.UpdateUI(1.0f - levelTimeLimitTimer/levelTimeLimit);
			else Debug.Log ("UI Updater not found");			
		}

		if(gameState == GAME_STATE.PLAYING)
		{

		}
		else if(gameState == GAME_STATE.WIN)
		{
			GameObject.Find ("Camera").GetComponent<GameOver>().GameOverActivate(true);
			StartCoroutine(PrepareNewLevel());
			lvlCounter++;
			gameState = GAME_STATE.WAITING;
		}
		else if(gameState == GAME_STATE.LOSE)
		{
			//Call Losing Animation
			GameObject.Find ("Camera").GetComponent<GameOver>().GameOverActivate(false);
			GameObject.FindGameObjectWithTag("UI").GetComponent<ShowPanels>().EnableGameOverButtons();
			GameObject.FindGameObjectWithTag("UI").GetComponent<ShowPanels>().GameOverActivate();
			gameState = GAME_STATE.WAITING;
		}
		else if(gameState == GAME_STATE.WAITING)
		{

		}

	}
}
