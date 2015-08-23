using UnityEngine;
using System.Collections;

public class Haunt : MonoBehaviour {


	Player player;

	bool choosingDirection =false;

	public GameObject arrowPrefab;
	public GameObject hauntPrefab;

	public Vector3 hauntDirection;

	public float arrowOffset = 2.0f;

	public float hauntDuration;
	float hauntDurationTimer;
	
	public float hauntCooldown;
	public float hauntCooldownTimer;
	
	public float amountOfFear;

	Tile[] hauntedTiles = new Tile[2];

	int lockNumber;

	public bool Activate(int lockNum)
	{
		if (hauntCooldownTimer > 0 || choosingDirection )return false;

		if (hauntedTiles [0] && hauntedTiles [1]) {
			hauntedTiles [0].effectsOnTile = Tile.TILE_EFFECTS.NONE;
			hauntedTiles [1].effectsOnTile = Tile.TILE_EFFECTS.NONE;
		}

		choosingDirection = true;

		arrowPrefab.SetActive (true);
		arrowPrefab.transform.position = this.transform.position + hauntDirection * arrowOffset;
		arrowPrefab.transform.localRotation = Quaternion.LookRotation (Vector3.up,hauntDirection);


		hauntDurationTimer = hauntDuration;
		lockNumber = lockNum;
		return true;
	}

	public Actors.ACTOR_DIRECTION HauntActor(Actors actor)
	{
		//play haunting animation
		hauntPrefab.SetActive (false);
		hauntedTiles [0].effectsOnTile = Tile.TILE_EFFECTS.NONE;
		hauntedTiles [1].effectsOnTile = Tile.TILE_EFFECTS.NONE;
		actor.AddFear (amountOfFear);
		actor.state = Actors.ACTOR_STATE.MOVING;
		if(hauntDirection.z >0)
		{
			return Actors.ACTOR_DIRECTION.UP;
		}
		else if( hauntDirection.z <0)
		{
			return Actors.ACTOR_DIRECTION.DOWN;
		}
		if(hauntDirection.x >0)
		{
			return Actors.ACTOR_DIRECTION.RIGHT;
		}
		if(hauntDirection.x <0)
		{
			return Actors.ACTOR_DIRECTION.LEFT;
		}
		return Actors.ACTOR_DIRECTION.NONE;
	}

	// Use this for initialization
	void Start () {
		if (arrowPrefab) {
			arrowPrefab = GameObject.Instantiate<GameObject>(arrowPrefab);
			arrowPrefab.SetActive (false);
		} else {
			Debug.LogError ("NO ARROW PREFAB FOR HAUNT");
		}

		if (hauntPrefab) {
			hauntPrefab = GameObject.Instantiate<GameObject>(hauntPrefab);
			hauntPrefab.SetActive (false);
		} else {
			Debug.LogError ("NO HAUNT PREFAB FOR HAUNT");
		}
		player = this.GetComponent<Player> ();
		if (player == null) {
			Debug.LogError("CANT FIND PLAYER");
		}
		hauntDirection = new Vector3 (0, 0, 1);
	}
	// Update is called once per frame
	void Update ()
	{
		if (hauntCooldownTimer > 0) {
			hauntCooldownTimer -= Time.deltaTime;
			if(hauntCooldown < 0 && hauntCooldown >0)
			{
				hauntCooldownTimer = 0;
			}
		}

		if (hauntDurationTimer > 0) {
			hauntDurationTimer -= Time.deltaTime;
			if(hauntDurationTimer < 0 && hauntDuration >0)
			{
				hauntPrefab.SetActive(false);
			}
		}

		if (choosingDirection) {
			if (Input.GetButtonUp ("Fire2")) {
				//release and spawn the haunt

				hauntPrefab.SetActive(true);
				hauntPrefab.transform.position = this.transform.position;
				hauntPrefab.transform.localRotation = Quaternion.LookRotation(hauntDirection,Vector3.up);
				arrowPrefab.gameObject.SetActive(false);
				player.CancelAction(lockNumber);
				hauntCooldownTimer = hauntCooldown;
				player.currentTile.effectsOnTile = Tile.TILE_EFFECTS.HAUNT;
				hauntedTiles[0]=player.currentTile;
				if(hauntDirection.z >0)
				{
					hauntedTiles[0].up.effectsOnTile = Tile.TILE_EFFECTS.HAUNT;
					hauntedTiles[1] = hauntedTiles[0].up;
				}
				else if( hauntDirection.z <0)
				{
					hauntedTiles[0].down.effectsOnTile = Tile.TILE_EFFECTS.HAUNT;
					hauntedTiles[1] = hauntedTiles[0].down;
				}
				if(hauntDirection.x >0)
				{
					hauntedTiles[0].right.effectsOnTile = Tile.TILE_EFFECTS.HAUNT;
					hauntedTiles[1] = hauntedTiles[0].right;
				}
				if(hauntDirection.x <0)
				{
					hauntedTiles[0].left.effectsOnTile = Tile.TILE_EFFECTS.HAUNT;
					hauntedTiles[1] = hauntedTiles[0].left;
				}
				choosingDirection = false;
			}

			Vector3 direction = Vector3.zero;
			direction.x = Input.GetAxis ("Horizontal");
			direction.z = Input.GetAxis ("Vertical");
			if (direction.x != 0 || direction.z != 0) {
				if (Mathf.Abs (direction.z) > Mathf.Abs (direction.x)) {
					direction.x = 0;
				} else {
					direction.z = 0;
				}
				arrowPrefab.transform.position = this.transform.position + direction.normalized * arrowOffset;
				arrowPrefab.transform.localRotation = Quaternion.LookRotation (Vector3.up,direction);
				player.transform.localRotation = Quaternion.LookRotation(direction,Vector3.up);
				hauntDirection = direction.normalized;
			}
		}
	}
}
