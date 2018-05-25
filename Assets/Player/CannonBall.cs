using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CannonBall : NetworkBehaviour {

	public enum WeaponType {CannonOG, ScatterGun}
	public WeaponType type;

	public float ballSpeed = 40f, lifeTime = 4f;
	public float ballDamage = 20f;
	public GameObject attacker, hitFX;
	public Vector3 upVelocity = new Vector3 (0f, 6.5f, 0f);

	private Transform playerProjectiles;
	private Rigidbody rigidBody;
	//private float lifeCount = 0f;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
//		if (lifeCount < lifeTime) {
//			lifeCount += Time.deltaTime;
//		} else {
//			Destroy (gameObject);
//		}
	}

	public void CannonFire (GameObject theAttacker,Transform parent, Vector3 fireVector) {
		attacker = theAttacker;
		playerProjectiles = parent;
		rigidBody = GetComponent<Rigidbody> ();
		rigidBody.velocity = (fireVector * ballSpeed) + upVelocity;
		Destroy (gameObject, lifeTime);
	}

	void OnCollisionEnter (Collision obj){
//		if (!isServer) {
//			return;
//		}
		if (type == WeaponType.CannonOG) {
			GameObject Fx = Instantiate (hitFX, transform.position, Quaternion.identity);
			Fx.transform.SetParent (playerProjectiles);
		}else if (type == WeaponType.ScatterGun) {
			GameObject Fx = Instantiate (hitFX, transform.position, Quaternion.identity);
			Fx.transform.SetParent (playerProjectiles);
		}
		GameObject target = obj.gameObject;
		if (target.tag == "Player") {
			target.GetComponent<Health> ().OnTakeDamage (ballDamage,attacker);
		}
		Destroy (gameObject,0.1f);
	}

//	public void SetAttacker(GameObject theAttacker){
//		attacker = theAttacker;
//	}
}
