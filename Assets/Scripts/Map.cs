using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class Map : MonoBehaviour {

	public int numberSteps;
	public int xDimension;
	public int zDimension;
	public Transform hallway;
	public Transform hallwayCorner;
	public Transform hallwayT;
	public Transform hallwayPlus;
	public Transform hallwayEnd;

	GridPosition[,] mapGrid;
	public float hallwayGridUnitSize;
	public float yOffset;

	// Use this for initialization
	void Start () {
		mapGrid = new GridPosition[xDimension, zDimension];
		for (int xPos = 0; xPos < xDimension; xPos++) {
			for (int zPos = 0; zPos < zDimension; zPos++){
				mapGrid[xPos, zPos] = new GridPosition(xPos, zPos);
			}
		}
		GridPosition currentPosition = mapGrid[xDimension / 2, zDimension / 2];
		Direction currentDirection = getNewDirection(currentPosition.xPosition, currentPosition.zPosition);
		currentPosition.addExitToGridSquare (currentDirection);
		currentPosition = GridPosition.getPosition(mapGrid, currentDirection, currentPosition);
		Direction newDirection;
		int stepNumber = 1;
		while (stepNumber < numberSteps) {

			currentPosition.addExitToGridSquare(GridPosition.getOppositeDirection(currentDirection));

			newDirection = getNewDirection(currentPosition.xPosition, currentPosition.zPosition);

			if (stepNumber < numberSteps - 1){
				currentPosition.addExitToGridSquare(newDirection);
			}
			currentPosition = GridPosition.getPosition(mapGrid, newDirection, currentPosition);

			currentDirection = newDirection;
			stepNumber++;
		}

		for (int xPos = 0; xPos < xDimension; xPos++){
			for (int zPos = 0; zPos < zDimension; zPos++){
				HallwayType hallType = mapGrid[xPos,zPos].gridType;
				Direction hallDirection = mapGrid[xPos,zPos].gridDirection;
				float xCoord = xPos * hallwayGridUnitSize;
				float yCoord = yOffset;
				float zCoord = -zPos * hallwayGridUnitSize;
				Transform newHall;

				mapGrid[xPos,zPos].setAdjacents(mapGrid);

				switch (hallType){
				case HallwayType.hall:
					newHall = Instantiate(hallway, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection), 0));
					newHall.parent = transform;
					break;
				case HallwayType.corner:
					newHall = Instantiate(hallwayCorner, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					//These hallways, and the subsequent ones, end up being rotated 180 degrees from the direction we want to face. The +2 fixes that
					newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
					newHall.parent = transform;
					break;
				case HallwayType.plus:
					newHall = Instantiate(hallwayPlus, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					//newHall.Rotate(new Vector3(0, -90 * ((int)hallDirection), 0));
					newHall.parent = transform;
					break;
				case HallwayType.T:
					newHall = Instantiate(hallwayT, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
					newHall.parent = transform;
					break;
				case HallwayType.end:
					newHall = Instantiate(hallwayEnd, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
					newHall.parent = transform;
					break;
				}
			}
		}
	}

	public GridPosition inefficientGetRandomMapPosition(){
		GridPosition toReturn;
		do {
			int xPos = Random.Range(0, xDimension);
			int zPos = Random.Range(0, zDimension);
			toReturn = mapGrid[xPos,zPos];

		} while(toReturn.gridType == HallwayType.none);
		return toReturn;
	}


	//This code is based on A* search algorithms found on http://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
	public static GridPosition[] getPathToPosition(GridPosition start, GridPosition end, int mapSize){
		//Evaluate each adjacent node, adding them to the open set sorted by their cost. Then repeat for the lowest estimated cost node in the open set. 
		Dictionary<GridPosition, GridPosition> cameFrom = new Dictionary<GridPosition, GridPosition> ();
		Dictionary<GridPosition, int> costSoFar	= new Dictionary<GridPosition, int>();
		HeapPriorityQueue<GridPosition> frontier = new HeapPriorityQueue<GridPosition>(mapSize);
		frontier.Enqueue(start, 0);
		
		cameFrom.Add(start, start);
		costSoFar.Add(start, 0);
		
		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			
			if (current.Equals(end))
			{
				break;
			}
			
			foreach (GridPosition next in current.getAdjacents())
			{
				if (next != null){
					int newCost = costSoFar[current] + 1;
					if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
					{
						costSoFar.Add(next, newCost);
						int priority = newCost + Heuristic(next, end);
						frontier.Enqueue(next, priority);
						cameFrom.Add(next, current);
					}
				}
			}
		}
		
		return new GridPosition[0];
	}
	
	//Note: This should be changed if the map moves away from a simple grid to more optimized hallways
	static public int Heuristic(GridPosition a, GridPosition b)
	{
		return Mathf.Abs(a.xPosition - b.xPosition) + Mathf.Abs(a.zPosition - b.zPosition);
	}


	Direction getNewDirection(int xCoord, int zCoord){
		Direction newDirection;

		if (xCoord == 0 && zCoord == 0) {
			//Northwest corner
			newDirection = (Direction)Random.Range (1, 3);
		} 
		else if (xCoord == xDimension - 1 && zCoord == 0) {
			//Northeast corner
			newDirection = (Direction)Random.Range (2, 4);
		} 
		else if (xCoord == xDimension - 1 && zCoord == zDimension - 1) {
			//Southeast corner
			newDirection = (Direction)Random.Range (0, 2);
			if (newDirection == Direction.east){
				newDirection = Direction.west;
			}
		} 
		else if (xCoord == 0 && zCoord == zDimension - 1) {
			//Southwest corner
			newDirection = (Direction)Random.Range (0, 2);
		} 
		else if (zCoord == 0) {
			//North wall
			newDirection = (Direction)Random.Range (1, 4);
		} 
		else if (xCoord == xDimension - 1) {
			//East wall
			newDirection = (Direction)Random.Range (0, 3);
			if (newDirection == Direction.south){
				newDirection = Direction.west;
			}
			if (newDirection == Direction.east){
				newDirection = Direction.south;
			}
		} 
		else if (zCoord == zDimension - 1) {
			//South wall
			newDirection = (Direction)Random.Range (0, 3);
			if (newDirection == Direction.south){
				newDirection = Direction.west;
			}
		} 
		else if (xCoord == 0) {
			//West wall
			newDirection = (Direction)Random.Range (0, 3);
		} 
		else {
			//Somewhere in the middle
			newDirection = (Direction)Random.Range (0, 4);
		}
		
		return newDirection;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
