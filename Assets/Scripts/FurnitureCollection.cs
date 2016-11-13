using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FurnitureCollection : AgentCollection {

    public EnemyCollection enemyCollection;
    public GuardCollection guardCollection;
    public RestFurnitureController furniture;
    public float furnitureScale;
    public float defaultRestfulnessValue;

    override protected void createAgent()
    {
        RestFurnitureController newFurniture = Instantiate(furniture, Vector3.zero, Quaternion.identity) as RestFurnitureController;
        newFurniture.initialize(RestType.nap, defaultRestfulnessValue, ourMap, ourMap.reallyinefficientGetRandomMapPosition(), furnitureScale, enemyCollection, guardCollection);
        newFurniture.transform.parent = transform;
        agentList.Add(newFurniture);
    }

    //TODO: A common use case is to use this to pathfind to the closest furniture object. In the process we will be pathfinding, 
    //throwing away the resulting path, and then getting the pathfinding agent to find that path again. Instead we could just give
    //the path to the agent. So the TODO is to build a method that will do that.
    //This is O(n) where n is the number of furniture objects of the given type in the world
    public FurnitureController getClosestFurniture(Agent closestTo, FurnitureType typeToGet)
    {
        IMapPosition initialPosition = closestTo.getPosition();
        FurnitureController closestFurniture = null;
        
        int mapSize = closestTo.ourMap.getMapSize();
        int closestPathLength = mapSize;
        foreach (FurnitureController currentFurniture in agentList)
        {
            if (currentFurniture.getFurnitureType() == typeToGet)
            {
                if (closestFurniture == null)
                {
                    closestFurniture = currentFurniture;
                    if (agentList.Count > 1)
                    {
                        //Perform pathfinding, count the path length
                        IMapPosition[] path = Map.getPathToPosition(initialPosition, currentFurniture.getPosition(), mapSize);
                        closestPathLength = path.Length;
                    }
                }
                else
                {
                    //Perform pathfinding, count the path length
                    IMapPosition[] path = Map.getPathToPosition(initialPosition, currentFurniture.getPosition(), mapSize);
                    if (path.Length < closestPathLength)
                    {
                        closestPathLength = path.Length;
                        closestFurniture = currentFurniture;
                    }
                }
            }
        }
        return closestFurniture;
    }
}

