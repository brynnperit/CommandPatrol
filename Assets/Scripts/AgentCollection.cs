using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AgentCollection : MonoBehaviour {

	public Map ourMap;
	public int agentCount;

    protected List<Agent> agentList;

    // Use this for initialization
    protected void Start()
    {
        agentList = new List<Agent>();
    }

    public void pause(){
		foreach (Agent toPause in agentList) {
			if (toPause != null){
				toPause.pauseAgent();
			}
		}
	}
	
	public void unpause(){
		foreach (Agent toUnpause in agentList) {
			if (toUnpause != null){
				toUnpause.unpauseAgent();
			}
		}
	}

    public List<Agent> getAgentList()
    {
        return agentList;
    }
	
	// Update is called once per frame
	void Update () {
		int currentAgentCount = transform.childCount;
		if (currentAgentCount < agentCount) {
			int agentsToSpawn = agentCount - currentAgentCount;
			for (int agentSpawnNumber = 0; agentSpawnNumber < agentsToSpawn; agentSpawnNumber++){
				createAgent();
			}
		}
	}

	public void removeAgent(Agent toRemove){
		agentList.Remove (toRemove);
	}
	
	protected abstract void createAgent();
}
