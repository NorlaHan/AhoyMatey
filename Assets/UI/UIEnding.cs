using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIEnding : NetworkBehaviour {

	public Text playerName;

	//[SyncVar(hook = "GameResult")]
	//public GameObject playerWinner;

	[SyncVar(hook = "GameResult")]
	public string playerWinName;

	// Use this for initialization
	void Start () {
		if (!playerName) {
			Text[] texts = GameObject.FindObjectsOfType<Text> ();
			foreach (Text item in texts) {
				if (item.name == "UIPlayerName") {
					playerName = item;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GameResult (string playerWinName){
		playerName.text = playerWinName;
		Debug.Log ("Winner");
	}

}
