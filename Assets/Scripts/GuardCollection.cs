using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuardCollection : PathfindingAgentCollection {

	public EnemyCollection enemyCollection;
	public GuardController guard;
	public float guardScale;
	
	public Text GuardAlertness;

	override protected void createAgent(){
		GuardController newGuard = Instantiate (guard, Vector3.zero, Quaternion.identity) as GuardController;
		newGuard.initialize (ourMap, ourMap.reallyinefficientGetRandomMapPosition (), ourMap.reallyinefficientGetRandomMapPosition (), guardScale, enemyCollection, GuardAlertness);
		newGuard.transform.parent = transform;
		agentList.Add (newGuard);
	}
}
