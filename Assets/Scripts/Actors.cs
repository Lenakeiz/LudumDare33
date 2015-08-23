﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actors : MonoBehaviour {

	public Sprite Face;
	public int uiIndex;

	public Vector2 InitialOnGUIPos;
	//public float GuiBarScriptOffet;
	public GUIBarScript GuiScript;

	public enum ACTOR_STATE
	{
		CHOOSING,
		STANDING,
		MOVING,
		FIENTED
	}

	enum ACTOR_DIRECTION{
		NONE =-1,
		UP =0,
		DOWN = 1,
		LEFT = 2,
		RIGHT = 3,
	}


	public LevelController.ACTOR_NAMES actorName;
	public ACTOR_STATE state = ACTOR_STATE.CHOOSING;
	ACTOR_DIRECTION moveDirection = ACTOR_DIRECTION.NONE;

	public Tile currentTile;
	Tile previousTile;

	public bool isPanicking;

	public float fear = 0;
	public float fearSpookedAmount =30;
	public float fearPanicAmount = 60;
	public float fearReductionPerSecond = 1.0f;

	public float movementSpeed = 1.0f;
	public float panicSpeed = 1.4f;
	float movementT = 0.0f;
	Tile movementTarget;

	public float maxRoamingWaitTime;
	public float minRoamingWaitTime;

	float roamingWaitTime =0.0f;
	float roamingT = 0.0f;

	private Astar pathHelper;
	public int markedTilesPercentage = 35;

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

	static ACTOR_DIRECTION IntToDirection(int dir)
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

	ACTOR_DIRECTION MakeChoice()
	{
		List<ACTOR_DIRECTION> possibleChoices = new List<ACTOR_DIRECTION> ();
		int panicModifier = (isPanicking ? 3 : 1);

		if (currentTile.up) {
				possibleChoices.Add(ACTOR_DIRECTION.UP);
			if(currentTile.up != previousTile)
			{
				possibleChoices.Add(ACTOR_DIRECTION.UP);
			}
		}
		if (currentTile.down) {
			possibleChoices.Add(ACTOR_DIRECTION.DOWN);
			if(currentTile.down != previousTile)
			{
				possibleChoices.Add(ACTOR_DIRECTION.DOWN);
			}
		}
		if (currentTile.left) {
			possibleChoices.Add(ACTOR_DIRECTION.LEFT);
			if(currentTile.left != previousTile)
			{
				possibleChoices.Add(ACTOR_DIRECTION.LEFT);
			}
		}if (currentTile.right) {
			possibleChoices.Add(ACTOR_DIRECTION.RIGHT);
			if(currentTile.right != previousTile)
			{
				possibleChoices.Add(ACTOR_DIRECTION.RIGHT);
			}
		}

		return possibleChoices [Random.Range (0, possibleChoices.Count)];
	}

	public void AddFear (float argFear)
	{
		//we can trigger animations in here
		fear += argFear;
	}

	// Use this for initialization
	void Start () {
		this.transform.position = currentTile.characterPosition;
		pathHelper = gameObject.GetComponent<Astar>() as Astar;

		GameObject uiElement = GameObject.FindGameObjectWithTag("UILife");
		if(uiElement != null)
		{
			Transform targetChild = uiElement.transform.GetChild(uiIndex);
			if(targetChild != null)
			{
				GuiScript = targetChild.gameObject.GetComponentInChildren<GUIBarScript>();
				if(GuiScript != null)
				{
					GuiScript.Position = InitialOnGUIPos;
				}
			}
			else{Debug.Log("UI Child Not Found");};
		}
		else{Debug.Log("UI for lifes not found");}

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
	
	// Update is called once per frame
	void Update () {

		if(GuiScript != null)
		{
			GuiScript.Value = fear * 0.01f;
		}

		if (fear > fearPanicAmount) {
			isPanicking = true;
		} else {
			isPanicking = false;
		}
		if (fear == 100) {
			state = ACTOR_STATE.FIENTED;
		}
		if (fear > 0) {
			fear -= Time.deltaTime * fearReductionPerSecond;
		}
		fear = Mathf.Max(fear, 0);

		if (state == ACTOR_STATE.MOVING) 
		{
			movementT += Time.deltaTime * (isPanicking ? panicSpeed : movementSpeed);
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
				//movementTarget = null;
				movementTarget = pathHelper.GetNextMove();
				currentTile.occupant = this.gameObject;
				movementT = 0;
				//state = ACTOR_STATE.CHOOSING;
				if(movementTarget == null)
				{
					state = ACTOR_STATE.CHOOSING;
				}

				roamingWaitTime = Random.Range(minRoamingWaitTime,maxRoamingWaitTime);
				if(isPanicking)
				{
					roamingWaitTime = 0.1f;
				}
			}
		}
		if (state == ACTOR_STATE.CHOOSING)
		{
			roamingT += Time.deltaTime;
			if(roamingT > roamingWaitTime)
			{
				//moveDirection = MakeChoice();
				roamingT = 0;
				pathHelper.RequestNewRandomPath(currentTile);
				Debug.Log(moveDirection);

				state = ACTOR_STATE.STANDING;
			}

		}

		if (state == ACTOR_STATE.STANDING) {
			movementTarget = pathHelper.GetNextMove();
			if(movementTarget != null)
			{
				currentTile.occupant = null;
				state = ACTOR_STATE.MOVING;
			}
			else
			{
				state = ACTOR_STATE.CHOOSING;
			}
//			if (moveDirection ==ACTOR_DIRECTION.RIGHT) 
//			{
//				if (currentTile.right) 
//				{
//					state = ACTOR_STATE.MOVING;
//					movementTarget = currentTile.right;
//					currentTile.occupant = null;
//				}
//			} 
//			else if(moveDirection == ACTOR_DIRECTION.LEFT)
//			{
//				if (currentTile.left) 
//				{
//					state = ACTOR_STATE.MOVING;
//					movementTarget = currentTile.left;
//					currentTile.occupant = null;
//				}
//			}
//			else if (moveDirection == ACTOR_DIRECTION.UP)
//			{
//				if(currentTile.up)
//				{
//					state = ACTOR_STATE.MOVING;
//					movementTarget = currentTile.up;
//					currentTile.occupant = null;
//				}
//			}
//			else if (moveDirection == ACTOR_DIRECTION.DOWN)
//			{
//				if(currentTile.down)
//				{
//					state = ACTOR_STATE.MOVING;
//					movementTarget = currentTile.down;
//					currentTile.occupant = null;
//				}
//			}
		}
	}
}
