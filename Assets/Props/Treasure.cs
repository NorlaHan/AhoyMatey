﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Treasure : NetworkBehaviour {

	public static int treasureCount;

	[SyncVar]
	public float treasureAmount;

	public int minTreasure = 50, maxTreasure = 201;
	// Use this for initialization

	void Start () {
		Treasure.treasureCount++;
		//if (!isServer) {return;}
		CmdRollTreasureAmount ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[Command]
	void CmdRollTreasureAmount (){
		treasureAmount = Random.Range (minTreasure, maxTreasure);
		RpcSetTreasureAmount (treasureAmount);
	}

	[ClientRpc]
	void RpcSetTreasureAmount (float amount){
		treasureAmount = amount;
	}

	void OnTriggerEnter (Collider obj){
		if (obj.tag == "PlayerStash") {
			GameObject target = obj.transform.parent.transform.parent.gameObject;
			if (target.GetComponent<PlayerTreasureStash>()) {
				target.GetComponent<PlayerTreasureStash> ().TreasureLoot (treasureAmount);
				treasureAmount = 0;
				// TODO player can only pick the amount it can carry. the rest will left behind
				if (treasureAmount == 0) {
					Treasure.treasureCount--;
					Destroy (gameObject, 1);
				}
			}
		}

	}
}
