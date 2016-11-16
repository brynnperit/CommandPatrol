using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PathfindingAgent : Agent {

	IMapPosition destination;
	IMapPosition[] pathToDestination;
	int stepAlongPathToDestination;

	Transform[] subgridPath;
	int currentPathNode;

	public float agentSpeed;

	public Transform pathEndMarker;

	GameObject endMarker;

	// Don't use this for initialization, it doesn't happen at the correct time.
	void Start () {

	}

	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination, float agentScale){
		initialize (parentMap, initialGridPosition, initialDestination, agentScale, null);
	}

	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination, float agentScale, AgentCollection enclosingCollection){
        base.initialize(parentMap, initialGridPosition, agentScale, enclosingCollection);
        subgridPath = new Transform[3];
        //If the initial destination isn't valid just set a random one.
        if (!setDestination(initialDestination))
        {
            setRandomDestination();
        }

	}

    /// <summary>
    /// Sets the agent's destination to the provided gridPosition, returning false if that destination is the same as our current position
    /// and returning true otherwise
    /// </summary>
    /// <param name="destinationToSet">The agent's new destination</param>
    /// <returns>False if the agent is already at the provided position, true otherwise</returns>
	public bool setDestination(IMapPosition destinationToSet){
        //This cleans up the old path end marker if there is one
        removeEndMarker();
		if (destinationToSet.xPosition == ourPosition.xPosition && destinationToSet.zPosition == ourPosition.zPosition) {
            return false;
		} else {
			destination = destinationToSet;
			pathToDestination = Map.getPathToPosition (ourPosition, destination, ourMap.getMapSize ());
			
			Transform endMarkerTransform = Instantiate (pathEndMarker, destinationToSet.getCenterPathNode ().position, Quaternion.identity) as Transform;
			endMarkerTransform.localScale = new Vector3 (agentScale, agentScale, agentScale);
			endMarker = endMarkerTransform.gameObject;
			
			stepAlongPathToDestination = -1;
			MoveToNextStep();
            return true;
        }
	}

	bool MoveToNextStep ()
	{
        bool hasNextStep = true;
		//If the array size 12, length is 12, final position is 11 which equals length -1, < that is 10 and below. < -2 below that is 9 and below
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
            removeEndMarker();
            hasNextStep = false;
        }
        return hasNextStep;
	}

    protected void removeEndMarker()
    {
        if (endMarker != null) {
            GameObject oldEndMarker = endMarker;
            Destroy(oldEndMarker);
        }
    }

    // Update is called once per frame
    //TODO: Move this into fixedUpdate, either make motion entirely physics and acceleration based or for now just calculate the velocity in each frame and hand that to the physics
    protected new void Update()
    {
        base.Update();
        
    }

    protected float performMoveForFrame(float moveRemaining){
        //TODO: Find out what this line actually does/why it is here
		if (subgridPath != null && subgridPath [currentPathNode] != null) {
            //TODO: This 0.01 is the magic number for movement precision. Move it, along with all other occurrences of it, somewhere sensible
            bool hasNextStep = true;
            while (moveRemaining > 0.01 && hasNextStep) {
				moveRemaining = performMove(subgridPath [currentPathNode].position, moveRemaining);
				if (Vector3.Distance (transform.position, subgridPath [currentPathNode].position) < 0.01) {
					if (currentPathNode == subgridPath.Length - 1) {
						hasNextStep = MoveToNextStep ();
					} else {
						currentPathNode++;
					}
				}
			}
        }
        //TODO: Find out why uncommenting the following lines makes the agents stop at their destinations and not make new ones.
        //else
        //{
        //    moveRemaining = 0;
        //}
		return moveRemaining;
	}

	/// <summary>
	/// Moves this object towards the destination by moveToPerform units. If moveToPerform is greater than the distance to the destination then the difference will be returned
	/// </summary>
	/// <returns>Any leftover movement.</returns>
	/// <param name="destination">Destination.</param>
	/// <param name="moveToPerform">Move to perform.</param>
	protected float performMove(Vector3 destination, float moveToPerform){
		Vector3 newPosition = Vector3.MoveTowards (transform.position, destination, moveToPerform);
		Vector3 difference = newPosition - transform.position;
		moveToPerform -= difference.magnitude;
		transform.position = newPosition;
		return moveToPerform;
	}

	protected void setRandomDestination(){
        //If the random map position is the same as our current map
        if (ourMap.getMapSize() != 1)
        {
            bool destinationValid = false;
            while (!destinationValid)
            {
                destinationValid = setDestination(ourMap.reallyinefficientGetRandomMapPosition());
            }
        }
	}

    protected new void fixedUpdate()
    {
        base.fixedUpdate();
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
        if (endMarker != null)
        {
            Destroy(endMarker);
        }
    }
}
