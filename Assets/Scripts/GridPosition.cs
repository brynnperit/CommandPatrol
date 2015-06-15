using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class GridPosition : PriorityQueueNode {

	GridPosition[] adjacentPositions;

	public GridPosition(int x, int z){
		xPosition = x;
		zPosition = z;
		gridType = HallwayType.none;
		adjacentPositions = new GridPosition[4];
	}

	public void setAdjacent(Direction adjDirection, GridPosition toSet){
		if (toSet.gridType != HallwayType.none) {
			adjacentPositions [(int)adjDirection] = toSet;
		}
	}

	public void setAdjacents(GridPosition[,] mapGrid){
		for (int i = 0; i < 4; i++){
			adjacentPositions[i] = getPosition(mapGrid, (Direction)i, this);
		}
	}

	public GridPosition[] getAdjacents(){
		return adjacentPositions;
	}

	public static Direction getCornerOtherDirection(Direction ofCorner){
		switch (ofCorner) {
		case Direction.north:
			return Direction.east;
		case Direction.east:
			return Direction.south;
		case Direction.south:
			return Direction.west;
		}
		//case Direction.west:
		return Direction.north;
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
		
		return new GridPosition[0];
	}

	//Note: This should be changed if the map moves away from a simple grid to more optimized hallways
	static public int Heuristic(GridPosition a, GridPosition b)
	{
		return Mathf.Abs(a.xPosition - b.xPosition) + Mathf.Abs(a.zPosition - b.zPosition);
	}

	
	public static GridPosition getPosition(GridPosition[,] mapGrid, Direction direction, GridPosition toGetFrom){
		switch (direction) {
		case (Direction.north):
			if (toGetFrom.zPosition != 0){
				return mapGrid[toGetFrom.xPosition, toGetFrom.zPosition - 1];
			}
			break;
		case (Direction.east):
			if (toGetFrom.xPosition != mapGrid.GetLength(0) - 1){
				return mapGrid[toGetFrom.xPosition + 1, toGetFrom.zPosition];
			}
			break;
		case(Direction.south):
			if (toGetFrom.zPosition != mapGrid.GetLength(1) - 1){
				return mapGrid[toGetFrom.xPosition, toGetFrom.zPosition + 1];
			}
			break;
		case(Direction.west):
			if (toGetFrom.xPosition != 0){
				return mapGrid[toGetFrom.xPosition - 1, toGetFrom.zPosition];
			}
			break;
		}
		return null;
	}

	public int xPosition{ get; set; }
	public int zPosition{ get; set; }

	public Direction gridDirection{ get; set; }
	public HallwayType gridType{ get; set; }
}

public enum Direction{ north=0, east=1, south=2, west=3};
public enum HallwayType{ none=-1, hall=0, corner=1, T=2,plus=3,end=4 };