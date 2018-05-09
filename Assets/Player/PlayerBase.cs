using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBase : NetworkBehaviour {

	[SyncVar]
	public float treasureStorage = 0;

	public float winTreasureAmount = 2000;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider obj){
		GameObject target = obj.gameObject;
		//Debug.Log (name + ", trigger.");
		if (target.tag == "PlayerStash") {
			if (target.transform.parent.transform.parent.transform.parent == transform) {
				treasureStorage += target.GetComponent<PlayerTreasureStash> ().TreasureStoreToBase ();
				Debug.Log (target.name + "Player back to base, storage is now : " + treasureStorage);
				if (treasureStorage >= winTreasureAmount) {
					Debug.Log (target.transform.parent.transform.parent.name + " Win!");
				}
			} else {
				// WIP
				BaseTreasureBeenLooted (1111);
			}

		}
	}

	void BaseTreasureBeenLooted (float looting){
		if (treasureStorage > 0) {
			treasureStorage -= looting;
		}
	}
}
