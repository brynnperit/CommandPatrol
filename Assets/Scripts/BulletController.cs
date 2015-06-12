using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public float bulletLifetime;
	float timeLeft;
	public string enemyTag;

	// Use this for initialization
	void Start () {
		timeLeft = bulletLifetime;
	}
	
	// Update is called once per frame
	void Update () {
		timeLeft -= Time.deltaTime;
		if (timeLeft < 0) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag (enemyTag)) {
			Destroy (other.gameObject);
		}
	}
}
