﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public enum UnitType {Player, Mob, Boss};
	public UnitType type;

	public GameObject lastAttacker;
	public float fullHealth = 100f;
	public bool destroyOnDeath = false;

	[SyncVar /* (hook = "OnChangeHealth")*/ ]	// Whenever current health  changed, call OnChangeHealth method.
	public float currentHealth;

	// Use this for initialization
	void Start () {
		if (type == UnitType.Player) {
			OnChangeHealth (currentHealth);
			if (!isServer) {return;}
			CmdFullHealth ();
		}
		// Add other types of units.
	}

	[Command]
	void CmdFullHealth (){
		currentHealth = fullHealth;
	}

	// Update is called once per frame
	void Update () {
		
	}
		
	public void OnTakeDamage (float damage, GameObject theAttacker){
		// Only server handle the health.
		//if (!isServer) {return;}
		if (!hasAuthority) {
			return;
		}
		lastAttacker = theAttacker;
		currentHealth -= damage;
		OnChangeHealth (currentHealth);
		if (currentHealth <= 0) {
			if (type == UnitType.Player) {
				if (destroyOnDeath) {
					Destroy (gameObject);
				} else {
					Debug.LogWarning (name + ", is dead.");
				}
				PlayerTreasureStash treasureStash =  GetComponentInChildren<PlayerTreasureStash> ();
				// Take the treasure automatically.
				//			float lootedTreasure = treasureStash.TreasureBeenLooted ();
				//			lastAttacker.GetComponentInChildren<PlayerTreasureStash> ().TreasureLoot (lootedTreasure);

				// Treasure loot spawn at spot;
				//treasureStash.CmdSpawnTreasureLoot ();

				//RpcOnUnitRespawn ();
				//OnDeath ();
			}
			// Add other types of units.

			SendMessage ("RpcOnUnitDeath");
		}
	}

//	[ClientRpc]
//	void RpcOnTakeDamage(){
//		
//	}

	public void OnGetRepair (float repair){
		// Only server handle the health.
		if (!isServer) {return;}
		Mathf.Clamp(currentHealth += repair, 0 , fullHealth);
	}


	// Hook to currentHealth.
	public void OnChangeHealth(float crtHealth){
		//GetComponent<Player> ().UpdatePlayerHealth (crtHealth / fullHealth);
		SendMessage ("UpdatePlayerHealth", crtHealth / fullHealth);
		Debug.Log (name + ", OnChangeHealth");
	}

	void OnRespawnHealth (){
		currentHealth = fullHealth;
		OnChangeHealth (currentHealth);
		Debug.LogWarning (name + ", is revive!");
	}
}
