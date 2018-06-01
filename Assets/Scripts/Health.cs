using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public enum UnitType {Player, Mob, Boss};
	public UnitType type;

	public GameObject lastAttacker;
	public float fullHealth = 100f, armor= 0f, shield = 0f ;
	public bool isDebugMode = false, destroyOnDeath = false;

	[SyncVar /* (hook = "OnChangeHealth")*/ ]	// Whenever current health  changed, call OnChangeHealth method.
	public float currentHealth;

	private Player player;

	// Use this for initialization
	void Start () {
		if (type == UnitType.Player) {
			OnChangeHealth (currentHealth);
			if (GetComponent<Player> ()) {
				player = GetComponent<Player> ();
			} else {Debug.LogWarning (name + ", missing Player");}

			if (isServer) {
				CmdFullHealth ();
			}

		}
		// Add other types of units.
	}

	[Command]
	void CmdFullHealth (){
		currentHealth = fullHealth;
	}

	[Command]
	void CmdHealOverTime (float heal) {
		if (isDebugMode && currentHealth < fullHealth) {
			currentHealth =Mathf.Clamp ((currentHealth += Time.deltaTime * heal),0,fullHealth);
			OnChangeHealth (currentHealth);
		}
	}

	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			CmdHealOverTime (100);
		}
	}

	void OnLandDamege (float evDamage){
		OnTakeDamage (evDamage, null);
	}

	public void OnTakeDamage (float damage, GameObject theAttacker){
		// Only server handle the health.
		//if (!isServer) {return;}
		if (!hasAuthority) {return;}
		armor = player.armor;
		lastAttacker = theAttacker;
		if (!lastAttacker) {
			currentHealth -= damage;
		} else {
			currentHealth -= Mathf.Clamp ((damage * ((10 - armor) / 10)), 1, 9999);
		}
		OnChangeHealth (currentHealth);
		if (currentHealth <= 0) {
			if (type == UnitType.Player) {
				if (destroyOnDeath) {
					Destroy (gameObject);
				} else {
					Debug.LogWarning (name + ", is dead.");
				}
				//PlayerTreasureStash treasureStash =  GetComponentInChildren<PlayerTreasureStash> ();
				// Take the treasure automatically.
				//			float lootedTreasure = treasureStash.TreasureBeenLooted ();
				//			lastAttacker.GetComponentInChildren<PlayerTreasureStash> ().TreasureLoot (lootedTreasure);
			}
			// Add other types of units.

			// Treasure loot spawn at spot;
			SendMessage ("RpcOnUnitDeath");
		}
	}

//	[ClientRpc]
//	void RpcOnTakeDamage(){
//		
//	}

	public void OnGetRepair (float repair){
		// Only server handle the health.
		if (!hasAuthority) {return;}
		currentHealth = Mathf.Clamp(currentHealth += repair, 0 , fullHealth);
		OnChangeHealth (currentHealth);
	}


	// Hook to currentHealth.
	public void OnChangeHealth(float crtHealth){
		SendMessage ("UpdatePlayerHealth", crtHealth / fullHealth);
		//Debug.Log (name + ", OnChangeHealth");
	}

	void OnRespawnHealth (){
		currentHealth = fullHealth;
		OnChangeHealth (currentHealth);
		Debug.LogWarning (name + ", is revive!");
	}
}
