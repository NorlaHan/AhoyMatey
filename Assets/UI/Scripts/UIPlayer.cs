using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;


public class UIPlayer : MonoBehaviour {
	
	public bool isDebugMode = false;
    public MusicManager musicManager;
    public GameObject miniMap, menu, repairConfirm, startMenu, startMenuCamera, helpPanel;
	public GameObject playerIndicatorPrefab , enemyIndicatorPrefab;
	public GameObject[] playerBaseIndicators;
	//public PlayerSpawnPoint[] playerSpawnPoints = new PlayerSpawnPoint[4];
	public PlayerBase[] playerBases = new PlayerBase[4];

	public MyNetworkManager networkManager;
	public Player player;
	public PlayerBase playerBase;
	//public Health health;
	public float MaxSpeed, MaxArmor;
	public Text treasureInBase, treasurePlayerCarried, playerText, playerSpeed, playerArmor;
	public Slider healthBar, baseDefenceHealthBar;
	public Image weaponIcon;
	public Sprite[] weaponIcons;
	public Player[] players, enemys;
	public int playerCount;
	public GameObject[] enemyIndicators;
	public RectTransform PIRectTrans;

	private GameObject playerIndicator;
	private Rect mapRect;

	UnityEngine.Networking.NetworkManagerHUDSRCFontSize NMHudSRCFontSize;

	// Call from player Rpc instantiate. faster than Start

	public void LinkUIToPlayer(Player playerOnClient, PlayerBase playerBaseOnClient){
		player = playerOnClient;
		playerBase = playerBaseOnClient;
		MaxSpeed = player.MaxSpeed;
		MaxArmor = player.MaxArmor;

		playerBases = GameObject.FindObjectsOfType<PlayerBase> (); 
		for (int i = 0; i < playerBases.Length; i++) {
			Vector3 pbPos = playerBases [i].transform.position;
			Vector2 pbRecPos;
			pbRecPos = new Vector2 (pbPos.x / 500 * mapRect.width / 2, pbPos.z / 500 * mapRect.height / 2);
			playerBaseIndicators [i].GetComponent<RectTransform> ().anchoredPosition = pbRecPos;
			//Debug.Log ("pbPos is : " + pbPos);
			if (playerBases [i] == playerBase) {
				playerBaseIndicators [i].GetComponent<Image> ().color = Color.green;
				if (player.name == "Player1") {
					playerBaseIndicators [i].GetComponentInChildren<Text> ().text = "1";
				}else if (player.name == "Player2") {
					playerBaseIndicators [i].GetComponentInChildren<Text> ().text = "2";
				}else if (player.name == "Player3") {
					playerBaseIndicators [i].GetComponentInChildren<Text> ().text = "3";
				}else if (player.name == "Player4") {
					playerBaseIndicators [i].GetComponentInChildren<Text> ().text = "4";
				};
				//Debug.Log ("my base : " + pbRecPos);
			} else {
				playerBaseIndicators [i].GetComponent<Image> ().color = Color.red;
				playerBaseIndicators [i].GetComponentInChildren<Text> ().text = "X";
				//Debug.Log ("enemy base : "+ pbRecPos);
			}

		}
//		if (player) {
//			UIUpdatePlayerPowerUp ("Speed");
//			UIUpdatePlayerPowerUp ("Armor");
//		}
	}

