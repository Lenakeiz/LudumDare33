using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shout : MonoBehaviour {


	public AudioClip shoutClip;
	
	public string shoutAnimName;

	public float radius;


	public float castTime;
	public float castTimer;

	public float shoutCooldown;
	public float shoutCooldownTimer;
	
	public float amountOfFear;

	private List<Actors.ACTOR_DIRECTION> possibleDirections;

	int lockNumber;

	private void FillRandomDirection(Tile currTile)
	{
		possibleDirections.Clear();
		if(currTile != null)
		{
			if(currTile.up != null)
			{
				//Actors.ACTOR_DIRECTION dir = Actors.ACTOR_DIRECTION.UP;
				possibleDirections.Add(Actors.ACTOR_DIRECTION.UP);
			}
			if(currTile.down != null)
			{
				possibleDirections.Add(Actors.ACTOR_DIRECTION.DOWN);
			}
			if(currTile.right != null)
			{
				possibleDirections.Add(Actors.ACTOR_DIRECTION.RIGHT);
			}
			if(currTile.left != null)
			{
				possibleDirections.Add(Actors.ACTOR_DIRECTION.LEFT);
			}
		}
		else 
		{
			Debug.LogError("Cannot decide scream direction since tile is null");
		}
	}

	public bool Activate(int lockNum)
	{
		if (shoutCooldownTimer > 0)return false;

		shoutCooldownTimer = shoutCooldown;
		castTimer = castTime;

		gameObject.GetComponent<Animator> ().Play (shoutAnimName);
		gameObject.GetComponent<AudioSource> ().clip = shoutClip;
		gameObject.GetComponent<AudioSource> ().Play ();

		Collider[] colliders = Physics.OverlapSphere (this.transform.position, radius);

		for (int i = 0; i < colliders.Length; ++i) {
			if (colliders [i].tag == "Actor" && colliders[i].isTrigger==false) {

				Actors actor = colliders[i].GetComponent<Actors>();
				actor.AddFear(amountOfFear);

				Vector3 dirDifference = actor.transform.position - this.transform.position;
				dirDifference = dirDifference.normalized;

				Actors.ACTOR_DIRECTION scareDir = new Actors.ACTOR_DIRECTION();

				FillRandomDirection(actor.currentTile);

				Actors.ACTOR_DIRECTION randDir = new Actors.ACTOR_DIRECTION();

				if(possibleDirections.Count != 0)
				{
					randDir = possibleDirections[Random.Range(0,possibleDirections.Count)];
				}
				else
				{
					Debug.Log ("No possible direction for the scream");
				}

				if(Vector3.SqrMagnitude(dirDifference) < 0.0001f)
				{
					scareDir = randDir;
				}
				else if(Mathf.Abs(dirDifference.x) > Mathf.Abs(dirDifference.z))
				{
					if(dirDifference.x 	> 0)
					{
						scareDir = actor.currentTile.right != null ? Actors.ACTOR_DIRECTION.RIGHT : randDir;
					}
					else if(dirDifference.x < 0)
					{
						scareDir = actor.currentTile.left != null ? Actors.ACTOR_DIRECTION.LEFT : randDir;
					}
				}
				else{
					if(dirDifference.z >0)
					{
						scareDir = actor.currentTile.up != null ? Actors.ACTOR_DIRECTION.UP : randDir;
					}
					else if( dirDifference.z <0)
					{
						scareDir = actor.currentTile.down != null ? Actors.ACTOR_DIRECTION.DOWN : randDir;
					}
				}
				actor.ScreamedAt(scareDir);
			}
		}
		lockNumber = lockNum;
		return true;
	}

	// Use this for initialization
	void Start () {
		possibleDirections = new List<Actors.ACTOR_DIRECTION>();
		shoutCooldownTimer = 0;
		castTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (castTimer > 0) {
			castTimer -= Time.deltaTime;
			if(castTimer < 0)
			{
				castTimer = 0;
				this.GetComponent<Player>().CancelAction(lockNumber);	
			}
		}
		if (shoutCooldownTimer > 0) {
		
			shoutCooldownTimer -=Time.deltaTime;
			if(shoutCooldownTimer<=0)
			{
				shoutCooldownTimer =0;
			}
		}
	}
}
