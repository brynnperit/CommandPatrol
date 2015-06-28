using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using Priority_Queue;

public class GridPosition : PriorityQueueNode, IMapPosition {

	IMapPosition[] adjacentPositions;
	public int xPosition{ get; set; }
	public int zPosition{ get; set; }

	public int xSize{ get; set; }
	public int zSize{ get; set; }
	
	public Direction gridDirection{ get; set; }
	public HallwayType gridType{ get; set; }

	public Transform visualRepresentation{ get; set; }

	PathfindingNodeCollection ourNodes;

	public GridPosition(int x, int z){
		xPosition = x;
		zPosition = z;
		gridType = HallwayType.none;
		adjacentPositions = new IMapPosition[4];
		xSize = 1;
		zSize = 1;
	}

	public void setPathNodes(PathfindingNodeCollection nodes){
		ourNodes = nodes;
		ourNodes.owner = this;
	}

	public Transform getCenterPathNode(){
		return ourNodes.centerNode;
	}

	public Transform getPathNode(int xPosition, int zPosition){
		return getCenterPathNode();
	}

	public Transform getPathNode(Direction ofNode){
		return ourNodes.getBorderNode (ofNode);
	}

	//TODO: Double check that this is a sane coding practice. I feel bad about how this makes sure it returns a good or bad result.
	public Transform[] getPathThrough(IMapPosition startingPosition, IMapPosition endingPosition, Transform[] toStorePathIn){
		Direction startingDirection = getDirectionOfAdjacentNonEmptyConnectedMapPosition(startingPosition);
		Direction endingDirection = getDirectionOfAdjacentNonEmptyConnectedMapPosition(endingPosition);
		if (ourNodes != null) {
			return ourNodes.nodesToFollow(startingDirection, endingDirection, toStorePathIn);
		} else {
			if (toStorePathIn != null){
				for (int i = 0; i < toStorePathIn.Length; i++){
					toStorePathIn[i] = null;
				}
			}
			return toStorePathIn;
		}
	}

	public Transform[] getPathThrough(IMapPosition startingPosition, IMapPosition endingPosition){
		Direction startingDirection = getDirectionOfAdjacentNonEmptyConnectedMapPosition(startingPosition);
		Direction endingDirection = getDirectionOfAdjacentNonEmptyConnectedMapPosition(endingPosition);
		if (ourNodes != null) {
			return ourNodes.nodesToFollow(startingDirection, endingDirection);
		} else {
			return new Transform[0];
		}
	}

	public void setAdjacent(Direction adjDirection, IMapPosition toSet){
		adjacentPositions [(int)adjDirection] = toSet;
	}
	
	public void setAdjacents(GridPosition[,] mapGrid){
		for (int i = 0; i < 4; i++){
			setAdjacent((Direction)i, null);
		}

		Direction[] validDirections;
		switch (gridType){
		case HallwayType.hall:
			validDirections = new Direction[2];
			validDirections[0] = gridDirection;
			validDirections[1] = getOppositeDirection(gridDirection);
			break;
		case HallwayType.corner:
			validDirections = new Direction[2];
			validDirections[0] = gridDirection;
			validDirections[1] = getCornerOtherDirection(gridDirection);
			break;
		case HallwayType.T:
			validDirections = new Direction[3];
			validDirections[0] = getLeftDirectionFrom(gridDirection);
			validDirections[1] = gridDirection;
			validDirections[2] = getRightDirectionFrom(gridDirection);
			break;
		case HallwayType.plus:
			validDirections = new Direction[4];
			validDirections[0] = Direction.north;
			validDirections[1] = Direction.east;
			validDirections[2] = Direction.south;
			validDirections[3] = Direction.west;
			break;
		case HallwayType.end:
			validDirections = new Direction[1];
			validDirections[0] = gridDirection;
			break;
		default:
			validDirections = new Direction[0];
			break;
		}
		for (int i = 0; i < validDirections.Length; i++){
			setAdjacent(validDirections[i], getPosition(mapGrid, validDirections[i], this));
		}

	}

	public ReadOnlyCollection<IMapPosition> getAdjacents(){
		return new ReadOnlyCollection<IMapPosition>(adjacentPositions);
	}

