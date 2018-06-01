using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUpSpawnMaster : NetworkBehaviour {
	
	public GameObject[] PowerUpPrefabs;

	// Debug Purpose.
	[Tooltip("Activate isDebugMode to Update.")]
	public int showPowerUpCount;

	public bool isDebugMode = false;

	public float RespawnRatio = 1f;

	private PowerUpSpawnPoint[] powerUpSpawnPoints;

	private int rollSpawnStart;
	// Use this for initialization
	void Start () {
		//if (!isServer) {return;}
		powerUpSpawnPoints = GameObject.FindObjectsOfType<PowerUpSpawnPoint>(); 

	}


	public override void OnStartClient ()
	{
		base.OnStartClient ();

		if (isServer) {return;}
		OnCheckSpawnedPowerUp ();
	}

	[Client]
	void OnCheckSpawnedPowerUp (){
		Debug.Log ("OnCheckSpawnedPowerUp");
		powerUpSpawnPoints = GameObject.FindObjectsOfType<PowerUpSpawnPoint>();

		OnCheckSpawnedPowerUp2 ();

	}

	//[Command]
	void OnCheckSpawnedPowerUp2 (/* int i */){
		//		Debug.Log ("ServerCheckSpawnedTreasure (int i), i =" + i);

		//if (!isServer) {return;}
		Debug.Log ("Finding spawnpoints which has no child.");
		for (int i = 0; i < powerUpSpawnPoints.Length; i++) {
			if (powerUpSpawnPoints [i].transform.childCount != 0) {
				GameObject powerUp = powerUpSpawnPoints [i].transform.GetChild (0).gameObject;
				RpcCheckSpawnedPowerUp (i, powerUp);
			} else {
				Debug.Log ("PowerUp spawn point has no child");
			}
		}
	}

	[ClientRpc]
	void RpcCheckSpawnedPowerUp (int i ,GameObject powerUp){
		if (powerUpSpawnPoints [i].transform.childCount == 0) {
			powerUp.transform.SetParent(powerUpSpawnPoints [i].transform);
			Debug.Log ("Set power up to spawn point on client");
		}
	}

	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			showPowerUpCount = PowerUp.powerUpCount;
			Debug.Log ("Repair.repairCount is " + PowerUp.powerUpCount);
			isDebugMode = false;
		}
		if (!isServer) {return;}
		RpcSpawnPowerUp ();
	}

	// Decided to spawn repair or not.
	[Server]
	void RpcSpawnPowerUp (){
		//Debug.Log ("Call Spawn");
		// make respawn not always start at 0.
		rollSpawnStart = Random.Range (0, powerUpSpawnPoints.Length);
		if (PowerUp.powerUpCount < powerUpSpawnPoints.Length*RespawnRatio) {
			//Debug.Log ("PowerUp.powerUpCount < repairSpawnPoints.Length*RespawnRatio");
			for (int i = rollSpawnStart ; i < (powerUpSpawnPoints.Length*RespawnRatio+rollSpawnStart); i++) {
				int ii = (i % powerUpSpawnPoints.Length);
				int j = 0,k;
				if (powerUpSpawnPoints [ii].transform.childCount == 0  &&  powerUpSpawnPoints [ii].canSpawn) {
					//Debug.Log ("i = " + ii + "Spawn repair");
					// Spawn on server.
					// [Error] Rpc Call on client
					k = Random.Range(0,PowerUpPrefabs.Length+4);
					if (k == 0 || k == 1 || k == 6) {
						j = 0;
					}else if (k == 2 || k == 3 || k == 7) {
						j = 1;
					}else if (k == 4) {
						j = 2;
					}else if (k == 5) {
						j = 3;
					}
					GameObject powerUp = Instantiate(PowerUpPrefabs[j], powerUpSpawnPoints[ii].transform.position, powerUpSpawnPoints[ii].transform.rotation);
					Debug.Log ("Rolled " + k + ", index = "+ j + ", Spawn " + powerUp);
					if (isServer) {
						NetworkServer.Spawn(powerUp);
						powerUp.transform.SetParent (powerUpSpawnPoints [ii].transform);
					}
						
					RpcSpawnPowerUp2 (ii, powerUp);


				}
			}
		}
	}
		

	[ClientRpc]
	void RpcSpawnPowerUp2 (int i, GameObject powerUp){
		powerUp.transform.SetParent (powerUpSpawnPoints [i].transform);
	}

}
