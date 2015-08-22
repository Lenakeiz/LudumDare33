using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	
	public GameObject occupant;
	public string effectsOnTile;
	public bool searched = false;

	public Vector3 characterPosition;
	
	public Tile up;
	public Tile right;
	public Tile left; 
	public Tile down;

	// Use this for initialization
	void Start () {
		characterPosition = this.transform.position + new Vector3 (0, 1, 0);
	}
	
	// Update is called once per frame
	void Update () {
	

		if (right) {
			if(right.left == this)
			{
			Debug.DrawLine(this.transform.position,
			               right.transform.position,Color.blue);
			}
			else{
				Debug.DrawLine(this.transform.position,
				               right.transform.position,Color.red);
			}
		}
		if (down) {
			if(down.up == this)
			{
			Debug.DrawLine(this.transform.position,
			               down.transform.position,Color.blue);
			}
			else{
				Debug.DrawLine(this.transform.position,
				               down.transform.position,Color.red);
			}
		}
	}
}