	public void addExitToGridSquare(Direction dirToAddExit){
		bool interceptingExistingHallway;
		
		if (gridType == HallwayType.none){
			interceptingExistingHallway = false;
		}else{
			interceptingExistingHallway = true;
		}
		
		if (interceptingExistingHallway){
			switch(gridType){
			case HallwayType.hall:
				if (dirToAddExit == gridDirection || dirToAddExit == getOppositeDirection(gridDirection)){
					//Do nothing, we're just passing through an existing hallways, no changes needed
				}else {
					//Make it into a T towards dirToAddExit
					gridType = HallwayType.T;
					gridDirection = dirToAddExit;
				}
				break;
			case HallwayType.corner:
				Direction otherExistingDirection = GridPosition.getCornerOtherDirection(gridDirection);
				if (dirToAddExit == gridDirection || dirToAddExit == otherExistingDirection){
					//Do nothing, we're just passing through an existing hallways, no changes needed
				}else if (dirToAddExit == getOppositeDirection(gridDirection)){
					//dirToAddExit is gonna be opposite one of the existing directions. Find out which one it is and point the t to the other one
					gridType =  HallwayType.T;
					gridDirection = otherExistingDirection;
				}else{
					gridType = HallwayType.T;
					gridDirection = gridDirection;
				}
				
				break;
			case HallwayType.T:
				if (dirToAddExit == getOppositeDirection(gridDirection)){
					gridType = HallwayType.plus;
				}
				break;
			case HallwayType.plus:
				//We're good, do nothing
				break;
			case HallwayType.end:
				Direction oppositeExisting = getOppositeDirection(gridDirection);
				if (dirToAddExit == gridDirection){
					//Going out the way we came in, do nothing
				}else if (dirToAddExit == oppositeExisting){
					gridType = HallwayType.hall;
				}else{
					gridType = HallwayType.corner;
					gridDirection = getCornerDirection(dirToAddExit, gridDirection);
				}
				break;
			}
		}else{
			gridType = HallwayType.end;
			gridDirection = dirToAddExit;
		}
	}
	
	public static Direction getLeftDirectionFrom(Direction toTurn){
		switch (toTurn) {
		case Direction.north:
			return Direction.east;
		case Direction.east:
			return Direction.south;
		case Direction.south:
			return Direction.west;
		}
		return Direction.north;
	}
	
	public static Direction getRightDirectionFrom(Direction toTurn){
		switch (toTurn) {
		case Direction.north:
			return Direction.west;
		case Direction.east:
			return Direction.north;
		case Direction.south:
			return Direction.east;
		}
		return Direction.south;
	}
	
	public static Direction getOppositeDirection(Direction toOppose){
		switch (toOppose) {
		case Direction.north:
			return Direction.south;
		case Direction.east:
			return Direction.west;
		case Direction.south:
			return Direction.north;
		}
		return Direction.east;
	}
	
	public static Direction getCornerDirection (Direction newDirection, Direction oldDirection){
		if ((newDirection == Direction.north && oldDirection == Direction.east) || (newDirection == Direction.east && oldDirection == Direction.north)) {
			return Direction.north;
		} else if ((newDirection == Direction.south && oldDirection == Direction.east) || (newDirection == Direction.east && oldDirection == Direction.south)) {
			return Direction.east;
		} else if ((newDirection == Direction.south && oldDirection == Direction.west) || (newDirection == Direction.west && oldDirection == Direction.south)) {
			return Direction.south;
		} else {
			return Direction.west;
		}
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

	public bool isEmpty(){
		if (gridType == HallwayType.none) {
			return true;
		} else {
			return false;
		}
	}

	public static Direction getDirectionBetweenAdjacentGridPositions(GridPosition firstPosition, GridPosition secondPosition){
		if (firstPosition.zPosition > secondPosition.zPosition) {
			return Direction.north;
		} else if (firstPosition.xPosition < secondPosition.xPosition){
			return Direction.east;
		} else if (firstPosition.zPosition < secondPosition.zPosition){
			return Direction.south;
		} else{
			return Direction.west;
		}
	}

	public static int getGridDistanceBetweenGridPositions(GridPosition first, GridPosition second){
		return Mathf.Abs(first.xPosition - second.xPosition) + Mathf.Abs(first.zPosition - second.zPosition);
	}

	public Direction getDirectionOfAdjacentNonEmptyConnectedMapPosition(IMapPosition adjacentPosition){
		if (adjacentPositions != null){
			for (int dir = 0; dir < adjacentPositions.Length; dir++){
				if (adjacentPositions[dir] != null && adjacentPositions[dir].xPosition == adjacentPosition.xPosition && adjacentPositions[dir].zPosition == adjacentPosition.zPosition){
					return (Direction)dir;
				}
			}
		}
		//TODO: Find a better thing to do here than just return north. I mean, yes, GIGO applies but it feels impolite.
		return Direction.north;
	}
}

public enum Direction{ north=0, east=1, south=2, west=3};
public enum HallwayType{ none=-1, hall=0, corner=1, T=2,plus=3,end=4 };