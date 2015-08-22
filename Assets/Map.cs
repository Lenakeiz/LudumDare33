using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	public Tile startingTile;
	public float tileSearchRadius;

	// Use this for initialization
	void Start () {
		Queue<Tile> searchQueue = new Queue<Tile> ();
		List<Tile> searched = new List<Tile> ();


		searchQueue.Enqueue (startingTile);
		while (searchQueue.Count>0) {
			Tile curr = searchQueue.Dequeue();
			searched.Add(curr);
			Collider[] colliders;
			if((colliders = Physics.OverlapSphere(curr.transform.position,tileSearchRadius)).Length > 1) //Presuming the object you are testing also has a collider 0 otherwise
			{
				foreach(Collider collider in colliders)
				{
					if(collider.gameObject == curr.gameObject || collider.tag !="Tile")continue;
					Tile other = collider.GetComponent<Tile>();
					if(collider.transform.position == curr.transform.position + new Vector3(0,0,tileSearchRadius))
					{
						curr.up = other;
					}
					if(collider.transform.position == curr.transform.position + new Vector3(0,0,-tileSearchRadius))
					{
						curr.down = other;
					}
					if(collider.transform.position == curr.transform.position + new Vector3(tileSearchRadius,0,0))
					{
						curr.right = other;
					}
					if(collider.transform.position == curr.transform.position + new Vector3(-tileSearchRadius,0,0))
					{
						curr.left = other;
					}
					if(!searched.Contains(other))
					{
						searchQueue.Enqueue(other);
					}
				}
			}


		}



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
