using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	enum PLAYER_STATE{
		STANDING,
		MOVING
	}
	PLAYER_STATE state;

	public Tile currentTile;

	public float movementKeyThreshhold = 0.5f;


	public float movementSpeed = 1.0f;
	float movementT = 0.0f;
	Tile movementTarget;


	// Use this for initialization
	void Start () {
	
	}

	
	// Update is called once per frame
	void Update () {
	
		if (state == PLAYER_STATE.MOVING) 
		{
			movementT += Time.deltaTime;
			if(movementT < 1)
			{
				this.transform.position = Vector3.Lerp(currentTile.characterPosition,
			                                       movementTarget.characterPosition,
			                                       movementT);
			}
			else{
				currentTile = movementTarget;
				movementTarget=null;
				movementT = 0.0f;
				this.transform.position = currentTile.characterPosition;
				state = PLAYER_STATE.STANDING;
				currentTile.occupant = this.gameObject;
			}
		
		}



		if (state == PLAYER_STATE.STANDING) {
			if (Input.GetAxis ("Horizontal") > movementKeyThreshhold) 
			{
				if (currentTile.right) 
				{
					state = PLAYER_STATE.MOVING;
					movementTarget = currentTile.right;
					currentTile.occupant = null;
				}
			} 
			else if(Input.GetAxis ("Horizontal") < -movementKeyThreshhold)
			{
				if (currentTile.left) 
				{
					state = PLAYER_STATE.MOVING;
					movementTarget = currentTile.left;
					currentTile.occupant = null;
				}
			}
			else if (Input.GetAxis("Vertical") > movementKeyThreshhold)
			{
				if(currentTile.up)
				{
					state = PLAYER_STATE.MOVING;
					movementTarget = currentTile.up;
					currentTile.occupant = null;
				}
			}
			else if (Input.GetAxis("Vertical") < -movementKeyThreshhold)
			{
				if(currentTile.down)
				{
					state = PLAYER_STATE.MOVING;
					movementTarget = currentTile.down;
					currentTile.occupant = null;
				}
			}
		}

	}
}
