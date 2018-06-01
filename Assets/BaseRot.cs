using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRot : MonoBehaviour {

	public float rotSpeed = 30;

	void RotateToForward (PlayerRotator playerRotator){
		//Debug.Log ("forward received, " + forward);
		float step = playerRotator.RotSpeed * Time.deltaTime;
		// filter out the tilt, keep the vecter horizontal.
		Vector3 turrentRot = new Vector3 (playerRotator.ForwardDir.x, 0, playerRotator.ForwardDir.z);
		Vector3 newDir = Vector3.RotateTowards (transform.forward, turrentRot, step, 0.0f);
		transform.rotation = Quaternion.LookRotation (newDir);
	}

}
