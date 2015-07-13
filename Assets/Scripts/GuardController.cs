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
	GuardMovementMode moveMode;

	public Text guardAlertnessUIOutput;

	public float guardPerception;

	Dictionary<Transform,float> visibleEnemies;

	// Use this for initialization
	void Start () {

	}

	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination, float agentScale, GameObject enemyCollection, Text guardAlertnessUIOutput){
		base.initialize(parentMap, initialGridPosition, initialDestination, agentScale);
		this.enemyCollection = enemyCollection;
		this.guardAlertnessUIOutput = guardAlertnessUIOutput;
		visibleEnemies = new Dictionary<Transform,float> ();
		moveMode = GuardMovementMode.patrol;
		UpdateGroupVisibility ();
	}
	
	// Update is called once per frame
	new void Update () {
		if (!paused) {
			UpdateGroupVisibility ();
			float mostNoticedEnemyVisibility = 0; 
			if (mostNoticedEnemy != null) {
				mostNoticedEnemyVisibility = visibleEnemies [mostNoticedEnemy];
			}else{
				mostNoticedEnemyVisibility = 0;
			}
			if (guardAlertnessUIOutput != null){
				guardAlertnessUIOutput.text = mostNoticedEnemyVisibility.ToString();
			}
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

	void UpdateGroupVisibility(){
		if (enemyCollection != null) {
			mostNoticedEnemy = base.UpdateGroupVisibility (enemyCollection.GetComponentsInChildren<Transform> (), transformArrayOffset, visibleEnemies);
			if (mostNoticedEnemy != null) {
				mostNoticedEnemyLastPosition = mostNoticedEnemy.position;
			}
		}
	}

	override protected float getAddedVisibilityValue(float currentEnemyDistance){
		//TODO: Implement a formula for adding more visibility value to an enemy the closer they are to the guard.
		return guardPerception * Time.deltaTime;
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
