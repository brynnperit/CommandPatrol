using UnityEngine;
using System.Collections;

public class HallwayPathController : MonoBehaviour {

	public Transform pathNode;

	// Use this for initialization
	void Start () {
		if (gameObject.CompareTag("Hallway")) 
		{
			for (int nodeNum = -1; nodeNum < 2; nodeNum++) {
				Transform newPathNode = Instantiate (pathNode, transform.position + new Vector3 (0, 0, (nodeNum / 2.0f) * transform.localScale.z), Quaternion.identity) as Transform;
				newPathNode.parent = transform;
			}
		} 
		else if (gameObject.CompareTag("HallwayCorner")) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				Transform newPathNode = Instantiate (pathNode, transform.position + new Vector3 (0, 0, (nodeNum / 2.0f) * transform.localScale.z), Quaternion.identity) as Transform;
				newPathNode.parent = transform;
			}
			Transform newPathNodeMinusX = Instantiate (pathNode, transform.position + new Vector3 (-0.5f * transform.localScale.x, 0, 0), Quaternion.identity) as Transform;
			newPathNodeMinusX.parent = transform;
		} 
		else if (gameObject.CompareTag("HallwayT")) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				Transform newPathNode = Instantiate (pathNode, transform.position + new Vector3 (0, 0, (nodeNum / 2.0f) * transform.localScale.z), Quaternion.identity) as Transform;
				newPathNode.parent = transform;
			}
			Transform newPathNodePlusX = Instantiate (pathNode, transform.position + new Vector3 (0.5f * transform.localScale.x, 0, 0), Quaternion.identity) as Transform;
			newPathNodePlusX.parent = transform;
			Transform newPathNodeMinusX = Instantiate (pathNode, transform.position + new Vector3 (-0.5f * transform.localScale.x, 0, 0), Quaternion.identity) as Transform;
			newPathNodeMinusX.parent = transform;
		} 
		else if (gameObject.CompareTag("HallwayPlus")) 
		{
			for (int nodeNum = -1; nodeNum < 2; nodeNum++) {
				Transform newPathNode = Instantiate (pathNode, transform.position + new Vector3 (0, 0, (nodeNum / 2.0f) * transform.localScale.z), Quaternion.identity) as Transform;
				newPathNode.parent = transform;
			}
			Transform newPathNodePlusX = Instantiate (pathNode, transform.position + new Vector3 (0.5f * transform.localScale.x, 0, 0), Quaternion.identity) as Transform;
			newPathNodePlusX.parent = transform;
			Transform newPathNodeMinusX = Instantiate (pathNode, transform.position + new Vector3 (-0.5f * transform.localScale.x, 0, 0), Quaternion.identity) as Transform;
			newPathNodeMinusX.parent = transform;
		}
		else if (gameObject.CompareTag("HallwayEnd")) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				Transform newPathNode = Instantiate (pathNode, transform.position + new Vector3 (0, 0, (nodeNum / 2.0f) * transform.localScale.z), Quaternion.identity) as Transform;
				newPathNode.parent = transform;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
