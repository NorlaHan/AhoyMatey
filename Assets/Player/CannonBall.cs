using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

	public float ballSpeed = 40f, lifeTime = 4f;
	public float ballDamage = 20f;
	public Vector3 upVelocity = new Vector3 (0f, 6.5f, 0f);

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

	public void CannonFire (Vector3 fireVector) {
		rigidBody = GetComponent<Rigidbody> ();
		rigidBody.velocity = (fireVector * ballSpeed) + upVelocity;
		Destroy (gameObject, lifeTime);
	}

	void OnCollisionEnter (Collision obj){
		GameObject target = obj.gameObject;
		if (target.tag == "Player") {
			target.GetComponent<Health> ().OnTakeDamage (ballDamage);
		}
		//Destroy (gameObject);
	}
}
