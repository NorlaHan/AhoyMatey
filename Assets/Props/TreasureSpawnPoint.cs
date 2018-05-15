using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawnPoint : MonoBehaviour {

	public bool canSpawn = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay (Collider obj){
		GameObject target = obj.gameObject;
		//Debug.Log ("Trigger");
		if (target.tag == "Player") {
			canSpawn = false;
			Debug.Log ("Trigger player");
		}
	}

	void OnTriggerExit (Collider obj){
		GameObject target = obj.gameObject;
		if (target.tag == "Player") {
			canSpawn = true;
		}
	}
}
