using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTreasureStash : NetworkBehaviour {

	[SyncVar]
	public float playerTreasureCarry;

	public float lootPenalty = 4f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
