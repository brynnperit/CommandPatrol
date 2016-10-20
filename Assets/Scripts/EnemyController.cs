using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyController : PathfindingAgent {

	GuardCollection guardCollection;
    FurnitureCollection furnitureCollection;
	int transformArrayOffset = 1;
	public float fireRate;
	float fireTimer = 0;
	public Rigidbody bullet;
	public float bulletSpeed;
	Transform mostNoticedGuard;
	Vector3 mostNoticedGuardLastPosition;
	public Transform pathfindingNodeCollection;

	public string guardTag;
	EnemyMovementMode moveMode;

	public Text enemyAlertnessUIOutput;

	public float enemyPerception;

	Dictionary<Transform,float> visibleGuards;

	// Use this for initialization
	void Start () {

	}

	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination, float agentScale, EnemyCollection enclosingCollection, GuardCollection guardCollection, FurnitureCollection furnitureCollection){
		base.initialize (parentMap, initialGridPosition, initialDestination, agentScale, enclosingCollection);
		this.guardCollection = guardCollection;
        this.furnitureCollection = furnitureCollection;
		visibleGuards = new Dictionary<Transform,float> ();
		moveMode = EnemyMovementMode.patrol;
		UpdateGroupVisibility ();
	}

	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination, float agentScale, GuardCollection guardCollection, FurnitureCollection furnitureCollection){
		initialize (parentMap, initialGridPosition, initialDestination, agentScale, null, guardCollection, furnitureCollection);
	}
	
	// Update is called once per frame
	new void Update () {
		if (!paused) {
			UpdateGroupVisibility ();
			float mostNoticedEnemyVisibility = 0; 
			if (mostNoticedGuard != null) {
				mostNoticedEnemyVisibility = visibleGuards [mostNoticedGuard];
			}else{
				mostNoticedEnemyVisibility = 0;
			}
			if (enemyAlertnessUIOutput != null){
				enemyAlertnessUIOutput.text = mostNoticedEnemyVisibility.ToString();
			}

			moveMode = EnemyMovementMode.patrol;

			if (moveMode == EnemyMovementMode.patrol){
				base.Update ();
			}
		}

	}

	void UpdateGroupVisibility(){
		if (guardCollection != null) {
			mostNoticedGuard = base.UpdateGroupVisibility (guardCollection.GetComponentsInChildren<Transform> (), transformArrayOffset, visibleGuards);
			if (mostNoticedGuard != null){
				mostNoticedGuardLastPosition = mostNoticedGuard.position;
			}
		}
	}

	override protected float getAddedVisibilityValue(float currentEnemyDistance){
		//TODO: Implement a formula for adding more visibility value to an enemy the closer they are to the guard.
		return enemyPerception * Time.deltaTime;
	}


	
	//TODO: Make the enemies have their own alert meters for the guards, where upon seeing a guard they will move to break line of sight as quickly as possible.
	void FixedUpdate () {
		base.fixedUpdate ();
		if (!paused) {
			fireTimer -= Time.deltaTime;
			while (fireTimer <= 0) {
				if (mostNoticedGuard != null && visibleGuards[mostNoticedGuard] >= (int)EnemyVisibilityThresholds.shoot) {
//					Rigidbody newBullet = Instantiate (bullet, transform.position, Quaternion.identity) as Rigidbody;
//					Transform newBulletTransform = newBullet.GetComponent<Transform> ();
//					newBulletTransform.LookAt (mostNoticedGuard.position);
//					newBulletTransform.Rotate (new Vector3 (90, 0, 0));
//					newBulletTransform.position += newBulletTransform.up * transform.localScale.x;
//
//					newBullet.AddForce (newBulletTransform.up * bulletSpeed, ForceMode.Impulse);
				}
				fireTimer += 1.0f / fireRate;
			}
		}
	}

	void OnTriggerEnter(Collider other){
//		if (other.gameObject.CompareTag (guardTag)) {
//			Destroy (other.gameObject);
//		}
	}
	
}

public enum EnemyVisibilityThresholds{ stare=33, moveTowards=66, shoot=100};
public enum EnemyMovementMode{ patrol=0, investigate=1};
