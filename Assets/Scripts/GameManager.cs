using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	public GameObject uiEnding;
	public bool isDebugMod = false;

	[SyncVar]
	public bool showEnding = false;

	// Use this for initialization
	void Start () {
		uiEnding.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		// Debug delete after complete.
//		if (isDebugMod) {
//			uiEnding.SetActive (isDebugMod);
//		} else {
//			uiEnding.SetActive (isDebugMod);
//		}
	}

	public void EndingSync (){
	
	}

	public void OnPlayerGameSettle (GameObject playerWinner){
		uiEnding.SetActive (true);
		//uiEnding.GetComponent<UIEnding> ().playerWinName = playerWinName;
		uiEnding.GetComponent<UIEnding> ().GameResult (playerWinner);
		Debug.Log (name + ", OnPlayerGameSettle");
	}
}
