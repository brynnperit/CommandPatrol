using UnityEngine;
using System.Collections;

public class PathfindingAgent : MonoBehaviour {

	GridPosition ourPosition;
	GridPosition destination;
	public Map ourMap{ get; set; }
	GridPosition[] pathToDestination;
	int stepAlongPathToDestination;

	Transform[] subgridPath;
	int currentPathNode;

	public float agentSpeed;
	public string nodeTag;

	public Transform pathEndMarker;

	GameObject endMarker;

	public float agentScale;


	// Use this for initialization
	void Start () {

	}

	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination){
		ourMap = parentMap;
		subgridPath = new Transform[3];
		transform.localScale = new Vector3 (agentScale, agentScale, agentScale);
		resetPosition (initialGridPosition);
		setDestination (initialDestination);
	}

	public void resetPosition(GridPosition toSet){
		ourPosition = toSet;
		transform.position = ourPosition.getCenterPathNode().position;
	}

	public void setDestination(GridPosition destinationToSet){
		if (destinationToSet.xPosition == ourPosition.xPosition && destinationToSet.zPosition == ourPosition.zPosition) {
			setRandomDestination();
		} else {
			destination = destinationToSet;
			pathToDestination = Map.getPathToPosition (ourPosition, destination, ourMap.getMapSize ());
			
			Transform endMarkerTransform = Instantiate (pathEndMarker, destinationToSet.getCenterPathNode ().position, Quaternion.identity) as Transform;
			endMarkerTransform.localScale = new Vector3 (agentScale, agentScale, agentScale);
			endMarker = endMarkerTransform.gameObject;
			
			stepAlongPathToDestination = -1;
			MoveToNextStep();
			
			//			GridPosition nextPosition = pathToDestination [stepAlongPathToDestination + 1];
			//
			//			Direction startingDirection = GridPosition.getOppositeDirection(ourPosition.getDirectionOfAdjacentGridPosition (nextPosition));
			//			//TODO: Code the case that there's only one entry in the path to the destination, preferably by just combining with the code at the MARK below
			//			directionOfNextStep = nextPosition.getDirectionOfAdjacentGridPosition (pathToDestination [stepAlongPathToDestination + 1]);
			//
			//			stepAlongPathToDestination++;
			//			ourPosition = pathToDestination [stepAlongPathToDestination];
			//
			//			ourPosition.getPathThrough (startingDirection, directionOfNextStep, subgridPath);
			//			currentPathNode = 0;
		}
	}

	void MoveToNextStep ()
	{

		//array size 12, length is 12, final position is 11 which equals length -1, < that is 10 and below. < -2 below that is 9 and below
		if (stepAlongPathToDestination < pathToDestination.Length - 2) {
			GridPosition precedingPosition = ourPosition;

			stepAlongPathToDestination++;
			ourPosition = pathToDestination [stepAlongPathToDestination];

			Direction directionFromPreviousToCurrent = precedingPosition.getDirectionOfAdjacentGridPosition (ourPosition);
			Direction directionFromCurrentToPrevious = GridPosition.getOppositeDirection (directionFromPreviousToCurrent);

			Direction directionFromCurrentToNext = ourPosition.getDirectionOfAdjacentGridPosition (pathToDestination [stepAlongPathToDestination + 1]);

			ourPosition.getPathThrough (directionFromCurrentToPrevious, directionFromCurrentToNext, subgridPath);
			currentPathNode = 0;

		} else if (stepAlongPathToDestination == pathToDestination.Length - 2) {
			//We've reached the final square, set the path agent to go straight to the end marker.

			stepAlongPathToDestination++;
			ourPosition = pathToDestination [stepAlongPathToDestination];

			subgridPath [subgridPath.Length - 1] = endMarker.transform;
			currentPathNode = subgridPath.Length - 1;

		} else {
			//If we hit this point then we're in the final grid square and have hit subgridPath[subgridPath.Length - 1], which is the end marker as seen just above
			GameObject oldEndMarker = endMarker;
			setRandomDestination ();
			Destroy (oldEndMarker);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (subgridPath != null && subgridPath[currentPathNode] != null) {
			float moveRemaining = agentSpeed * Time.deltaTime;
			while (moveRemaining > 0.01){
				Vector3 newPosition = Vector3.MoveTowards (transform.position, subgridPath [currentPathNode].position, moveRemaining);
				Vector3 difference = newPosition - transform.position;
				moveRemaining -= difference.magnitude;
				transform.position = newPosition;
				if (Vector3.Distance(transform.position, subgridPath [currentPathNode].position) < 0.01){
					if (currentPathNode == subgridPath.Length - 1){
						MoveToNextStep ();
					}else{
						currentPathNode++;
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider other){

	}



	void setRandomDestination(){
		setDestination(ourMap.reallyinefficientGetRandomMapPosition());
	}
}
