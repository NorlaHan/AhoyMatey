using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public GameObject lastAttacker;
	public float fullHealth = 100f;
	public bool destroyOnDeath = false;

	//[SyncVar(hook = "OnChangeHealth")]	// Whenever current health  changed, call OnChangeHealth method.
	[SyncVar]
	public float currentHealth;

	// Use this for initialization
	void Start () {
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

	[Command]
	public void CmdOnTakeDamage (float damage, GameObject theAttacker){
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
			float lootedTreasure = GetComponentInChildren<PlayerTreasureStash> ().TreasureBeenLooted ();
			lastAttacker.GetComponentInChildren<PlayerTreasureStash> ().TreasureLoot (lootedTreasure);

			RpcRespawn ();
			//OnDeath ();
		}
	}

	[ClientRpc]
	void RpcRespawn (){
		if (isLocalPlayer) {
			transform.localPosition = Vector3.zero;
		}
	}

//	void OnDeath(){
//		destroyOnDeath = true;
//		Debug.LogWarning (name + ", is dead.");
//		//Destroy (gameObject);
//	}
}
