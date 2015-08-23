using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	enum PLAYER_STATE{
		STANDING,
		MOVING,
		DOING_ACTION,
	}
	PLAYER_STATE state;

	int actionLockNumber;

	public Tile currentTile;

	public float movementKeyThreshhold = 0.5f;


	public float movementSpeed = 1.0f;
	float movementT = 0.0f;
	Tile movementTarget;


	// Use this for initialization
	void Start () {
		this.transform.position = currentTile.characterPosition;
	}


	void Move()
	{

			movementT += Time.deltaTime * movementSpeed;
			if(movementT < 1)
			{
				this.transform.position = Vector3.Lerp(currentTile.characterPosition,
				                                       movementTarget.characterPosition,
				                                       movementT);
			}
			else
			{
			this.GetComponent<Animator>().Play("Walk 15 Sick Zombie");
				currentTile = movementTarget;
				movementTarget=null;
				movementT = 0.0f;
				this.transform.position = currentTile.characterPosition;
				state = PLAYER_STATE.STANDING;
				currentTile.occupant = this.gameObject;
			}
			

	}

	public void CancelAction(int lockNumber)
	{
		if (lockNumber == actionLockNumber) {
			state = PLAYER_STATE.STANDING;
		} else {
			Debug.Log("Wrong lock number");
		}
	}

	void ReadActionInput()
	{
		if (Input.GetButtonDown ("Fire1")) {
			int lockNumber = Random.Range(0,100);
			if(this.GetComponent<Chill>().Activate(lockNumber))
			{
				actionLockNumber =lockNumber;
				state = PLAYER_STATE.DOING_ACTION;
			}
		}
		else if (Input.GetButtonDown ("Fire2")) {
			int lockNumber = Random.Range(0,100);
			if(this.GetComponent<Haunt>().Activate(lockNumber))
			{
				actionLockNumber =lockNumber;
				state = PLAYER_STATE.DOING_ACTION;
			}
		}
	}

	void ReadMoveInput()
	{
		if (Input.GetAxis ("Horizontal") > movementKeyThreshhold) 
		{
			if (currentTile.right) 
			{
				state = PLAYER_STATE.MOVING;
				movementTarget = currentTile.right;
				transform.LookAt(this.transform.position + Vector3.right,Vector3.up);
				currentTile.occupant = null;
			}
		} 
		else if(Input.GetAxis ("Horizontal") < -movementKeyThreshhold)
		{
			if (currentTile.left) 
			{
				state = PLAYER_STATE.MOVING;
				movementTarget = currentTile.left;
				transform.LookAt(this.transform.position + Vector3.left,Vector3.up);
				currentTile.occupant = null;
			}
		}
		else if (Input.GetAxis("Vertical") > movementKeyThreshhold)
		{
			if(currentTile.up)
			{
				state = PLAYER_STATE.MOVING;
				movementTarget = currentTile.up;
				transform.LookAt(this.transform.position + Vector3.forward,Vector3.up);
				currentTile.occupant = null;
			}
		}
		else if (Input.GetAxis("Vertical") < -movementKeyThreshhold)
		{
			if(currentTile.down)
			{
				state = PLAYER_STATE.MOVING;
				movementTarget = currentTile.down;
				transform.LookAt(this.transform.position + Vector3.back,Vector3.up);
				currentTile.occupant = null;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (state == PLAYER_STATE.MOVING) {
			Move ();
		}

		if (state == PLAYER_STATE.STANDING) {
			ReadActionInput();
		}
		if (state == PLAYER_STATE.STANDING) {
			ReadMoveInput();
		}

	}
}
