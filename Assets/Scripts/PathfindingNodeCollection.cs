using UnityEngine;
using System.Collections;

public class PathfindingNodeCollection {

	Transform[] borderNodes;
	public Transform centerNode{ get; set; }

	public PathfindingNodeCollection(){
		borderNodes = new Transform[4];
	}

	public Transform getBorderNode(Direction toGetFrom){
		return borderNodes[(int)toGetFrom];
	}

	public void setBorderNode(Direction toSetTo, Transform toSet){
		borderNodes [(int)toSetTo] = toSet;
	}

	public void addPathNode(Transform toAdd){
		//South is -z, east is +x
		float xCoord = toAdd.position.x;
		float zCoord = toAdd.position.z;
		float parentXCoord = toAdd.parent.position.x;
		float parentZCoord = toAdd.parent.position.z;
		if (xCoord > parentXCoord) {
			setBorderNode (Direction.east, toAdd);
		} else if (xCoord < parentXCoord) {
			setBorderNode (Direction.west, toAdd);
		} else if (zCoord > parentZCoord) {
			setBorderNode (Direction.north, toAdd);
		} else if (zCoord < parentZCoord) {
			setBorderNode (Direction.south, toAdd);
		} else {
			centerNode = toAdd;
		}
	}

	public Transform[] nodesToFollow(Direction startingDirection, Direction endingDirection){
		//This is going to get called a lot by the user agents as they progress. Optimization idea: Get them to send in their own transform array for us to fill.
		return new Transform[3]{getBorderNode (startingDirection), centerNode, getBorderNode (endingDirection)};
	}
}
