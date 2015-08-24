using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Actors : MonoBehaviour {

	public Sprite Face;

	public int uiIndex;
	public BarScript barScript;

	public Vector2 InitialOnGUIPos;

	public enum ACTOR_STATE
	{
		CHOOSING,
		MOVING,
		TALKING,
		HAUNTED,
		FIENTED
	}

	public enum ACTOR_DIRECTION{
		NONE =-1,
		UP =0,
		DOWN = 1,
		LEFT = 2,
		RIGHT = 3,
	}

	public LevelController.ACTOR_NAMES actorName;
	public ACTOR_STATE state = ACTOR_STATE.CHOOSING;
	public ACTOR_DIRECTION moveDirection = ACTOR_DIRECTION.NONE;
	public bool forceDirection = false;

	public AudioSource audioSource;
	public AudioMixerGroup sfx;
	public float minDelayTimeTalking = 0.0f;
	public float maxDelayTimeTalking = 3.0f;
	public List<AudioClip> clips;

	public string talkAnimation ="Happy 01";

	public float talkCooldown = 5.0f;
	float talkCooldownTimer =0.0f;
	public Actors talkTarget; 

	public Tile currentTile;
	Tile previousTile;

	public bool isPanicking;

	public float fear = 0;
	public float fearSpookedAmount =30;
	public float fearPanicAmount = 60;
	public float fearReductionPerSecond = 1.0f;
	private string fearStatus;

	public float movementSpeed = 1.0f;
	public float spookedSpeed = 1.4f;
	public float panicSpeed = 2.0f;
	float movementT = 0.0f;
	Tile movementTarget;

	public float maxRoamingWaitTime;
	public float minRoamingWaitTime;

	float roamingWaitTime =0.0f;
	float roamingT = 0.0f;

	private Astar pathHelper;
	public int markedTilesPercentage = 35;

	//private AudioManager audioManager;

	public void PlaySound(int indexSound, float delay)
	{
		if(indexSound >= clips.Count)
		{
			Debug.LogError("Index for audio is wrong");
		}
		else
		{
			if(audioSource.isPlaying)
			{
				audioSource.Stop();
			}
			audioSource.clip = clips[indexSound];
			audioSource.PlayDelayed(delay);
		}

	}

	float GetSpeed()
	{
		if (forceDirection) {
			return spookedSpeed;
		}

		LevelController.ACTOR_STATES state = GetActorFearState ();
		if (state == LevelController.ACTOR_STATES.PANICKED) {
			return panicSpeed;
		}
		if (state == LevelController.ACTOR_STATES.SPOOKED) {
			return spookedSpeed;
		}
		return movementSpeed;
	}


	public LevelController.ACTOR_STATES GetActorFearState()
	{
		if (fear > fearPanicAmount) {

			return LevelController.ACTOR_STATES.PANICKED;
		}
		if (fear > fearSpookedAmount) {

			return LevelController.ACTOR_STATES.SPOOKED;
		}
		if (fear >= 100) {
			return LevelController.ACTOR_STATES.FAINTED;
		}

		return LevelController.ACTOR_STATES.NORMAL;

	}

	static ACTOR_DIRECTION GetOppositeDirection( ACTOR_DIRECTION dir)
	{
		if (dir == ACTOR_DIRECTION.DOWN) {
			return ACTOR_DIRECTION.UP;
		} else if (dir == ACTOR_DIRECTION.UP) {
			return ACTOR_DIRECTION.DOWN;
		} else if (dir == ACTOR_DIRECTION.LEFT) {
			return ACTOR_DIRECTION.RIGHT;
		} else if (dir == ACTOR_DIRECTION.RIGHT) {
			return ACTOR_DIRECTION.LEFT;
		}
		return ACTOR_DIRECTION.NONE;
	}

	public static ACTOR_DIRECTION IntToDirection(int dir)
	{
		if (dir == 0)
			return ACTOR_DIRECTION.UP;
		else if (dir == 1)
			return ACTOR_DIRECTION.DOWN;
		else if (dir == 2)
			return ACTOR_DIRECTION.LEFT;
		else if (dir == 3) 
			return ACTOR_DIRECTION.RIGHT;
		

		return ACTOR_DIRECTION.NONE;
	}
	
	public void AddFear (float argFear)
	{
		//we can trigger animations in here
		fear += argFear;
	}

	void OnTriggerEnter(Collider gameobject)
	{
		if (gameobject.tag == "Actor") {
			Actors a = gameobject.GetComponent<Actors>();
			if ((a.state == ACTOR_STATE.MOVING || a.state == ACTOR_STATE.CHOOSING) &&
			    a.talkCooldownTimer ==0 && this.talkCooldownTimer == 0 && !a.forceDirection &&
			    !forceDirection)
			{
				a.talkTarget = this;
				talkTarget = a;
			}
		}
	}


	// Use this for initialization
	void Start () {
		this.transform.position = currentTile.characterPosition;
		this.talkTarget = null;
		pathHelper = gameObject.GetComponent<Astar>() as Astar;

		audioSource = GetComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.outputAudioMixerGroup = sfx;
//		audioManager = GameObject.FindObjectOfType<AudioManager>();
//
//		if(audioManager == null)
//		{
//			Debug.LogError("Audio Manager is not in the scene");
//		}

		GameObject uiElement = GameObject.FindGameObjectWithTag("UILife");
		if(uiElement != null)
		{
			Transform targetChild = uiElement.transform.GetChild(uiIndex);
			if(targetChild != null)
			{
				barScript = targetChild.gameObject.GetComponentInChildren<BarScript>();
				barScript.SetName(gameObject.name);
			}
			else{Debug.LogError("UI Child Not Found");};
		}
		else{Debug.LogError("UI for lifes not found");}

		GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
		Tile currTile = null;
		for (int i = 0; i < tiles.Length; i++) {
			currTile = tiles[i].GetComponent<Tile>();
			if(currTile.useAstar || Random.Range(0,101) < markedTilesPercentage)
			{
				pathHelper.AddTile(currTile);
			}
		}
		roamingT = 0f;
	}

	public void ScreamedAt(ACTOR_DIRECTION dir)
	{
		moveDirection = dir;
		forceDirection = true;
		state = ACTOR_STATE.MOVING;
		gameObject.GetComponent<Animator>().Play("Run 01");
		talkTarget = null;

	}

	// Update is called once per frame
	void Update () {

		if(barScript != null)
		{
			if(fear < fearSpookedAmount)
			   fearStatus = "Normal";
			else if (fear >= fearSpookedAmount && fear < fearPanicAmount)
				fearStatus = "Spooked";
			else if(fear >= fearPanicAmount && fear < 100)
				fearStatus = "PANIC!";
			else if(fear >= 100)
				fearStatus = "FAINTED!";
		   barScript.SetAmount(fear * 0.01f, fearStatus);
		}

		if (fear > fearPanicAmount) {
			isPanicking = true;
		} else {
			isPanicking = false;
		}
		if (fear >= 100 && state != ACTOR_STATE.FIENTED) {
			PlaySound(4,0.0f);
			gameObject.GetComponent<Animator>().Play("Fall 03");
			state = ACTOR_STATE.FIENTED;
		}
		if (fear > 0) {
			fear -= Time.deltaTime * fearReductionPerSecond;
		}
		fear = Mathf.Max(fear, 0);	

		if (talkCooldownTimer > 0) {
			talkCooldownTimer -= Time.deltaTime;
			if(talkCooldownTimer<0)
			{
				talkCooldownTimer = 0;
			}
		}



		if (state == ACTOR_STATE.MOVING) 
		{
			movementT += Time.deltaTime * GetSpeed();
			if(movementT < 1)
			{
				this.transform.position = Vector3.Lerp(currentTile.characterPosition,
				                                       movementTarget.characterPosition,
				                                       movementT);
			}
			else
			{
				previousTile = currentTile;
				currentTile = movementTarget;
				movementT = 0;
				//movementTarget = null;

				currentTile.occupant = this.gameObject;
				if(currentTile.effectsOnTile == Tile.TILE_EFFECTS.HAUNT)
				{
					GameObject player = GameObject.FindGameObjectWithTag("Player");
					if(player)
					{
						Haunt h = player.GetComponent<Haunt>();
						moveDirection = h.HauntActor(this);
						forceDirection = true;
						PlaySound(3,0);
						gameObject.GetComponent<Animator>().Play("Run 01");
					}
				}

				if(talkTarget != null)
				{
					this.GetComponent<Animator>().Play(talkAnimation);
					state = ACTOR_STATE.TALKING;
				}


				//state = ACTOR_STATE.CHOOSING;
				roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
				if(!forceDirection)
				{

					movementTarget = pathHelper.GetNextMove();
				}
				if(forceDirection)
				{
					state = ACTOR_STATE.MOVING;
					if(moveDirection== ACTOR_DIRECTION.UP)
					{
						if(currentTile.up)
						{
							movementTarget = currentTile.up;
						}
						else{
							forceDirection = false;
							state = ACTOR_STATE.CHOOSING;
						}
					}
					else if(moveDirection == ACTOR_DIRECTION.DOWN)
					{
						if(currentTile.down)
						{
							movementTarget = currentTile.down;
						}
						else{
							forceDirection = false;
							state = ACTOR_STATE.CHOOSING;
						}
					}
					else if(moveDirection== ACTOR_DIRECTION.LEFT)
					{
						if(currentTile.left)
						{
							movementTarget = currentTile.left;
						}
						else{
							forceDirection = false;
							state = ACTOR_STATE.CHOOSING;
						}
					}
					else if(moveDirection == ACTOR_DIRECTION.RIGHT)
					{
						if(currentTile.right)
						{
							movementTarget = currentTile.right;
						}
						else{
							forceDirection = false;
							state = ACTOR_STATE.CHOOSING;
						}
					}
				}

				if(movementTarget == null || movementTarget.effectsOnTile == Tile.TILE_EFFECTS.CHILL)
				{
					state = ACTOR_STATE.CHOOSING;
				}
				else{
					transform.LookAt(movementTarget.characterPosition,Vector3.up);
				}
				if(isPanicking)
				{
					roamingWaitTime = 0.1f;
				}
			}
		}
		if (state == ACTOR_STATE.CHOOSING)
		{
			if(roamingT == 0)
			{
				LevelController.ACTOR_STATES fearState = GetActorFearState();
				if(fearState == LevelController.ACTOR_STATES.NORMAL)
				{
					string str = "Idle 0"+Random.Range(1,4);
					Debug.Log(str);
					gameObject.GetComponent<Animator>().Play(str);
				}
				if(fearState == LevelController.ACTOR_STATES.SPOOKED)
					gameObject.GetComponent<Animator>().Play("Scared 01");
				if(fearState == LevelController.ACTOR_STATES.PANICKED)
					gameObject.GetComponent<Animator>().Play("Scared 02");
			}
			roamingT += Time.deltaTime;
			if(roamingT > roamingWaitTime)
			{
				//moveDirection = MakeChoice();
				roamingT = 0;
				for(int i =0; i < 10; i++)
				{
					pathHelper.RequestNewRandomPath(currentTile);
					movementTarget =pathHelper.GetNextMove();
					if(movementTarget == null)
					{
						continue;
					}
					if(movementTarget.effectsOnTile == Tile.TILE_EFFECTS.CHILL)
					{
						Collider[] colliders = Physics.OverlapSphere(this.transform.position,3);
						foreach(Collider col in colliders)
						{
							if(col.tag=="Tile" && col.GetComponent<Tile>().effectsOnTile!=
							   Tile.TILE_EFFECTS.CHILL)
							{
								pathHelper.RequestPath(currentTile,col.GetComponent<Tile>());
								movementTarget = pathHelper.GetNextMove();
							}
						}
					}
				}
				if(movementTarget == null)
				{
					state = ACTOR_STATE.CHOOSING;
					return;
				}
				transform.LookAt(movementTarget.characterPosition,Vector3.up);
				currentTile.occupant = null;
				if(GetActorFearState()==LevelController.ACTOR_STATES.NORMAL &&
				   !forceDirection)
				{
					gameObject.GetComponent<Animator>().Play("Walk 01");
				}
				else{
					gameObject.GetComponent<Animator>().Play("Run 01");
				}
				state = ACTOR_STATE.MOVING;
			}

		}
		if (state == ACTOR_STATE.TALKING) {

			//PLAY TALK ANIMATION
			transform.LookAt(talkTarget.transform.position,Vector3.up);
			if(this.currentTile == talkTarget.currentTile)
			{
				state = ACTOR_STATE.MOVING;
				movementTarget = previousTile;
			}
			if(currentTile.effectsOnTile == Tile.TILE_EFFECTS.HAUNT)
			{
				GameObject player = GameObject.FindGameObjectWithTag("Player");
				if(player)
				{
					Haunt h = player.GetComponent<Haunt>();
					moveDirection = h.HauntActor(this);
					forceDirection = true;
					talkTarget = null;
					talkCooldownTimer = talkCooldown;
					gameObject.GetComponent<Animator>().Play("Run 01");
					state = ACTOR_STATE.MOVING;
					PlaySound(3,0);
				}
			}
			if(talkTarget == null || talkTarget.talkTarget == null || talkTarget.talkTarget != this||
			   currentTile.effectsOnTile == Tile.TILE_EFFECTS.CHILL)
			{
				talkTarget = null;
				state = ACTOR_STATE.CHOOSING;
				talkCooldownTimer = talkCooldown;
			}

			if(!audioSource.isPlaying)
			{
				PlaySound(Random.Range(0,3),Random.Range(minDelayTimeTalking,maxDelayTimeTalking));
			}

		}

	}
}
