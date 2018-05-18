using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttack : MonoBehaviour {
	
	public GameObject cannonBall, currentTarget;
	public float fireRate = 2f;


	private SpawnPointIndicator spawnPos;
	private GameObject playerProjectiles;
	private float fireCount = 0;

	// Use this for initialization
	void Start () {
		if (GetComponentInChildren<SpawnPointIndicator> ()) {
			spawnPos = GetComponentInChildren<SpawnPointIndicator> ();
		} else {Debug.LogWarning (name + ", missing SpawnPointIndicator");}

		if (GameObject.Find ("PlayerProjectiles")) {
			playerProjectiles = GameObject.Find ("PlayerProjectiles");
		}else {
			Debug.LogWarning (name + ", missing PlayerProjectiles. Auto generate.");
			playerProjectiles = new GameObject ("playerProjectiles");
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
		if (target.tag == "Player" && !player.isDead) {
			Vector3 fireVector;
			GameObject FiredCannon;

			if (currentTarget == null) {
				currentTarget = target;
			}
				
			//Debug.Log (name + ", Target in sight, fire !!");
			if (fireCount>= fireRate) {
				fireCount = 0;
				// where to fire
				fireVector = (currentTarget.transform.position - spawnPos.transform.position).normalized;
				// put all cannon under playerProjectiles
				FiredCannon = Instantiate (cannonBall, spawnPos.transform.position, Quaternion.identity);
				FiredCannon.transform.SetParent (playerProjectiles.transform);
				
				// who is the attacker
				GameObject attacker = transform.parent.transform.parent.gameObject;
				FiredCannon.GetComponent<CannonBall> ().SetAttacker (attacker);
				
				FiredCannon.GetComponent<CannonBall> ().CannonFire (fireVector);
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
