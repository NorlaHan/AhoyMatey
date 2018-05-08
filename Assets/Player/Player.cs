using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public GameObject playerSpawnPoint;
	public SpawnPointIndicator[] playerSpawnPoints;

	public Vector3 playerPos, playerRot;
	public float vx , vz, vy, speed = 5f, jumpSpeed = 6f;
	Rigidbody rigidBody;
	Camera playerCamera;
	AudioListener audioListener;


	// Use this for initialization
	void Start () {
		rigidBody = GetComponentInChildren<Rigidbody> ();

		playerCamera = GetComponentInChildren<Camera> ();
		audioListener = GetComponentInChildren<AudioListener> ();

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
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}
		if (CrossPlatformInputManager.GetButton ("Horizontal")) {
			playerPos.x += CrossPlatformInputManager.GetAxis ("Horizontal");
			vx = CrossPlatformInputManager.GetAxis ("Horizontal");
		} else {
			vx = 0;
		}
		if (CrossPlatformInputManager.GetButton ("Vertical")) {
			playerPos.z += CrossPlatformInputManager.GetAxis ("Vertical");
			vz = CrossPlatformInputManager.GetAxis ("Vertical");
		} else {
			vz = 0;
		}
		if (CrossPlatformInputManager.GetButtonDown ("Jump")) {
			playerPos.z += CrossPlatformInputManager.GetAxis ("Jump");
			vy = CrossPlatformInputManager.GetAxisRaw ("Jump")*jumpSpeed;
			rigidBody.velocity += new Vector3 (0, vy, 0);
		} else {
			vy = 0;
		}
		if (rigidBody.velocity.magnitude < speed) {
			rigidBody.velocity += new Vector3 (vx, 0 , vz).normalized;
			BroadcastMessage ("RotateToForward", new Vector3 (vx, 0, vz));
		}

		if (GameObject.FindObjectsOfType<Camera> ().Length>1) {
			playerCamera = GetComponentInChildren<Camera> ();
			Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
			foreach (var camera in cameras) {
				camera.gameObject.SetActive(false);
			}
			playerCamera.gameObject.SetActive (true);
		}
			

		//transform.SetPositionAndRotation(playerPos, Quaternion.Euler(playerRot));
	}

	[Command]
	public override void OnStartLocalPlayer (){
		playerCamera = GetComponentInChildren<Camera> ();
		Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
		foreach (var camera in cameras) {
			camera.gameObject.SetActive(false);
			//camera.SendMessageUpwards ("OnNewPlayerStart");
		}
		playerCamera.gameObject.SetActive (true);

		//Debug.Log ("Test point pass");

		playerSpawnPoint = GameObject.FindObjectOfType<PlayerSpawnPoint> ().gameObject;
		playerSpawnPoints = playerSpawnPoint.transform.GetComponentsInChildren<SpawnPointIndicator> ();

		for (int i = 0; i < playerSpawnPoints.Length ; i++) {
			print (playerSpawnPoints[i].name + ", child count is " +playerSpawnPoints[i].transform.childCount);
			if (playerSpawnPoints[i].transform.childCount == 0) {
				gameObject.transform.SetParent(playerSpawnPoints[i].transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.name = "Player" + (i+1);
				Debug.Log (name + ", Set parent to , " + playerSpawnPoints [i].name);
				break;
			}
		}
		RpcSpawn ();

//		GetComponentInChildren<Camera> ().enabled = true;
//		GetComponentInChildren<AudioListener>().enabled = true;
	}

	[ClientRpc]
	void RpcSpawn (){
		if (NetworkServer.active) return;
		Debug.Log ("Not a server, RPC");
		playerSpawnPoint = GameObject.FindObjectOfType<PlayerSpawnPoint> ().gameObject;
		playerSpawnPoints = playerSpawnPoint.transform.GetComponentsInChildren<SpawnPointIndicator> ();

		for (int i = 0; i < playerSpawnPoints.Length ; i++) {
			print (playerSpawnPoints[i].name + ", child count is " +playerSpawnPoints[i].transform.childCount);
			if (playerSpawnPoints[i].transform.childCount == 0) {
				gameObject.transform.SetParent(playerSpawnPoints[i].transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.name = "Player" + (i+1);
				Debug.Log (name + ", Set parent to , " + playerSpawnPoints [i].name);
				break;
			}
		}
	}

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
