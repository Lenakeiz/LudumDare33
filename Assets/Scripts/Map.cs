using UnityEngine;
using UnityEngine.UI;
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

	public bool startGame = false;

	public GameObject ActorUIPrefabHolder;

	public Vector3 ImageInitialLocalPos;
	public float ImageGuiOffset;

	//a list of objects to be deleted when restarting

	List<GameObject> tempObjects;

	// Use this for initialization

	bool breakout = false;

	public void ResetLevel()
	{
		actorNameIndex = 0;
		foreach(GameObject g in tempObjects)
		{
			GameObject.Destroy(g);
		}
	}

	void Start () {
		tempObjects = new List<GameObject> ();

		Queue<Tile> searchQueue = new Queue<Tile> ();

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
			indexCount++;

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

	}

	public void StartGame()
	{
		startGame = true;
	}

	public void PreparePrefabs()
	{
		if (playerSpawnPositions.Count > 0) {
			Tile t = playerSpawnPositions[Random.Range(0,playerSpawnPositions.Count)];
			GameObject g=(GameObject)GameObject.Instantiate(playerPrefab,t.characterPosition,Quaternion.identity);
			g.GetComponent<Player>().currentTile = t;

			tempObjects.Add(g);
		}
		for (int i = 0; i < actorPrefabs.Count; ++i) {
			Tile t =actorSpawnPositions[Random.Range(0,actorSpawnPositions.Count)];
			GameObject g =(GameObject)GameObject.Instantiate(actorPrefabs[i],t.characterPosition,Quaternion.identity);
			g.GetComponent<Actors>().currentTile = t;
			g.transform.position = t.characterPosition;
			g.name = g.GetComponent<Actors>().actorName.ToString();
			GameObject.FindObjectOfType<LevelController>().activeActors.Add(g.GetComponent<Actors>().actorName);
			actorNameIndex++;
			tempObjects.Add(g);

			GameObject uiElement = GameObject.Instantiate(Resources.Load("UIPrefabs/Actor"),Vector3.zero,Quaternion.identity) as GameObject;

			uiElement.GetComponent<RectTransform>().SetParent(ActorUIPrefabHolder.GetComponent<RectTransform>(),false);
			uiElement.GetComponent<Image>().sprite = actorPrefabs[i].GetComponent<Actors>().Face;
			actorPrefabs[i].GetComponent<Actors>().uiIndex = i;

			Vector3 imagePos = ImageInitialLocalPos;
			imagePos.y += i * ImageGuiOffset;
			uiElement.GetComponent<RectTransform>().localPosition = imagePos;//new Vector2(imagePos.x, imagePos.y);
			tempObjects.Add(uiElement);
		}
	}

	// Update is called once per frame
	void Update () {
		//controlled by level controller
		if(startGame)
		{
			startGame = false;
			PreparePrefabs();
		}
	}
}
