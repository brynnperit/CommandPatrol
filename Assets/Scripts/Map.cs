using UnityEngine;
using System.Collections;

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
		Direction currentDirection = getNewDirection (currentPosition);
		addExitToGridSquare (currentDirection, currentPosition);
		currentPosition = GridPosition.getPosition(mapGrid, currentDirection, currentPosition);
		Direction newDirection;
		int stepNumber = 1;
		while (stepNumber < numberSteps) {

			addExitToGridSquare(getOppositeDirection(currentDirection), currentPosition);

			newDirection = getNewDirection(currentPosition);

			if (stepNumber < numberSteps - 1){
				addExitToGridSquare(newDirection, currentPosition);
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

	void addExitToGridSquare(Direction dirToAddExit, GridPosition gridSquare){
		bool interceptingExistingHallway;

		if (gridSquare.gridType == HallwayType.none){
			interceptingExistingHallway = false;
		}else{
			interceptingExistingHallway = true;
		}

		if (interceptingExistingHallway){
			HallwayType existingType = gridSquare.gridType;
			Direction existingDirection = gridSquare.gridDirection;
			switch(existingType){
			case HallwayType.hall:
				if (dirToAddExit == existingDirection || dirToAddExit == getOppositeDirection(existingDirection)){
					//Do nothing, we're just passing through an existing hallways, no changes needed
				}else {
					//Make it into a T towards dirToAddExit
					gridSquare.gridType = HallwayType.T;
					gridSquare.gridDirection = dirToAddExit;
				}
				break;
			case HallwayType.corner:
				Direction otherExistingDirection = GridPosition.getCornerOtherDirection(existingDirection);
				if (dirToAddExit == existingDirection || dirToAddExit == otherExistingDirection){
					//Do nothing, we're just passing through an existing hallways, no changes needed
				}else if (dirToAddExit == getOppositeDirection(existingDirection)){
				//dirToAddExit is gonna be opposite one of the existing directions. Find out which one it is and point the t to the other one
					gridSquare.gridType =  HallwayType.T;
					gridSquare.gridDirection = otherExistingDirection;
				}else{
					gridSquare.gridType = HallwayType.T;
					gridSquare.gridDirection = existingDirection;
				}
			
				break;
			case HallwayType.T:
				if (dirToAddExit == getOppositeDirection(existingDirection)){
					gridSquare.gridType = HallwayType.plus;
				}
				break;
			case HallwayType.plus:
				//We're good, do nothing
				break;
			case HallwayType.end:
				Direction oppositeExisting = getOppositeDirection(existingDirection);
				if (dirToAddExit == existingDirection){
					//Going out the way we came in, do nothing
				}else if (dirToAddExit == oppositeExisting){
					gridSquare.gridType = HallwayType.hall;
				}else{
					gridSquare.gridType = HallwayType.corner;
					gridSquare.gridDirection = getCornerDirection(dirToAddExit, existingDirection);
				}
				break;
			}
		}else{
			gridSquare.gridType = HallwayType.end;
			gridSquare.gridDirection = dirToAddExit;
		}
	}
	
	Direction leftDirectionFrom(Direction toTurn){
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

	Direction rightDirectionFrom(Direction toTurn){
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

	Direction getOppositeDirection(Direction toOppose){
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

	Direction getCornerDirection (Direction newDirection, Direction oldDirection){
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



	Direction getNewDirection(GridPosition currentPosition){
		Direction newDirection;

		if (currentPosition.xPosition == 0 && currentPosition.zPosition == 0) {
			//Northwest corner
			newDirection = (Direction)Random.Range (1, 3);
		} 
		else if (currentPosition.xPosition == xDimension - 1 && currentPosition.zPosition == 0) {
			//Northeast corner
			newDirection = (Direction)Random.Range (2, 4);
		} 
		else if (currentPosition.xPosition == xDimension - 1 && currentPosition.zPosition == zDimension - 1) {
			//Southeast corner
			newDirection = (Direction)Random.Range (0, 2);
			if (newDirection == Direction.east){
				newDirection = Direction.west;
			}
		} 
		else if (currentPosition.xPosition == 0 && currentPosition.zPosition == zDimension - 1) {
			//Southwest corner
			newDirection = (Direction)Random.Range (0, 2);
		} 
		else if (currentPosition.zPosition == 0) {
			//North wall
			newDirection = (Direction)Random.Range (1, 4);
		} 
		else if (currentPosition.xPosition == xDimension - 1) {
			//East wall
			newDirection = (Direction)Random.Range (0, 3);
			if (newDirection == Direction.south){
				newDirection = Direction.west;
			}
			if (newDirection == Direction.east){
				newDirection = Direction.south;
			}
		} 
		else if (currentPosition.zPosition == zDimension - 1) {
			//South wall
			newDirection = (Direction)Random.Range (0, 3);
			if (newDirection == Direction.south){
				newDirection = Direction.west;
			}
		} 
		else if (currentPosition.xPosition == 0) {
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
