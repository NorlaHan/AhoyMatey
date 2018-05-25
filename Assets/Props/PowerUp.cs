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

	public float minArmor = 1, maxArmor = 2;
	public float minSpeed = 2, maxSpeed = 4;

	// Use this for initialization

	public enum PowerUpType {Armor,Speed,Weapon};
	public PowerUpType type;

	//private Player PlayerBeenLooted;

	void Start () {
		if (isServer && !isLoot) {
			PowerUp.powerUpCount++;
			if (!isServer) {return;}
		if (type == PowerUpType.Armor) {
				ServerRollArmorAmount ();
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
		powerUpAmount = Random.Range (minArmor, maxArmor);
		RpcSetPowerUpAmount (powerUpAmount);
	}
// TODO WIP here
	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			Debug.Log (name + "isClient = " + isClient+", isServer = "+ isServer +", repairAmount :" + powerUpAmount);
		}
		if (!transform.parent && !isLoot) {
			transform.SetParent (GameObject.Find(parentName).transform);
		}
		//CheckRepairOnClient ();
	}




	// Client get the amount and sync wuth server.
	[ClientRpc]
	void RpcSetPowerUpAmount (float amount){
		powerUpAmount = amount;
	}

	void OnTriggerEnter (Collider obj){
		if (obj.tag == "PlayerStash") {
			//GameObject target = obj.GetComponentInParent<Health>().gameObject;
			if (obj.GetComponentInParent<Health>()) {
				obj.GetComponentInParent<Health>().OnGetRepair(powerUpAmount);
				powerUpAmount = 0;
				// TODO player can only pick the amount it can carry. the rest will left behind
				if (powerUpAmount == 0) {
					if (isServer && !isLoot && !isTaken) {
						isTaken = true;
						PowerUp.powerUpCount--;
						Debug.Log (name + ", PowerUp.powerUpCount--");
					}
					Destroy (gameObject, 1);
				}
			}
		}
	}
}
