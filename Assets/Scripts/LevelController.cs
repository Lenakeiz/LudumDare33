using UnityEngine;
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
		"Blond Bomb",
		"Green Goon",
		"Blue Bloke",
		"Red Rube",
		"Purple Person",
		"any",
	};

	public enum ACTOR_STATES{
		NORMAL,
		SPOOKED,
		PANICKED,
	}

	string[] actor_states = new string[3]{
		"normal",
		"spooked",
		"panicked",
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
	public int numTasks;
	public float levelTimeLimit;
	float levelTimeLimitTimer;

	bool CheckConditions(Situation s, int task)
	{
		if (s.actorNames.Count != tasks [task].actorNames.Count ||
			s.actorStates.Count != tasks [task].actorStates.Count ||
			s.cameraNum != tasks [task].cameraNum) {
			return false;

		}
		for (int i = 0; i < s.actorNames.Count; i++) {
			if(s.actorNames[i] != tasks[task].actorNames[i])
			{
				return false;
			}
		}
		for (int i = 0; i< s.actorStates.Count; ++i) {
			if(s.actorStates[i] != tasks[task].actorStates[i])
			{
				return false;
			}
		}
		return true;
	}

	public void BuildSituation(int cameraNumber)
	{
		Camera cam = cameras [cameraNumber];

		Plane[] planes = GeometryUtility.CalculateFrustumPlanes (cam);

		GameObject[] actors = GameObject.FindGameObjectsWithTag ("Actor");
		foreach (GameObject g in actors) {
			
		
		}

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

	void RandomizeTasks()
	{
		tasks = new List<Situation> ();
		for (int i=0; i < numTasks; ++i) {
			Situation task = new Situation ();
			task.actorNames.Add( ConvertIntToName(Random.Range (0, actor_names.Length)));
			task.actorStates.Add( ConvertIntToState(Random.Range (0, actor_states.Length)));
			task.cameraNum = Random.Range (0, cameras.Count);
			tasks.Add(task);
		}
	}

	// Use this for initialization
	void Start () {
		RandomizeTasks ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
