using UnityEngine;
using System.Collections;

public class EnemyCollection: PathfindingAgentCollection {
	
	public GuardCollection guardCollection;
	public EnemyController enemy;
	public float enemyScale;
	
	override protected void createAgent(){
		EnemyController newEnemy = Instantiate (enemy, Vector3.zero, Quaternion.identity) as EnemyController;
		newEnemy.initialize (ourMap, ourMap.reallyinefficientGetRandomMapPosition (), ourMap.reallyinefficientGetRandomMapPosition (), enemyScale, guardCollection.gameObject);
		newEnemy.transform.parent = transform;
	}
}
