using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRot : MonoBehaviour {

	public float rotSpeed = 30;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void RotateToForward (Vector3 forwardDir){
		//Debug.Log ("forward received, " + forward);
		float step = rotSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards (transform.forward, forwardDir, step, 0.5f);
		transform.rotation = Quaternion.LookRotation (newDir);
	}
}
