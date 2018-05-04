using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider obj){
			//Debug.Log ("something trigger, " + obj.name);
			GameObject target = obj.gameObject;
			if (target.tag == "Player") {
				Debug.Log (name + ", Target in sight, fire !!");
			}
	}
}
