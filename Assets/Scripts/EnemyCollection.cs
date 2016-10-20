using UnityEngine;
using System.Collections;

public class EnemyCollection: AgentCollection {
	
	public GuardCollection guardCollection;
    public FurnitureCollection furnitureCollection;
	public EnemyController enemy;
	public float enemyScale;
	
	override protected void createAgent(){
		EnemyController newEnemy = Instantiate (enemy, Vector3.zero, Quaternion.identity) as EnemyController;
		newEnemy.initialize (ourMap, ourMap.reallyinefficientGetRandomMapPosition (), ourMap.reallyinefficientGetRandomMapPosition (), enemyScale, guardCollection, furnitureCollection);
		newEnemy.transform.parent = transform;
		agentList.Add (newEnemy);
	}
}
