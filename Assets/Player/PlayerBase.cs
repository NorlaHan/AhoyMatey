using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBase : NetworkBehaviour {

	public bool isDebugMode = false;
	public float baseRepair = 10f , lootCount = 0;

	[SyncVar(hook = "OnBaseTreasureStorageChange")]
	public float treasureStorage = 0;

	[SyncVar]
	public GameObject player;

	//public string playerName;


	public float winTreasureAmount = 2000;

	// Use this for initialization
	void Start () {
		OnBaseTreasureStorageChange (treasureStorage);
	}
	
	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			treasureStorage += 1;
		}
	}

	void OnTriggerEnter (Collider obj){
		//GameObject target = obj.gameObject;
		//Debug.Log (name + ", trigger.");
		if (obj.tag == "PlayerStash") {
			if (player && obj.GetComponentInParent<Player>().gameObject == player) {
				treasureStorage += obj.GetComponentInParent<PlayerTreasureStash> ().TreasureStoreToBase ();
				//Debug.Log (target.name + "Player back to base, storage is now : " + treasureStorage);
				if (treasureStorage >= player.GetComponent<Player>().treasureToWin) {
					Debug.Log (obj.GetComponentInParent<Player>().name + " Win!");
					//player.GetComponent<Player>().OnGameSettle ();
				}
			} 
//			else {
//				// TODO opponent will steal treasure while near (WIP)
//				//Debug.Log ("Base is under attack by " + target.transform.parent.transform.parent.name);
//
//
			}
	}

	void OnTriggerStay (Collider obj){
		if (player && obj.tag == "Player") {
			//Debug.Log (obj.name +", in hub");
			if (obj.GetComponentInParent<Player>().gameObject == player) {
				Health health = obj.GetComponentInParent<Health> ();
				if (health.currentHealth < health.fullHealth) {
					health.OnGetRepair (Time.deltaTime * baseRepair);
				}
			}else{
				if (treasureStorage > 0) {
					PlayerTreasureStash	playerStash = obj.GetComponentInParent<PlayerTreasureStash> ();
					lootCount += Time.deltaTime;
					if (lootCount > 0.5f) {
						lootCount = 0;
						if (treasureStorage > playerStash.pillage) {
							BaseTreasureBeenLooted (playerStash.pillage);
							playerStash.TreasureLoot (playerStash.pillage);
						} else {
							playerStash.TreasureLoot (treasureStorage);
							treasureStorage = 0;
						}
					}
				}
			}
		}
	}


	void BaseTreasureBeenLooted (float looting){
		if (treasureStorage > 0) {
			treasureStorage -= looting;
		} else {
			treasureStorage = 0;
		}
	}

	public void BaseLinkToPlayer (GameObject playerOnClient){
		player = playerOnClient;
	}


	void OnBaseTreasureStorageChange (float treasure){
		//if (!isServer) {return;}
//		if (!player) {
//			Debug.Log (name + ", no player assigned ,find player");
//			player = GameObject.Find ("playerName");
//		}
		player.GetComponent<Player> ().ReceiveBaseTreasureStorageChange (treasure);
		Debug.Log (name + ", TreasureStorageChange, call " + player.name);
		//BroadcastMessage ("ReceiveBaseTreasureStorageChange", treasure);
	}
}
