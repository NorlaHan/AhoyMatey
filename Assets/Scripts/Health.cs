using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public enum UnitType {Player, Mob, Boss, Base};
	public UnitType type;

	public GameObject lastAttacker;
	public float fullHealth = 100f, armor= 0f, shield = 0f, recovery = 0f , aggroCount = 0;
	public bool isDebugMode = false, destroyOnDeath = false;

	[SyncVar /* (hook = "OnChangeHealth")*/]	// Whenever current health  changed, call OnChangeHealth method.
	public float currentHealth;

	private Player player;
	private BaseDefence baseDefence;

	// Use this for initialization
	void Start () {
		if (type == UnitType.Player) {
			if (GetComponent<Player> ()) {
				player = GetComponent<Player> ();
			} else {Debug.LogWarning (name + ", missing Player");}
		}else if (type == UnitType.Base) {
			if (GetComponent<BaseDefence>()) {
				baseDefence = GetComponent<BaseDefence> ();
			}else {Debug.LogWarning (name + ", missing BaseDefence");}
		}
		FullHealth ();
		// Add other types of units.
	}
		
	void FullHealth (){
			currentHealth = fullHealth;
			OnChangeHealth (currentHealth);
			Debug.Log ("FullHealth ()");
	}


	void HealOverTime (float heal) {
		if (currentHealth < fullHealth) {
			currentHealth =Mathf.Clamp ((currentHealth += Time.deltaTime * heal),0,fullHealth);
			OnChangeHealth (currentHealth);
		}
	}

	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			HealOverTime (100);
		}
		if (type == UnitType.Base && hasAuthority) {
			HealOverTime (recovery);
			if (baseDefence.isDead) {
				if (currentHealth/fullHealth > 0.33f) {
					SendMessage ("OnBaseDefenceRespawn");
				}
			}
		}

		if (lastAttacker) {
			aggroCount += Time.deltaTime;
			if (aggroCount > 5) {
				aggroCount = 0;
				lastAttacker = null;
			}
		}


	}

	void OnLandDamege (float evDamage){
		OnTakeDamage (evDamage, null);
	}

	public void OnTakeDamage (float damage, GameObject theAttacker){
		// Only server handle the health.
		//if (isServer) {
		if (hasAuthority) {
			if (type == UnitType.Player) {
				armor = player.armor;
			} else if (type == UnitType.Base) {
				armor = baseDefence.armor;
			}

			lastAttacker = theAttacker;

			if (!lastAttacker) {
				lastAttacker = null;
				currentHealth -= damage;
			} else {
				aggroCount = 0;
				currentHealth -= Mathf.Clamp ((damage * ((10 - armor) / 10)), 1, 9999);
			}
			OnChangeHealth (currentHealth);
			if (currentHealth <= 0) {
//				if (type == UnitType.Player) {
//					if (destroyOnDeath) {
//						Destroy (gameObject);
//					} else {
//						Debug.LogWarning (name + ", is dead.");
//					}
//					//PlayerTreasureStash treasureStash =  GetComponentInChildren<PlayerTreasureStash> ();
//					// Take the treasure automatically.
//					//			float lootedTreasure = treasureStash.TreasureBeenLooted ();
//					//			lastAttacker.GetComponentInChildren<PlayerTreasureStash> ().TreasureLoot (lootedTreasure);
//				} else if (type == UnitType.Base) {
//					//Debug.Log (name + ", is destroyed.");
//				}
				// Add other types of units.

				// Treasure loot spawn at spot;
				if (type == UnitType.Player) {
					if (lastAttacker) {
						SendMessage ("RpcOnUnitDeath", lastAttacker);
					} else {
						SendMessage ("RpcOnUnitSuicide");
					}

				} else if (type == UnitType.Base) {
					SendMessage ("OnBaseDefenceDestroy");
				}

			}
		}
	}

//	[ClientRpc]
//	void RpcOnTakeDamage(){
//		
//	}

	public void OnGetRepair (float repair){
		// Only server handle the health.
		//if (hasAuthority) {
			currentHealth = Mathf.Clamp (currentHealth += repair, 0, fullHealth);
			OnChangeHealth (currentHealth);
		//} 
//		else if (isClient) {
//			CmdOnGetRepair (repair);
//		}
	}

//	[Command]
//	void CmdOnGetRepair (float repair){
//		currentHealth = Mathf.Clamp (currentHealth += repair, 0, fullHealth);
//		OnChangeHealth (currentHealth);
//	}

	// Hook to currentHealth.

	public void OnChangeHealth(float crtHealth){
		if (type == UnitType.Player) {
			SendMessage ("UpdatePlayerHealth", crtHealth / fullHealth);
		}else if (type == UnitType.Base) {
			SendMessage ("CmdUpdateBaseDefenceHealth", crtHealth / fullHealth);
			//Debug.Log (name + ", OnChangeHealth");
		}
	}

	void OnRespawnHealth (){
		if (hasAuthority) {
			currentHealth = fullHealth;
			OnChangeHealth (currentHealth);
			Debug.LogWarning (name + ", is revive!");
		}
	}

	public float LostHealth(){
		float lostHealth = fullHealth - currentHealth;
		return lostHealth;
	}
}
