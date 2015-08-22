using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	public Tile startingTile;
	public float tileSearchRadius;


	//public int markedTileProbability = 0;

	//private List<Astar> astars;
	public GameObject playerPrefab;
	public List<GameObject> actorPrefabs;

	public List<Tile> playerSpawnPositions;
	public List<Tile> actorSpawnPositions;

	static int actorNameIndex =0;

	// Use this for initialization

	bool breakout = false;

	void Start () {
		Queue<Tile> searchQueue = new Queue<Tile> ();

//		astars = new List<Astar>();
//		GameObject[] actors = GameObject.FindGameObjectsWithTag("Actor");
//
//		for (int i = 0; i < actors.Length; i++) {
//			astars.Add(actors[i].GetComponent<Astar>() as Astar);
//		}

		searchQueue.Enqueue (startingTile);
		int indexCount = 0;
		while (searchQueue.Count>0) {
			if(breakout)
			{
				break;
			}
			Tile curr = searchQueue.Dequeue();
			curr.searched = true;
			curr.Index = indexCount;

			curr.name = "Tile_" + indexCount;

			Debug.Log (indexCount);
			indexCount++;
//			//Adding tiles to the astar
//			foreach (Astar helperPath in astars) {
//				if(Random.Range(5,101) < markedTileProbability || curr.useAstar == true)
//				{
//					helperPath.AddTile(curr);
//				}
//			}


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
						other.searched = true;
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
