using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public Vector3 playerPos, playerRot;
	public float vx , vz, vy, speed = 5f, jumpSpeed = 6f;
	Rigidbody rigidBody;
	Camera playerCamera;


	// Use this for initialization
	void Start () {
		rigidBody = GetComponentInChildren<Rigidbody> ();
		playerPos = transform.position;
		playerRot = transform.eulerAngles;
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

	public override void OnStartLocalPlayer (){
		playerCamera = GetComponentInChildren<Camera> ();
		Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
		foreach (var camera in cameras) {
			camera.gameObject.SetActive(false);
			//camera.SendMessageUpwards ("OnNewPlayerStart");
		}
		playerCamera.gameObject.SetActive (true);
	}

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
