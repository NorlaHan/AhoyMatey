using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

	public GameObject uiStartMenu, uiEnding;
    public MusicManager musicManager;
	public bool isDebugMod = false;

	[SyncVar]
	public bool showEnding = false;

	// Use this for initialization
	void Start () {
        // uiStartMenu.SetActive(true);
		uiEnding.SetActive (false);
        if (!musicManager) {
            musicManager = GameObject.FindObjectOfType<MusicManager>();
        }
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
        musicManager.OnGameSettle();
        uiEnding.SetActive (true);
		//uiEnding.GetComponent<UIEnding> ().playerWinName = playerWinName;
		uiEnding.GetComponent<UIEnding> ().GameResult (playerWinner);
		Debug.Log (name + ", OnPlayerGameSettle");
	}
}
