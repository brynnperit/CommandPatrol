using UnityEngine;
using UnityEngine.UI;
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

	public Transform pathNode;
	public float pathBoxScale;

	public PathfindingAgent pathAgent;
	public PathfindingAgent[] pathAgents;

	bool lastClickedValid;
	Vector3 lastMouseCoordsOnMap;
	GridPosition lastClickedPosition;

	public Camera playerCamera;

	public Text coordsClickedOn;

	Plane mapBasePlane;

	bool editMode;

	// Use this for initialization
	void Start () {
		editMode = false;

		mapBasePlane = new Plane(transform.up, transform.position);

		mapGrid = new GridPosition[xDimension, zDimension];
		for (int xPos = 0; xPos < xDimension; xPos++) {
			for (int zPos = 0; zPos < zDimension; zPos++){
				mapGrid[xPos, zPos] = new GridPosition(xPos, zPos);
			}
		}
		GridPosition currentPosition = mapGrid[xDimension / 2, zDimension / 2];
//		GridPosition currentPosition = mapGrid[0,0];
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
				generateGridPosition(mapGrid[xPos, zPos]);
			}
		}
		pathAgents = new PathfindingAgent[1];
		PathfindingAgent mainAgent = Instantiate (pathAgent, Vector3.zero, Quaternion.identity) as PathfindingAgent;
		mainAgent.initialize (this, reallyinefficientGetRandomMapPosition (), reallyinefficientGetRandomMapPosition ());
		pathAgents [0] = mainAgent;



	}

	public void generateGridPosition(GridPosition toGenerate){
		if (toGenerate.visualRepresentation != null) {
			Destroy(toGenerate.visualRepresentation.gameObject);
		}
		HallwayType hallType = toGenerate.gridType;
		if (hallType != HallwayType.none){
			Direction hallDirection = toGenerate.gridDirection;
			float xCoord = toGenerate.xPosition * hallwayGridUnitSize;
			float yCoord = yOffset;
			float zCoord = toGenerate.zPosition * hallwayGridUnitSize * -1.0f;
			Transform newHall;
			
			toGenerate.setAdjacents(mapGrid);
			
			switch (hallType){
			case HallwayType.hall:
				newHall = Instantiate(hallway, transform.position, Quaternion.identity) as Transform;
				newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection), 0));
				newHall.parent = transform;
				newHall.localPosition = new Vector3(xCoord, yCoord, zCoord);
				toGenerate.setPathNodes(createNodeCollection(newHall, toGenerate, pathBoxScale, pathNode));
				toGenerate.visualRepresentation = newHall;
				break;
			case HallwayType.corner:
				newHall = Instantiate(hallwayCorner, transform.position, Quaternion.identity) as Transform;
				//These hallways, and the subsequent ones, end up being rotated 180 degrees from the direction we want to face. The +2 fixes that
				newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
				newHall.parent = transform;
				newHall.localPosition = new Vector3(xCoord, yCoord, zCoord);
				toGenerate.setPathNodes(createNodeCollection(newHall, toGenerate, pathBoxScale, pathNode));
				toGenerate.visualRepresentation = newHall;
				break;
			case HallwayType.plus:
				newHall = Instantiate(hallwayPlus, transform.position, Quaternion.identity) as Transform;
				//newHall.Rotate(new Vector3(0, -90 * ((int)hallDirection), 0));
				newHall.parent = transform;
				newHall.localPosition = new Vector3(xCoord, yCoord, zCoord);
				toGenerate.setPathNodes(createNodeCollection(newHall, toGenerate, pathBoxScale, pathNode));
				toGenerate.visualRepresentation = newHall;
				break;
			case HallwayType.T:
				newHall = Instantiate(hallwayT, transform.position, Quaternion.identity) as Transform;
				newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
				newHall.parent = transform;
				newHall.localPosition = new Vector3(xCoord, yCoord, zCoord);
				toGenerate.setPathNodes(createNodeCollection(newHall, toGenerate, pathBoxScale, pathNode));
				toGenerate.visualRepresentation = newHall;
				break;
			case HallwayType.end:
				newHall = Instantiate(hallwayEnd, transform.position, Quaternion.identity) as Transform;
				newHall.Rotate(new Vector3(0, 90 * ((int)hallDirection + 2), 0));
				newHall.parent = transform;
				newHall.localPosition = new Vector3(xCoord, yCoord, zCoord);
				toGenerate.setPathNodes(createNodeCollection(newHall, toGenerate, pathBoxScale, pathNode));
				toGenerate.visualRepresentation = newHall;
				break;
			}
		}
	}

	public GridPosition getGridPosition(int xCoord, int zCoord){
		return mapGrid [xCoord, zCoord];
	}

	public GridPosition reallyinefficientGetRandomMapPosition(){
		GridPosition toReturn;
		do {
			int xPos = Random.Range(0, xDimension);
			int zPos = Random.Range(0, zDimension);
			toReturn = mapGrid[xPos,zPos];

		} while(toReturn.gridType == HallwayType.none);
		return toReturn;
	}

	public int getMapSize(){
		return xDimension * zDimension;
	}

	//This code is based on A* search algorithms found on http://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
	public static IMapPosition[] getPathToPosition(IMapPosition start, IMapPosition end, int mapSize){
		//Evaluate each adjacent node, adding them to the open set sorted by their cost. Then repeat for the lowest estimated cost node in the open set. 
		Dictionary<IMapPosition, IMapPosition> cameFrom = new Dictionary<IMapPosition, IMapPosition> ();
		Dictionary<IMapPosition, int> costSoFar	= new Dictionary<IMapPosition, int>();
		HeapPriorityQueue<IMapPosition> frontier = new HeapPriorityQueue<IMapPosition>(mapSize);
		frontier.Enqueue(start, 0);
		
		cameFrom.Add(start, start);
		costSoFar.Add(start, 0);

		IMapPosition current = start;
		
		while (frontier.Count > 0)
		{
			current = frontier.Dequeue();
			
			if (current.Equals(end))
			{
				break;
			}

			foreach (IMapPosition next in current.getAdjacents())
			{
				if (next != null){
					int newCost = costSoFar[current] + 1;
					bool containsKey = costSoFar.ContainsKey(next);
					if (!containsKey || newCost < costSoFar[next])
					{
						if (!containsKey){
							costSoFar.Add(next, newCost);
						}else{
							//This prevents an infinite loop for some reason.
							break;
						}

						int priority = newCost + Heuristic(next, end);
						frontier.Enqueue(next, priority);
						if (!cameFrom.ContainsKey(next)){
							cameFrom.Add(next, current);
						}else{
							cameFrom[next] = current;
						}
					}
				}
			}
		}
		if (current == end) {
			List<IMapPosition> reversePathToDestination = new List<IMapPosition>();
			IMapPosition nextItem = cameFrom[current];
			reversePathToDestination.Add(current);
			while (nextItem != null && nextItem != start){
				reversePathToDestination.Add(nextItem);
				nextItem = cameFrom[nextItem];
			}
			IMapPosition[] toReturn = new IMapPosition[reversePathToDestination.Count];
			int position = toReturn.GetUpperBound(0);
			foreach (GridPosition currentPosition in reversePathToDestination){
				toReturn[position] = currentPosition;
				position--;
			}
			return toReturn;

		} else {
			return new GridPosition[0];
		}
	}
	
	//Note: This should be changed if the map moves away from a simple grid to more optimized hallways
	static public int Heuristic(IMapPosition a, IMapPosition b)
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

	//The following two methods are located in Map instead of pathfindingnodecollection because they need to instantiate path nodes which can only be done from classes that extend
	//MonoBehaviour
	public static PathfindingNodeCollection createNodeCollection (Transform gridTransform, GridPosition hallway, float pathBoxScale, Transform pathNode) {
		PathfindingNodeCollection ourNodes = new PathfindingNodeCollection ();
		if (hallway.gridType == HallwayType.hall) 
		{
			for (int nodeNum = -1; nodeNum < 2; nodeNum++) {
				ourNodes.addPathNode(createNewPathNode(0,0,(nodeNum / 2.0f) * gridTransform.localScale.z, pathBoxScale, pathNode, gridTransform));
			}
		} 
		else if (hallway.gridType == HallwayType.corner) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				ourNodes.addPathNode(createNewPathNode(0,0,(nodeNum / 2.0f) * gridTransform.localScale.z, pathBoxScale, pathNode, gridTransform));
			}
			ourNodes.addPathNode(createNewPathNode(-0.5f * gridTransform.localScale.x,0,0, pathBoxScale, pathNode, gridTransform));
		} 
		else if (hallway.gridType == HallwayType.T) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				ourNodes.addPathNode(createNewPathNode(0,0,(nodeNum / 2.0f) * gridTransform.localScale.z, pathBoxScale, pathNode, gridTransform));
			}
			ourNodes.addPathNode(createNewPathNode(0.5f * gridTransform.localScale.x,0,0, pathBoxScale, pathNode, gridTransform));
			
			ourNodes.addPathNode(createNewPathNode(-0.5f * gridTransform.localScale.x,0,0, pathBoxScale, pathNode, gridTransform));
		} 
		else if (hallway.gridType == HallwayType.plus) 
		{
			for (int nodeNum = -1; nodeNum < 2; nodeNum++) {
				ourNodes.addPathNode(createNewPathNode(0,0,(nodeNum / 2.0f) * gridTransform.localScale.z, pathBoxScale, pathNode, gridTransform));
			}
			ourNodes.addPathNode(createNewPathNode(0.5f * gridTransform.localScale.x,0,0, pathBoxScale, pathNode, gridTransform));
			
			ourNodes.addPathNode(createNewPathNode(-0.5f * gridTransform.localScale.x,0,0, pathBoxScale, pathNode, gridTransform));
		}
		else if (hallway.gridType == HallwayType.end) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				ourNodes.addPathNode(createNewPathNode(0,0,(nodeNum / 2.0f) * gridTransform.localScale.z, pathBoxScale, pathNode, gridTransform));
			}
		}
		return ourNodes;
	}
	
	private static Transform createNewPathNode(float x, float y, float z, float boxScale, Transform pathNode, Transform gridTransform){
		Transform newPathNode = Instantiate (pathNode, gridTransform.position, Quaternion.identity) as Transform;
		newPathNode.parent = gridTransform;
		newPathNode.localScale = new Vector3(boxScale,boxScale,boxScale);
		newPathNode.localRotation = Quaternion.identity;
		newPathNode.Translate(new Vector3(x,y,z));
		return newPathNode;
	}
	
	public void toggleEditMode(){
		if (!editMode) {
			//pause the agents
			foreach (PathfindingAgent currentAgent in pathAgents){
				currentAgent.pauseAgent();
			}
			//We need the user to just be able to drag out their mouse to create a path, or be able to make a selection box over an area and turn that whole area into open space.
			//Hallway mode and room mode, then.
			//Hallway mode: left click deletes walls that the mouse cursor encounters. A per-frame check of the mouse cursor's position could easily skip over walls
			//so we need to translate the mouse cursor's position into a grid position and adapt the step-based map generator to follow the direct path between the mouse's position each frame
			//Right click deletes squares that the mouse encounters. Use the same method of getting a list of grid squares encountered as in the left click case. Call a method on each grid position
			//that, for each position in the adjacency list, deletes the connection between it and the tile to be deleted. Then wipes out the deleted tile's adjacency list and sets its hallway type to none
			//
			//We also need a method that regenerates the visual representation of the map in a rectangle between the two coordinates provided.
			//
			//Room mode: Get grid position at mouse down, grid position at mouse up. Highlight all the affected tiles as the cursor is moved. On mouse up change the map in the affected coords
			//into an open room in three passes (center tiles open, north/east/south/west rows/columns, corner bits), then do another pass along the edges where if there is a hall tile 
			//opening into the edge we do an addExitToGridSquare from the inside of the edge towards the outside.

			//TODO: Display markers for each square that is available to start building hallway on.
			editMode = true;
		} else {
			foreach (PathfindingAgent currentAgent in pathAgents){
				currentAgent.unpauseAgent();
			}

			editMode = false;
		}
	}

	void buildHallwayAtClickedCoordinates ()
	{
		Vector3 mouseCoordsOnMap = new Vector3 ();
		if (getMouseCoordsOnMap (ref mouseCoordsOnMap)) {
			GridPosition clickedPosition = getMapPositionAtCoords (mouseCoordsOnMap);
			if (clickedPosition != null) {
				if (lastClickedValid && !lastClickedPosition.isEmpty () ) {
					if (lastClickedPosition.xPosition != clickedPosition.xPosition || lastClickedPosition.zPosition != clickedPosition.zPosition) {
						//TODO: Handle the situation where the user moves the mouse fast enough to skip over a grid position. Best done by making a line between 
						//lastMouseCoordsOnMap and mouseCoordsOnMap and looping through the following code for each grid position that the line intersects.
						Direction fromLastToCurrent = GridPosition.getDirectionBetweenAdjacentGridPositions (lastClickedPosition, clickedPosition);
						lastClickedPosition.addExitToGridSquare (fromLastToCurrent);
						clickedPosition.addExitToGridSquare (GridPosition.getOppositeDirection (fromLastToCurrent));
						lastClickedPosition.setAdjacents (mapGrid);
						clickedPosition.setAdjacents (mapGrid);
						generateGridPosition(lastClickedPosition);
						generateGridPosition(clickedPosition);
					}
				}
				coordsClickedOn.text = "X: " + clickedPosition.xPosition + ", Z: " + clickedPosition.zPosition;
				lastMouseCoordsOnMap = mouseCoordsOnMap;
				lastClickedPosition = clickedPosition;
				lastClickedValid = true;
			} else {
				lastClickedValid = false;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (editMode) {
			if (Input.GetMouseButtonDown(0)){
				buildHallwayAtClickedCoordinates ();
			}else if (Input.GetMouseButton(0)){
				buildHallwayAtClickedCoordinates ();
			}else if (Input.GetMouseButtonUp(0)){
				buildHallwayAtClickedCoordinates ();
				lastClickedValid = false;
			}
		}
	}

	GridPosition getMapPositionAtCoords(Vector3 mapCoords){
		int xCoord = (int)(mapCoords.x / hallwayGridUnitSize);
		int zCoord = (int)(mapCoords.z / hallwayGridUnitSize * -1);
		if ((xCoord >= 0 && xCoord < xDimension) && (zCoord >= 0 && zCoord < zDimension)) {
			return mapGrid [xCoord, zCoord];
		} else {
			return null;
		}
	}
	
	bool getMouseCoordsOnMap(ref Vector3 mouseCoords){
		Ray throughCamera = playerCamera.ScreenPointToRay (Input.mousePosition);
		float distanceAlongRay;
		bool rayIntersectsMap = mapBasePlane.Raycast(throughCamera, out distanceAlongRay);
		if (rayIntersectsMap) {
			mouseCoords = throughCamera.GetPoint (distanceAlongRay);
			mouseCoords-= transform.position;
			//TODO: Undo the rotation of the mapPlane on mouseCoords and then do math to figure out the actual grid coordinates in the event that the grid is rotated. For now assume it is not
			//Vector3 mouseCoordsInXZPlane = new Vector3(mouseCoords.x, 0, mouseCoords.z);
			return true;
		} else {
			return false;
		}
	}

}
