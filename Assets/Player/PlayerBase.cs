﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBase : NetworkBehaviour {

	public bool isDebugMode = false, isActivated = false;
	public float baseRepair = 10f , lootCount = 0;

	[SyncVar(hook = "OnBaseTreasureStorageChange")]
	public float treasureStorage = 0;

	[SyncVar]
	public GameObject player;

	[SyncVar]
	public GameObject baseDefence;

	//public string playerName;


	public float winTreasureAmount = 2000;


//	public override void OnStartClient ()
//	{
//		base.OnStartClient ();
//		if (treasureStorage != 0) {
//
//		}
//	}

	public void BaseLinkToPlayer (GameObject playerOnClient){
		isActivated = true;
		player = playerOnClient;
		baseDefence.GetComponent<BaseDefence>().player = playerOnClient;
		player.GetComponent<Player> ().LinkBaseDefenceToPlayer (baseDefence);
		if (treasureStorage != 0) {
			
			Invoke ("PokeBaseTreasureStorageChange" ,2f);
		}
	}

	void PokeBaseTreasureStorageChange(){
		//treasureStorage += 0;
		OnBaseTreasureStorageChange (treasureStorage);
		Debug.Log ("Poke treasureStorage");
	}

	void OnBaseTreasureStorageChange (float treasure){

		if (player) {
			//if (!hasAuthority) {return;}
			player.GetComponent<Player> ().ReceiveBaseTreasureStorageChange (treasure);
			Debug.Log (name + ", TreasureStorageChange, call " + player.name);
		} else {
			if (isActivated) {
				Debug.Log ("TryToFindPlayerToStoreageChange");
				//if player doesn't exist, try again 0.2 sec later. 
				Invoke ("TryToFindPlayerToStoreageChange", 0.2f);
			}
		}
	}

	// if still not exist, trigger by adding 0.
	void TryToFindPlayerToStoreageChange (){
		if (player) {
			//if (!hasAuthority) {return;}
			player.GetComponent<Player> ().ReceiveBaseTreasureStorageChange (treasureStorage);
		} else {
			treasureStorage += 0;
		}
	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			treasureStorage += 1;
			OnBaseTreasureStorageChange (treasureStorage);
		}
	}

	void OnTriggerEnter (Collider obj){
		//GameObject target = obj.gameObject;
		//Debug.Log (name + ", trigger.");
		if (obj.tag == "PlayerStash") {
			if (player && obj.GetComponentInParent<Player>().gameObject == player) {
				treasureStorage += 0;
				treasureStorage += obj.GetComponentInParent<PlayerTreasureStash> ().TreasureStoreToBase ();
				Debug.Log (player.name + "Player back to base, storage is now : " + treasureStorage);
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
			Player playerInRange = obj.GetComponentInParent<Player>();
			if (playerInRange.gameObject == player) {
				if (!playerInRange.isDead) {	// Repairs only when the player is alive.
					Health health = obj.GetComponentInParent<Health> ();
					if (health.currentHealth < health.fullHealth) {
						health.OnGetRepair (Time.deltaTime * baseRepair);
					}
				}
			}else{	// Player in range is not own player.
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

	//[Command]
	public void BaseOnRepairButtonPressed (){
		Health bDHealth = baseDefence.GetComponent<Health> ();
		float lostHealth = bDHealth.LostHealth ();
		Debug.Log ("lostHealth = " + lostHealth);
		if (lostHealth < treasureStorage) {
			bDHealth.OnGetRepair (lostHealth);
			treasureStorage = Mathf.Floor(treasureStorage -= lostHealth);
		} else {
			bDHealth.OnGetRepair (treasureStorage);
			treasureStorage = 0;
		}
		//baseDefence.GetComponent<BaseDefence> ();
		Debug.Log ("BaseOnRepairButtonPressed");
	}
}
