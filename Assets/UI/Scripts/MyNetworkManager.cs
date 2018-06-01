using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class MyNetworkManager : NetworkManager {

	public int count = 0;
	public NetworkManagerHUD NMHud;

	// Use this for initialization
	void Start () {
		NMHud = GetComponent<NetworkManagerHUD> ();
	}

	void Update(){
//		if (IsClientConnected()) {
//			
//		}
	}
		
	public void MyStartHost(){
		Debug.Log (Time.timeSinceLevelLoad + ", Starting Host...");
		StartHost ();
	}

	public override void OnStartHost (){
		Debug.Log (Time.timeSinceLevelLoad + ", Host started. Waiting for client to connect.");
		// TODO Find out whether the tutrorial have this.
		NetworkServer.SpawnObjects ();
//		if (!IsClientConnected()) {
//			InvokeRepeating ("CountDots", 1, 1);
//		}

	}

	public override void OnStartClient (NetworkClient myClient){
		Debug.Log (Time.timeSinceLevelLoad + ", Client started.");
		if (count != 0) {
			count = 0;
		}
		InvokeRepeating ("CountDots", 1, 1);
	}

	public override void OnClientConnect (NetworkConnection myConnection){
		Debug.Log (Time.timeSinceLevelLoad + ", client is connected to IP : " + myConnection.address);
		CancelInvoke ();
	}

	void CountDots (){
		count += 1;
		Debug.Log (count + "...");
	}
}
