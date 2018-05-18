using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCommunicate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CommunicateToPlayer(){
		SendMessageUpwards ("RpcOnUnitRespawn");
		transform.localPosition = Vector3.zero;
	}
}
