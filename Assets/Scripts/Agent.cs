using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Agent : MonoBehaviour
{

    protected IMapPosition ourPosition;
    public Map ourMap { get; set; }

    protected AgentCollection enclosingCollection;

    public float visibilityFadeSpeed;

    protected float agentScale;

    protected bool paused;

    // Don't use this for initialization, it doesn't happen at the correct time.
    void Start()
    {

    }

    public void initialize(Map parentMap, GridPosition initialGridPosition, float agentScale)
    {
        initialize(parentMap, initialGridPosition, agentScale, null);
    }

    public void initialize(Map parentMap, GridPosition initialGridPosition, float agentScale, AgentCollection enclosingCollection)
    {
        ourMap = parentMap;
        paused = false;
        this.agentScale = agentScale;
        this.enclosingCollection = enclosingCollection;
        transform.localScale = new Vector3(agentScale, agentScale, agentScale);
        resetPosition(initialGridPosition);
    }

    public IMapPosition getPosition()
    {
        return ourPosition;
    }

    public void resetPosition(IMapPosition toSet)
    {
        resetPosition(toSet, toSet.getCenterPathNode());
    }

    public void resetPosition(IMapPosition toSet, Transform pathNode)
    {
        ourPosition = toSet;
        transform.position = pathNode.position;
    }


    public void pauseAgent()
    {
        paused = true;
    }

    public void unpauseAgent()
    {
        paused = false;
    }

    abstract protected float getAddedVisibilityValue(float otherAgentDistance);

    protected bool hasLineOfSight(Transform toCheck)
    {
        if (toCheck != null)
        {
            Vector3 rayDirection = toCheck.position - transform.position;
            RaycastHit hitResult = new RaycastHit();
            //TODO: Limit this check to rays within a number of degrees in a cone around the forward axis of the guard, representing the guard's vision
            if (Physics.Raycast(transform.position, rayDirection, out hitResult))
            {

                if (hitResult.transform == toCheck)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected Transform UpdateGroupVisibility(Transform[] toCheck, int checkOffset, Dictionary<Transform, float> visibilityList)
    {
        if (toCheck.Length > checkOffset)
        {
            for (int currentTransformNum = checkOffset; currentTransformNum < toCheck.Length; currentTransformNum++)
            {
                Transform currentTransform = toCheck[currentTransformNum];
                if (hasLineOfSight(currentTransform))
                {
                    float currentTransformDistance = Vector3.Distance(transform.position, currentTransform.position);
                    if (!visibilityList.ContainsKey(currentTransform))
                    {
                        visibilityList.Add(currentTransform, 0);
                    }
                    visibilityList[currentTransform] += getAddedVisibilityValue(currentTransformDistance) + (visibilityFadeSpeed * Time.deltaTime);
                }
            }
        }
        //Decrementing the visibility values of all enemies prevents memory leaks, since references to removed enemies will shortly disappear from here.
        List<Transform> visibleTransformList = new List<Transform>(visibilityList.Keys);
        Transform mostNoticedTransform = null;
        foreach (Transform visible in visibleTransformList)
        {
            if (visible != null)
            {
                visibilityList[visible] -= (visibilityFadeSpeed * Time.deltaTime);
                if (visibilityList[visible] < 0)
                {
                    visibilityList.Remove(visible);
                }
                else if (mostNoticedTransform == null || visibilityList[visible] > visibilityList[mostNoticedTransform])
                {
                    mostNoticedTransform = visible;
                }

            }
            else
            {
                visibilityList.Remove(visible);
            }
        }

        return mostNoticedTransform;
    }

    // Update is called once per frame
    //TODO: Move this into fixedUpdate, either make motion entirely physics and acceleration based or for now just calculate the velocity in each frame and hand that to the physics
    protected void Update()
    {
    }

    protected void fixedUpdate()
    {

    }

    protected void OnDestroy()
    {
        if (enclosingCollection != null)
        {
            enclosingCollection.removeAgent(this);
        }
    }
}
