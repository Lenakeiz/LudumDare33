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
		NONE,
		CHOOSING,
		MOVING,
		GETNEXTTILE,
		TALKING,
		CHILLED,
		HAUNTED,
		FIENTED
	}

	public enum ACTOR_DIRECTION{
		NONE = -1,
		UP = 0,
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
	public float talkRefresh = 5.0f;
	float talkRefreshTimer = 0.0f;
	float talkCooldownTimer =0.0f;
	public Actors talkTarget; 

	public Tile currentTile;
	Tile previousTile;

	public bool isPanicking;

	public float fear = 0;
	public float fearSpookedAmount =40;
	public float fearPanicAmount = 70;
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

	private int currIdleAnimationPlaying = 1;

	public ACTOR_STATE processedState = ACTOR_STATE.NONE;

	public string scaredAnimation = string.Empty;
	private bool deActivateChilled = false;

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
		else if (fear > fearSpookedAmount) {

			return LevelController.ACTOR_STATES.SPOOKED;
		}
		else if (fear >= 100) {
			return LevelController.ACTOR_STATES.FAINTED;
		}
		else
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

	/// <summary>
	/// We will detect the characters talking in this collision detection
	/// </summary>
	/// <param name="gameobject">Gameobject.</param>
	void OnTriggerEnter(Collider gameobject)
	{
		if (gameobject.tag == "Actor") {
			Actors a = gameobject.GetComponent<Actors>();
			if ((a.state == ACTOR_STATE.MOVING || a.state == ACTOR_STATE.CHOOSING || a.state == ACTOR_STATE.GETNEXTTILE) &&
			    Mathf.Approximately(a.talkCooldownTimer,talkCooldown) && Mathf.Approximately(this.talkCooldownTimer,talkCooldown) 
			    && !a.forceDirection && !forceDirection
			    && talkTarget == null && a.talkTarget == null
			    && Mathf.Approximately(talkRefreshTimer,0.0f) && Mathf.Approximately(a.talkRefreshTimer,0.0f))
			{
				if (Random.Range(0,11) > 5)
				{
					a.talkTarget = this;
					talkTarget = a;
				}
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
		TryForceActorToNextTile();
		if(forceDirection)
		{
			transform.LookAt(movementTarget.characterPosition,Vector3.up);
			gameObject.GetComponent<Animator>().Play("Run 01");
			state = ACTOR_STATE.MOVING;
		}
		else
		{
			Debug.LogError("Screamed Actor in a non valid direction");
			state = ACTOR_STATE.CHOOSING;
		}
		talkRefreshTimer = talkRefresh;
		talkCooldownTimer = talkCooldown;
		ResetTalkingActor();
	}

	private ACTOR_STATE CheckTileState()
	{
		processedState = ACTOR_STATE.NONE;
		switch (currentTile.effectsOnTile) {
			
			case Tile.TILE_EFFECTS.CHILL | Tile.TILE_EFFECTS.HAUNT:
			case Tile.TILE_EFFECTS.HAUNT:
				GameObject player = GameObject.FindGameObjectWithTag("Player");
				if(player)
				{
					Haunt h = player.GetComponent<Haunt>();
					moveDirection = h.HauntActor(this);
					forceDirection = true;
					PlaySound(3,0);
					gameObject.GetComponent<Animator>().Play("Run 01");
					
				}
				else
				{
					Debug.LogError("Found haunted tile but no player for getting the haunt direction");
				}
				processedState = ACTOR_STATE.HAUNTED;
				break;
			case Tile.TILE_EFFECTS.CHILL:
				processedState = ACTOR_STATE.CHILLED;
				break;
			default:
				processedState = ACTOR_STATE.NONE;
				break;
		}

		return processedState;
	}

	private void TryForceActorToNextTile()
	{
		if(moveDirection== ACTOR_DIRECTION.UP)
		{
			if(currentTile.up)
			{
				movementTarget = currentTile.up;
			}
			else{
				forceDirection = false;
				moveDirection = ACTOR_DIRECTION.NONE;
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
				moveDirection = ACTOR_DIRECTION.NONE;
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
				moveDirection = ACTOR_DIRECTION.NONE;
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
				moveDirection = ACTOR_DIRECTION.NONE;
			}
		}
	}

	public void ResetTalkingActor()
	{
		if(talkTarget != null)
		{
			if(talkTarget.talkTarget !=  null)
			{
				talkTarget.talkTarget = null;
			}
			else 
			{
				Debug.LogError("Try to close conversation but talking target has already talking actor set to null");
			}
			talkTarget = null;
		}
	}

	public void DeactivateChill()
	{
		deActivateChilled = true;
	}

	// Update is called once per frame
	void Update () {


		#if UNITY_EDITOR
		if(currentTile == null)
		{
			Debug.Log(this.actorName + " no current tile");
		}
		#endif

		//Updating the UI
		if(barScript != null)
		{
			if(fear < fearSpookedAmount)
			   fearStatus = "Normal";
			else if (fear >= fearSpookedAmount && fear < fearPanicAmount)
				fearStatus = "Spooked";
			else if(fear >= fearPanicAmount && fear < 100)
				fearStatus = "PANIC!";
			else if(fear >= 100)
				fearStatus = "FAINTED!!!";
		   barScript.SetAmount(fear * 0.01f, fearStatus);
		}

		//Updating state for the player
		if (fear > fearPanicAmount) {
			isPanicking = true;
		} else {
			isPanicking = false;
		}

		if (fear >= 100 && state != ACTOR_STATE.FIENTED) {
			PlaySound(4,0.0f);
			string randoFall = "Fall 0" + Random.Range(1,4);
			gameObject.GetComponent<Animator>().Play(randoFall);
			state = ACTOR_STATE.FIENTED;
		}

		//Always decrease fear
		if (fear > 0) {
			fear -= Time.deltaTime * fearReductionPerSecond;
		}
		//Lower limit is zero
		fear = Mathf.Max(fear, 0);	

		if (state == ACTOR_STATE.FIENTED)
		{
			if(fear < 80)
			{
				roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
				roamingT = 0f;
				state = ACTOR_STATE.CHOOSING;
			}
		}
		else if (state == ACTOR_STATE.MOVING) 
		{
			if(talkRefreshTimer > 0)
			{
				talkRefreshTimer -= Time.deltaTime;
				if(talkRefreshTimer < 0)
				{
					talkRefreshTimer = 0;
				}
			}

			if(GetActorFearState() == LevelController.ACTOR_STATES.NORMAL && !forceDirection)
			{
				gameObject.GetComponent<Animator>().Play("Walk 01");
			}
			else
			{
				gameObject.GetComponent<Animator>().Play("Run 01");
			}

			movementT += Time.deltaTime * GetSpeed();
			if(movementT < 1.0f)
			{
				if(currentTile != null && movementTarget != null)
				{
					this.transform.position = Vector3.Lerp(currentTile.characterPosition,
					                                       movementTarget.characterPosition,
					                                       movementT);
				}
				else
				{
					Debug.LogError("Movement attempt without start or end tile");					
				}
			}
			else
			{
				this.transform.position = movementTarget.characterPosition;
				state = ACTOR_STATE.GETNEXTTILE;
			}

		}
		else if(state == ACTOR_STATE.GETNEXTTILE)
		{
			if(talkRefreshTimer > 0)
			{
				talkRefreshTimer -= Time.deltaTime;
				if(talkRefreshTimer < 0)
				{
					talkRefreshTimer = 0;
				}
			}

			previousTile = currentTile;
			currentTile = movementTarget;
			movementT = 0;
						
			currentTile.occupant = this.gameObject;

			//Checking if the tile is marked with something;
			//This will set Processed state
			//If none then next operation is talking or continue moving if panicked

			CheckTileState();

			switch (processedState) {

			case ACTOR_STATE.NONE:
				if(forceDirection)
				{
					//forcing actor to move in a direction, if failing forcedirection will be setted to false
					TryForceActorToNextTile();
					if(forceDirection)
					{
						transform.LookAt(movementTarget.characterPosition,Vector3.up);
						state = ACTOR_STATE.MOVING;
					}
					else
					{
						this.GetComponent<Animator>().Play("Run 06 Stop");
						roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
						roamingT = 0f;
						currIdleAnimationPlaying = Random.Range(1,5);
						state = ACTOR_STATE.CHOOSING;
					}
				}
				else
				{
					//can talk or continue to move
					if(talkTarget != null && !isPanicking && Mathf.Approximately(talkRefreshTimer,0f))
					{
						this.GetComponent<Animator>().Play(talkAnimation);
						talkCooldownTimer = talkCooldown;
						talkRefreshTimer = talkRefresh;
						state = ACTOR_STATE.TALKING;
					}
					else
					{
						if(isPanicking)
						{
							talkRefreshTimer = talkRefresh;
							talkCooldownTimer = talkCooldown;
							ResetTalkingActor();
						}
						
						movementTarget = pathHelper.GetNextMove();
						if(movementTarget == null)
						{
							
							//string str = "Idle 0"+Random.Range(1,5);
							//Debug.Log(str);
							//gameObject.GetComponent<Animator>().Play(str);
							roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
							roamingT = 0f;
							currIdleAnimationPlaying = Random.Range(1,5);
							state = ACTOR_STATE.CHOOSING;
						}
						else
						{
							transform.LookAt(movementTarget.characterPosition,Vector3.up);
							state = ACTOR_STATE.MOVING;
						}
					}
				}
				break;
			case ACTOR_STATE.HAUNTED:
				if(forceDirection)
				{
					//forcing actor to move in a direction, if failing forcedirection will be setted to false
					TryForceActorToNextTile();
					if(forceDirection)
					{
						transform.LookAt(movementTarget.characterPosition,Vector3.up);
						state = ACTOR_STATE.MOVING;
					}
					else
					{
						roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
						roamingT = 0f;
						currIdleAnimationPlaying = Random.Range(1,5);
						state = ACTOR_STATE.CHOOSING;
					}
				}
				break;
			case ACTOR_STATE.CHILLED:
				scaredAnimation = "Scared 0" + Random.Range(1,3);
				GameObject player = GameObject.FindGameObjectWithTag("Player");
				if(player)
				{
					Chill c = player.GetComponent<Chill>();
					if(c)
					{
						c.AddActorToCurrentChill(this);
					}
				}
				state = ACTOR_STATE.CHILLED;
			break;
			default:
			break;
			}
		}
		else if (state == ACTOR_STATE.CHOOSING)
		{
			roamingT += Time.deltaTime;

			CheckTileState();
			
			switch (processedState) {
				
			case ACTOR_STATE.NONE:
				//If someone is asking me to talk do that
				if(talkTarget != null && !isPanicking)
				{
					this.GetComponent<Animator>().Play(talkAnimation);
					talkCooldownTimer = talkCooldown;
					state = ACTOR_STATE.TALKING;
				}
				//playing idle animation when choosing
				else if(roamingT < roamingWaitTime)
				{
					LevelController.ACTOR_STATES fearState = GetActorFearState();
					if(fearState == LevelController.ACTOR_STATES.NORMAL)
					{
						string str = "Idle 0"+currIdleAnimationPlaying.ToString();
						//Debug.Log(str);
						gameObject.GetComponent<Animator>().Play(str);
					}
					if(fearState == LevelController.ACTOR_STATES.SPOOKED)
						gameObject.GetComponent<Animator>().Play("Scared 01");
					if(fearState == LevelController.ACTOR_STATES.PANICKED)
						gameObject.GetComponent<Animator>().Play("Scared 02");
				}
				else
				{
					roamingT = 0;
					
					pathHelper.RequestNewRandomPath(currentTile);
					movementTarget = pathHelper.GetNextMove();
					
					if(movementTarget == null)
					{
						roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
						roamingT = 0f;
						currIdleAnimationPlaying = Random.Range(1,5);
						state = ACTOR_STATE.CHOOSING;
					}
					else
					{
						transform.LookAt(movementTarget.characterPosition,Vector3.up);
						currentTile.occupant = null;
						state = ACTOR_STATE.MOVING;
					}
				}
				break;

			case ACTOR_STATE.HAUNTED:
				if(forceDirection)
				{
					//forcing actor to move in a direction, if failing forcedirection will be setted to false
					TryForceActorToNextTile();
					if(forceDirection)
					{
						transform.LookAt(movementTarget.characterPosition,Vector3.up);
						state = ACTOR_STATE.MOVING;
					}
					else
					{
						roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
						roamingT = 0f;
						currIdleAnimationPlaying = Random.Range(1,5);
						state = ACTOR_STATE.CHOOSING;
					}
				}
				break;

			case ACTOR_STATE.CHILLED:
				scaredAnimation = "Scared 0" + Random.Range(1,3);
				GameObject player = GameObject.FindGameObjectWithTag("Player");
				if(player)
				{
					Chill c = player.GetComponent<Chill>();
					if(c)
					{
						c.AddActorToCurrentChill(this);
					}
				}
				state = ACTOR_STATE.CHILLED;
				break;
			default:
				break;
			}
		}
		else if (state == ACTOR_STATE.TALKING) {

			fear -= Time.deltaTime * fearReductionPerSecond;

			if (talkCooldownTimer > 0) {
				talkCooldownTimer -= Time.deltaTime;
				if(talkCooldownTimer < 0)
				{
					talkCooldownTimer = 0;
				}
			}

			CheckTileState();

			switch (processedState) {
				
			case ACTOR_STATE.NONE:
				//PLAY TALK ANIMATION
				if(talkTarget != null)
				{
					transform.LookAt(talkTarget.transform.position,Vector3.up);

					if(Mathf.Approximately(talkCooldownTimer,0.0f))
					{
						talkCooldownTimer = talkCooldown;
						talkRefreshTimer = talkRefresh;
						ResetTalkingActor();
						roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
						roamingT = 0f;
						currIdleAnimationPlaying = Random.Range(1,5);

						if(audioSource.isPlaying)
						{
							audioSource.Stop();
						}

						state = ACTOR_STATE.CHOOSING;
					}
					else if(!audioSource.isPlaying)
					{
						PlaySound(Random.Range(0,3),Random.Range(minDelayTimeTalking,maxDelayTimeTalking));
					}
				}
				//NICE! This makes them go back to the previous tile for talking: 
				//TODO can be dangerous when applying powers
//				if(talkTarget != null && this.currentTile == talkTarget.currentTile)
//				{
//					movementTarget = previousTile;
//					state = ACTOR_STATE.MOVING;
//				}
				//STOP talking go to do something else
				else if(talkTarget == null)
				{
					roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
					roamingT = 0f;
					currIdleAnimationPlaying = Random.Range(1,5);
					talkCooldownTimer = talkCooldown;
					state = ACTOR_STATE.CHOOSING;

				}

				break;
			case ACTOR_STATE.HAUNTED:
				talkRefreshTimer = talkRefresh;
				talkCooldownTimer = talkCooldown;
				ResetTalkingActor();
				if(forceDirection)
				{
					//forcing actor to move in a direction, if failing forcedirection will be setted to false
					TryForceActorToNextTile();
					if(forceDirection)
					{
						transform.LookAt(movementTarget.characterPosition,Vector3.up);
						state = ACTOR_STATE.MOVING;
					}
					else
					{
						roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
						roamingT = 0f;
						currIdleAnimationPlaying = Random.Range(1,5);
						state = ACTOR_STATE.CHOOSING;
					}
				}
				break;
			case ACTOR_STATE.CHILLED:
				scaredAnimation = "Scared 0" + Random.Range(1,3);
				GameObject player = GameObject.FindGameObjectWithTag("Player");
				if(player)
				{
					Chill c = player.GetComponent<Chill>();
					if(c)
					{
						c.AddActorToCurrentChill(this);
					}
					else
					{
						Debug.LogError("Chilled activated but not detected from actrors");
					}
				}
				talkRefreshTimer = talkRefresh;
				talkCooldownTimer = talkCooldown;
				ResetTalkingActor();
				state = ACTOR_STATE.CHILLED;
				break;
			default:
				break;
			}
		}
		else if (state == ACTOR_STATE.CHILLED)
		{
			if(scaredAnimation != string.Empty)
				gameObject.GetComponent<Animator>().Play(scaredAnimation);

			if(deActivateChilled)
			{
				deActivateChilled = false;
				roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
				roamingT = 0f;
				currIdleAnimationPlaying = Random.Range(1,5);
				state = ACTOR_STATE.CHOOSING;
			}
		}
		else if (state == ACTOR_STATE.NONE)
		{
			talkCooldownTimer = talkCooldown;
			talkRefreshTimer = talkRefresh;
			roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
			roamingT = 0f;
			currIdleAnimationPlaying = Random.Range(1,5);
			state = ACTOR_STATE.CHOOSING;
		}

	}
}
