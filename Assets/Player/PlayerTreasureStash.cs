using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTreasureStash : NetworkBehaviour {

	public bool isDebugMode = false;
	public GameObject treasureLootPrefab;

	[SyncVar(hook = "OnChangeTreasureCarry")]
	public float playerTreasureCarry;

	public Player player;

	public float lootPenalty = 4f, startTreasure = 0;

	// Use this for initialization
	void Start () {
		// Start with 100 treasure.
		playerTreasureCarry = startTreasure;
		player = GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			playerTreasureCarry += 1;
		}
	}

	// Store the treasure carried to the base.
	public float TreasureStoreToBase (){
		float treasureToStore = playerTreasureCarry;
		playerTreasureCarry = 0;
		//Debug.Log ("Store treasure to base : "+ treasureToStore);
		return treasureToStore;
	}

	// Lost treasure to the attacker when dead.
	public float TreasureBeenLooted (){
		float treasureLooted = Mathf.Floor(playerTreasureCarry / lootPenalty);
		playerTreasureCarry = 0;
		Debug.Log ("Store treasure to base : "+ treasureLooted);
		return treasureLooted;
	}

	[Command]
	public void CmdSpawnTreasureLoot (Vector3 position){
		float treasureForLoot = TreasureBeenLooted ();
		// Dnn't spawn treasure if there're nothing to loot.
		if (treasureForLoot<=0) {
			Debug.Log (name + ", treasureForLoot = " + treasureForLoot);
			return;
		}
		GameObject treasureLoot = Instantiate (treasureLootPrefab, position, Quaternion.identity);
		NetworkServer.Spawn (treasureLoot);
		treasureLoot.GetComponent<Treasure>().TreasureLootFromPlayer(treasureForLoot /* , player*/ );
		Debug.Log ("CmdSpawnTreasureLoot ()");
		RpcSpawnTreasureLoot (treasureLoot);
	}

	[ClientRpc]
	void RpcSpawnTreasureLoot (GameObject treasureLoot) {
		Debug.Log ("RpcSpawnTreasureLoot");
	}

	public void TreasureLoot (float lootedTreasure){
		playerTreasureCarry += lootedTreasure;
	}

	// Send message to player
	void OnChangeTreasureCarry (float treasure){
		SendMessage ("ReceiveTreasureCarryChange",treasure);
		//Debug.Log (name + ", TreasureCarryChange");
	}
		
}
