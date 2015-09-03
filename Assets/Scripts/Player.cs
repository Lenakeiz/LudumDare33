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

	public string runAnimName;
	public string idleAnimName;

	public float movementSpeed = 1.0f;
	float movementT = 0.0f;
	Tile movementTarget;

	private GameObject camera;
	Vector3 velDampening;
	Vector3 moveAmount;
	// Use this for initialization
	void Start () {
		this.transform.position = currentTile.characterPosition;
	}

	void Awake()
	{
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
		moveAmount = camera.transform.position;
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
				this.GetComponent<Animator>().Play("Magic 01");
				actionLockNumber =lockNumber;
				state = PLAYER_STATE.DOING_ACTION;
			}
		}
		else if (Input.GetButtonDown ("Fire3")) {
			int lockNumber = Random.Range(0,100);
			if(this.GetComponent<Shout>().Activate(lockNumber))
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
				this.GetComponent<Animator>().Play(runAnimName);
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
				this.GetComponent<Animator>().Play(runAnimName);
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
				this.GetComponent<Animator>().Play(runAnimName);
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
				this.GetComponent<Animator>().Play(runAnimName);
				currentTile.occupant = null;
			}
		}
		if (!movementTarget) {
			this.GetComponent<Animator> ().Play (idleAnimName);
		}
	}

	void FixedUpdate()
	{
		//camera.transform.position =  moveAmount * Time.fixedDeltaTime;

		//Vector3 amount = new Vector3();

		//camera.transform.position = Vector3.SmoothDamp (camera.transform.position, this.transform.position + 
       // new Vector3 (0, 11, -6),ref amount,0.15f);

	}

	// Update is called once per frame
	void Update () {

		camera.transform.position = Vector3.SmoothDamp (camera.transform.position, 
        this.transform.position + new Vector3 (0, 11, -6),ref velDampening,0.015f,10, Time.deltaTime);
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
