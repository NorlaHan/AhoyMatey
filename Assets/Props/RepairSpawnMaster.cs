using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RepairSpawnMaster : NetworkBehaviour {

	public GameObject repairKitPrefab;

	// Debug Purpose.
	[Tooltip("Activate isDebugMode to Update.")]
	public int showRepairCount;

	public bool isDebugMode = false;

	public float RespawnRatio = 1f;

	private RepairSpawnPoint[] repairSpawnPoints;

	private int rollSpawnStart;
	// Use this for initialization
	void Start () {
		//if (!isServer) {return;}
		repairSpawnPoints = GameObject.FindObjectsOfType<RepairSpawnPoint>(); 

	}

//	public override void OnStartClient ()
//	{
//		base.OnStartClient ();
//		//if (!isServer){return;}
//
//		OnCheckSpawnedTreasure ();
//	}
	public override void OnStartClient ()
	{
		base.OnStartClient ();
		//ClientScene.RegisterPrefab (repairPrefab);
		//Debug.Log ("ClientScene.RegisterPrefab (repairPrefab)");
		if (isServer) {return;}
		OnCheckSpawnedRepair ();
	}

	[Client]
	void OnCheckSpawnedRepair (){
		Debug.Log ("ClientCheckSpawnedRepair");
		repairSpawnPoints = GameObject.FindObjectsOfType<RepairSpawnPoint>();

		OnCheckSpawnedRepair2 ();

	}

	//[Command]
	void OnCheckSpawnedRepair2 (/* int i */){
//		Debug.Log ("ServerCheckSpawnedTreasure (int i), i =" + i);

		//if (!isServer) {return;}
		Debug.Log ("Finding spawnpoints which has no child.");
		for (int i = 0; i < repairSpawnPoints.Length; i++) {
			if (repairSpawnPoints [i].transform.childCount != 0) {
				GameObject repair = repairSpawnPoints [i].transform.GetChild (0).gameObject;
				RpcCheckSpawnedRepair (i, repair);
			} else {
				Debug.Log ("Repair spawn point has no child");
			}
		}
	}

	[ClientRpc]
	void RpcCheckSpawnedRepair (int i ,GameObject repair){
		if (repairSpawnPoints [i].transform.childCount == 0) {
			repair.transform.SetParent(repairSpawnPoints [i].transform);
			Debug.Log ("Set repair to spawn point on client");
		}
	}

	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			showRepairCount = RepairKit.repairCount;
			Debug.Log ("Repair.repairCount is " +RepairKit.repairCount);
			isDebugMode = false;
		}
		if (!isServer) {return;}
		RpcSpawnRepair ();
	}

	// Decided to spawn repair or not.
	[Server]
	void RpcSpawnRepair (){
		//Debug.Log ("Call Spawn");
		// make respawn not always start at 0.
		rollSpawnStart = Random.Range (0, repairSpawnPoints.Length);
		if (RepairKit.repairCount < repairSpawnPoints.Length*RespawnRatio) {
			//Debug.Log ("RepairKit.repairCount < repairSpawnPoints.Length*RespawnRatio");
			for (int i = rollSpawnStart ; i < (repairSpawnPoints.Length*RespawnRatio+rollSpawnStart); i++) {
				int ii = (i % repairSpawnPoints.Length);
				if (repairSpawnPoints [ii].transform.childCount == 0  &&  repairSpawnPoints [ii].canSpawn) {
					//Debug.Log ("i = " + ii + "Spawn repair");
					// Spawn on server.
					// [Error] Rpc Call on client

					var repair = (GameObject)Instantiate (repairKitPrefab, repairSpawnPoints [ii].transform.position, repairSpawnPoints [ii].transform.rotation);
					//NetworkServer.Spawn(repair);
					if (isServer) {
						NetworkServer.Spawn(repair);
						repair.transform.SetParent (repairSpawnPoints [ii].transform);
					}

					//if (!isServer) {return;}
					//RpcSpawnRepair (ii);
					RpcSpawnRepair2 (ii, repair);

//					if (!isServer) {return;}
					// GameObject repair = Instantiate (repairPrefab, repairSpawnPoints [ii].transform.position, repairSpawnPoints [ii].transform.rotation);
					//					repair.transform.SetParent (repairSpawnPoints [ii].transform);
				}
			}
		}
	}

	// Spawn on client.
//	[ClientRpc]
//		void RpcSpawnRepair (int i) {
//		var repair = (GameObject)Instantiate (repairPrefab, repairSpawnPoints [i].transform.position, repairSpawnPoints [i].transform.rotation);
//		//NetworkServer.Spawn(repair);
//
//			NetworkServer.Spawn(repair);
//			repair.transform.SetParent (repairSpawnPoints [i].transform);
//			if (!isServer) {return;}
//			//repairPrefab.GetComponent<Repair> ().ServerRollRepairAmount ();
//	}

	[ClientRpc]
	void RpcSpawnRepair2 (int i, GameObject repair){
		repair.transform.SetParent (repairSpawnPoints [i].transform);
	}

}
