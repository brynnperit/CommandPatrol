using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuardController : PathfindingAgent {

	public GameObject enemyCollection;
	int transformArrayOffset = 1;
	public float fireRate;
	float fireTimer = 0;
	public Rigidbody bullet;
	public float bulletSpeed;
	Transform mostNoticedEnemy;
	Vector3 mostNoticedEnemyLastPosition;
	public Transform pathfindingNodeCollection;

	public string enemyTag;

	GridPosition ourPosition;
	public float visibilityFadeSpeed;
	GuardMovementMode moveMode;

	public Text guardAlertnessUIOutput;

	public float guardPerception;

	Dictionary<Transform,float> visibleEnemies;

	// Use this for initialization
	void Start () {
		visibleEnemies = new Dictionary<Transform,float> ();
		moveMode = GuardMovementMode.patrol;
		UpdateEnemyVisibility ();
	}
	
	// Update is called once per frame
	new void Update () {
		if (!paused) {
			UpdateEnemyVisibility ();
			float mostNoticedEnemyVisibility = 0; 
			if (mostNoticedEnemy != null) {
				mostNoticedEnemyVisibility = visibleEnemies [mostNoticedEnemy];
			}else{
				mostNoticedEnemyVisibility = 0;
			}
			guardAlertnessUIOutput.text = mostNoticedEnemyVisibility.ToString();
			if (mostNoticedEnemyVisibility >= (int)GuardVisibilityThresholds.moveTowards) {
				moveMode = GuardMovementMode.investigate;
			}else{
				//TODO: When transitioning from investigate mode back into patrol mode the guard should pathfind back to wherever it left off on its patrol. Right now it moves in a straight line that can clip through walls
				moveMode = GuardMovementMode.patrol;
			}
			if (moveMode == GuardMovementMode.patrol){
				base.Update ();
			}else if (moveMode == GuardMovementMode.investigate){
				float moveRemaining = agentSpeed * Time.deltaTime;
				moveRemaining = performMove(mostNoticedEnemyLastPosition, moveRemaining);
				if (hasLineOfSight(mostNoticedEnemy)){
					moveRemaining = performMove (mostNoticedEnemy.position, moveRemaining);

					if (moveRemaining > 0.001){
						visibleEnemies.Remove(mostNoticedEnemy);
						Destroy(mostNoticedEnemy.gameObject);
						mostNoticedEnemy = null;
					}
				}else{
					performMoveForFrame(moveRemaining);
				}
			}
		}

	}

	void UpdateEnemyVisibility(){
		Transform[] enemies = enemyCollection.GetComponentsInChildren<Transform> ();
		if (enemies.Length > transformArrayOffset) {
			float closestEnemyDistance = float.MaxValue;
			for (int currentEnemyNum = transformArrayOffset; currentEnemyNum < enemies.Length; currentEnemyNum++) {
				Transform currentEnemy = enemies [currentEnemyNum];
				if (hasLineOfSight (currentEnemy)) {
					float currentEnemyDistance = Vector3.Distance (transform.position, currentEnemy.position);
					if (!visibleEnemies.ContainsKey (currentEnemy)) {
						visibleEnemies.Add (currentEnemy, 0);
					}
					visibleEnemies [currentEnemy] += getAddedVisibilityValue (currentEnemyDistance) + (visibilityFadeSpeed * Time.deltaTime);
					if (currentEnemyDistance < closestEnemyDistance) {
						closestEnemyDistance = currentEnemyDistance;
					}
				}
			}
		}
		//Decrementing the visibility values of all enemies prevents memory leaks, since references to removed enemies will shortly disappear from here.
		List<Transform> visibleEnemiesList = new List<Transform>(visibleEnemies.Keys);
		mostNoticedEnemy = null;
		foreach (Transform visible in visibleEnemiesList){
			if (visible != null){
				visibleEnemies[visible] -= (visibilityFadeSpeed * Time.deltaTime);
				if (visibleEnemies[visible] < 0){
					visibleEnemies.Remove(visible);
				}else if (mostNoticedEnemy == null || visibleEnemies[visible] > visibleEnemies[mostNoticedEnemy]){
					mostNoticedEnemy = visible;
					mostNoticedEnemyLastPosition = visible.position;
				}

			}else{
				visibleEnemies.Remove(visible);
			}
		}
	}

	float getAddedVisibilityValue(float currentEnemyDistance){
		//TODO: Implement a formula for adding more visibility value to an enemy the closer they are to the guard.
		return guardPerception * Time.deltaTime;
	}

	bool hasLineOfSight(Transform toCheck){
		if (toCheck != null) {
			Vector3 rayDirection = toCheck.position - transform.position;
			RaycastHit hitResult = new RaycastHit ();
			//TODO: Limit this check to rays within a number of degrees in a cone around the forward axis of the guard, representing the guard's vision
			if (Physics.Raycast (transform.position, rayDirection, out hitResult)) {
			
				if (hitResult.transform == toCheck) {
					return true;
				} else {
					return false;
				}
			
			} else {
				return false;
			}
		} else {
			return false;
		}
	}
	
	//TODO: Make the enemies have their own alert meters for the guards, where upon seeing a guard they will move to break line of sight as quickly as possible.
	//TODO: Make the enemies move around the map randomly.
	void FixedUpdate () {
		base.fixedUpdate ();
		if (!paused) {
			fireTimer -= Time.deltaTime;
			while (fireTimer <= 0) {
				if (mostNoticedEnemy != null && visibleEnemies[mostNoticedEnemy] >= (int)GuardVisibilityThresholds.shoot) {
					Rigidbody newBullet = Instantiate (bullet, transform.position, Quaternion.identity) as Rigidbody;
					Transform newBulletTransform = newBullet.GetComponent<Transform> ();
					newBulletTransform.LookAt (mostNoticedEnemy.position);
					newBulletTransform.Rotate (new Vector3 (90, 0, 0));
					newBulletTransform.position += newBulletTransform.up * transform.localScale.x;

					newBullet.AddForce (newBulletTransform.up * bulletSpeed, ForceMode.Impulse);
				}
				fireTimer += 1.0f / fireRate;
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag (enemyTag)) {
			Destroy (other.gameObject);
		}
	}
	
}

public enum GuardVisibilityThresholds{ stare=33, moveTowards=66, shoot=100};
public enum GuardMovementMode{ patrol=0, investigate=1};
