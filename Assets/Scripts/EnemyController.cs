using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

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
            base.Update();

			UpdateGroupVisibility ();
			float mostNoticedGuardVisibility = 0; 
			if (mostNoticedGuard != null) {
				mostNoticedGuardVisibility = visibleGuards [mostNoticedGuard];
			}else{
				mostNoticedGuardVisibility = 0;
			}
			if (enemyAlertnessUIOutput != null){
				enemyAlertnessUIOutput.text = mostNoticedGuardVisibility.ToString();
			}

            determineInitialMovementMode(mostNoticedGuardVisibility);
            

            float moveRemaining = agentSpeed * Time.deltaTime;
            while (moveRemaining > 0.01)
            {
                switch (moveMode)
                {
                    case EnemyMovementMode.patrol:
                        moveRemaining = patrol(moveRemaining);
                        break;
                    case EnemyMovementMode.evade:
                        moveRemaining = evade(moveRemaining);
                        break;
                }
            }
		}

	}

    private float patrol(float moveRemaining)
    {
        moveRemaining = performMoveForFrame(moveRemaining);
        if (moveRemaining > 0.01)
        {
            setRandomDestination();
        }

        return moveRemaining;
    }

    private float evade(float moveRemaining)
    {
        moveRemaining = 0;
        //TODO: Find out how to get the actual guard game object from the transform that we have.
        //GuardController toEvade = (GuardController)mostNoticedGuard.gameObject.;
        return moveRemaining;
    }

    private void determineInitialMovementMode(float mostNoticedGuardVisibility)
    {
        if (mostNoticedGuardVisibility > (int)EnemyVisibilityThresholds.evade)
        {
            moveMode = EnemyMovementMode.evade;
        } else {
            moveMode = EnemyMovementMode.patrol;
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
		}
	}

	void OnTriggerEnter(Collider other){
//		if (other.gameObject.CompareTag (guardTag)) {
//			Destroy (other.gameObject);
//		}
	}
	
}

public enum EnemyVisibilityThresholds { evade = 50 };
public enum EnemyMovementMode{ patrol=0, evade=1};
