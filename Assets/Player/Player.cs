using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	[SyncVar]
	public bool isDebugMode = false , isGameEnd = false;

	public PlayerSpawnPoint[] playerSpawnPoints;
	public GameObject playerUIPrefab ;
	public GameManager gameManager;

	[SyncVar]
	public GameObject playerBase;

	[SyncVar]
	public string playerName, parentName, playerBaseName;
//	[SyncVar]
//	public string playerName , playerParentName;

	public float treasureCarried, treasureStoraged , treasureToWin = 2000f;

	// public Vector3 playerPos, playerRot;
	public float vx , vz, vy, speed = 5f, jumpSpeed = 6f; 


	private Rigidbody rigidBody;
	private Camera playerCamera;
	private AudioListener audioListener;
	private UIPlayer uiPlayer;


	#region StartLocalPlayer
	// This is trigger on player spawn on the client.
	public override void OnStartLocalPlayer (){
		playerCamera = GetComponentInChildren<Camera> ();
		Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
		foreach (var camera in cameras) {
			camera.gameObject.SetActive(false);
		}
		playerCamera.gameObject.SetActive (true);

		CmdPlayerSpawn ();

	}

	// check which spawn point to spawn, then pass the spawn i to client.

	// This is called by the player to excicute on the server.
	[Command]
	void CmdPlayerSpawn () {
		bool isFull = true;
		//Debug.Log (name + ", Command called");

		playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint> ();

		for (int i = 0; i < playerSpawnPoints.Length; i++) {
			//Debug.Log ("Checking " + playerSpawnPoints [i].name + "...");
			if (!playerSpawnPoints [i].GetComponentInChildren<Player>()) {
				//Debug.Log (playerSpawnPoints [i].name + " have no player.");

				gameObject.transform.SetParent (playerSpawnPoints [i].transform);
				gameObject.name = "Player" + (i + 1);
				//Debug.Log ("Spawn " + name + " to " + playerSpawnPoints [i].name);

				RpcPlayerSpawn (i);
				isFull = false;
				break;
			}
		}
		if (isFull) {
			Debug.LogWarning (name + " sorry. The game is full. Please connect to another server.");
		}
	}


	[ClientRpc]
	void RpcPlayerSpawn (int i){
		//Debug.Log (name + ", ClientRpc called");

		// Set parent
		playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint> ();
		gameObject.transform.SetParent (playerSpawnPoints [i].transform);
		parentName = playerSpawnPoints [i].name;

		// Set player
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.name = "Player" + (i + 1);
		playerName = name;

		//Debug.Log ("Spawn " + name + " to " + playerSpawnPoints [i].name);

		// Set base
		playerBase = transform.parent.GetComponentInChildren<PlayerBase> ().gameObject;
		playerBase.name = "Player" + (i + 1) + "Base";
		playerBaseName = playerBase.name;
		// Cast self to the UI on the client.
		playerBase.GetComponentInChildren<PlayerBase> ().BaseLinkToPlayer (gameObject);
	}



	#endregion


	// Use this for initialization
	void Start () {
		rigidBody = GetComponentInChildren<Rigidbody> ();

		playerCamera = GetComponentInChildren<Camera> ();
		audioListener = GetComponentInChildren<AudioListener> ();
		gameManager = GameObject.FindObjectOfType<GameManager> ();

		Invoke ("InstantiatePlayerUI" , 0.2f);
		//InstantiatePlayerUI ();
	}

	[Client]
	void InstantiatePlayerUI ()
	{
		uiPlayer = Instantiate (playerUIPrefab).GetComponentInChildren<UIPlayer> ();
		TryToGetplayerBase ();
	}

	void TryToGetplayerBase(){
		if (playerBase) {
			uiPlayer.LinkUIToPlayer (this, playerBase.GetComponent<PlayerBase> ());
		} else {
			Debug.Log (name + "Can't find playerBase");
			if (transform.parent.GetComponentInChildren<PlayerBase> ()) {
				playerBase = transform.parent.GetComponentInChildren<PlayerBase> ().gameObject;
			}
			Invoke ("TryToGetplayerBase", 0.1f);
		}
	}

	// Update is called once per frame
	void Update () {
		if (isGameEnd) {
			return;
		}
		// Not local player will stop here.
		if (!isLocalPlayer) {
			try {
				if (!transform.parent && !isServer) {
					transform.SetParent (GameObject.Find (parentName).transform);
					//Debug.Log (name+", missing parent. set parent to " + parentName);
				}
				if (name != playerName) {
					name = playerName;
				}
				if (playerBase.name != playerBaseName) {
					playerBase.name = playerBaseName;
				}
			} catch{
				Debug.Log ("Something happen when new client join the game.");
			}

			return;
		}
		if (CrossPlatformInputManager.GetButton ("Horizontal")) {
			//playerPos.x += CrossPlatformInputManager.GetAxis ("Horizontal");
			vx = CrossPlatformInputManager.GetAxis ("Horizontal");
		}  else {
			vx = 0;
		}
		if (CrossPlatformInputManager.GetButton ("Vertical")) {
			//playerPos.z += CrossPlatformInputManager.GetAxis ("Vertical");
			vz = CrossPlatformInputManager.GetAxis ("Vertical");
		}  else {
			vz = 0;
		}
		if (CrossPlatformInputManager.GetButtonDown ("Jump")) {
			//playerPos.z += CrossPlatformInputManager.GetAxis ("Jump");
			vy = CrossPlatformInputManager.GetAxisRaw ("Jump")*jumpSpeed;
			rigidBody.velocity += new Vector3 (0, vy, 0);
		}  else {
			vy = 0;
		}
		if (rigidBody.velocity.magnitude < speed) {
			rigidBody.velocity += new Vector3 (vx, 0 , vz).normalized;
			BroadcastMessage ("RotateToForward", new Vector3 (vx, 0, vz));
		}

		// disable any other cameras other than the player's.
		if (GameObject.FindObjectsOfType<Camera> ().Length>1) {
			playerCamera = GetComponentInChildren<Camera> ();
			Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
			foreach (var camera in cameras) {
				camera.gameObject.SetActive(false);
			}
			playerCamera.gameObject.SetActive (true);
		}

		// Destroy UI if player is not local.
		if (GameObject.FindObjectsOfType<Player>().Length >1) {
			//Debug.Log ("Too many player, start plean up.");
			Player[] players = GameObject.FindObjectsOfType<Player> ();
			foreach (Player item in players) {
				if (!item.isLocalPlayer) {
					//Debug.Log (item.name + ", is not local, destroy UI");
					item.DestroyUnusedUI ();
					//item.enabled = false;

				}
			}
		}

		//CheckClientHierarchy ();

		if (isDebugMode) {
			uiPlayer.DebugTest (name);
		}
	}

	public void DestroyUnusedUI (){
		if (uiPlayer) {
			uiPlayer.SelfDestruct ();
		}
	}
		
	void ReceiveTreasureCarryChange (float treasureCarryInStash){
		treasureCarried = treasureCarryInStash;
		UpdateUITreasure ();
	}

	[Client]
	public void ReceiveBaseTreasureStorageChange (float treasureStorageInBase){
		treasureStoraged = treasureStorageInBase;
		UpdateUITreasure ();
		if (treasureStoraged >= treasureToWin && !isGameEnd) {
			CmdOnGameSettle (gameObject);
		}
	}

	[Command]
	public void CmdOnGameSettle (GameObject playerWinner){
		//if (!isServer) {return;}
		// Game settled , set all player isGameEnd to true.
		Player[] players = GameObject.FindObjectsOfType<Player> ();
		foreach (var item in players) {
			item.isGameEnd = true;
		}
		RpcOnGameSettle (playerWinner);
	}

	[ClientRpc]
	void RpcOnGameSettle (GameObject playerWinner ){
		//isGameEnd = true;
		//isGameEnd = true;
		gameManager.OnPlayerGameSettle (playerWinner);
	}

	[Client]
	void UpdateUITreasure(){
		if (uiPlayer) {
			uiPlayer.UIUpdateTreasure (treasureStoraged, treasureCarried);
		}
	}

	[Client]
	void UpdatePlayerHealth(float playerHealthPercentage){
		if (uiPlayer) {
			uiPlayer.UIUpdatePlayerHealth (playerHealthPercentage);
		}

	}

	public bool IsLocalPlayer (){
		return isLocalPlayer;
	}


