using UnityEngine;
using System.Collections;

public class Shout : MonoBehaviour {


	public AudioClip shoutClip;
	
	
	
	public Vector3 hauntDirection;
	
	public float arrowOffset = 2.0f;
	
	public float hauntDuration;
	float hauntDurationTimer;
	
	public float hauntCooldown;
	public float hauntCooldownTimer;
	
	public float amountOfFear;
	
	Tile[] hauntedTiles = new Tile[2];
	
	int lockNumber;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
