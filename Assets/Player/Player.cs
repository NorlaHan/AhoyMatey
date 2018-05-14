﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public bool isDebugMode = false;

	public PlayerSpawnPoint[] playerSpawnPoints;
	public GameObject playerUIPrefab ;
	public PlayerBase playerBase;
	public GameManager gameManager;
//	[SyncVar]
//	public string playerName , playerParentName;

	public float treasureCarried, treasureStoraged , treasureToWin = 2000f;

	public Vector3 playerPos, playerRot;
	public float vx , vz, vy, speed = 5f, jumpSpeed = 6f; 


	private Rigidbody rigidBody;
	private Camera playerCamera;
	private AudioListener audioListener;
	private UIPlayer uiPlayer;


	#region StartLocalPlayer
	public override void OnStartLocalPlayer (){
		playerCamera = GetComponentInChildren<Camera> ();
		Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
		foreach (var camera in cameras) {
			camera.gameObject.SetActive(false);
			//camera.SendMessageUpwards ("OnNewPlayerStart");
		}
		playerCamera.gameObject.SetActive (true);
		//playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint> ();

		CmdPlayerSpawn ();
		//RpcPlayerSpawn ();
		//PlayerSpawn ();
	}

	// check which spawn point to spawn, then pass the spawn i to client.


	[Command]
	void CmdPlayerSpawn () {
		bool isFull = true;
		Debug.Log (name + ", Command called");

		playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint> ();

		for (int i = 0; i < playerSpawnPoints.Length; i++) {
			Debug.Log ("Checking " + playerSpawnPoints [i].name + "...");
			if (!playerSpawnPoints [i].GetComponentInChildren<Player>()) {
				Debug.Log (playerSpawnPoints [i].name + " have no player.");

				// Same as Rpc later on.
				gameObject.transform.SetParent (playerSpawnPoints [i].transform);
				//gameObject.transform.localPosition = Vector3.zero;
				gameObject.name = "Player" + (i + 1);
				Debug.Log ("Spawn " + name + " to " + playerSpawnPoints [i].name);

				// Cast self to the UI on the client.
				playerBase = transform.parent.GetComponentInChildren<PlayerBase> ();
				playerBase.BaseLinkToPlayer (gameObject);
				//

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
		Debug.Log (name + ", ClientRpc called");

		playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint> ();
		gameObject.transform.SetParent (playerSpawnPoints [i].transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.name = "Player" + (i + 1);
		Debug.Log ("Spawn " + name + " to " + playerSpawnPoints [i].name);

		// Cast self to the UI on the client.

		playerBase = transform.parent.GetComponentInChildren<PlayerBase> ();
		playerBase.BaseLinkToPlayer (gameObject);
	}



	#endregion


	// Use this for initialization
	void Start () {
		rigidBody = GetComponentInChildren<Rigidbody> ();

		playerCamera = GetComponentInChildren<Camera> ();
		audioListener = GetComponentInChildren<AudioListener> ();

		InstantiatePlayerUI ();

		gameManager = GameObject.FindObjectOfType<GameManager> ();

		//uiTreasure = GameObject.FindObjectOfType<UIPlayer> ();

		//		playerSpawnPoint = GameObject.FindObjectOfType<PlayerSpawnPoint> ().gameObject;
		//		playerSpawnPoints = playerSpawnPoint.transform.GetComponentsInChildren<SpawnPointIndicator> ();
		//		for (int i = 0; i < playerSpawnPoints.Length; i++) {
		//			print (playerSpawnPoints [i]);
		//		}

		//		playerCamera.enabled = false;
		//		audioListener.enabled = false;

		//		playerPos = transform.position;
		//		playerRot = transform.eulerAngles;
	}
	[Client]
	void InstantiatePlayerUI ()
	{
		uiPlayer = Instantiate (playerUIPrefab).GetComponentInChildren<UIPlayer> ();
		uiPlayer.LinkUIToPlayer (this, playerBase);
	}

	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}
		if (CrossPlatformInputManager.GetButton ("Horizontal")) {
			playerPos.x += CrossPlatformInputManager.GetAxis ("Horizontal");
			vx = CrossPlatformInputManager.GetAxis ("Horizontal");
		}  else {
			vx = 0;
		}
		if (CrossPlatformInputManager.GetButton ("Vertical")) {
			playerPos.z += CrossPlatformInputManager.GetAxis ("Vertical");
			vz = CrossPlatformInputManager.GetAxis ("Vertical");
		}  else {
			vz = 0;
		}
		if (CrossPlatformInputManager.GetButtonDown ("Jump")) {
			playerPos.z += CrossPlatformInputManager.GetAxis ("Jump");
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

	// Check if it's parent and name is same as the server.
//	[Client]
//	void CheckClientHierarchy (){
//		if (!transform.parent) {
//			Debug.Log (name + ", have no parent. command server");
//			CmdGetServerParent ();
//		}
//
//	}
//
//	[Command]
//	void CmdGetServerParent (){
//		Debug.Log ("CmdGetServerParent ()");
//		RpcSetClientParent (playerBase.name);
//		Debug.Log (name + ", passing playerbase name :" + playerBase.name);
//	}
//
//	[ClientRpc]
//	void RpcSetClientParent (string serverPlayerBaseName){
//		Debug.Log ("RpcSetClientParent ()");
//		Debug.Log (serverPlayerBaseName);
//		transform.SetParent (GameObject.Find ("serverPlayerBaseName").transform.parent);
//		playerBase = GameObject.Find("serverPlayerBaseName").GetComponent<PlayerBase>();
//		transform.SetParent (playerBase.transform.parent);
//		playerBase.BaseLinkToPlayer (gameObject);
//	}


	public void DestroyUnusedUI (){
		if (uiPlayer) {
			uiPlayer.SelfDestruct ();
		}
	}

	// TODO sync player name and parent on other client.
	//	#region OnStartClient
	//	public override void OnStartClient ()
	//	{
	//		base.OnStartClient ();
	//		Player[] players = GameObject.FindObjectsOfType<Player> ();
	//		foreach (var item in players) {
	//			Debug.Log (name + ", OnStartClient");
	//			if (!isLocalPlayer) {
	//				CmdSyncPlayer ();
	//				Debug.Log (name + ", RpcSyncPlayer");
	//			}
	//		}
	//	}
	//
	//	[Command]
	//	void CmdSyncPlayer(){
	//		Transform playerParent = transform.parent;
	//		string playerName = name;
	//		OnSyncPlayer ( playerParent , playerName);
	//		Debug.Log ("playerParent :"+playerParent.name + ", playerName : " + playerName);
	//	}
	//
	//
	//	[Client]
	//	void OnSyncPlayer (Transform playerParent, string playerName){
	//		name = playerName;
	//		transform.SetParent (playerParent);
	//	}
	//	#endregion


	void ReceiveTreasureCarryChange (float treasureCarryInStash){
		treasureCarried = treasureCarryInStash;
		UpdateUITreasure ();
	}

	[Client]
	public void ReceiveBaseTreasureStorageChange (float treasureStorageInBase){
		treasureStoraged = treasureStorageInBase;
		UpdateUITreasure ();
		if (treasureStoraged >= treasureToWin) {
			OnGameSettle ();
		}
	}

	[Client]
	void UpdateUITreasure(){
		//if (!uiTreasure) {uiTreasure = GameObject.FindObjectOfType<UIPlayer> ();}
		if (uiPlayer) {
			uiPlayer.UIUpdateTreasure (treasureStoraged, treasureCarried);
		}
	}

	[Client]
	void UpdatePlayerHealth(float playerHealthPercentage){
		//if (!uiTreasure) {uiTreasure = GameObject.FindObjectOfType<UIPlayer> ();}
		if (uiPlayer) {
			uiPlayer.UIUpdatePlayerHealth (playerHealthPercentage);
		}

	}

	[Server]
	public void OnGameSettle (){
		if (!isServer) {return;}
		GameSettle ();
	}

	[Server]
	void GameSettle () {
		if (!isServer) {return;}
		gameManager.OnPlayerGameSettle (name);
		Debug.Log (name + ", OnGameSettle");
	}
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

