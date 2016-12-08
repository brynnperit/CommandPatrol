using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GuardController : PathfindingAgent {

	EnemyCollection enemyCollection;
    FurnitureCollection furnitureCollection;

	public float fireRate;
	float fireTimer = 0;
	public Rigidbody bullet;
	public float bulletSpeed;
	Agent mostNoticedEnemy;
	Vector3 mostNoticedEnemyLastPosition;
	public Transform pathfindingNodeCollection;
    protected RestFurnitureController targetRestFurniture;

    float needsTimer;
    public float maxNeedsTimer;
    public float needsThreshold;
    bool shouldTakeBreak = false;

	public string enemyTag;

    GuardMovementMode moveModeBeforeInvestigation;
	GuardMovementMode moveMode;

	Text guardAlertnessUIOutput;
    Text guardRestfulnessUIOutput;

	public float guardPerception;
    public float guardBreakPerceptionModifier;

	Dictionary<Agent,float> visibleEnemies;

	// Use this for initialization
	void Start () {

	}

    //TODO: Figure out if we should be using the new keyword here to hide the base initialize methods since they won't work for a guard and then do the same for enemies, etc.
	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination, float agentScale, GuardCollection enclosingCollection, EnemyCollection enemyCollection, FurnitureCollection furnitureCollection, Text guardAlertnessUIOutput, Text guardRestfulnessUIOutput){
		base.initialize(parentMap, initialGridPosition, initialDestination, agentScale, enclosingCollection);
		this.enemyCollection = enemyCollection;
        this.furnitureCollection = furnitureCollection;
		this.guardAlertnessUIOutput = guardAlertnessUIOutput;
        this.guardRestfulnessUIOutput = guardRestfulnessUIOutput;
		visibleEnemies = new Dictionary<Agent,float> ();
		moveMode = GuardMovementMode.patrol;
		UpdateGroupVisibility ();
        needsTimer = maxNeedsTimer;
	}

	public void initialize(Map parentMap, GridPosition initialGridPosition, GridPosition initialDestination, float agentScale, EnemyCollection enemyCollection, FurnitureCollection furnitureCollection, Text guardAlertnessUIOutput, Text guardRestfulnessUIOutput)
    {
		initialize(parentMap, initialGridPosition, initialDestination, agentScale, null, enemyCollection, furnitureCollection, guardAlertnessUIOutput, guardRestfulnessUIOutput);
	}
	
	// Update is called once per frame
	new void Update () {
		if (!paused)
        {
            base.Update();

            //Decrease the time until the guard needs to take a break unless they are already actually resting
            if (moveMode != GuardMovementMode.onBreak)
            {
                needsTimer -= Time.deltaTime;
            }
            if (needsTimer < needsThreshold)
            {
                shouldTakeBreak = true;
            }

            //I'm not liking how many method calls in here I'm making the are full of side effects. 
            //Update group visibility sets the most noticed enemy
            UpdateGroupVisibility();
            float mostNoticedEnemyVisibility;
            if (mostNoticedEnemy != null)
            {
                mostNoticedEnemyVisibility = visibleEnemies[mostNoticedEnemy];
            }
            else
            {
                mostNoticedEnemyVisibility = 0;
            }
            if (guardAlertnessUIOutput != null)
            {
                guardAlertnessUIOutput.text = mostNoticedEnemyVisibility.ToString();
            }
            if (guardRestfulnessUIOutput != null)
            {
                guardRestfulnessUIOutput.text = needsTimer.ToString();
            }
            determineInitialMovementMode(mostNoticedEnemyVisibility);

            float moveRemaining = agentSpeed * Time.deltaTime;
            while (moveRemaining > 0.01)
            {
                switch (moveMode)
                {
                    case GuardMovementMode.patrol:
                        moveRemaining = patrol(moveRemaining);
                        break;
                    case GuardMovementMode.investigate:
                        moveRemaining = investigate(mostNoticedEnemyVisibility, moveRemaining);
                        break;
                    case GuardMovementMode.takeBreak:
                        moveRemaining = takeBreak(moveRemaining);
                        break;
                    case GuardMovementMode.onBreak:
                        moveRemaining = onBreak(moveRemaining);
                        break;
                }
            }
        }

    }

    private float onBreak(float moveRemaining)
    {
        float timeLeft = moveRemaining / agentSpeed;
        float restSpeed = targetRestFurniture.getRestfulnessValue();
        //If we finished resting this frame
        if (((timeLeft * restSpeed) + needsTimer) > maxNeedsTimer)
        {
            timeLeft = timeLeft - ((maxNeedsTimer - needsTimer) / restSpeed);
            needsTimer = maxNeedsTimer;
            shouldTakeBreak = false;
            moveMode = GuardMovementMode.patrol;
            setRandomDestination();
        }
        else
        {
            //We didn't finish resting this frame, rest as hard as possible with the time left!
            needsTimer = needsTimer + (timeLeft * restSpeed);
            timeLeft = 0;
        }
        moveRemaining = timeLeft * agentSpeed;
        return moveRemaining;
    }

    private float takeBreak(float moveRemaining)
    {
        moveRemaining = performMoveForFrame(moveRemaining);
        if (moveRemaining > 0.01)
        {
            //We're at the destination! Time to go on break
            moveMode = GuardMovementMode.onBreak;
        }

        return moveRemaining;
    }

    private float patrol(float moveRemaining)
    {
        moveRemaining = performMoveForFrame(moveRemaining);
        //If the guard completes its patrol move and has movement left to do then it reached its destination
        //and needs a new one
        if (moveRemaining > 0.01)
        {
            setRandomDestination();
        }

        return moveRemaining;
    }

    private float investigate(float mostNoticedEnemyVisibility, float moveRemaining)
    {
        //If this is true then we no longer need to be in investigate mode
        if (mostNoticedEnemy == null || mostNoticedEnemyVisibility < (int)GuardVisibilityThresholds.moveTowards)
        {
            moveMode = moveModeBeforeInvestigation;
        }
        else
        {
            if (Visibility.HasLineOfSight(transform, mostNoticedEnemy.transform, maxVisibilityDistance))
            {
                moveRemaining = performMove(mostNoticedEnemy.transform.position, moveRemaining);

                if (moveRemaining > 0.001)
                {
                    visibleEnemies.Remove(mostNoticedEnemy);
                    Destroy(mostNoticedEnemy.gameObject);
                    mostNoticedEnemy = null;
                }
            }
            else
            {
                moveRemaining = performMove(mostNoticedEnemyLastPosition, moveRemaining);
            }
        }

        return moveRemaining;
    }

    private void determineInitialMovementMode(float mostNoticedEnemyVisibility)
    {
        if (moveMode != GuardMovementMode.onBreak)
        {

            if (moveMode != GuardMovementMode.investigate && mostNoticedEnemyVisibility >= (int)GuardVisibilityThresholds.moveTowards)
            {
                moveModeBeforeInvestigation = moveMode;
                moveMode = GuardMovementMode.investigate;
            }
            if (moveMode == GuardMovementMode.patrol && shouldTakeBreak == true)
            {
                moveMode = GuardMovementMode.takeBreak;
                //Path to all furniture objects, determine the path length of the closest one, and then path to it
                targetRestFurniture = (RestFurnitureController)furnitureCollection.getClosestFurniture(this, FurnitureType.rest);
                setDestination(targetRestFurniture.getPosition());
            }
        }
    }

    void UpdateGroupVisibility(){
		if (enemyCollection != null) {
            //TODO: Perform culling here of which enemies to send in to the visibility check.
			mostNoticedEnemy = Visibility.UpdateGroupVisibility (this, enemyCollection.getAgentList(), visibleEnemies, getAddedVisibilityValue, visibilityFadeSpeed, maxVisibilityDistance);
			if (mostNoticedEnemy != null) {
				mostNoticedEnemyLastPosition = mostNoticedEnemy.transform.position;
			}
		}
	}

	override public float getAddedVisibilityValue(Agent otherAgent){
        float visValue = Time.deltaTime * baseVisibilityPerSecond;
        return visValue;
	}

    public override float getPerception()
    {
        float perception = guardPerception;
        if (moveMode == GuardMovementMode.onBreak || moveMode == GuardMovementMode.takeBreak)
        {
            perception *= guardBreakPerceptionModifier;
        }
        return perception;
    }



    //TODO: Make the enemies have their own alert meters for the guards, where upon seeing a guard they will move to break line of sight as quickly as possible.
    void FixedUpdate () {
		base.fixedUpdate ();
		if (!paused) {
			fireTimer -= Time.deltaTime;
			while (fireTimer <= 0) {
				if (mostNoticedEnemy != null && visibleEnemies[mostNoticedEnemy] >= (int)GuardVisibilityThresholds.shoot) {
					Rigidbody newBullet = Instantiate (bullet, transform.position, Quaternion.identity) as Rigidbody;
					Transform newBulletTransform = newBullet.GetComponent<Transform> ();
					newBulletTransform.LookAt (mostNoticedEnemy.transform.position);
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
public enum GuardMovementMode{ patrol=0, investigate=1, takeBreak=2, onBreak=3};
