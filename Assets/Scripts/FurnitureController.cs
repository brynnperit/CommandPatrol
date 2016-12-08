using UnityEngine;
using System.Collections;
using System;

public abstract class FurnitureController : Agent {

    protected EnemyCollection enemyCollection;
    protected GuardCollection guardCollection;

    protected void initialize(FurnitureType ourFurnitureType, Map parentMap, GridPosition initialGridPosition, float agentScale, FurnitureCollection enclosingCollection, EnemyCollection enemyCollection, GuardCollection guardCollection)
    {
        base.initialize(parentMap, initialGridPosition, agentScale, enclosingCollection);
        this.enemyCollection = enemyCollection;
        this.guardCollection = guardCollection;
    }

    protected void initialize(FurnitureType ourFurnitureType, Map parentMap, GridPosition initialGridPosition, float agentScale, EnemyCollection enemyCollection, GuardCollection guardCollection)
    {
        initialize(ourFurnitureType, parentMap, initialGridPosition, agentScale, null, enemyCollection, guardCollection);
    }

    abstract public FurnitureType getFurnitureType();
}

public enum FurnitureType { rest=0 };