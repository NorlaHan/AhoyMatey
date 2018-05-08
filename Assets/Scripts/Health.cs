using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public float fullHealth = 100f;

	public float currentHealth;

	// Use this for initialization
	void Start () {
		currentHealth = fullHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnTakeDamage (float damage){
		if (!isServer) {
			return;
		}
		currentHealth -= damage;
		if (currentHealth <= 0) {
			OnDeath ();
		}
	}

	void OnDeath(){
		Debug.LogWarning (name + ", is dead.");
		//Destroy (gameObject);
	}
}
