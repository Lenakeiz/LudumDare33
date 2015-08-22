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
	public int numTasks;
	public float levelTimeLimit;
	float levelTimeLimitTimer;

	bool CheckConditiosnEASY(Situation s, int task)
	{

		    if(tasks[task].cameraNum != -1 && s.cameraNum != tasks [task].cameraNum) {
			return false;
			
		}
		int matches = 0;
		for (int i = 0; i < s.actorNames.Count; ++i) {
			for(int j = 0; j < tasks[task].actorNames.Count; ++j)
			{
				if(s.actorNames[i] == tasks[task].actorNames[j] &&
				   s.actorStates[i] == tasks[task].actorStates[j])
				{
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
				Debug.Log("I SEE A GUY");
				sit.actorNames.Add(actor.actorName);
				sit.actorStates.Add (actor.GetActorFearState());
			}
		}
		sit.cameraNum = cameraNumber;
		return sit;
	}

	public string ParseSituation(Situation s)
	{
		string sitString = "I want you to bring " + GetName(s.actorNames [0]) + " looking " + GetName(s.actorStates [0]);
		for (int i = 1; i < s.actorNames.Count; ++i) {
			sitString += ", and " + GetName(s.actorNames[i]) + " looking " + GetName(s.actorStates[i]);
		}
		sitString += " infront of " + cameraNames + s.cameraNum;
		Debug.Log (sitString);
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

	void RandomizeTasks()
	{
		tasks = new List<Situation> ();
		for (int i=0; i < numTasks; ++i) {
			Situation task = new Situation ();
			task.actorNames.Add( ConvertIntToName(Random.Range (0, actor_names.Length)));
			task.actorStates.Add( ConvertIntToState(Random.Range (0, actor_states.Length)));
			task.cameraNum = Random.Range (0, cameras.Count);
			ParseSituation(task);
			tasks.Add(task);
		}
	}

	// Use this for initialization
	void Start () {
		RandomizeTasks ();
	}
	
	// Update is called once per frame
	void Update () {
		BuildSituation (0);
	}
}
