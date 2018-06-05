using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseDefence : NetworkBehaviour {

	[SyncVar]
	public GameObject playerBase, player;

	[SyncVar]
	public float armor = 0;

	[SyncVar]
	public bool isDead = false;

	public GameObject currentTarget, turret, BaseCannonPrefab, FxPrefab;
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
		//Debug.Log ("trigger!!!");
		GameObject target = obj.gameObject;
		if (!isDead && obj.GetComponentInParent<Player> () && obj.GetComponentInParent<Player> ().gameObject != player) {
			playerEnemy = obj.GetComponentInParent<Player> ();
			//Debug.Log ("target lock, attack " + playerEnemy.name);
		}else {
			//Debug.Log ("player in range, return");
			return;
		} //else {return;}
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
				GameObject FiredCannon = Instantiate (BaseCannonPrefab, spawnPos.position, Quaternion.identity);
				//NetworkServer.Spawn (FiredCannon);
				FiredCannon.transform.SetParent (playerProjectiles.transform);
				FiredCannon.GetComponent<CannonBall> ().CannonFire (player, playerProjectiles, fireVector);
			}
		}
	}

	void OnTriggerExit (Collider obj){
		GameObject target = obj.gameObject;
		if (currentTarget == target) {
			currentTarget = null;
		}
	}

	void OnBaseDestroy(){
		isDead = true;
		GameObject Fx = Instantiate (FxPrefab, transform.position, Quaternion.identity);
		Fx.transform.SetParent (transform);
		Debug.Log (name + ", is destroyed, "+ player.name + " lose.");
	}

	void UpdateBaseDefenceHealth (float bDPercentage){
		if (player) {
			player.GetComponent<Player> ().UpdateBaseDefenceHealth (bDPercentage);
		}
	}
}
