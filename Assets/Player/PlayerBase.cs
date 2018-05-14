using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBase : NetworkBehaviour {

	public bool isDebugMode = false;

	[SyncVar(hook = "OnBaseTreasureStorageChange")]
	public float treasureStorage = 0;

	[SyncVar]
	public GameObject player;

	//public string playerName;


	public float winTreasureAmount = 2000;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			treasureStorage += 1;
		}
	}

	void OnTriggerEnter (Collider obj){
		GameObject target = obj.gameObject;
		//Debug.Log (name + ", trigger.");
		if (target.tag == "PlayerStash") {
			if (target.transform.parent.transform.parent.transform.parent == transform.parent) {
				treasureStorage += target.transform.parent.transform.parent.GetComponent<PlayerTreasureStash> ().TreasureStoreToBase ();
				Debug.Log (target.name + "Player back to base, storage is now : " + treasureStorage);
				if (treasureStorage >= winTreasureAmount) {
					Debug.Log (target.transform.parent.transform.parent.name + " Win!");
					//player.GetComponent<Player>().OnGameSettle ();
				}
			} else {
				// TODO opponent will steal treasure while near (WIP)
				Debug.Log ("Base is under attack by " + target.transform.parent.transform.parent.name);
				BaseTreasureBeenLooted (1111);
			}

		}
	}

	void BaseTreasureBeenLooted (float looting){
		if (treasureStorage > 0) {
			treasureStorage -= looting;
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
