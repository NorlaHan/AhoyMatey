using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UITreasure : MonoBehaviour {

	public Player player;
	public PlayerBase playerBase;

	public Text treasureInBase, treasurePlayerCarried;

	// Use this for initialization
	void Start () {
		Text[] texts = GetComponentsInChildren<Text> ();
		foreach (var item in texts) {
			if (item.name == "TreasureInBase") {
				treasureInBase = item;
			}else if (item.name == "TreasureCarried") {
				treasurePlayerCarried = item;
			}
		}


	}

	// Update is called once per frame
	void Update () {
//		if (!player) {
//			Player[] players = GameObject.FindObjectsOfType<Player> ();
//			foreach (var item in players) {
//				Debug.Log (item.name);
//				if (isLocalPlayer) {
//					player = item;
//				}
//			}
//			Debug.Log ("UI target player is " + player.name);
//		}
	}

	public void LinkUIToPlayer(Player playerOnClient, PlayerBase playerBaseOnClient){
		player = playerOnClient;
		playerBase = playerBaseOnClient;
	}

	public void UpdateTreasure(float treasureStoraged, float treasureCarried){
		treasureInBase.text = treasureStoraged.ToString ();
		treasurePlayerCarried.text = treasureCarried.ToString ();
	}
}
