using UnityEngine;
using System.Collections;

public class EnemySpawner: MonoBehaviour {

	public GameObject boundingPlane;
	public Transform enemy;
	public int enemyCount;
	public float yOffset;

	// Use this for initialization
	void Start () {
		for (int currentEnemy = 0; currentEnemy < enemyCount; currentEnemy++) {
			createEnemy();
		}
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
		float boundingPlaneXSize = boundingPlane.transform.localScale.x;
		float boundingPlaneZSize = boundingPlane.transform.localScale.z;
		float xPosition = Random.Range(-boundingPlaneXSize, boundingPlaneXSize) * 4.0f;
		float zPosition = Random.Range(-boundingPlaneZSize, boundingPlaneZSize) * 4.0f;
		Transform newEnemy = Instantiate(enemy, new Vector3 (xPosition, yOffset, zPosition), Quaternion.identity) as Transform;
		newEnemy.parent = transform;
	}
}
