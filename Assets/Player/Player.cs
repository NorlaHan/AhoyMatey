using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	[SyncVar]
	public bool isDebugMode = false , isGameEnd = false /*, isChangingWeapon = false*/;

	[SyncVar]
	public bool isDead = false, onSea , onLand;

	[SyncVar]
	public float armor = 0f , landCount = 0;

	[SyncVar]
	public float speed = 10f;

	public GameObject playerUIPrefab , playerFoam; 
	public PlayerAttack[] playerAttacks;
	public PlayerTreasureStash playerStash;
	public GameManager gameManager;

	[SyncVar]
	public GameObject playerBase;

	[SyncVar]
	public string playerName, parentName, playerBaseName, weaponName;

	public enum WeaponType {CannonOG, ScatterGun, SuperCannon};

	//[SyncVar]
	public WeaponType weaponType;

	public float treasureCarried, treasureStoraged;

	[Tooltip("Rule Setting.")]
	public float treasureToWin = 2000f ,MaxSpeed = 20, MaxArmor = 8;

	// public Vector3 playerPos, playerRot;
	public float vx , vz, vy, rc = 0f, jumpSpeed = 6f; 
	public Transform cameraRoot;

	private Vector3 camForward, move;
	private PlayerSpawnPoint[] playerSpawnPoints;
	private Rigidbody rigidBody;
	private Camera playerCamera;
	//private AudioListener audioListener;
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
			weaponName = "CannonOG";
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
//		if (GetComponentInChildren<AudioListener> ()) {
//			audioListener = GetComponentInChildren<AudioListener> ();
//		}else {Debug.Log (name + ", missing AudioListener");}
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
			Invoke ("InitializeUI", 0.5f);
		} else {
			Debug.Log (name + "Can't find playerBase");
			if (transform.parent && transform.parent.GetComponentInChildren<PlayerBase> ()) {
				playerBase = transform.parent.GetComponentInChildren<PlayerBase> ().gameObject;
			}
		Invoke ("TryToLinkPlayerToUI", 0.1f);
		}
	}

	void InitializeUI (){
		UpdatePlayerWeaponType ();
		uiPlayer.UIUpdatePlayerPowerUp ("Speed", speed);
		uiPlayer.UIUpdatePlayerPowerUp ("Armor", armor);
		//isChangingWeapon = true;
	}

	#endregion

	// Update is called once per frame

	public override void OnStartClient ()
	{
		base.OnStartClient ();
		CheckOnStartClient ();
	}

	void CheckOnStartClient () {
		if (!isLocalPlayer && checking && !isDebugMode) {
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

	// Pick up power ups. SyncVar requires server change to sync client.
	[Command]
	public void CmdOnGetPowerUp (string type, float parameter){
		Debug.Log ("Consume " + type + ", "+parameter);
		if (type == "Armor") {
			armor = Mathf.Clamp ((armor + parameter),0, MaxArmor);
			RpcUpdatePlayerPowerUp (type, armor);
		}else if (type == "Speed") {
			speed = Mathf.Clamp ((speed + parameter), 5, MaxSpeed);
			RpcUpdatePlayerPowerUp (type, speed);
		}else if (type == "WeaponScatter") {
			weaponName = "ScatterGun";
			//isChangingWeapon = true;
			//Debug.Log ("isChangingWeapon = true;");
//			weaponType = WeaponType.ScatterGun; 
//			CmdSetWeaponType ();
		}else if (type == "WeaponSuper") {
			weaponName = "SuperCannon";
			//isChangingWeapon = true;
			//Debug.Log ("isChangingWeapon = true;");
//			weaponType = WeaponType.SuperCannon;
//			CmdSetWeaponType ();
		}
	}

//	[Command]
//	void CmdChangingWeapon(bool isChanging){
//		isChangingWeapon = isChanging;
//	}

	// This only need to happen locally
	//[Command]
	void SetWeaponType ()
	{
		if (weaponType == WeaponType.CannonOG) {
			weaponName = weaponType.ToString ();
		}else if (weaponType == WeaponType.ScatterGun) {
			weaponName = weaponType.ToString ();
		}else if (weaponType == WeaponType.SuperCannon) {
			weaponName = weaponType.ToString ();
		}
		if (isServer) {
			RpcSetWeaponType (weaponName);
		}
	//ChangeWeaponType ();
	}

	[ClientRpc]
	void RpcSetWeaponType (string weapon){
		if (!hasAuthority) {
			weaponName = weapon;
			ChangeWeaponType ();
		}
	}


	// This action has to be done on every player 
	void ChangeWeaponType (){
		//if (weaponName == "CannonOG") {
			if (weaponName == playerAttacks[0].weaponName) {
				return;
			}
			foreach (PlayerAttack item in playerAttacks) {
				//item.type = PlayerAttack.WeaponType.CannonOG;
				item.weaponName = weaponName;
				item.OnWeaponChange ();
			}
//		}else if (weaponName == "ScatterGun") {
//			foreach (PlayerAttack item in playerAttacks) {
//				item.type = PlayerAttack.WeaponType.ScatterGun;
//				item.OnWeaponChange ();
//			}
//		}else if (weaponName == "SuperCannon") {
//			foreach (PlayerAttack item in playerAttacks) {
//				item.type = PlayerAttack.WeaponType.SuperCannon;
//				item.OnWeaponChange ();
//			}
//		}
	}



	void Update () {
		// Cut off control when game ends.
		if (isGameEnd || isDead) {
			return;
		}
		// Not local player will stop here.
		CheckOnStartClient ();
		if (isDebugMode) {
			SetWeaponType ();
			//isChangingWeapon = true;
		}



		if (!isLocalPlayer) {
			return;
		}


		if (isDebugMode) {
			uiPlayer.DebugTest (name);
		}

		//CmdSetWeaponType ();
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
		if (CrossPlatformInputManager.GetButton ("RotCam")) {
			rc += CrossPlatformInputManager.GetAxis ("RotCam");
			cameraRoot.localEulerAngles = new Vector3 (0, rc, 0);
		} 
//		else {
//			rc = 0;
//			cameraRoot.localEulerAngles = Vector3.zero;
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

		// Take damage on land.
		if (!onSea && onLand) {
			landCount += Time.deltaTime;
			if (landCount > 0.25) {
				landCount = 0;
				SendMessage ("OnLandDamege", 12.5f);
			}
		}
		//CheckClientHierarchy ();
	}


	void FixedUpdate(){
		if (isGameEnd || isDead) {
			return;
		}
		// TODO find a better way to change weapon type...
		ChangeWeaponType ();

		if (!isLocalPlayer) {return;}

		// Camera relative  direction to move.
		camForward = Vector3.Scale (cameraRoot.forward, new Vector3 (1, 0, 1)).normalized;
		move = (vz * camForward + vx * cameraRoot.right).normalized;

		float adjSpeed = speed/10;
		//BroadcastMessage ("RotateToForward", new Vector3 (vx, 0, vz));
		BroadcastMessage ("RotateToForward",new PlayerRotator(move, adjSpeed));

		if (CrossPlatformInputManager.GetButtonDown ("Jump") && onSea) {
			//playerPos.z += CrossPlatformInputManager.GetAxis ("Jump");
			vy = CrossPlatformInputManager.GetAxisRaw ("Jump")*jumpSpeed;
			rigidBody.velocity += new Vector3 (0, vy, 0);
		}  else {
			vy = 0;
		}
		if (rigidBody.velocity.magnitude < speed) {
			if (speed <= 10) {
				//rigidBody.velocity += new Vector3 (vx, 0, vz).normalized;
				rigidBody.velocity += move;
			}else {
			//	rigidBody.velocity += new Vector3 (vx, 0 , vz).normalized * (speed/10);
				rigidBody.velocity += move * adjSpeed;
			}
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
			}else if (weaponName == "SuperCannon") {
				uiPlayer.UIUpdatePlayerWeapon ("SuperCannon");
			}
		}
	}

	[ClientRpc]
	void RpcUpdatePlayerPowerUp (string type, float parameter){
		if (hasAuthority && uiPlayer) {
			uiPlayer.UIUpdatePlayerPowerUp (type,parameter);
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
		
	//[ClientRpc]
	public void OnUnitRespawn (){
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
			CmdOnUnitResapwn ();
		}
	}

	void CmdOnUnitResapwn ()
	{
		speed = 10;
		armor = 0;
		//			weaponType = WeaponType.CannonOG;
		weaponName = "CannonOG";
		if (uiPlayer) {
			InitializeUI ();
		}
	}


	[Command]
	void CmdIsPlayerDead (bool dead){
		isDead = dead;
	}

	public bool IsLocalPlayer (){
		return isLocalPlayer;
	}

	void OnCollisionStay (Collision obj){
		GameObject target = obj.gameObject;
		if (target.tag == "Sea") {
			onSea = true;
			landCount = 0;
		}
		if (target.tag == "Land") {
			onLand = true;
		}
	}

	void OnCollisionExit(Collision obj){
		GameObject target = obj.gameObject;
		if (target.tag == "Sea") {
			onSea = false;
		}
		if (target.tag == "Land") {
			onLand = false;
		}
	}

}

