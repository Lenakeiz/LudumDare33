using UnityEngine;
using System.Collections;

public class Chill : MonoBehaviour {


	Player player;

	public AudioClip chillSound;

	public float chillSphereRadius;
	public GameObject chillObject;
	public float chillDuration;
	float chillDurationTimer;

	public float chillCooldown;
	public float chillCooldownTimer;

	public float amountOfFear;

	int lockNumber;


	void MarkChilledNodes(bool unMark)
	{
		Collider[] colliders;
		if ((colliders = Physics.OverlapSphere (chillObject.transform.position, chillSphereRadius)).Length > 0) {

			foreach(Collider col in colliders)
			{
				if(col.transform.parent !=null)
				{
					if(col.transform.parent.tag == "Tile")
					{
						if(!unMark)
						{
							chillObject.GetComponent<AudioSource>().clip = chillSound;
							chillObject.GetComponent<AudioSource>().Play();
							if(col.transform.parent.GetComponent<Tile>().effectsOnTile != Tile.TILE_EFFECTS.HAUNT)
							{
								col.transform.parent.GetComponent<Tile>().effectsOnTile = Tile.TILE_EFFECTS.CHILL;
							}
						}
						else
						{
							if(col.transform.parent.GetComponent<Tile>().effectsOnTile != Tile.TILE_EFFECTS.HAUNT)
							{
								col.transform.parent.GetComponent<Tile>().effectsOnTile = Tile.TILE_EFFECTS.NONE;
							}
						}
					}
				}
				else if(!unMark && col.transform.tag == "Actor")
				{
					col.GetComponent<Actors>().AddFear(amountOfFear);
				}
			}
		
		}
	}

	public bool Activate(int lockNum)
	{
		if (chillCooldownTimer > 0) {
			return false;
		}
		lockNumber = lockNum;
		chillObject.SetActive (true);
		MarkChilledNodes (true);
		chillObject.transform.position = this.transform.position;
		chillObject.transform.localScale = new Vector3 (chillSphereRadius*2,
		                                                chillSphereRadius/10,
		                                                chillSphereRadius*2);
		chillCooldownTimer = chillCooldown;
		chillDurationTimer = chillDuration;
	
		MarkChilledNodes (false);
		return true;
	}


	// Use this for initialization
	void Start () {
		player = this.GetComponent<Player> ();
		if (player == null) {
			Debug.LogError("CANT FIND PLAYER");
		}
		if (chillObject) {
			chillObject = GameObject.Instantiate<GameObject>(chillObject);
			chillObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(chillCooldownTimer == chillCooldown)
		{
			player.CancelAction(lockNumber);
		}
		if (chillCooldownTimer > 0) {
			chillCooldownTimer -= Time.deltaTime;
			if(chillCooldownTimer <= 0 && chillCooldown >0)
			{
				chillCooldownTimer = 0;
			}
		}

		if (chillDurationTimer > 0) {
			chillDurationTimer -= Time.deltaTime;
			chillObject.GetComponent<AudioSource>().volume = chillDurationTimer/chillDuration;
			if(chillDurationTimer <= 0 && chillDuration >0)
			{
				chillObject.GetComponent<AudioSource>().Stop();
				MarkChilledNodes(true);
				chillDurationTimer = 0;
				chillObject.SetActive(false);
			}
		}
	}
}
