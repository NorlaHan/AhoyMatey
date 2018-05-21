using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairSpawnPoint : MonoBehaviour {
	public bool canSpawn = true;

	public bool isDebugMode = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (isDebugMode) {
			Debug.Log (transform.childCount + ", child = " + transform.GetChild(0));
			isDebugMode = false;
		}
	}
		

	void OnTriggerStay (Collider obj){
		GameObject target = obj.gameObject;
		//Debug.Log ("Trigger");
		if (target.tag == "Player") {
			if (canSpawn) {canSpawn = false;}
			//Debug.Log ("Trigger player");
		}
	}

	void OnTriggerExit (Collider obj){
		GameObject target = obj.gameObject;
		if (target.tag == "Player") {
			canSpawn = true;
		}
	}

}
