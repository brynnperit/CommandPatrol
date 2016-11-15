using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuardCollection : AgentCollection {

	public EnemyCollection enemyCollection;
    public FurnitureCollection furnitureCollection;
	public GuardController guard;
	public float guardScale;
	
	public Text GuardAlertness;
    public Text GuardRestfulness;

	override protected void createAgent(){
		GuardController newGuard = Instantiate (guard, Vector3.zero, Quaternion.identity) as GuardController;
		newGuard.initialize (ourMap, ourMap.reallyinefficientGetRandomMapPosition (), ourMap.reallyinefficientGetRandomMapPosition (), guardScale, enemyCollection, furnitureCollection, GuardAlertness, GuardRestfulness);
		newGuard.transform.parent = transform;
		agentList.Add (newGuard);
	}
}
