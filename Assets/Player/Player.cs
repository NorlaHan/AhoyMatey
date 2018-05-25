using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	[SyncVar]
	public bool isDebugMode = false , isGameEnd = false;

	[SyncVar]
	public bool isDead = false;

	[SyncVar]
	public float armor = 0f ,speed = 10f;

	public GameObject playerUIPrefab , playerFoam; 
	public PlayerAttack[] playerAttacks;
	public PlayerTreasureStash playerStash;
	public GameManager gameManager;

	[SyncVar]
	public GameObject playerBase;

	[SyncVar]
	public string playerName, parentName, playerBaseName, weaponName;

	public enum WeaponType {CannonOG, ScatterGun};

	//[SyncVar]
	public WeaponType weaponType;

	public float treasureCarried, treasureStoraged , treasureToWin = 2000f;

	// public Vector3 playerPos, playerRot;
	public float vx , vz, vy,  jumpSpeed = 6f; 

	private PlayerSpawnPoint[] playerSpawnPoints;
	private Rigidbody rigidBody;
	private Camera playerCamera;
	private AudioListener audioListener;
	private UIPlayer uiPlayer;
	private Animator animator;
	private bool checkParent = false, checkName = false, checkBase = false, checking = true;


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
		TryToLinkPlayerToBase (i);
	}

	void TryToLinkPlayerToBase(int i){
		if (transform.parent.GetComponentInChildren<PlayerBase> ()) {
			playerBase = transform.parent.GetComponentInChildren<PlayerBase> ().gameObject;
			playerBase.name = "Player" + (i + 1) + "Base";
			playerBaseName = playerBase.name;
			// Cast self to the UI on the client.
			playerBase.GetComponentInChildren<PlayerBase> ().BaseLinkToPlayer (gameObject);
		} else {
			Invoke ("CmdPlayerSpawn", 0.1f);
		}
	}

	#endregion


	// Use this for initialization
	void Start () {
		
		// Get component in children
		if (GetComponentInChildren<Rigidbody> ()) {
			rigidBody = GetComponentInChildren<Rigidbody> ();
		}else {Debug.Log (name + ", missing Rigidbody");}
		if (GetComponentInChildren<Camera> ()) {
			playerCamera = GetComponentInChildren<Camera> ();
		}else {Debug.Log (name + ", missing Camera");}
		if (GetComponentInChildren<AudioListener> ()) {
			audioListener = GetComponentInChildren<AudioListener> ();
		}else {Debug.Log (name + ", missing AudioListener");}
		if (GetComponentInChildren<Animator> ()) {
			animator = GetComponent<Animator> ();
			//NetworkAnimator
		} else {Debug.Log (name + ", missing Animator");}
//		if (GetComponentInChildren<PlayerFoam> ().gameObject) {
//			playerFoam = GetComponentInChildren<PlayerFoam> ().gameObject;
//		}else {Debug.Log (name + ", missing PlayerFoam");}
		if (GetComponent<PlayerTreasureStash> ()) {
			playerStash = GetComponent<PlayerTreasureStash> ();
		} else {Debug.Log (name + ", missing PlayerTreasureStash");}


		// Find GameObject
		if (GameObject.FindObjectOfType<GameManager> ()) {
			gameManager = GameObject.FindObjectOfType<GameManager> ();
		} else {Debug.Log (name + ", missing GameManager");}

		if (isLocalPlayer) {
			Invoke ("InstantiatePlayerUI" , 0.2f);
		}
		//InstantiatePlayerUI ();
	}

	#region Spawn UI locally
	[Client]
	void InstantiatePlayerUI ()
	{
		uiPlayer = Instantiate (playerUIPrefab).GetComponentInChildren<UIPlayer> ();
		TryToLinkPlayerToUI ();
	}

	void TryToLinkPlayerToUI(){
		if (playerBase) {
			uiPlayer.LinkUIToPlayer (this, playerBase.GetComponent<PlayerBase> ());
		} else {
			Debug.Log (name + "Can't find playerBase");
			if (transform.parent && transform.parent.GetComponentInChildren<PlayerBase> ()) {
				playerBase = transform.parent.GetComponentInChildren<PlayerBase> ().gameObject;
			}
		Invoke ("TryToLinkPlayerToUI", 0.1f);
		}
	}
	#endregion

	// Update is called once per frame

	public override void OnStartClient ()
	{
		base.OnStartClient ();
		CheckOnStartClient ();
	}

	void CheckOnStartClient () {
		if (!isLocalPlayer && checking) {
			try {
				if (!checkParent && !transform.parent && !isServer) {
					transform.SetParent (GameObject.Find (parentName).transform);
					//Debug.Log (name+", missing parent. set parent to " + parentName);
				}
				else {
					checkParent = true;
				}
				if (!checkName && name != playerName) {
					name = playerName;
				}
				else {
					checkName = true;
				}
				if (!checkBase && playerBase.name != playerBaseName) {
					playerBase.name = playerBaseName;
				}
				else {
					checkBase = true;
				}
				if (checkBase && checkName && checkParent) {
					Debug.Log(name + ", Check complete!");
					checking = false;
				} else {
					Debug.Log(name + ", checkBase = " +checkBase + ", checkName = "+checkName+", checkParent = "+checkParent);
					Invoke("CheckOnStartClient",0.1f);
				} 
			}
			catch {
				Debug.Log ("Something happen when new client join the game.");
			}
		}
	}

	// This action has to be done on every player 
	void ChangeWeaponType (){
		if (weaponName == "CannonOG") {
			foreach (PlayerAttack item in playerAttacks) {
				item.type = PlayerAttack.WeaponType.CannonOG;
				item.OnWeaponChange ();
			}
		}else if (weaponName == "ScatterGun") {
			foreach (PlayerAttack item in playerAttacks) {
				item.type = PlayerAttack.WeaponType.ScatterGun;
				item.OnWeaponChange ();
			}
		}
	}

	// This only need to happen locally
	[Command]
	void CmdSetWeaponType ()
	{
		if (weaponType == WeaponType.CannonOG) {
			weaponName = weaponType.ToString ();
		}
		else
			if (weaponType == WeaponType.ScatterGun) {
				weaponName = weaponType.ToString ();
			}
	}

	void Update () {
		// Cut off control when game ends.
		if (isGameEnd || isDead) {
			return;
		}
		// Not local player will stop here.
		CheckOnStartClient ();

		ChangeWeaponType ();


		if (!isLocalPlayer) {
			return;
		}

		CmdSetWeaponType ();
		UpdatePlayerWeaponType ();

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

//		if (rigidBody.velocity != Vector3.zero) {
//			animator.SetBool ("isMoving", true);
//			Debug.Log (name + " is Moving");
//		} else {
//			animator.SetBool ("isMoving", false);
//			Debug.Log (name + " Stop");
//		}

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




	#region UI related region
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
		// Game settled , set all player isGameEnd to true.
		Player[] players = GameObject.FindObjectsOfType<Player> ();
		foreach (var item in players) {
			item.isGameEnd = true;
		}
		RpcOnGameSettle (playerWinner);
	}

	[ClientRpc]
	void RpcOnGameSettle (GameObject playerWinner ){
		gameManager.OnPlayerGameSettle (playerWinner);
	}

	[Client]
	void UpdateUITreasure(){
		if (uiPlayer) {
			uiPlayer.UIUpdateTreasure (treasureStoraged, treasureCarried);
		}
	}

	[Client]
	public void UpdatePlayerHealth(float playerHealthPercentage){
		if (uiPlayer) {
			uiPlayer.UIUpdatePlayerHealth (playerHealthPercentage);
			//Debug.Log ("UpdatePlayerHealth = " + playerHealthPercentage);
		}
	}

	[Client]
	void UpdatePlayerWeaponType (){
		if (uiPlayer) {
			if (weaponName == "CannonOG") {
				uiPlayer.UIUpdatePlayerWeapon ("CannonOG");
			}else if (weaponName == "ScatterGun") {
				uiPlayer.UIUpdatePlayerWeapon ("ScatterGun");
			}
		}
	}
	#endregion

	[ClientRpc]
	void RpcOnUnitDeath (){
		if (hasAuthority) {
			CmdIsPlayerDead (true);
			playerFoam.SetActive (false);
			if (!animator) {animator = GetComponent<Animator> ();}
			animator.SetBool ("isDead", true);
			Debug.Log ("Player is dead.");
		}
	}
		
	[ClientRpc]
	public void RpcOnUnitRespawn (){
		if (hasAuthority) {
			Vector3 position = transform.position;
			transform.localPosition = Vector3.zero;
			if (!animator) {animator = GetComponent<Animator> ();}
			animator.SetBool ("isDead", false);
			playerFoam.SetActive (true);
			CmdIsPlayerDead (false);
			SendMessage ("OnRespawnHealth");
			playerStash.CmdSpawnTreasureLoot (position);
			// Reset player state
			speed = 10;
			armor = 0;
			weaponType = WeaponType.CannonOG;
			CmdSetWeaponType ();
		}
	}

	[Command]
	void CmdIsPlayerDead (bool dead){
		isDead = dead;
	}

	public bool IsLocalPlayer (){
		return isLocalPlayer;
	}

	[Command]
	void CmdOnGetPowerUp (string type, float parameter){
		if (type == "Armor") {
			armor = Mathf.Clamp ((armor + parameter),0,20);
		}else if (type == "Speed") {
			speed = Mathf.Clamp ((speed + parameter), 5, 40);
		}
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