	void Awake (){
		transform.SetParent (GameObject.Find("UICanvas").GetComponent<Transform>());
		GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0, 0, 0);
		GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		mapRect = miniMap.GetComponent<RectTransform> ().rect;
	}

	// Use this for initialization
	void Start () {


        if (!musicManager) {
            musicManager = GameObject.FindObjectOfType<MusicManager>();
        }
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
		Slider[] bars = GetComponentsInChildren<Slider> ();
		foreach (Slider item in bars) {
			if (item.name == "HealthBar") {
				healthBar = item;
			}else if (item.name == "BDHealthBar") {
				baseDefenceHealthBar = item;
			}
		}

		healthBar = GetComponentInChildren<Slider> ();
		if (!startMenu) {
			startMenu = GameObject.Find("StartMenu");
		}

		startMenuCamera = GameObject.Find("StartMenuCamera");
		//playerText.text = player.name;
		//miniMap = GetComponentInChildren<RawImage>().rectTransform.rect;
		playerIndicator = Instantiate(playerIndicatorPrefab,miniMap.transform.position,Quaternion.identity);
		playerIndicator.transform.SetParent (miniMap.transform);
		PIRectTrans = playerIndicator.GetComponent<RectTransform> ();


		networkManager = GameObject.FindObjectOfType<MyNetworkManager> ();

		startMenu.SetActive (false);
		//startMenuCamera.SetActive (false);
		menu.SetActive (false);
		//networkManager.NMHud.showGUI = false;


		NMHudSRCFontSize = GameObject.FindObjectOfType<UnityEngine.Networking.NetworkManagerHUDSRCFontSize> ();
		NMHudSRCFontSize.showGUI = false;
		//networkManager.gameObject.GetComponent<NetworkManagerHUDSRCFontSize>().showGUI = false;

		miniMap.SetActive(false);
		repairConfirm.SetActive (false);

		// Refresh checking every second.
		InvokeRepeating ("CheckEnemyIndicator",1,2);

        // Music
        musicManager.OnStartSceneBGM();

		helpPanel.SetActive (true);
		menu.SetActive (true);
		miniMap.SetActive (true);
//		PlayerSpawnPoint[] SPArray = GameObject.FindObjectsOfType<PlayerSpawnPoint> ();
//		foreach (var item in SPArray) {
//			if (item.name == "SpawnPoint1") {
//				playerSpawnPoints [0] = item;
//			}else if (item.name == "SpawnPoint2") {
//				playerSpawnPoints [1] = item;
//			}else if (item.name == "SpawnPoint3") {
//				playerSpawnPoints [2] = item;
//			}else if (item.name == "SpawnPoint4") {
//				playerSpawnPoints [3] = item;
//			}
//		}
//
//		for (int i = 0; i < playerSpawnPoints.Length; i++) {
//			playerBaseIndicators [i].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (playerSpawnPoints [i].transform.position.x / 500 * mapRect.width / 2, playerSpawnPoints [i].transform.position.z / 500 * mapRect.height / 2);
//		}

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
		}

		if (CrossPlatformInputManager.GetButtonDown("Cancel")) {
			OnToggleMenu ();
		}

		if (CrossPlatformInputManager.GetButtonDown("MapToggle")) {
			OnToggleMiniMap ();
		}

		if (CrossPlatformInputManager.GetButtonDown("Help")) {
			OnToggleHelp ();
		}

		#region MiniMap update
		// Only update when the miniMap is showen.
		if (miniMap.activeSelf) {
			if (player) {
				PIRectTrans.anchoredPosition = new Vector2 (player.transform.position.x / 500 * mapRect.width/2, player.transform.position.z / 500 * mapRect.height/2);
				//Debug.Log("Update player indicator");
			}
			for (int i = 0; i < enemys.Length; i++) {
				enemyIndicators [i].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (enemys [i].transform.position.x / 500 * mapRect.width / 2, enemys [i].transform.position.z / 500 * mapRect.height / 2);
				//Debug.Log("Update enemies indicator");
			}
		}
		#endregion

		// Rename if the name is not right
		if (player && playerText.text != player.name) {
			playerText.text = player.name;
		}

		//Debug.Log (player.transform.position + ", width = " +mapRect.width+", height = "+ mapRect.height + ", " + PIRectTrans.anchoredPosition);
	}

	public void OnPlayerExitGame(){
		Application.Quit ();
	}

	public void OnToggleMenu ()
	{
		if (menu.activeSelf) {
			menu.SetActive (false);
		}
		else {
			menu.SetActive (true);
		}
	}

	public void OnToggleNetworkManagerHUD (){
//		if (networkManager.NMHud.showGUI) {
//			networkManager.NMHud.showGUI = false;
//		} else {
//			networkManager.NMHud.showGUI = true;
//		}
		if (NMHudSRCFontSize.showGUI) {
			NMHudSRCFontSize.showGUI = false;
		} else {
			NMHudSRCFontSize.showGUI = true;
		}
	}

	public void OnToggleMiniMap (){
		if (miniMap.activeSelf) {
			miniMap.SetActive(false);
		} else {
			miniMap.SetActive(true);
		}
	}

	public void OnToggleHelp () {
		if (helpPanel.activeSelf) {
			helpPanel.SetActive (false);
		} else {
			helpPanel.SetActive (true);
		}
	}

	public void UIUpdateTreasure(float treasureStoraged, float treasureCarried){
		treasureInBase.text = treasureStoraged.ToString ();
		treasurePlayerCarried.text = treasureCarried.ToString ();

	}

	public void UIUpdatePlayerHealth(float playerHealthPercentage){
		healthBar.value = playerHealthPercentage;
	}

	public void UIUpadteBaseDefenceHealth(float baseDefenceHealthPercentage){
		baseDefenceHealthBar.value = baseDefenceHealthPercentage;
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

	public void UIOnRepairButtonPressed (){
		if (!repairConfirm.activeSelf) {
			repairConfirm.SetActive (true);
		} else {
			repairConfirm.SetActive (false);
		}
	}

	public void UIOnConfirmRepairAgree () {
		player.OnRepairButtonPressed ();
		repairConfirm.SetActive (false);
	}

	public void UIOnConfirmRepairCancel (){
		repairConfirm.SetActive (false);
	}
}
