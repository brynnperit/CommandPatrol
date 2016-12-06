using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Agent : MonoBehaviour
{

    protected IMapPosition ourPosition;
    public Map ourMap { get; set; }

    protected AgentCollection enclosingCollection;

    public float visibilityFadeSpeed;
    public float maxVisibilityDistance;

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

    //This currently is used to define how well an agent can see. It should be changed, along with changes in Visibility.cs, to define how well an agent can be seen.
    abstract protected float getAddedVisibilityValue(float otherAgentDistance);

    // Update is called once per frame
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
