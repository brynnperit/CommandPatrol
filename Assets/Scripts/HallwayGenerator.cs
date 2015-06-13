using UnityEngine;
using System.Collections;

public class HallwayGenerator : MonoBehaviour {

	public int numberSteps;
	public int xDimension;
	public int zDimension;
	public Transform hallway;
	public Transform hallwayCorner;
	public Transform hallwayT;
	public Transform hallwayPlus;
	public Transform hallwayEnd;
	public enum Direction{ north=0, east=1, south=2, west=3};
	public enum HallwayTypes{ hall=0, corner=1, T=2,plus=3,end=4 };
	private class GridPosition {
		public GridPosition(int x, int z){
			xPosition = x;
			zPosition = z;
		}
		public int xPosition;
		public int zPosition;
	}
	public float hallwayGridUnitSize;
	public float yOffset;

	// Use this for initialization
	void Start () {
		//X coord, Z coord, (type of hallway, direction of hallway)
		int[,,] hallwayGrid = new int[xDimension,zDimension,2];
		for (int xPos = 0; xPos < xDimension; xPos++) {
			for (int zPos = 0; zPos < zDimension; zPos++){
				hallwayGrid[xPos,zPos,0] = -1;
			}
		}
//		GridPosition currentPosition = new GridPosition (xDimension / 2, zDimension / 2);
		GridPosition currentPosition = new GridPosition (0, 0);
		Direction currentDirection = getNewDirection (currentPosition);
		addExitToGridSquare (currentDirection, hallwayGrid, currentPosition);
		setNewPosition(currentDirection, currentPosition);
		Direction newDirection;
		int stepNumber = 1;
		while (stepNumber < numberSteps) {

			addExitToGridSquare(getOppositeDirection(currentDirection), hallwayGrid, currentPosition);

			newDirection = getNewDirection(currentPosition);

			if (stepNumber < numberSteps - 1){
				addExitToGridSquare(newDirection, hallwayGrid, currentPosition);
			}
			setNewPosition(newDirection, currentPosition);

			currentDirection = newDirection;
			stepNumber++;
		}

		for (int xPos = 0; xPos < xDimension; xPos++){
			for (int zPos = 0; zPos < zDimension; zPos++){
				HallwayTypes hallType = (HallwayTypes)hallwayGrid[xPos,zPos,0];
				Direction hallDirection = (Direction)hallwayGrid[xPos,zPos,1];
				float xCoord = xPos * hallwayGridUnitSize;
				float yCoord = yOffset;
				float zCoord = -zPos * hallwayGridUnitSize;
				Transform newHall;

				switch (hallType){
				case HallwayTypes.hall:
					newHall = Instantiate(hallway, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection), 0));
					newHall.parent = transform;
					break;
				case HallwayTypes.corner:
					newHall = Instantiate(hallwayCorner, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
					newHall.parent = transform;
					break;
				case HallwayTypes.plus:
					newHall = Instantiate(hallwayPlus, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					//newHall.Rotate(new Vector3(0, -90 * ((int)hallDirection), 0));
					newHall.parent = transform;
					break;
				case HallwayTypes.T:
					newHall = Instantiate(hallwayT, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
					newHall.parent = transform;
					break;
				case HallwayTypes.end:
					newHall = Instantiate(hallwayEnd, new Vector3 (xCoord, yCoord, zCoord), Quaternion.identity) as Transform;
					newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
					newHall.parent = transform;
					break;
				}
			}
		}
	}

	void addExitToGridSquare(Direction dirToAddExit, int[,,] grid, GridPosition gridSquare){
		bool interceptingExistingHallway;
		int bla = 0;
		if (gridSquare.xPosition < 0 || gridSquare.xPosition >= xDimension) {
			bla++;
		}
		if (gridSquare.zPosition < 0 || gridSquare.zPosition >= zDimension){
			bla++;
		}
		if (grid[gridSquare.xPosition,gridSquare.zPosition,0] == -1){
			interceptingExistingHallway = false;
		}else{
			interceptingExistingHallway = true;
		}

		if (interceptingExistingHallway){
			HallwayTypes existingType = (HallwayTypes)grid[gridSquare.xPosition,gridSquare.zPosition,0];
			Direction existingDirection = (Direction)grid[gridSquare.xPosition,gridSquare.zPosition,1];
			switch(existingType){
			case HallwayTypes.hall:
				if (dirToAddExit == existingDirection || dirToAddExit == getOppositeDirection(existingDirection)){
					//Do nothing, we're just passing through an existing hallways, no changes needed
				}else {
					//Make it into a T towards dirToAddExit
					grid[gridSquare.xPosition,gridSquare.zPosition,0] = (int)HallwayTypes.T;
					grid[gridSquare.xPosition,gridSquare.zPosition,1] = (int)dirToAddExit;
				}
				break;
			case HallwayTypes.corner:
				Direction otherExistingDirection = getCornerOtherDirection(existingDirection);
				if (dirToAddExit == existingDirection || dirToAddExit == otherExistingDirection){
					//Do nothing, we're just passing through an existing hallways, no changes needed
				}else if (dirToAddExit == getOppositeDirection(existingDirection)){
				//dirToAddExit is gonna be opposite one of the existing directions. Find out which one it is and point the t to the other one
					grid[gridSquare.xPosition,gridSquare.zPosition,0] = (int)HallwayTypes.T;
					grid[gridSquare.xPosition,gridSquare.zPosition,1] = (int)otherExistingDirection;
				}else{
					grid[gridSquare.xPosition,gridSquare.zPosition,0] = (int)HallwayTypes.T;
					grid[gridSquare.xPosition,gridSquare.zPosition,1] = (int)existingDirection;
				}
			
				break;
			case HallwayTypes.T:
				if (dirToAddExit == getOppositeDirection(existingDirection)){
					grid[gridSquare.xPosition,gridSquare.zPosition,0] = (int)HallwayTypes.plus;
				}
				break;
			case HallwayTypes.plus:
				//We're good, do nothing
				break;
			case HallwayTypes.end:
				Direction oppositeExisting = getOppositeDirection(existingDirection);
				if (dirToAddExit == existingDirection){
					//Going out the way we came in, do nothing
				}else if (dirToAddExit == oppositeExisting){
					grid[gridSquare.xPosition,gridSquare.zPosition,0] = (int)HallwayTypes.hall;
				}else{
					grid[gridSquare.xPosition,gridSquare.zPosition,0] = (int)HallwayTypes.corner;
					grid[gridSquare.xPosition,gridSquare.zPosition,1] = (int)getCornerDirection(dirToAddExit, existingDirection);
				}
				break;
			}
		}else{
			grid[gridSquare.xPosition,gridSquare.zPosition,0] = (int)HallwayTypes.end;
			grid[gridSquare.xPosition,gridSquare.zPosition,1] = (int)dirToAddExit;
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

	Direction getCornerOtherDirection(Direction ofCorner){
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


	void setNewPosition(Direction direction, GridPosition toSet){
		switch (direction) {
			case (Direction.north):
				toSet.zPosition = toSet.zPosition - 1;
				break;
			case (Direction.east):
				toSet.xPosition = toSet.xPosition + 1;
				break;
			case(Direction.south):
				toSet.zPosition = toSet.zPosition + 1;
				break;
			case(Direction.west):
				toSet.xPosition = toSet.xPosition - 1;
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
