using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpawnMaster : NetworkBehaviour {
	
	//public PlayerSpawnPoint[] playerSpawnPoints;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

//	public override void OnStartLocalPlayer (){
//		Debug.Log ("OnStartLocalPlayer override by " + name);
//		playerSpawnPoints = GetComponentsInChildren<PlayerSpawnPoint> ();
//		for (int i = 0; i < playerSpawnPoints.Length; i++) {
//			print (playerSpawnPoints [i].name + ", child count is " + playerSpawnPoints [i].transform.childCount);
//			if (playerSpawnPoints [i].transform.childCount == 0) {
//				gameObject.transform.SetParent (playerSpawnPoints [i].transform);
//				gameObject.transform.localPosition = Vector3.zero;
//				//gameObject.transform.position = playerSpawnPoints [i].transform.position;
//				gameObject.name = "Player" + (i + 1);
//				Debug.Log (name + ", Set parent to , " + playerSpawnPoints [i].name);
//				break;
//			}
//		}
//	}
}
