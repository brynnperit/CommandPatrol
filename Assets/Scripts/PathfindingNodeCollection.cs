using UnityEngine;
using System.Collections;

public class PathfindingNodeCollection {

	Transform[] borderNodes;
	public Transform centerNode{ get; set; }
	public GridPosition owner{ get; set; }

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
		Vector2 nodePosition = new Vector2 (xCoord, zCoord);
		Vector2 parentPosition = new Vector2 (parentXCoord, parentZCoord);

		float distanceBetween = Mathf.Abs (Vector2.Distance (parentPosition, nodePosition));
		float angleBetween = getAngleBetween (new Vector2 (1, 0), nodePosition - parentPosition);
//		float northeastAngle = getAngleBetween (new Vector2 (1, 0), new Vector2 (1, 1)); //315
//		float southeastAngle = getAngleBetween (new Vector2 (1, 0), new Vector2 (1, -1)); //45
//		float southwestAngle = getAngleBetween (new Vector2 (1, 0), new Vector2 (-1, -1)); //135
//		float northwestAngle = getAngleBetween (new Vector2 (1, 0), new Vector2 (-1, 1)); //225

		if (distanceBetween < 0.1f) {
			centerNode = toAdd;
		} else {
			if (angleBetween >= 225 && angleBetween < 315) {
				setBorderNode (Direction.north, toAdd);
			} else if (angleBetween >= 315 || angleBetween < 45) {
				setBorderNode (Direction.east, toAdd);
			} else if (angleBetween >=45 && angleBetween < 135) {
				setBorderNode (Direction.south, toAdd);
			} else if (angleBetween >= 135 && angleBetween < 225) {
				setBorderNode (Direction.west, toAdd);
			}
		}
	}

	//Grabbed from http://answers.unity3d.com/questions/162177/vector2angles-direction.html
	static public float getAngleBetween(Vector2 firstVector, Vector2 secondVector){
		float ang = Vector2.Angle(firstVector, secondVector);
		Vector3 cross = Vector3.Cross(firstVector, secondVector);
		
		if (cross.z > 0) {
			ang = 360 - ang;
		}
		return ang;
	}

	public Transform[] nodesToFollow(Direction startingDirection, Direction endingDirection, Transform[] toStoreNodesIn){
		if (toStoreNodesIn == null || toStoreNodesIn.Length != 3) {
			return nodesToFollow (startingDirection, endingDirection);
		} else {
			toStoreNodesIn[0] = getBorderNode (startingDirection);
			toStoreNodesIn[1] = centerNode;
			toStoreNodesIn[2] = getBorderNode (endingDirection);
			return toStoreNodesIn;
		}
	}

	public Transform[] nodesToFollow(Direction startingDirection, Direction endingDirection){
		return nodesToFollow (startingDirection, endingDirection, new Transform[3]);
	}
}
