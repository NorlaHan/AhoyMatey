using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public GameObject lastAttacker;
	public float fullHealth = 100f;
	public bool destroyOnDeath = false;

	[SyncVar(hook = "OnChangeHealth")]	// Whenever current health  changed, call OnChangeHealth method.
	public float currentHealth;

	// Use this for initialization
	void Start () {
		OnChangeHealth (currentHealth);
		if (!isServer) {return;}
		CmdFullHealth ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[Command]
	void CmdFullHealth (){
		currentHealth = fullHealth;
	}
		
	public void OnTakeDamage (float damage, GameObject theAttacker){
		if (!isServer) {
			return;
		}
		lastAttacker = theAttacker;
		currentHealth -= damage;
		if (currentHealth <= 0) {

			if (destroyOnDeath) {
				Destroy (gameObject);
			} else {
				currentHealth = fullHealth;
				Debug.LogWarning (name + ", is dead, but revive!");
			}
			PlayerTreasureStash treasureStash =  GetComponentInChildren<PlayerTreasureStash> ();

			// Take the treasure automatically.
//			float lootedTreasure = treasureStash.TreasureBeenLooted ();
//			lastAttacker.GetComponentInChildren<PlayerTreasureStash> ().TreasureLoot (lootedTreasure);

			// Treasure loot spawn at spot;
			treasureStash.CmdSpawnTreasureLoot ();

			RpcRespawn ();
			//OnDeath ();
		}
	}

//	[ClientRpc]
//	void RpcOnTakeDamage(){
//		
//	}

	[ClientRpc]
	void RpcRespawn (){
		if (isLocalPlayer) {
			transform.localPosition = Vector3.zero;
		}
	}

	public void OnChangeHealth(float crtHealth){
		SendMessage ("UpdatePlayerHealth", crtHealth / fullHealth);
	}

//	void OnDeath(){
//		destroyOnDeath = true;
//		Debug.LogWarning (name + ", is dead.");
//		//Destroy (gameObject);
//	}
}
