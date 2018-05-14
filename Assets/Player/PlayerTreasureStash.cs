using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTreasureStash : NetworkBehaviour {

	public bool isDebugMode = false;

	[SyncVar(hook = "OnChangeTreasureCarry")]
	public float playerTreasureCarry;

	public Player player;

	public float lootPenalty = 4f, startTreasure = 0;

	// Use this for initialization
	void Start () {
		// Start with 100 treasure.
		playerTreasureCarry = startTreasure;
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
		Debug.Log ("Store treasure to base : "+ treasureToStore);
		return treasureToStore;
	}

	// Lost treasure to the attacker when dead.
	public float TreasureBeenLooted (){
		float treasureLooted = Mathf.Floor(playerTreasureCarry / lootPenalty);
		playerTreasureCarry = 0;
		Debug.Log ("Store treasure to base : "+ treasureLooted);
		return treasureLooted;
	}

	public void TreasureLoot (float lootedTreasure){
		playerTreasureCarry += lootedTreasure;
	}

	// Send message to player
	void OnChangeTreasureCarry (float treasure){
		SendMessage ("ReceiveTreasureCarryChange",treasure);
		Debug.Log (name + ", TreasureCarryChange");
	}
		
}
