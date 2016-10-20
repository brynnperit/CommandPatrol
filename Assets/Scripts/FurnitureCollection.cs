using UnityEngine;
using System.Collections;
using System;

public class FurnitureCollection : AgentCollection {

    public EnemyCollection enemyCollection;
    public GuardCollection guardCollection;
    public FurnitureController furniture;
    public float furnitureScale;

    override protected void createAgent()
    {
        FurnitureController newFurniture = Instantiate(furniture, Vector3.zero, Quaternion.identity) as FurnitureController;
        newFurniture.initialize(FurnitureType.couch, ourMap, ourMap.reallyinefficientGetRandomMapPosition(), furnitureScale, enemyCollection, guardCollection);
        newFurniture.transform.parent = transform;
        agentList.Add(newFurniture);
    }
}
