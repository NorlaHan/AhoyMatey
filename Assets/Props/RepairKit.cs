using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RepairKit : NetworkBehaviour {

	public static int repairCount;

	public bool isDebugMode= false , isLoot = false , isTaken = false;

	[SyncVar]
	public string parentName;

	[SyncVar]
	public float repairAmount;

	public int minRepair = 25, maxRepair = 60;
	// Use this for initialization

	//private Player PlayerBeenLooted;

	void Start () {
		if (isServer && !isLoot) {
			RepairKit.repairCount++;
			if (!isServer) {return;}
			//if (hasAuthority) {
			ServerRollRepairAmount ();
			//}
			parentName = transform.parent.name;
		}

	}

	public void RepairLootFromPlayer(float repairLoot){
		repairAmount = repairLoot;
	}
		

	// Roll the amout at start sever only
	[Server]
	public void ServerRollRepairAmount (){
		Debug.Log ("Roll repair Amount");
		repairAmount = Random.Range (minRepair, maxRepair);
		//Invoke ("RpcSetRepairAmount",1);
		//if (!isServer) {return;}
		RpcSetRepairAmount (repairAmount);
	}

	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			Debug.Log (name + "isClient = " + isClient+", isServer = "+ isServer +", repairAmount :" + repairAmount);
		}
		if (!transform.parent && !isLoot) {
			transform.SetParent (GameObject.Find(parentName).transform);
		}
		//CheckRepairOnClient ();
	}




	// Client get the amount and sync wuth server.
	[ClientRpc]
	void RpcSetRepairAmount (float amount){
		repairAmount = amount;
	}

	void OnTriggerEnter (Collider obj){
		if (obj.tag == "PlayerStash") {
			//GameObject target = obj.GetComponentInParent<Health>().gameObject;
			if (obj.GetComponentInParent<Health>()) {
				obj.GetComponentInParent<Health>().OnGetRepair(repairAmount);
				repairAmount = 0;
				// TODO player can only pick the amount it can carry. the rest will left behind
				if (repairAmount == 0) {
					if (isServer && !isLoot && !isTaken) {
						isTaken = true;
						RepairKit.repairCount--;
						Debug.Log (name + ", Repair.repairCount--");
					}
					Destroy (gameObject, 1);
				}
			}
		}
	}
}
