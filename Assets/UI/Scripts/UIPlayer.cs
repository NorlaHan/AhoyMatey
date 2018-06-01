using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIPlayer : MonoBehaviour {
	
	public bool isDebugMode = false;
	public GameObject  miniMap, playerIndicatorPrefab , enemyIndicatorPrefab;
	public MyNetworkManager networkManager;
	public Player player;
	public PlayerBase playerBase;
	//public Health health;
	public float MaxSpeed, MaxArmor;
	public Text treasureInBase, treasurePlayerCarried, playerText, playerSpeed, playerArmor;
	public Slider healthBar;
	public Image weaponIcon;
	public Sprite[] weaponIcons;
	public Player[] players, enemys;
	public int playerCount;
	public GameObject[] enemyIndicators;
	public RectTransform PIRectTrans;

	private GameObject playerIndicator;
	private Rect mapRect;

	// Call from player Rpc instantiate. faster than Start

	public void LinkUIToPlayer(Player playerOnClient, PlayerBase playerBaseOnClient){
		player = playerOnClient;
		playerBase = playerBaseOnClient;
		MaxSpeed = player.MaxSpeed;
		MaxArmor = player.MaxArmor;
//		if (player) {
//			UIUpdatePlayerPowerUp ("Speed");
//			UIUpdatePlayerPowerUp ("Armor");
//		}
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
			}else if (item.name == "StatusSpeed") {
				playerSpeed = item;
			}else if (item.name == "StatusArmor") {
				playerArmor = item;
			}
		}
		Image[] images = GetComponentsInChildren<Image> ();
		foreach (Image item in images) {
			if (item.name == "WeaponIcon") {
				weaponIcon = item;
			}
		}
		healthBar = GetComponentInChildren<Slider> ();
		//playerText.text = player.name;
		//miniMap = GetComponentInChildren<RawImage>().rectTransform.rect;
		playerIndicator = Instantiate(playerIndicatorPrefab,miniMap.transform.position,Quaternion.identity);
		playerIndicator.transform.SetParent (miniMap.transform);
		PIRectTrans = playerIndicator.GetComponent<RectTransform> ();
		mapRect = miniMap.GetComponent<RectTransform> ().rect;

		networkManager = GameObject.FindObjectOfType<MyNetworkManager> ();

		// Refresh checking every second.
		InvokeRepeating ("CheckEnemyIndicator",1,2);
	}

	void CheckEnemyIndicator (){
		// Recatch player if player count changes.
		players = GameObject.FindObjectsOfType<Player> ();
		if (playerCount != players.Length) {
			foreach (var item in enemyIndicators) {
				Destroy(item);
			}
			players = GameObject.FindObjectsOfType<Player> ();
			playerCount = players.Length;
			int k = 0;
			enemys = new Player[players.Length - 1];
			enemyIndicators = new GameObject[players.Length - 1];
			for (int i = 0; i < players.Length; i++) {
				if (players[i] != player) {
					enemys [k] = players [i];
					k++;
				}
			}
			// refresh enemy indicator position.
			for (int i = 0; i < enemys.Length; i++) {
				enemyIndicators[i] = Instantiate(enemyIndicatorPrefab, miniMap.transform.position,Quaternion.identity);
				enemyIndicators[i].transform.SetParent (miniMap.transform);
			}
		}
	}

	void Update (){
		// Destroy UI if player doesn't exist.
		if (!player) {
			SelfDestruct ();
		}

		if (isDebugMode) {
			networkManager.NMHud.showGUI = false;
		}

		#region MiniMap update
		if (player) {
			PIRectTrans.anchoredPosition = new Vector2 (player.transform.position.x / 1000 * mapRect.width/2, player.transform.position.z / 1000 * mapRect.height/2);
		}
		for (int i = 0; i < enemys.Length; i++) {
			enemyIndicators [i].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (enemys [i].transform.position.x / 1000 * mapRect.width / 2, enemys [i].transform.position.z / 1000 * mapRect.height / 2);
		}
		#endregion

		// Rename if the name is not right
		if (player && playerText.text != player.name) {
			playerText.text = player.name;
		}

		//Debug.Log (player.transform.position + ", width = " +mapRect.width+", height = "+ mapRect.height + ", " + PIRectTrans.anchoredPosition);
	}

	public void UIUpdateTreasure(float treasureStoraged, float treasureCarried){
		treasureInBase.text = treasureStoraged.ToString ();
		treasurePlayerCarried.text = treasureCarried.ToString ();

	}

	public void UIUpdatePlayerHealth(float playerHealthPercentage){
		healthBar.value = playerHealthPercentage;
	}

	public void UIUpdatePlayerWeapon (string weaponType ){
		if (weaponType == "CannonOG") {
			weaponIcon.sprite = weaponIcons [0];
		}else if (weaponType == "ScatterGun") {
			weaponIcon.sprite = weaponIcons [1];
		}else if (weaponType == "SuperCannon") {
			weaponIcon.sprite = weaponIcons [2];
		}
	}

	public void UIUpdatePlayerPowerUp (string type, float parameter){
		if (type == "Speed") {
			if (parameter < MaxSpeed) {
				playerSpeed.text = parameter.ToString ();
			} else {
				playerSpeed.text = "Max";
			}


		}else if (type == "Armor") {
			if (parameter < MaxArmor) {
				playerArmor.text = parameter.ToString ();
			}else{
				playerArmor.text = "Max";
			}
		}
		//Debug.Log ("type = " + type + ", parameter = "+ parameter);
	}


	public void SelfDestruct (){
		Destroy (gameObject);
	}

	public void DebugTest(string playerName){
		Debug.Log (playerName + " can call " + name);
	}

}
