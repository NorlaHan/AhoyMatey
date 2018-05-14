using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIEnding : NetworkBehaviour {

	public Text playerName, Result;

	//[SyncVar(hook = "GameResult")]
	//public GameObject playerWinner;

//	[SyncVar]
//	public string playerWinName;

	// Use this for initialization
	void Start () {
//		if (!playerName) {
//			Text[] texts = GameObject.FindObjectsOfType<Text> ();
//			foreach (Text item in texts) {
//				if (item.name == "UIPlayerName") {
//					playerName = item;
//				}
//				if (item.name == "UIResult") {
//					Result = item;
//				} else {
//					
//				}
//			}
//		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GameResult (GameObject playerWinner){
		if (playerWinner.GetComponent<Player>().IsLocalPlayer()) {
			Result.text = "You Win !!";
		}else{
			Result.text = "You Lose !!";
		}
		playerName.text = playerWinner.name;
	}

}
