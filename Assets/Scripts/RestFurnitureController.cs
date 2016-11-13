using UnityEngine;
using System.Collections;

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
}

public enum RestType
{
    nap = 0
}
