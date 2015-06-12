using UnityEngine;
using System.Collections;

public class GuardController : MonoBehaviour {

	public GameObject enemyCollection;
	public float guardSpeed;
	int transformArrayOffset = 1;
	public float fireRate;
	float fireTimer = 0;
	public Rigidbody bullet;
	public float bulletSpeed;
	Transform closestEnemy;
	public Transform pathfindingNodeCollection;
	Transform[] pathfindingNodes;
	int currentPathNode;
	public string nodeTag;

	// Use this for initialization
	void Start () {
		SetClosestEnemy ();
		pathfindingNodes = pathfindingNodeCollection.GetComponentsInChildren<Transform> ();
		currentPathNode = transformArrayOffset;
	}
	
	// Update is called once per frame
	void Update () {
		SetClosestEnemy ();

		transform.position = Vector3.MoveTowards(transform.position, pathfindingNodes[currentPathNode].position, guardSpeed * Time.deltaTime);
		//TODO: Make the guard fire bullets towards the closest enemy and move along a path of waypoints.

		//Camera.current.

	}

	void SetClosestEnemy(){
		Transform[] enemies = enemyCollection.GetComponentsInChildren<Transform> ();
		closestEnemy = enemies[transformArrayOffset];
		float closestEnemyDistance = Vector3.Distance(transform.position, enemies[transformArrayOffset].position);
		
		for (int currentEnemy = transformArrayOffset + 1; currentEnemy < enemies.Length; currentEnemy++) {
			float currentEnemyDistance = Vector3.Distance(transform.position, enemies[currentEnemy].position);
			if (currentEnemyDistance < closestEnemyDistance){
				closestEnemyDistance = currentEnemyDistance;
				closestEnemy = enemies[currentEnemy];
			}
		}
	}

	void FixedUpdate () {
		fireTimer -= Time.deltaTime;
		while (fireTimer <= 0) {
			Rigidbody newBullet = Instantiate (bullet, transform.position, Quaternion.identity) as Rigidbody;
			Transform newBulletTransform = newBullet.GetComponent<Transform>();
			newBulletTransform.LookAt(closestEnemy.position);
			newBulletTransform.Rotate(new Vector3(90, 0, 0));
			newBulletTransform.position += newBulletTransform.up * 1;

			newBullet.AddForce (newBulletTransform.up * bulletSpeed, ForceMode.Impulse);
			fireTimer += 1.0f/fireRate;
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag (nodeTag)) {
			currentPathNode++;
			if (currentPathNode == pathfindingNodes.Length){
				currentPathNode = transformArrayOffset;
			}
		}
	}
	
}
