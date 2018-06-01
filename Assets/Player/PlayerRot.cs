using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRot : MonoBehaviour {

	public float rotSpeed = 30;

	void RotateToForward (PlayerRotator playerRotator){
		//Debug.Log ("forward received, " + forward);
		float step = playerRotator.RotSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards (transform.forward, playerRotator.ForwardDir, step, 0.0f);
		transform.rotation = Quaternion.LookRotation (newDir);
	}


}

struct PlayerRotator{
	public Vector3 ForwardDir;
	public float RotSpeed;

	public PlayerRotator (Vector3 forwardDir, float rotSpeed){
		this.ForwardDir = forwardDir;
		this.RotSpeed = rotSpeed;
	}
}
