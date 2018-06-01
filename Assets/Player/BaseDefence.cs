using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseDefence : NetworkBehaviour {

	[SyncVar]
	public GameObject playerBase, player;

	public GameObject currentTarget, turret, BaseCannonPrefab;
	public Transform spawnPos;

	private Transform playerProjectiles;
	private float  fireRate = 2f,fireCount = 0;
	private Player playerEnemy;

	// Use this for initialization
	void Start () {
		if (GameObject.Find ("PlayerProjectiles")) {
			playerProjectiles = GameObject.Find ("PlayerProjectiles").transform;
		}else {
			Debug.LogWarning (name + ", missing PlayerProjectiles. Auto generate.");
			playerProjectiles = new GameObject ("playerProjectiles").transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (fireCount < fireRate) {
			fireCount += Time.deltaTime;
		}
	}

	void OnTriggerStay (Collider obj){
		GameObject target = obj.gameObject;
		if (obj.GetComponentInParent<Player> ()) {
			playerEnemy = obj.GetComponentInParent<Player> ();
		} else {return;}
		if (target.tag == "Player" && !playerEnemy.isDead) {
			// Clear current target when target dead. 
			if (playerEnemy.isDead) {
				currentTarget = null;
				return;
			}
			// set current target if no target.
			if (currentTarget == null) {
				currentTarget = target;
			}
			Vector3 fireVector = (currentTarget.transform.position - spawnPos.transform.position).normalized;
			Vector3 turrentVector = Vector3.Scale (fireVector, turret.transform.forward);
			BroadcastMessage ("RotateToForward", new PlayerRotator (fireVector, 0.5f));
		
			if (fireCount >= fireRate) {
				fireCount = 0;
				if (player) {
					GameObject FiredCannon = Instantiate (BaseCannonPrefab, spawnPos.position, Quaternion.identity);
					NetworkServer.Spawn (FiredCannon);
					//FiredCannon.transform.SetParent (playerProjectiles.transform);
					FiredCannon.GetComponent<CannonBall> ().CannonFire (player, playerProjectiles, fireVector);
				}
			}
		}
	}

	void OnTriggerExit (Collider obj){
		GameObject target = obj.gameObject;
		if (currentTarget == target) {
			currentTarget = null;
		}
	}

}
