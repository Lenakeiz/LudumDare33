using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Astar : MonoBehaviour {


	public float minTileSpanning;

	//private List<Tile> allTiles;

	private List<Tile> markedTiles;
	private Tile currTargetPosition;
	private Tile currInitialPosition;

	private Tile currTile;

	private List<Tile> openList;
	private List<Tile> closedList;

	private List<Tile> movements;
	private Dictionary<int,int> mappedMovements;


	// Use this for initialization
	void Awake () {
		markedTiles = new List<Tile>();
		movements = new List<Tile>();
		openList = new List<Tile>();
		closedList = new List<Tile>();
		mappedMovements = new Dictionary<int, int>();
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
		closedList.Clear();
		mappedMovements.Clear();
		currInitialPosition.Parent = null;
		currTargetPosition.Parent = null;
	}

	private void CalculatePath()
	{
		ResetAStar();

		openList.Add(currInitialPosition);

		Tile currTile = null;

		while(openList.Count != 0 || (closedList.FirstOrDefault(x => x.Index == currTargetPosition.Index)) != null)
		{
			openList = openList.OrderBy(x => x.cost).ToList();
			currTile = openList[0];

			openList.Remove(currTile);
			closedList.Add(currTile);

			if(currTile.Index == currTargetPosition.Index)
				break;

			if(currTile.up != null && (closedList.FirstOrDefault(x => x.Index == currTile.up.Index)) == null)
			{
				Tile detectedTile = currTile.up;
				Tile prevTile = openList.FirstOrDefault(x => x.Index == detectedTile.Index);
				float computed_cost = Mathf.Abs(currTargetPosition.transform.position.x - detectedTile.transform.position.x) + Mathf.Abs(currTargetPosition.transform.position.z - detectedTile.transform.position.z);

				if(prevTile == null)
				{
					//detectedTile.Parent = currTile;
					mappedMovements.Add(detectedTile.Index,currTile.Index);
					detectedTile.cost = computed_cost;
					openList.Add(detectedTile);
				}
				else
				{
					if(computed_cost < prevTile.cost)
					{
						mappedMovements[detectedTile.Index] = currTile.Index;
						prevTile.cost = computed_cost;
						//prevTile.Parent = currTile;
					}
				}		
			}

			if(currTile.right != null && (closedList.FirstOrDefault(x => x.Index == currTile.right.Index)) == null)
			{
				Tile detectedTile = currTile.right;
				Tile prevTile = openList.FirstOrDefault(x => x.Index == detectedTile.Index);
				float computed_cost = Mathf.Abs(currTargetPosition.transform.position.x - detectedTile.transform.position.x) + Mathf.Abs(currTargetPosition.transform.position.z - detectedTile.transform.position.z);
				
				if(prevTile == null)
				{
					//detectedTile.Parent = currTile;
					mappedMovements.Add(detectedTile.Index,currTile.Index);
					detectedTile.cost = computed_cost;
					openList.Add(detectedTile);
				}
				else
				{
					if(computed_cost < prevTile.cost)
					{
						prevTile.cost = computed_cost;
						//prevTile.Parent = currTile;
						mappedMovements[detectedTile.Index] = currTile.Index;
					}
				}		
			}

			if(currTile.down != null && (closedList.FirstOrDefault(x => x.Index == currTile.down.Index)) == null)
			{
				Tile detectedTile = currTile.down;
				Tile prevTile = openList.FirstOrDefault(x => x.Index == detectedTile.Index);
				float computed_cost = Mathf.Abs(currTargetPosition.transform.position.x - detectedTile.transform.position.x) + Mathf.Abs(currTargetPosition.transform.position.z - detectedTile.transform.position.z);
				
				if(prevTile == null)
				{
					//detectedTile.Parent = currTile;
					mappedMovements.Add(detectedTile.Index,currTile.Index);
					detectedTile.cost = computed_cost;
					openList.Add(detectedTile);
				}
				else
				{
					if(computed_cost < prevTile.cost)
					{
						prevTile.cost = computed_cost;
						//prevTile.Parent = currTile;
						mappedMovements[detectedTile.Index] = currTile.Index;
					}
				}		
			}

			if(currTile.left != null && (closedList.FirstOrDefault(x => x.Index == currTile.left.Index)) == null)
			{
				Tile detectedTile = currTile.left;
				Tile prevTile = openList.FirstOrDefault(x => x.Index == detectedTile.Index);
				float computed_cost = Mathf.Abs(currTargetPosition.transform.position.x - detectedTile.transform.position.x) + Mathf.Abs(currTargetPosition.transform.position.z - detectedTile.transform.position.z);
				
				if(prevTile == null)
				{
				    //detectedTile.Parent = currTile;
					mappedMovements.Add(detectedTile.Index,currTile.Index);
					detectedTile.cost = computed_cost;
					openList.Add(detectedTile);
				}
				else
				{
					if(computed_cost < prevTile.cost)
					{
						prevTile.cost = computed_cost;
						//prevTile.Parent = currTile;
						mappedMovements[detectedTile.Index] = currTile.Index;
					}
				}		
			}
		}

		int currIndexTile = currTargetPosition.Index;

		//walking backward
		while(mappedMovements.ContainsKey(currIndexTile))
		{
			currTile = closedList.FirstOrDefault(x => x.Index == currIndexTile);
			if(currTile != null)
				movements.Add(currTile);
			else 
				Debug.Log ("Found a mapped int not in the closed List");
			currIndexTile = mappedMovements[currIndexTile];
		}

	}

	public Tile GetNextMove()
	{
		if(currTile != null)
		{
			currTile.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,1.0f);
		}
		if(movements != null && movements.Count != 0)
		{
			Tile retTile = movements[movements.Count - 1];
			currTile = retTile;
			currTile.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f,0.0f,1.0f);
			movements.RemoveAt(movements.Count - 1);
			return retTile;
		}
		else
			return null;
	}

	public void RequestPath(Tile curren, Tile target)
	{
		currTargetPosition = target;
		currInitialPosition = curren;
	}

	public void RequestNewRandomPath(Tile currPos)
	{
		if(markedTiles.Count != 0)
		{
			currInitialPosition = currPos;
			int randIdx = Random.Range(0,markedTiles.Count);
			currTargetPosition = markedTiles[randIdx];

			currTargetPosition.gameObject.GetComponent<Renderer>().material.color = new Color(0.0f,0.0f,1.0f);
			currTile = currInitialPosition;
			currInitialPosition.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f,0.0f,1.0f);

			CalculatePath();
		}
		else{Debug.Log ("No Tiles Have been Added");}
	}



	// Update is called once per frame
	void Update () {
		
	}
}
