using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TreasureSpawnMaster : NetworkBehaviour {

	public GameObject treasurePrefab;

	public int showTreasureCount;

	private TreasureSpawnPoint[] treasureSpawnPoints;

	// Use this for initialization
	void Start () {
		//if (!isServer) {return;}
		treasureSpawnPoints = GetComponentsInChildren<TreasureSpawnPoint> ();
		//SpawnTreasure ();
	}
	
	// Update is called once per frame
	void Update () {
		
		showTreasureCount = Treasure.treasureCount;
		//if (!isServer) {return;}
		if (Treasure.treasureCount < treasureSpawnPoints.Length) {
			SpawnTreasure ();
		}
	}
		
	void SpawnTreasure () {
		for (int i = 0; i < treasureSpawnPoints.Length; i++) {
			if (treasureSpawnPoints [i].transform.childCount == 0  &&  treasureSpawnPoints [i].canSpawn) {
				GameObject treasure = Instantiate (treasurePrefab, treasureSpawnPoints [i].transform.position, treasureSpawnPoints [i].transform.rotation);
				treasure.transform.SetParent (treasureSpawnPoints [i].transform);
			}
		}
	}
}
