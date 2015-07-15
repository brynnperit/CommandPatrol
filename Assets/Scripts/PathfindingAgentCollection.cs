using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PathfindingAgentCollection : MonoBehaviour {

	public Map ourMap;
	public int agentCount;

	protected List<PathfindingAgent> agentList;
	
	// Use this for initialization
	void Start () {
		agentList = new List<PathfindingAgent> ();
	}
	
	public void pause(){
		foreach (PathfindingAgent toPause in agentList) {
			if (toPause != null){
				toPause.pauseAgent();
			}
		}
	}
	
	public void unpause(){
		foreach (PathfindingAgent toUnpause in agentList) {
			if (toUnpause != null){
				toUnpause.unpauseAgent();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		int currentAgentCount = transform.childCount;
		if (currentAgentCount < agentCount) {
			int agentsToSpawn = agentCount - currentAgentCount;
			for (int guardSpawnNumber = 0; guardSpawnNumber < agentsToSpawn; guardSpawnNumber++){
				createAgent();
			}
		}
	}

	public void removeAgent(PathfindingAgent toRemove){
		agentList.Remove (toRemove);
	}
	
	protected abstract void createAgent();
}
