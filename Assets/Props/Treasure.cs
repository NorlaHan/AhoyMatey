using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Treasure : NetworkBehaviour {

	public static int treasureCount;

	public bool isDebugMode= false , isLoot = false , isTaken = false;

	public float countTime = 0, protectedTime = 5f;

	[SyncVar]
	public string parentName;

	[SyncVar]
	public float treasureAmount;

	[SyncVar]
	public GameObject PlayerBeenLooted;

	public int minTreasure = 50, maxTreasure = 201;
	// Use this for initialization



	void Start () {
		if (isServer && !isLoot) {
			Treasure.treasureCount++;
			if (!isServer) {return;}
			//if (hasAuthority) {
			ServerRollTreasureAmount ();
			//}
			parentName = transform.parent.name;
		}

	}

	public void TreasureLootFromPlayer(float treasureLoot , GameObject victim){
		treasureAmount = treasureLoot;
		PlayerBeenLooted = victim;
	}

//	public override void OnStartClient ()
//	{
//		base.OnStartClient ();
//		//transform.SetPositionAndRotation(spawnTransform.position,spawnTransform.rotation);
//		transform.SetParent (GameObject.Find("parentName").transform);
//	}

	// Roll the amout at start sever only
	[Server]
	public void ServerRollTreasureAmount (){
		Debug.Log ("Roll Treasure Amount");
		treasureAmount = Random.Range (minTreasure, maxTreasure);
		//Invoke ("RpcSetTreasureAmount",1);
		//if (!isServer) {return;}
		RpcSetTreasureAmount (treasureAmount);
	}

	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			Debug.Log (name + "isClient = " + isClient+", isServer = "+ isServer +", treasureAmount :" + treasureAmount);
		}
		if (!transform.parent && !isLoot) {
			transform.SetParent (GameObject.Find(parentName).transform);
		}
		if (isLoot && PlayerBeenLooted) {
			countTime += Time.deltaTime;
			if (countTime >= protectedTime) {
				PlayerBeenLooted = null;
				countTime = 0;
			}
		}
		//CheckTreasureOnClient ();
	}

	// If the amount is 0, ask sever to sync.
//	[Client]
//	void CheckTreasureOnClient (){
//		if (treasureAmount == 0 && hasAuthority) {
//			// [Warning][Server/Client] trying to send command without authority
//			// [Warring][client] trying to send command without authority
//			CmdSyncTreasureAmount ();
//		}
//	}
//
//	// Sever recieved request, pass amount to client.
//	[Command]
//	void CmdSyncTreasureAmount (){
//		RpcSetTreasureAmount (treasureAmount);
//	}


	// Client get the amount and sync wuth server.
	[ClientRpc]
	void RpcSetTreasureAmount (float amount){
		treasureAmount = amount;
	}

	void OnTriggerEnter (Collider obj){
		if (obj.tag == "PlayerStash") {
			if (isLoot && obj.GetComponentInParent<Player> ().gameObject == PlayerBeenLooted) {
				return;
			}
			if (obj.GetComponentInParent<PlayerTreasureStash>()) {
				obj.GetComponentInParent<PlayerTreasureStash>().TreasureLoot (treasureAmount);
				treasureAmount = 0;
					// TODO player can only pick the amount it can carry. the rest will left behind
				if (treasureAmount == 0) {
					if (isServer && !isLoot && !isTaken) {
						isTaken = true;
						Treasure.treasureCount--;
						Debug.Log (name + ", Treasure.treasureCount--");
					}
					Debug.Log (name + ", Been comsumed");
					Destroy (gameObject, 1);
				}
			}
		}
	}
}
