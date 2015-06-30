using UnityEngine;
using System.Collections;

public class EnemySpawner: MonoBehaviour {

	public GameObject boundingPlane;
	public Map ourMap;
	public Transform enemy;
	public int enemyCount;
	public float enemySize;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		int currentEnemyCount = transform.childCount;
		if (currentEnemyCount < enemyCount) {
			int enemiesToSpawn = enemyCount - currentEnemyCount;
			for (int enemySpawnNumber = 0; enemySpawnNumber < enemiesToSpawn; enemySpawnNumber++){
				createEnemy();
			}
		}
	}

	void createEnemy(){
		GridPosition enemyPosition = ourMap.reallyinefficientGetRandomMapPosition ();
		float xPosition = enemyPosition.visualRepresentation.position.x;
		float yPosition = enemyPosition.visualRepresentation.position.y;
		float zPosition =  enemyPosition.visualRepresentation.position.z;
		Transform newEnemy = Instantiate(enemy, new Vector3 (xPosition, yPosition, zPosition), Quaternion.identity) as Transform;
		newEnemy.localScale = new Vector3(enemySize, enemySize, enemySize);
		newEnemy.parent = transform;
	}
}
