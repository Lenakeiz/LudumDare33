using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	public Tile startingTile;
	public float tileSearchRadius;

	public GameObject playerPrefab;
	public List<GameObject> actorPrefabs;

	public List<Tile> playerSpawnPositions;
	public List<Tile> actorSpawnPositions;

	static int actorNameIndex =0;

	// Use this for initialization
	void Start () {
		Queue<Tile> searchQueue = new Queue<Tile> ();

		searchQueue.Enqueue (startingTile);
		while (searchQueue.Count>0) {
			Tile curr = searchQueue.Dequeue();
			curr.searched = true;
			Collider[] colliders;
			if((colliders = Physics.OverlapSphere(curr.transform.position,tileSearchRadius)).Length > 1) //Presuming the object you are testing also has a collider 0 otherwise
			{
				foreach(Collider collider in colliders)
				{
					if(collider.transform.parent == null) continue;
					if(collider.transform.parent.gameObject == curr.gameObject || collider.transform.parent.gameObject.tag !="Tile")continue;
					Tile other = collider.transform.parent.gameObject.GetComponent<Tile>();
					if(other.transform.position == curr.transform.position + new Vector3(0,0,tileSearchRadius))
					{
						curr.up = other;
					}
					if(other.transform.position == curr.transform.position + new Vector3(0,0,-tileSearchRadius))
					{
						curr.down = other;
					}
					if(other.transform.position == curr.transform.position + new Vector3(tileSearchRadius,0,0))
					{
						curr.right = other;
					}
					if(other.transform.position == curr.transform.position + new Vector3(-tileSearchRadius,0,0))
					{
						curr.left = other;
					}
					if(!other.searched)
					{
						searchQueue.Enqueue(other);
					}
				}
			}
		}
		if (playerSpawnPositions.Count > 0) {
			Tile t = playerSpawnPositions[Random.Range(0,playerSpawnPositions.Count)];
			GameObject g=(GameObject)GameObject.Instantiate(playerPrefab,t.characterPosition,Quaternion.identity);
			g.GetComponent<Player>().currentTile = t;

		}
		for (int i = 0; i < actorPrefabs.Count; ++i) {
			Tile t =actorSpawnPositions[Random.Range(0,actorSpawnPositions.Count)];
			GameObject g =(GameObject)GameObject.Instantiate(actorPrefabs[i],t.characterPosition,Quaternion.identity);
			g.GetComponent<Actors>().currentTile = t;
			g.GetComponent<Actors>().actorName = LevelController.ConvertIntToName(actorNameIndex);
			actorNameIndex++;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
