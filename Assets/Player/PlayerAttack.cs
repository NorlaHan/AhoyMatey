using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttack : MonoBehaviour {

	public enum WeaponType {CannonOG, ScatterGun};
	public WeaponType type;

	public GameObject cannonBall, scatterGun, currentTarget;
	public float cannonOGRate = 2f, scatterGunRate = 1.8f;
	public SpawnPointIndicator[] spawnPos;

	private Transform playerProjectiles;
	private Player playerSelf;
	private float fireRate , fireCount = 0;
	private Vector3 fireVector;

	// Use this for initialization
	void Start () {
//		if (GetComponentInChildren<SpawnPointIndicator> ()) {
//			spawnPos = GetComponentInChildren<SpawnPointIndicator> ();
//		} else {Debug.LogWarning (name + ", missing SpawnPointIndicator");}
		// Use an empty Game Object to hierarchise the projectiles fired
		if (GameObject.Find ("PlayerProjectiles")) {
			playerProjectiles = GameObject.Find ("PlayerProjectiles").transform;
		}else {
			Debug.LogWarning (name + ", missing PlayerProjectiles. Auto generate.");
			playerProjectiles = new GameObject ("playerProjectiles").transform;
		}
		if (GetComponentInParent<Player>()) {
			playerSelf = GetComponentInParent<Player> ();
		}else {Debug.LogWarning (name + ", missing Self Player");}

		OnWeaponChange ();
	
	}

	public void OnWeaponChange (){
		if (type == WeaponType.CannonOG) {
			fireRate = cannonOGRate;
		}else if (type == WeaponType.ScatterGun) {
				fireRate = scatterGunRate;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (fireCount < fireRate) {
			fireCount += Time.deltaTime;
		}
	}

	void OnTriggerStay(Collider obj){
			//Debug.Log ("something trigger, " + obj.name);
		GameObject target = obj.gameObject;
		Player player = obj.GetComponentInParent<Player> ();
		if (!playerSelf.isDead && target.tag == "Player" && !player.isDead) {
			
			GameObject FiredCannon;

			if (currentTarget == null) {
				currentTarget = target;
			}
				
			//Debug.Log (name + ", Target in sight, fire !!");
			if (fireCount>= fireRate) {
				fireCount = 0;
				// where to fire
				if (type == WeaponType.CannonOG) {
					fireVector = (currentTarget.transform.position - spawnPos[0].transform.position).normalized;
					// put all cannon under playerProjectiles
					FiredCannon = Instantiate (cannonBall, spawnPos[0].transform.position, Quaternion.identity);
					FiredCannon.transform.SetParent (playerProjectiles);

					// who is the attacker
					GameObject attacker = GetComponentInParent<Player>().gameObject;
					//FiredCannon.GetComponent<CannonBall> ().SetAttacker (attacker);

					FiredCannon.GetComponent<CannonBall> ().CannonFire (attacker,playerProjectiles, fireVector);
				}else if (type == WeaponType.ScatterGun) {
					for (int i = 0; i < spawnPos.Length ; i++) {
						if (i == 0) {
							fireVector = (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}else if (i == 1) {
							fireVector = Quaternion.Euler(0,6,0) * (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}else if (i == 2) {
							fireVector = Quaternion.Euler(0,-6,0) * (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}else if (i == 3) {
							fireVector = Quaternion.Euler(0,12,0) * (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}else if (i == 4) {
							fireVector = Quaternion.Euler(0,-12,0) * (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}

						// put all cannon under playerProjectiles
						FiredCannon = Instantiate (scatterGun, spawnPos[i].transform.position, Quaternion.identity);
						FiredCannon.transform.SetParent (playerProjectiles);

						// who is the attacker
						GameObject attacker = GetComponentInParent<Player>().gameObject;
						//FiredCannon.GetComponent<CannonBall> ().SetAttacker (attacker);

						FiredCannon.GetComponent<CannonBall> ().CannonFire (attacker,playerProjectiles, fireVector);
					}

				}

			}
		}
	}

	void OnTriggerExit (Collider obj){
		GameObject target = obj.gameObject;
		if (currentTarget == target) {
			currentTarget = null;
		}
	}
}
