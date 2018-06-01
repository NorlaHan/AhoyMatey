using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttack : MonoBehaviour {

	public enum WeaponType {CannonOG, ScatterGun, SuperCannon};
	public WeaponType type;

	public string weaponName;
	public bool isDebugMode = false;
	public GameObject cannonBall, scatterGun, superCannon,currentTarget;
	public float cannonOGRate = 2f, scatterGunRate = 1.8f, superCannonGunRate = 2.2f;
	public SpawnPointIndicator[] spawnPos;

	private Transform playerProjectiles;
	private Player playerSelf, playerEnemy;
	private float  fireRate,fireCount = 0;
	private Vector3 fireVector;
	private BoxCollider boxCollider;

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

		if (GetComponent<BoxCollider> ()) {
			boxCollider = GetComponent<BoxCollider> ();
		}else {Debug.LogWarning (name + ", missing BoxCollider");}

		if (GetComponentInParent<Player>()) {
			playerSelf = GetComponentInParent<Player> ();
			weaponName = playerSelf.weaponName;
			fireRate = scatterGunRate;
		}else {
			Debug.LogWarning (name + ", missing Self Player");
			weaponName = "ScatterGun";
		}

		OnWeaponChange ();
	
	}

	public void OnWeaponChange (){
		if (!boxCollider) {
			boxCollider = GetComponent<BoxCollider> ();
		}
		if (weaponName == "CannonOG" /* type == WeaponType.CannonOG*/) {
			if (boxCollider.center.x < 0) {
				boxCollider.center = new Vector3 (-27, 4, 0);
			} else {
				boxCollider.center = new Vector3 (27, 4, 0);
			}
			boxCollider.size = new Vector3 (50, 8, 7.5f);
			fireRate = cannonOGRate;
		}else if (weaponName == "ScatterGun" /*type == WeaponType.ScatterGun*/) {
			if (boxCollider.center.x < 0) {
				boxCollider.center = new Vector3 (-27, 4, 0);
			} else {
				boxCollider.center = new Vector3 (27, 4, 0);
			}
			boxCollider.size = new Vector3 (50, 8, 15f);
			fireRate = scatterGunRate;
		}else if (weaponName == "SuperCannon" /* type == WeaponType.SuperCannon*/) {
			if (boxCollider.center.x < 0) {
				boxCollider.center = new Vector3 (-32, 4, 0);
			} else {
				boxCollider.center = new Vector3 (32, 4, 0);
			}
			boxCollider.size = new Vector3 (60, 8, 7.5f);
			fireRate = superCannonGunRate;
		}
//		if (GetComponentInParent<Player>().weaponName == type.ToString()) {
//			SendMessageUpwards ("CmdChangingWeapon", false);
//		}
		//Invoke ("Test", 0.5f);
	}

//	void Test ()
//	{
//		SendMessageUpwards ("CmdChangingWeapon", false);
//	}
	
	// Update is called once per frame
	void Update () {
		if (fireCount < fireRate) {
			fireCount += Time.deltaTime;
		}

		if (isDebugMode) {
			OnWeaponChange ();
			isDebugMode = false;
		}
	}

	void OnTriggerStay(Collider obj){
			//Debug.Log ("something trigger, " + obj.name);
		GameObject target = obj.gameObject;
		if (obj.GetComponentInParent<Player> ()) {
			playerEnemy = obj.GetComponentInParent<Player> ();
		}
		if (!playerSelf.isDead && target.tag == "Player" && !playerEnemy.isDead) {
			
			GameObject FiredCannon;

			if (currentTarget == null) {
				currentTarget = target;
			}
				
			//Debug.Log (name + ", Target in sight, fire !!");
			if (fireCount>= fireRate) {
				fireCount = 0;
				// where to fire
				if (weaponName == "CannonOG" /*type == WeaponType.CannonOG*/) {
					fireVector = (currentTarget.transform.position - spawnPos[0].transform.position).normalized;
					// put all cannon under playerProjectiles
					FiredCannon = Instantiate (cannonBall, spawnPos[0].transform.position, Quaternion.identity);
					FiredCannon.transform.SetParent (playerProjectiles);

					// who is the attacker
					GameObject attacker = GetComponentInParent<Player>().gameObject;
					//FiredCannon.GetComponent<CannonBall> ().SetAttacker (attacker);

					FiredCannon.GetComponent<CannonBall> ().CannonFire (attacker,playerProjectiles, fireVector);
				}else if (weaponName == "ScatterGun" /*type == WeaponType.ScatterGun*/) {
					for (int i = 0; i < spawnPos.Length ; i++) {
						if (i == 0) {
							fireVector = (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}else if (i == 1) {
							fireVector = Quaternion.Euler(0,4,0) * (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}else if (i == 2) {
							fireVector = Quaternion.Euler(0,-4,0) * (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}else if (i == 3) {
							fireVector = Quaternion.Euler(0,8,0) * (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}else if (i == 4) {
							fireVector = Quaternion.Euler(0,-8,0) * (currentTarget.transform.position - spawnPos[i].transform.position).normalized;
						}

						// put all cannon under playerProjectiles
						FiredCannon = Instantiate (scatterGun, spawnPos[i].transform.position, Quaternion.identity);
						FiredCannon.transform.SetParent (playerProjectiles);

						// who is the attacker
						GameObject attacker = GetComponentInParent<Player>().gameObject;
						//FiredCannon.GetComponent<CannonBall> ().SetAttacker (attacker);

						FiredCannon.GetComponent<CannonBall> ().CannonFire (attacker,playerProjectiles, fireVector);
					}

				}else if (weaponName == "SuperCannon" /*type == WeaponType.SuperCannon*/) {
					fireVector = (currentTarget.transform.position - spawnPos[0].transform.position).normalized;
					// put all cannon under playerProjectiles
					FiredCannon = Instantiate (superCannon, spawnPos[0].transform.position, Quaternion.identity);
					FiredCannon.transform.SetParent (playerProjectiles);

					// who is the attacker
					GameObject attacker = GetComponentInParent<Player>().gameObject;
					//FiredCannon.GetComponent<CannonBall> ().SetAttacker (attacker);

					FiredCannon.GetComponent<CannonBall> ().CannonFire (attacker,playerProjectiles, fireVector);
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
