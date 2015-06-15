using UnityEngine;
using System.Collections;

public class HallwayPathController : MonoBehaviour {
	
	public Transform pathNode;
	public float pathBoxScale;

	//TODO: Restructure code so that these path nodes are actually given to the grid position in an organized collection so that agents can traverse them to pass through

	// Use this for initialization
	void Start () {
		if (gameObject.CompareTag("Hallway")) 
		{
			for (int nodeNum = -1; nodeNum < 2; nodeNum++) {
				createNewPathNode(0,0,(nodeNum / 2.0f) * transform.localScale.z, pathBoxScale);
			}
		} 
		else if (gameObject.CompareTag("HallwayCorner")) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				createNewPathNode(0,0,(nodeNum / 2.0f) * transform.localScale.z, pathBoxScale);
			}
			createNewPathNode(-0.5f * transform.localScale.x,0,0, pathBoxScale);
		} 
		else if (gameObject.CompareTag("HallwayT")) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				createNewPathNode(0,0,(nodeNum / 2.0f) * transform.localScale.z, pathBoxScale);
			}
			createNewPathNode(0.5f * transform.localScale.x,0,0, pathBoxScale);

			createNewPathNode(-0.5f * transform.localScale.x,0,0, pathBoxScale);
		} 
		else if (gameObject.CompareTag("HallwayPlus")) 
		{
			for (int nodeNum = -1; nodeNum < 2; nodeNum++) {
				createNewPathNode(0,0,(nodeNum / 2.0f) * transform.localScale.z, pathBoxScale);
			}
			createNewPathNode(0.5f * transform.localScale.x,0,0, pathBoxScale);
			
			createNewPathNode(-0.5f * transform.localScale.x,0,0, pathBoxScale);
		}
		else if (gameObject.CompareTag("HallwayEnd")) 
		{
			for (int nodeNum = -1; nodeNum < 1; nodeNum++) {
				createNewPathNode(0,0,(nodeNum / 2.0f) * transform.localScale.z, pathBoxScale);
			}
		}

	}

	void createNewPathNode(float x, float y, float z, float boxScale){
		Transform newPathNode = Instantiate (pathNode, transform.position, Quaternion.identity) as Transform;
		newPathNode.parent = transform;
		newPathNode.localScale = new Vector3(boxScale,boxScale,boxScale);
		newPathNode.localRotation = Quaternion.identity;
		newPathNode.Translate(new Vector3(x,y,z));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
