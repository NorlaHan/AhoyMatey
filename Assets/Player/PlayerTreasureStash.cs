using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTreasureStash : NetworkBehaviour {

	public bool isDebugMode = false;
	public GameObject treasureLootPrefab;
	public GameObject[] powerUpLootPrefabs;

	[SyncVar(hook = "OnChangeTreasureCarry")]
	public float playerTreasureCarry;

	[SyncVar]
	public GameObject player;

	public float lootPenalty = 4f, startTreasure = 0, pillage = 20f;

	private Transform playerLoots;
	private Vector3 deathPosition;
	private float treasureForLoot;

	public override void OnStartClient ()
	{
		base.OnStartClient ();
		ClientScene.RegisterPrefab (treasureLootPrefab);
		Debug.Log ("ClientScene.RegisterPrefab (treasurePrefab)");
	}

	// Use this for initialization
	void Start () {
		// Start with 100 treasure.
		playerTreasureCarry = startTreasure;
		player = GetComponent<Player> ().gameObject;
		if (GameObject.Find ("PlayerLoots")) {
			playerLoots = GameObject.Find ("PlayerLoots").transform;
		} else {
			Debug.LogWarning (name + ", missing PlayerLoots. Auto generate.");
			playerLoots = new GameObject ("PlayerLoots").transform;
		}

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
		treasureForLoot = TreasureBeenLooted ();
		deathPosition = position;
		Invoke ("DelaySpawnLoot", 0.5f);
	}

	void DelaySpawnLoot (){
		int i = Random.Range (0, powerUpLootPrefabs.Length);
		GameObject PU = Instantiate (powerUpLootPrefabs[i],deathPosition, Quaternion.identity);
		NetworkServer.Spawn (PU);
		PU.transform.SetParent (playerLoots);

		// Don't spawn treasure if there're nothing to loot.
		if (treasureForLoot<=0) {
			Debug.Log (name + ", treasureForLoot = " + treasureForLoot);
			return;
		}
		GameObject treasureLoot = Instantiate (treasureLootPrefab, deathPosition, Quaternion.identity);
		NetworkServer.Spawn (treasureLoot);
		treasureLoot.GetComponent<Treasure>().TreasureLootFromPlayer(treasureForLoot  , player );
		treasureLoot.transform.SetParent (playerLoots);
		Debug.Log ("CmdSpawnTreasureLoot ()");
		//RpcSpawnTreasureLoot (treasureLoot, deathPosition);
	}

	[Command]
	public void CmdSpawnNoLoot ( Vector3 position){	// But still Spawn gold if carried.
		treasureForLoot = TreasureBeenLooted ();
		deathPosition = position;
		Invoke ("DelaySpawnNoPU", 0.5f);
	}

	void DelaySpawnNoPU (){
		if (treasureForLoot<=0) {
			Debug.Log (name + ", treasureForLoot = " + treasureForLoot);
			return;
		}
		GameObject treasureLoot = Instantiate (treasureLootPrefab, deathPosition, Quaternion.identity);
		NetworkServer.Spawn (treasureLoot);
		treasureLoot.GetComponent<Treasure>().TreasureLootFromPlayer(treasureForLoot  , player );
		treasureLoot.transform.SetParent (playerLoots);
		Debug.Log ("CmdSpawnTreasureLoot ()");
	}


	[ClientRpc]
	void RpcSpawnTreasureLoot (GameObject treasureLoot , Vector3 position) {
		treasureLoot.transform.position = position;
		Debug.Log ("RpcSpawnTreasureLoot");
	}

	public void TreasureLoot (float lootedTreasure){
		playerTreasureCarry += lootedTreasure;
		//playerTreasureCarry = Mathf.Floor(playerTreasureCarry += lootedTreasure);
	}

	// Send message to player
	void OnChangeTreasureCarry (float treasure){
		SendMessage ("ReceiveTreasureCarryChange",treasure);
		//Debug.Log (name + ", TreasureCarryChange");
	}
		
}
