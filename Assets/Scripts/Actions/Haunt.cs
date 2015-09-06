using UnityEngine;
using System.Collections;

public class Haunt : MonoBehaviour {


	Player player;

	bool choosingDirection =false;

	public AudioClip hauntClip;

	public GameObject arrowPrefab;
	public GameObject hauntPrefab;

	public Vector3 hauntDirection;

	public float arrowOffset = 2.0f;

	public float hauntDuration;
	float hauntDurationTimer;

//  We don t need a cooldown for this power as we want the player to react quickly to the actors movement
//	public float hauntCooldown;
//	public float hauntCooldownTimer;
	
	public float amountOfFear;

	//We need to chnge this and make it work only on the base
	Tile[] hauntedTiles = new Tile[2];

	int lockNumber;

	public bool Activate(int lockNum)
	{
		//Already setting 
		if (choosingDirection)return false;

		choosingDirection = true;

		//Cancelling haunt on previously registered tile
		if (hauntedTiles [0] != null 
		    && 
		    (hauntedTiles [0].effectsOnTile & Tile.TILE_EFFECTS.HAUNT) == Tile.TILE_EFFECTS.HAUNT)
		{
			hauntedTiles [0].effectsOnTile &= ~Tile.TILE_EFFECTS.HAUNT;
		}

		arrowPrefab.SetActive (true);

		SetHauntDirection();

		lockNumber = lockNum;
		return true;
	}

	public Actors.ACTOR_DIRECTION HauntActor(Actors actor)
	{
		gameObject.GetComponent<AudioSource> ().clip = hauntClip;
		gameObject.GetComponent<AudioSource> ().Play ();
		//play haunting animation
		hauntPrefab.SetActive (false);

		//Changed to bitwisw
		if((hauntedTiles[0].effectsOnTile & Tile.TILE_EFFECTS.HAUNT) == Tile.TILE_EFFECTS.HAUNT)
		{
			hauntedTiles[0].effectsOnTile &= ~Tile.TILE_EFFECTS.HAUNT;
		}
		else
		{
			Debug.LogError("Actor activated haunt but the tile is not the current one");
		}

		actor.AddFear (amountOfFear);

		//TODO NEVER CHANGE MACHINE STATES OUTSIDE OTHER CLASSES
		//actor.state = Actors.ACTOR_STATE.MOVING;

		if(hauntDirection.z >0)
		{
			//Debug.Log("Haunting up");
			return Actors.ACTOR_DIRECTION.UP;
		}
		else if( hauntDirection.z <0)
		{
			//Debug.Log("Haunting down");
			return Actors.ACTOR_DIRECTION.DOWN;
		}
		if(hauntDirection.x > 0)
		{
			//Debug.Log("Haunting right");
			return Actors.ACTOR_DIRECTION.RIGHT;
		}
		else if(hauntDirection.x < 0)
		{
			//Debug.Log("Haunting left");
			return Actors.ACTOR_DIRECTION.LEFT;
		}
		else
		{
			Debug.LogError("HAUNTING ACTOR HAS RETURNED WITH DIRECTION NONE");
			return Actors.ACTOR_DIRECTION.NONE;
		}
	}

	void OnDestroy()
	{
		Debug.Log("Haunt destroyed");
		GameObject.Destroy (hauntPrefab);
	}

	// Use this for initialization
	void Start ()
	{

		if (arrowPrefab) {
			arrowPrefab = GameObject.Instantiate<GameObject>(arrowPrefab);
			arrowPrefab.SetActive (false);
		} else {
			Debug.LogError ("NO ARROW PREFAB FOR HAUNT");
		}

		if (hauntPrefab) {
			hauntPrefab = GameObject.Instantiate<GameObject>(hauntPrefab);
			hauntPrefab.GetComponentInChildren<Animator>().speed = 0;
			hauntPrefab.GetComponentInChildren<Animator>().Play("Magic 01");
			hauntPrefab.SetActive (false);
		} else {
			Debug.LogError ("NO HAUNT PREFAB FOR HAUNT");
		}

		player = this.GetComponent<Player> ();
		if (player == null) {
			Debug.LogError("CANT FIND PLAYER");
		}

		if(hauntDuration < 2)
		{
			hauntDuration = 2;
			hauntDurationTimer = 0;
			Debug.LogError("CANNOT SET DURATION LESS THAN 2s");
		}

		hauntDirection = new Vector3 (0, 0, 1);
	}

	private void SetHauntDirection()
	{
		Vector3 direction = Vector3.zero;
		direction.x = Input.GetAxis ("Horizontal");
		direction.z = Input.GetAxis ("Vertical");

		//If no input we take the current forward facing of the player
		if (direction.x != 0 || direction.z != 0) {
			if (Mathf.Abs (direction.z) > Mathf.Abs (direction.x)) {
				direction.x = 0f;
			} else {
				direction.z = 0f;
			}
			arrowPrefab.transform.position = this.transform.position + direction.normalized * arrowOffset;
			arrowPrefab.transform.localRotation = Quaternion.LookRotation (Vector3.up, direction.normalized );
			player.transform.localRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
			hauntDirection = direction.normalized;
		}
		else
		{
			direction = this.transform.forward;
			if (Mathf.Abs (direction.z) > Mathf.Abs (direction.x)) {
				direction.x = 0f;
			} else {
				direction.z = 0f;
			}
			direction = direction.normalized;
			arrowPrefab.transform.position = this.transform.position + direction * arrowOffset;
			arrowPrefab.transform.localRotation = Quaternion.LookRotation (Vector3.up, direction);
			hauntDirection = direction;
		}

		//Debug.Log("Haunt Direction x-z:" + hauntDirection.x + " " + hauntDirection.z);

	}

	// Update is called once per frame
	void Update ()
	{
		if (!choosingDirection && hauntDurationTimer > 0)
		{
			hauntDurationTimer -= Time.deltaTime;
			if(hauntDurationTimer < 0)
			{
				hauntPrefab.SetActive(false);
				if((hauntedTiles [0].effectsOnTile & Tile.TILE_EFFECTS.HAUNT) == Tile.TILE_EFFECTS.HAUNT)
					hauntedTiles[0].effectsOnTile &= ~Tile.TILE_EFFECTS.HAUNT;
				hauntDurationTimer = 0;
			}
		}
		else if (choosingDirection)
		{

			SetHauntDirection();

			if (Input.GetButtonUp ("Fire2")) {
				//release and spawn the haunt

				hauntPrefab.SetActive(true);
				hauntPrefab.transform.position = this.transform.position;
				hauntPrefab.transform.localRotation = Quaternion.LookRotation(hauntDirection,Vector3.up);
				arrowPrefab.gameObject.SetActive(false);
				player.CancelAction(lockNumber);
				
				choosingDirection = false;
				hauntDurationTimer = hauntDuration;
				player.currentTile.effectsOnTile |= Tile.TILE_EFFECTS.HAUNT;
				hauntedTiles[0] = player.currentTile;

			}
		}
	}
}
