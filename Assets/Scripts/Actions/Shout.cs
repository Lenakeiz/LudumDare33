using UnityEngine;
using System.Collections;

public class Shout : MonoBehaviour {


	public AudioClip shoutClip;
	
	public string shoutAnimName;

	public float radius;


	public float castTime;
	public float castTimer;

	public float shoutCooldown;
	public float shoutCooldownTimer;
	
	public float amountOfFear;
	
	int lockNumber;

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
				if(dirDifference == Vector3.zero)
				{
					scareDir = Actors.IntToDirection(Random.Range(0,3));
				}
				if(Mathf.Abs(dirDifference.x) > Mathf.Abs(dirDifference.z))
				{
					if(dirDifference.x 	>0)
					{
						scareDir = Actors.ACTOR_DIRECTION.RIGHT;
					}
					else if( dirDifference.x <0)
					{
						scareDir = Actors.ACTOR_DIRECTION.LEFT;
					}
				}
				else{
					if(dirDifference.z >0)
					{
						scareDir = Actors.ACTOR_DIRECTION.UP;
					}
					else if( dirDifference.z <0)
					{
						scareDir = Actors.ACTOR_DIRECTION.DOWN;
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
		shoutCooldownTimer = 0;
		castTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (castTimer > 0) {
			castTimer -= Time.deltaTime;
			if(castTimer<=0)
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
