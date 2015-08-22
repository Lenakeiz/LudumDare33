using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Astar : MonoBehaviour {


	public float minTileSpanning;

	private List<Tile> markedTiles;
	private Tile currTargetPosition;
	private Tile currInitialPosition;

	private List<Tile> openList;
	private List<Tile> closedList;

	private Queue<Tile> movements;

	// Use this for initialization
	void Awake () {
		markedTiles = new List<Tile>();
		movements = new Queue<Tile>();
		openList = new List<Tile>();
		closedList = new List<Tile>();
	}

	public void AddTile(Tile tile)
	{
		if(markedTiles != null)
			markedTiles.Add(tile);
	}

	private void ResetAStar()
	{
		movements.Clear();
		openList.Clear();
		currInitialPosition.Parent = null;
		currTargetPosition.Parent = null;
	}

	private void CalculatePath()
	{
		ResetAStar();

		openList.Add(currInitialPosition);

		Tile currTile = null;

		while(openList.Count != 0 || closedList.Contains(currTargetPosition))
		{
			openList = openList.OrderBy(x => x.cost).ToList();
			currTile = openList[0];
			openList.Remove(currTile);

			closedList.Add(currTargetPosition);

			Collider[] colliders;
			if((colliders = Physics.OverlapSphere(currTile.transform.position, minTileSpanning)).Length > 1)
			{
				foreach(Collider collider in colliders)
				{
					Tile detectedTile = collider.transform.parent.gameObject.GetComponent<Tile>();

					Tile prevTile = null;
					prevTile = openList.FirstOrDefault(x => x.Index == detectedTile.Index);

					float computed_cost = Mathf.Abs(currTargetPosition.transform.position.x - detectedTile.transform.position.x) + Mathf.Abs(currTargetPosition.transform.position.z - detectedTile.transform.position.z);

					if(prevTile == null)
					{
						detectedTile.Parent = currTile;
						detectedTile.cost = computed_cost;
						openList.Add(detectedTile);
					}
					else
					{
						if(computed_cost < prevTile.cost)
						{
							prevTile.cost = computed_cost;
							prevTile.Parent = currTile;
						}
					}
				}
			}
		}

		currTile = currTargetPosition;
		//walking backward
		while (currTile.Parent != null)
		{
			movements.Enqueue(currTile);
			currTile = currTile.Parent;
		}

	}

	public Tile GetNextMove()
	{
		if(movements != null && movements.Count != 0)
			return movements.Dequeue();
		else
			return null;
	}

	public void RequestNewRandomPath(Tile currPos)
	{
		if(markedTiles.Count != 0)
		{
			currInitialPosition = currPos;
			int randIdx = Random.Range(0,markedTiles.Count);
			currTargetPosition = markedTiles[randIdx];

			CalculatePath();

		}
		else{Debug.Log ("No Tiles Have been Added");}
	}



	// Update is called once per frame
	void Update () {
		
	}
}