//	[Server]
//	void GameSettle () {
//		if (!isServer) {return;}
//		gameManager.OnPlayerGameSettle (name);
//		Debug.Log (name + ", OnGameSettle");
//	}
	//	void PlayerSpawn(){
	//		Debug.Log (name + ", PlayerSpawn called");
	//		playerSpawnMaster = GameObject.FindObjectOfType<PlayerSpawnMaster> ().gameObject;
	//		playerSpawnPoints = playerSpawnMaster.transform.GetComponentsInChildren<PlayerSpawnPoint> ();
	//		for (int i = 0; i < playerSpawnPoints.Length; i++) {
	//			print (playerSpawnPoints [i].name + ", child count is " + playerSpawnPoints [i].transform.childCount);
	//			if (playerSpawnPoints [i].transform.childCount == 0) {
	//				gameObject.transform.SetParent (playerSpawnPoints [i].transform);
	//				gameObject.transform.localPosition = Vector3.zero;
	//
	//				//gameObject.transform.position = playerSpawnPoints [i].transform.position;
	//				gameObject.name = "Player" + (i + 1);
	//				Debug.Log (name + ", Set parent to , " + playerSpawnPoints [i].name);
	//				break;
	//			}
	//		}
	//	}

	//	void OnTriggerStay(Collider obj){
	//		if (isLocalPlayer) {
	//			//Debug.Log ("something trigger, " + obj.name);
	//			GameObject target = obj.gameObject;
	//			if (target.tag == "Player") {
	//				Debug.Log ("Target in sight, fire !!");
	//			}
	//		}
	//	}

	//	public void OnNewPlayerStart (){
	//		playerCamera = GetComponentInChildren<Camera> ();
	//		Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
	//		foreach (var camera in cameras) {
	//			camera.gameObject.SetActive(false);
	//		}
	//		playerCamera.gameObject.SetActive (true);
	//	}

	//	public override void OnStartClient (){
	//		playerCamera = GetComponentInChildren<Camera> ();
	//		Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
	//		foreach (var camera in cameras) {
	//			print (camera.name);
	//			camera.gameObject.SetActive(false);
	//		}
	//
	//		playerCamera.gameObject.SetActive (true);
	//	}


}

