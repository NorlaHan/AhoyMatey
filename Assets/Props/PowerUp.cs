using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour {
	
	public static int powerUpCount;

	public bool isDebugMode= false , isLoot = false , isTaken = false;

	[SyncVar]
	public string parentName;

	[SyncVar]
	public float powerUpAmount;

	public int minArmor = 1, maxArmor = 2;
	public int minSpeed = 2, maxSpeed = 4;

	// Use this for initialization

	public enum PowerUpType {Armor,Speed,WeaponScatter,WeaponSuper};
	public PowerUpType type;

	//private Player PlayerBeenLooted;

	void Start () {
		if (isServer) {
			if (!isLoot) {
				PowerUp.powerUpCount++;
			}
		if (type == PowerUpType.Armor) {
				ServerRollArmorAmount ();
			}else if (type == PowerUpType.Speed) {
				ServerRollSpeedAmount ();
			}
			parentName = transform.parent.name;
		}

	}

	public void RepairLootFromPlayer(float repairLoot){
		powerUpAmount = repairLoot;
	}


	// Roll the amout at start sever only
	[Server]
	public void ServerRollArmorAmount (){
		Debug.Log ("Roll repair Amount");
		powerUpAmount = Random.Range (minArmor, maxArmor+1);
		RpcSetPowerUpAmount (powerUpAmount);
	}

	[Server]
	public void ServerRollSpeedAmount (){
		Debug.Log ("Roll repair Amount");
		powerUpAmount = Random.Range (minSpeed, maxSpeed+1);
		RpcSetPowerUpAmount (powerUpAmount);
	}

	[ClientRpc]
	void RpcSetPowerUpAmount (float amount){
		powerUpAmount = amount;
	}




// TODO WIP here
	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			Debug.Log (name +",type="+ type + ", isClient = " + isClient+", isServer = "+ isServer +", powerUpAmount = " + powerUpAmount);
			isDebugMode = false;
		}

		if (!transform.parent && !isLoot) {
			transform.SetParent (GameObject.Find(parentName).transform);
		}
		//CheckRepairOnClient ();
	}




	// Client get the amount and sync wuth server.


	void OnTriggerEnter (Collider obj){
		if (obj.tag == "PlayerStash") {
			//GameObject target = obj.GetComponentInParent<Health>().gameObject;
			if (obj.GetComponentInParent<Player>() && hasAuthority) {
				if (type == PowerUpType.Armor) {
					obj.GetComponentInParent<Player> ().CmdOnGetPowerUp ("Armor", powerUpAmount);
					powerUpAmount = 0;
				}else if (type == PowerUpType.Speed) {
					obj.GetComponentInParent<Player> ().CmdOnGetPowerUp ("Speed", powerUpAmount);
					powerUpAmount = 0;
				}else if (type == PowerUpType.WeaponScatter) {
					obj.GetComponentInParent<Player> ().CmdOnGetPowerUp ("WeaponScatter", powerUpAmount);
					powerUpAmount = 0;
				}else if (type == PowerUpType.WeaponSuper) {
					obj.GetComponentInParent<Player> ().CmdOnGetPowerUp ("WeaponSuper", powerUpAmount);
					powerUpAmount = 0;
				}

				// TODO player can only pick the amount it can carry. the rest will left behind
				if (powerUpAmount == 0) {
					if (isServer && !isLoot && !isTaken) {
						isTaken = true;
						PowerUp.powerUpCount--;
						Debug.Log (name + ", PowerUp.powerUpCount--");
					}
					Destroy (gameObject, 0.5f);
				}
			}
		}
	}
}
