using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TreasureSpawnMaster : NetworkBehaviour {

	public GameObject treasurePrefab;

	// Debug Purpose.
	[Tooltip("Activate isDebugMode to Update.")]
	public int showTreasureCount;

	public bool isDebugMode = false;

	public float RespawnRatio = 1f;

	private TreasureSpawnPoint[] treasureSpawnPoints;

	private int rollSpawnStart;
	// Use this for initialization
	void Start () {
		//if (!isServer) {return;}
		treasureSpawnPoints = GameObject.FindObjectsOfType<TreasureSpawnPoint>(); 

//		if (hasAuthority) {
//			Debug.Log ("hasAuthority = " + hasAuthority);
//		}
//		if (isServer) {
//			Debug.Log ("isServer = " + isServer);
//		}
//		if (isClient) {
//			Debug.Log ("isClient = " + isClient);
//		}
		// treasureSpawnPoints = GetComponentsInChildren<TreasureSpawnPoint> ();
		// Check if server already spawned.
//		if (!isServer /* && hasAuthority */ ) {
//			OnCheckSpawnedTreasure ();
//		}
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
		//ClientScene.RegisterPrefab (treasurePrefab);
		Debug.Log ("ClientScene.RegisterPrefab (treasurePrefab)");
		if (isServer) {return;}
		OnCheckSpawnedTreasure ();
	}

	[Client]
	void OnCheckSpawnedTreasure (){
		Debug.Log ("ClientCheckSpawnedTreasure");
		treasureSpawnPoints = GameObject.FindObjectsOfType<TreasureSpawnPoint>();
//		for (int i = 0; i < treasureSpawnPoints.Length; i++) {
//			if (treasureSpawnPoints[i].transform.childCount == 0) {
//				Debug.Log ("treasureSpawnPoints [i] , Has no child, i");
				//if (!hasAuthority) {
					OnCheckSpawnedTreasure2 ();
				//} 
//				else {
//					Debug.Log ("CmdCheckSpawnedTreasure (i) has no authority ");
//				}
//			}
//		}
		//CmdCheckSpawnedTreasure ();
	}

	//[Command]
	void OnCheckSpawnedTreasure2 (/* int i */){
//		Debug.Log ("ServerCheckSpawnedTreasure (int i), i =" + i);
//		if (treasureSpawnPoints[i].transform.childCount != 0) {
//			Debug.Log ("Called RpcCheckSpawnedTreasure (i, spawnedTreasure)");
//			GameObject spawnedTreasure = treasureSpawnPoints[i].transform.GetChild(0).gameObject;
//			RpcCheckSpawnedTreasure (i , spawnedTreasure);
//		}

//		foreach (var item in treasureSpawnPoints) {
//			if (item.transform.childCount != 0) {
//				GameObject spawnedPoint = item.gameObject;
//				GameObject spawnedTreasure = item.transform.GetChild (0).gameObject;
//				Debug.Log (item.name + ", spawnedPoint :" + spawnedPoint + ",spawnedTreasure" +spawnedTreasure);
//				RpcCheckSpawnedTreasure (spawnedPoint, spawnedTreasure);
//			}
//		}
		//if (!isServer) {return;}
		Debug.Log ("Finding spawnpoints which has no child.");
		for (int i = 0; i < treasureSpawnPoints.Length; i++) {
			if (treasureSpawnPoints [i].transform.childCount != 0) {
				GameObject treasure = treasureSpawnPoints [i].transform.GetChild (0).gameObject;
				RpcCheckSpawnedTreasure (i, treasure);
			} else {
				Debug.Log ("all spawn points has no child");
			}
		}
	}

	[ClientRpc]
	void RpcCheckSpawnedTreasure (int i ,GameObject treasure){
//		if (hasAuthority) {
//			treasureSpawnPoints [i].GetComponent<TreasureSpawnPoint> ().ClientStartFakeSpawn ();
//			Debug.Log ("RpcCheckSpawnedTreasure, i =" + i);
//			//treasure.transform.SetParent (treasureSpawnPoints [i].transform);
//		} else {
//			Debug.LogWarning ("Not Client");
//		}
		//if (!hasAuthority) {
		if (treasureSpawnPoints [i].transform.childCount == 0) {
			treasure.transform.SetParent(treasureSpawnPoints [i].transform);
			Debug.Log ("Set treasure to spawn point on client");
		}
			
		//}
	}

	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			showTreasureCount = Treasure.treasureCount;
			Debug.Log ("Treasure.treasureCount is " +Treasure.treasureCount);
			isDebugMode = false;
		}
		if (!isServer) {return;}
		RpcSpawnTreasure ();
	}

	// Decided to spawn treasure or not.
	[Server]
	void RpcSpawnTreasure (){
		//Debug.Log ("Call Spawn");
		// make respawn not always start at 0.
		rollSpawnStart = Random.Range (0, treasureSpawnPoints.Length);
		if (Treasure.treasureCount < treasureSpawnPoints.Length*RespawnRatio) {
			//Debug.Log ("Treasure.treasureCount < treasureSpawnPoints.Length*RespawnRatio");
			for (int i = rollSpawnStart ; i < (treasureSpawnPoints.Length*RespawnRatio+rollSpawnStart); i++) {
				int ii = (i % treasureSpawnPoints.Length);
				if (treasureSpawnPoints [ii].transform.childCount == 0  &&  treasureSpawnPoints [ii].canSpawn) {
					//Debug.Log ("i = " + ii + "Spawn treasure");
					// Spawn on server.
					// [Error] Rpc Call on client

					var treasure = (GameObject)Instantiate (treasurePrefab, treasureSpawnPoints [ii].transform.position, treasureSpawnPoints [ii].transform.rotation);
					//NetworkServer.Spawn(treasure);
					if (isServer) {
						NetworkServer.Spawn(treasure);
						treasure.transform.SetParent (treasureSpawnPoints [ii].transform);
					}

					//if (!isServer) {return;}
					//RpcSpawnTreasure (ii);
					RpcSpawnTreasure2 (ii, treasure);

//					if (!isServer) {return;}
//					GameObject treasure = Instantiate (treasurePrefab, treasureSpawnPoints [ii].transform.position, treasureSpawnPoints [ii].transform.rotation);
//					treasure.transform.SetParent (treasureSpawnPoints [ii].transform);
				}
			}
		}
	}

	// Spawn on client.
//	[ClientRpc]
//	void RpcSpawnTreasure (int i) {
//		var treasure = (GameObject)Instantiate (treasurePrefab, treasureSpawnPoints [i].transform.position, treasureSpawnPoints [i].transform.rotation);
//		//NetworkServer.Spawn(treasure);
//
//		NetworkServer.Spawn(treasure);
//		treasure.transform.SetParent (treasureSpawnPoints [i].transform);
//		if (!isServer) {return;}
//		//treasurePrefab.GetComponent<Treasure> ().ServerRollTreasureAmount ();
//	}

	[ClientRpc]
	void RpcSpawnTreasure2 (int i, GameObject treasure){
		treasure.transform.SetParent (treasureSpawnPoints [i].transform);
	}

}
