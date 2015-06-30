using UnityEngine;
using System.Collections;

public class GuardController : PathfindingAgent {

	public GameObject enemyCollection;
	public float guardSpeed;
	int transformArrayOffset = 1;
	public float fireRate;
	float fireTimer = 0;
	public Rigidbody bullet;
	public float bulletSpeed;
	Transform closestEnemy;
	public Transform pathfindingNodeCollection;
	int currentPathNode;
	GridPosition ourPosition;

	// Use this for initialization
	void Start () {

		SetClosestEnemy ();
	}
	
	// Update is called once per frame
	new void Update () {
		if (!paused) {
			base.Update ();

			SetClosestEnemy ();
		}

	}

	void SetClosestEnemy(){
		Transform[] enemies = enemyCollection.GetComponentsInChildren<Transform> ();
		if (enemies.Length > transformArrayOffset) {
			closestEnemy = null;
			float closestEnemyDistance = float.MaxValue;
		
			for (int currentEnemy = transformArrayOffset; currentEnemy < enemies.Length; currentEnemy++) {
				if (hasLineOfSight(enemies[currentEnemy])){
					float currentEnemyDistance = Vector3.Distance (transform.position, enemies [currentEnemy].position);
					if (currentEnemyDistance < closestEnemyDistance) {
						closestEnemyDistance = currentEnemyDistance;
						closestEnemy = enemies [currentEnemy];
					}
				}
			}
		}
	}

	bool hasLineOfSight(Transform toCheck){
		Vector3 rayDirection = toCheck.position - transform.position;
		RaycastHit hitResult = new RaycastHit();
		if (Physics.Raycast (transform.position, rayDirection, out hitResult)) {
			
			if (hitResult.transform == toCheck) {
				return true;
			} else {
				return false;
			}
			
		} else {
			return false;
		}
	}

	void FixedUpdate () {
		if (!paused) {
			fireTimer -= Time.deltaTime;
			while (fireTimer <= 0) {
				if (closestEnemy != null) {
					Rigidbody newBullet = Instantiate (bullet, transform.position, Quaternion.identity) as Rigidbody;
					Transform newBulletTransform = newBullet.GetComponent<Transform> ();
					newBulletTransform.LookAt (closestEnemy.position);
					newBulletTransform.Rotate (new Vector3 (90, 0, 0));
					newBulletTransform.position += newBulletTransform.up * transform.localScale.x;

					newBullet.AddForce (newBulletTransform.up * bulletSpeed, ForceMode.Impulse);
				}
				fireTimer += 1.0f / fireRate;
			}
		}
	}

	void OnTriggerEnter(Collider other){

	}
	
}
