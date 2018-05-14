using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIPlayer : MonoBehaviour {

	public Player player;
	public PlayerBase playerBase;
	//public Health health;

	public Text treasureInBase, treasurePlayerCarried, playerText;
	public Slider healthBar;

	// Call from player Rpc instantiate. faster than Start
	public void LinkUIToPlayer(Player playerOnClient, PlayerBase playerBaseOnClient){
		player = playerOnClient;
		playerBase = playerBaseOnClient;
	}

	void Awake (){
		transform.SetParent (GameObject.Find("UICanvas").GetComponent<Transform>());
		GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0, 490, 0);
		GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
	}

	// Use this for initialization
	void Start () {
		Text[] texts = GetComponentsInChildren<Text> ();
		foreach (Text item in texts) {
			if (item.name == "TreasureInBase") {
				treasureInBase = item;
			}else if (item.name == "TreasureCarried") {
				treasurePlayerCarried = item;
			}else if (item.name == "PlayerText") {
				playerText = item;
			}
		}
		healthBar = GetComponentInChildren<Slider> ();
		//playerText.text = player.name;
	}

	void Update (){

		// Destroy UI if player doesn't exist.
		if (!player) {
			SelfDestruct ();
		}

		// Rename if the name is not right
		if (playerText.text != player.name) {
			playerText.text = player.name;
		}
	}

	public void UIUpdateTreasure(float treasureStoraged, float treasureCarried){
		treasureInBase.text = treasureStoraged.ToString ();
		treasurePlayerCarried.text = treasureCarried.ToString ();

	}

	public void UIUpdatePlayerHealth(float playerHealthPercentage){
		healthBar.value = playerHealthPercentage;
	}

	public void SelfDestruct (){
		Destroy (gameObject);
	}

	public void DebugTest(string playerName){
		Debug.Log (playerName + " can call " + name);
	}
}
