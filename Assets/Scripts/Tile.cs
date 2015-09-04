using UnityEngine;
using System;
using System.Collections;

public class Tile : MonoBehaviour {

	[Flags]
	public enum TILE_EFFECTS{
		NONE = 0,
		CHILL = 1,
		HAUNT = 2,
	}

	public bool debugDrawGrid = false;
	
	public GameObject occupant;
	public TILE_EFFECTS effectsOnTile = TILE_EFFECTS.NONE;
	public bool searched = false;

	public Vector3 characterPosition;
	
	public Tile up;
	public Tile right;
	public Tile left; 
	public Tile down;

	//Used for astar
	public Tile Parent;
	public float cost = 0.0f;

	public bool useAstar = false;
	public int Index = -1;

	// Use this for initialization
	void Start () {
		characterPosition = this.transform.position + new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

		if ((effectsOnTile & TILE_EFFECTS.HAUNT) == TILE_EFFECTS.HAUNT) 
		{
			Debug.DrawLine(this.transform.position,
			               this.transform.position + new Vector3(0,10,0),Color.red);
			this.gameObject.GetComponent<Renderer>().material.color = new Color(0.0f,1.0f,0.0f);
		}
		else
		{
			this.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,1.0f);
		}

		if (debugDrawGrid) {
			Debug.DrawLine (this.transform.position + new Vector3 (this.transform.localScale.x, 0, -this.transform.localScale.y),
		                this.transform.position + new Vector3 (-this.transform.localScale.x, 0, -this.transform.localScale.y));



			Debug.DrawLine (this.transform.position + new Vector3 (-this.transform.localScale.x, 0, this.transform.localScale.y),
		                this.transform.position + new Vector3 (-this.transform.localScale.x, 0, -this.transform.localScale.y));
		}
	}
}
