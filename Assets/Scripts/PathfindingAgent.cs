using UnityEngine;
using System.Collections;

public class PathfindingAgent : MonoBehaviour {

	IMapPosition ourPosition;
	IMapPosition destination;
	public Map ourMap{ get; set; }
	IMapPosition[] pathToDestination;
	int stepAlongPathToDestination;

	Transform[] subgridPath;
	int currentPathNode;

	public float agentSpeed;
	public string nodeTag;

	public Transform pathEndMarker;

	GameObject endMarker;

	public float agentScale;

	bool paused;

	// Don't use this for initialization, it doesn't happen at the correct time.
	void Start () {

	}

	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination){
		ourMap = parentMap;
		subgridPath = new Transform[3];
		paused = false;
		transform.localScale = new Vector3 (agentScale, agentScale, agentScale);
		resetPosition (initialGridPosition);
		setDestination (initialDestination);
	}

	public void resetPosition(IMapPosition toSet){
		resetPosition(toSet, toSet.getCenterPathNode());
	}

	public void resetPosition(IMapPosition toSet, Transform pathNode){
		ourPosition = toSet;
		transform.position = pathNode.position;
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

		}
	}

	public void pauseAgent(){
		paused = true;
	}

	public void unpauseAgent(){
		paused = false;
	}

	void MoveToNextStep ()
	{

		//array size 12, length is 12, final position is 11 which equals length -1, < that is 10 and below. < -2 below that is 9 and below
		if (stepAlongPathToDestination < pathToDestination.Length - 2) {
			IMapPosition precedingPosition = ourPosition;

			stepAlongPathToDestination++;
			ourPosition = pathToDestination [stepAlongPathToDestination];

			subgridPath = ourPosition.getPathThrough (precedingPosition, pathToDestination [stepAlongPathToDestination + 1], subgridPath);
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
	//TODO: Move this into fixedUpdate, either make motion entirely physics and acceleration based or for now just calculate the velocity in each frame and hand that to the physics
	void Update () {
		if (!paused) {
			if (subgridPath != null && subgridPath [currentPathNode] != null) {
				float moveRemaining = agentSpeed * Time.deltaTime;
				while (moveRemaining > 0.01) {
					Vector3 newPosition = Vector3.MoveTowards (transform.position, subgridPath [currentPathNode].position, moveRemaining);
					Vector3 difference = newPosition - transform.position;
					moveRemaining -= difference.magnitude;
					transform.position = newPosition;
					if (Vector3.Distance (transform.position, subgridPath [currentPathNode].position) < 0.01) {
						if (currentPathNode == subgridPath.Length - 1) {
							MoveToNextStep ();
						} else {
							currentPathNode++;
						}
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
