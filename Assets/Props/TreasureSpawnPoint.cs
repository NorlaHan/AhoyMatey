using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class TreasureSpawnPoint : MonoBehaviour {

	public bool canSpawn = true;

	public bool isDebugMode = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			Debug.Log (transform.childCount + ", child = " + transform.GetChild(0));
			isDebugMode = false;
		}

//		if (transform.childCount == 0) {
//			Instantiate (SpawnedTreasure, transform.position, transform.rotation);
//		}

//		if (isServer) {
//			if (transform.childCount != 0) {
//				SpawnedTreasure = transform.GetChild (0).gameObject;
//			}
//		}

//		if (!isServer) {
//			return;
//		}
//		Debug.Log ("TreasureSpawnPoint first pass");
//		ServerCheckClientTreasureSpawn ();

	}

//	[Server]
//	void ServerCheckClientTreasureSpawn (){
//		if (transform.childCount != 0) {
//			Debug.Log ("ServerCheckClientTreasureSpawn ()");
//			GameObject treasure = transform.GetChild(0).gameObject;
//			RpcSyncTreasureOnServer (treasure);
//		}
//	}
//
//	[ClientRpc]
//	void RpcSyncTreasureOnServer (GameObject treasure){
//		if (transform.childCount == 0) {
//			Debug.Log ("RpcSyncTreasureOnServer (GameObject treasure)");
//			treasure.transform.SetParent (transform);
//		}
//	}


	void OnTriggerStay (Collider obj){
		GameObject target = obj.gameObject;
		//Debug.Log ("Trigger");
		if (target.tag == "Player") {
			if (canSpawn) {canSpawn = false;}
			//Debug.Log ("Trigger player");
		}
	}

	void OnTriggerExit (Collider obj){
		GameObject target = obj.gameObject;
		if (target.tag == "Player") {
			canSpawn = true;
		}
	}
		
}
