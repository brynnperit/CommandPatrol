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
	public enum Directions{ north=0, east=1, south=2, west=3};
	public enum HallwayTypes{ hall=0, corner=1, t=2,plus=3,end=4 };
	private struct GridPosition {
		public int xPosition;
		public int zPosition;
	}

	// Use this for initialization
	void Start () {
		if (xDimension * zDimension < numberSteps) {
			numberSteps = xDimension * zDimension;
		}
		//X coord, Z coord, direction of hallway
		int[,,] hallwayGrid = new int[xDimension,zDimension,2];
		for (int xPos = 0; xPos < xDimension; xPos++) {
			for (int zPos = 0; zPos < zDimension; zPos++){
				hallwayGrid[xPos,zPos,0] = -1;
			}
		}
		GridPosition currentPosition = new GridPosition ();
		GridPosition newPosition = new GridPosition ();
		currentPosition.xPosition = xDimension / 2;
		currentPosition.zPosition = zDimension / 2;
//		newPosition.xPosition;
//		newPosition.zPosition;
		Directions currentDirection = getNewDirection (currentPosition);
		hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,0] = (int)HallwayTypes.end;
		hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,1] = (int)currentDirection;
		setNewPosition((Directions)Random.Range (0, 4), currentPosition, currentPosition);
		Directions newDirection;
		int stepNumber = 1;
		while (stepNumber < numberSteps) {

			bool interceptingExistingHallway;
			if (hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,0] == -1){
				interceptingExistingHallway = false;
			}else{
				interceptingExistingHallway = true;
			}


			newDirection = getNewDirection(currentPosition);
			setNewPosition(newDirection, newPosition, currentPosition);

			if (interceptingExistingHallway){
			}else{
				if (isOppositeDirection(currentDirection, newDirection)){
					hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,0] = (int)HallwayTypes.end;
					hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,1] = (int)newDirection;
				}else if (newDirection == currentDirection){
					hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,0] = (int)HallwayTypes.hall;
					hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,1] = (int)newDirection;
				}else{
					hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,0] = (int)HallwayTypes.corner;
					hallwayGrid[currentPosition.xPosition,currentPosition.zPosition,1] = (int)getCornerDirection(newDirection, currentDirection);
				}
			}
			currentDirection = newDirection;
			currentPosition.xPosition = newPosition.xPosition;
			currentPosition.zPosition = newPosition.zPosition;
			stepNumber++;
		}

	}

	bool isOppositeDirection(Directions firstDirection, Directions secondDirection){
		if (firstDirection == Directions.north && secondDirection == Directions.south) {
			return true;
		} else if (firstDirection == Directions.east && secondDirection == Directions.west) {
			return true;
		} else if (firstDirection == Directions.south && secondDirection == Directions.north) {
			return true;
		} else if (firstDirection == Directions.west && secondDirection == Directions.east) {
			return true;
		} else {
			return false;
		}

	}

	Directions getCornerDirection (Directions newDirection, Directions oldDirection){
		if ((newDirection == Directions.north && oldDirection == Directions.east) || (newDirection == Directions.east && oldDirection == Directions.north)) {
			return Directions.north;
		} else if ((newDirection == Directions.south && oldDirection == Directions.east) || (newDirection == Directions.east && oldDirection == Directions.south)) {
			return Directions.east;
		} else if ((newDirection == Directions.south && oldDirection == Directions.west) || (newDirection == Directions.west && oldDirection == Directions.south)) {
			return Directions.south;
		} else {
			return Directions.west;
		}
	}

	Directions getNewDirection(GridPosition currentPosition){
		Directions newDirection;

		if (currentPosition.xPosition == 0 && currentPosition.zPosition == 0) {
			//Northwest corner
			newDirection = (Directions)Random.Range (1, 3);
		} 
		else if (currentPosition.xPosition == xDimension - 1 && currentPosition.zPosition == 0) {
			//Northeast corner
			newDirection = (Directions)Random.Range (2, 4);
		} 
		else if (currentPosition.xPosition == xDimension - 1 && currentPosition.zPosition == zDimension - 1) {
			//Southeast corner
			newDirection = (Directions)Random.Range (0, 2);
			if (newDirection == Directions.east){
				newDirection = Directions.west;
			}
		} 
		else if (currentPosition.xPosition == 0 && currentPosition.zPosition == zDimension - 1) {
			//Southwest corner
			newDirection = (Directions)Random.Range (0, 2);
		} 
		else if (currentPosition.zPosition == 0) {
			//North wall
			newDirection = (Directions)Random.Range (1, 4);
		} 
		else if (currentPosition.xPosition == xDimension - 1) {
			//East wall
			newDirection = (Directions)Random.Range (0, 3);
			if (newDirection != Directions.north){
				newDirection = newDirection + 1;
			}
		} 
		else if (currentPosition.zPosition == zDimension - 1) {
			//South wall
			newDirection = (Directions)Random.Range (0, 3);
			if (newDirection != Directions.south){
				newDirection = newDirection + 1;
			}
		} 
		else if (currentPosition.xPosition == 0) {
			//West wall
			newDirection = (Directions)Random.Range (0, 3);
		} 
		else {
			//Somewhere in the middle
			newDirection = (Directions)Random.Range (0, 4);
		}
		
		return newDirection;
	}


	void setNewPosition(Directions direction, GridPosition toSet, GridPosition toSetFrom){
		switch (direction) {
			case (Directions.north):
				toSet.xPosition = toSetFrom.xPosition;
				toSet.zPosition = toSetFrom.zPosition - 1;
				break;
			case (Directions.east):
				toSet.xPosition = toSetFrom.xPosition + 1;
				toSet.zPosition = toSetFrom.zPosition;
				break;
			case(Directions.south):
				toSet.xPosition = toSetFrom.xPosition;
				toSet.zPosition = toSetFrom.zPosition + 1;
				break;
			case(Directions.west):
				toSet.xPosition = toSetFrom.xPosition - 1;
				toSet.zPosition = toSetFrom.zPosition;
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
