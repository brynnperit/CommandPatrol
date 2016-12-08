using UnityEngine;
using System.Collections;
using System;

public class RestFurnitureController : FurnitureController {

    protected float restfulnessValue;
    protected RestType restType;

    public void initialize(RestType type, float restfulnessValue, Map parentMap, GridPosition initialGridPosition, float agentScale, FurnitureCollection enclosingCollection, EnemyCollection enemyCollection, GuardCollection guardCollection)
    {
        base.initialize(FurnitureType.rest, parentMap, initialGridPosition, agentScale, enclosingCollection, enemyCollection, guardCollection);
        this.restfulnessValue = restfulnessValue;
    }

    public void initialize(RestType type, float restfulnessValue, Map parentMap, GridPosition initialGridPosition, float agentScale, EnemyCollection enemyCollection, GuardCollection guardCollection)
    {
        initialize(type, restfulnessValue, parentMap, initialGridPosition, agentScale, null, enemyCollection, guardCollection);
    }

    public float getRestfulnessValue()
    {
        return restfulnessValue;
    }

    override public float getAddedVisibilityValue(Agent viewingAgent)
    {
        return baseVisibilityPerSecond * 5;
    }

    //It's a couch or a mattress or something. Given that it's not from Sqornshellous Zeta it's reasonable to say it can't see anything.
    override public float getPerception()
    {
        return 0;
    }

    public override FurnitureType getFurnitureType()
    {
        return FurnitureType.rest;
    }
}

public enum RestType
{
    nap = 0
}
