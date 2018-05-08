using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttack : MonoBehaviour {
	
	public GameObject cannonBall;
	public float fireRate = 2f;


	private SpawnPointIndicator spawnPos;
	private GameObject playerProjectiles;
	private float fireCount = 0;

	// Use this for initialization
	void Start () {
		if (GetComponentInChildren<SpawnPointIndicator> ()) {
			spawnPos = GetComponentInChildren<SpawnPointIndicator> ();
		} else {Debug.LogWarning (name + ", missing SpawnPointIndicator");}

		if (GameObject.Find ("PlayerProjectiles")) {
			playerProjectiles = GameObject.Find ("PlayerProjectiles");
		}else {
			Debug.LogWarning (name + ", missing PlayerProjectiles. Auto generate.");
			playerProjectiles = new GameObject ("playerProjectiles");
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (fireCount < fireRate) {
			fireCount += Time.deltaTime;
		}
	}

	void OnTriggerStay(Collider obj){
			//Debug.Log ("something trigger, " + obj.name);
			GameObject target = obj.gameObject;
			if (target.tag == "Player") {
				Vector3 fireVector;
				GameObject FiredCannon;
				Debug.Log (name + ", Target in sight, fire !!");
				if (fireCount>= fireRate) {
					fireCount = 0;
					fireVector = (target.transform.position - spawnPos.transform.position).normalized;
					FiredCannon = Instantiate (cannonBall, spawnPos.transform.position, Quaternion.identity);
					FiredCannon.transform.SetParent (playerProjectiles.transform);
					FiredCannon.GetComponent<CannonBall> ().CannonFire (fireVector);
				}
			}
	}
}
