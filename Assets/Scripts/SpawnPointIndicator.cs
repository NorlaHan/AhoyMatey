using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointIndicator : MonoBehaviour {

	[Tooltip("Sphere only use X for radius")]
	public Vector3 gizmosSize = new Vector3 (0.1f, 0.1f, 0.1f);
	public Color color = Color.red;
	public enum ShapeType{Cube, Sphere, WireCube, WireSphere};
	public ShapeType shapeType;
	public bool isLiftFromGround = true;

	void OnDrawGizmos(){
		Gizmos.color = color;
		if (isLiftFromGround) {
			if (shapeType == ShapeType.Cube) {
				Gizmos.DrawCube (transform.position + new Vector3 (0, gizmosSize.y / 2, 0), gizmosSize);
			} else if (shapeType == ShapeType.Sphere) {
				Gizmos.DrawSphere (transform.position + new Vector3 (0, gizmosSize.x, 0), gizmosSize.x);
			} else if (shapeType == ShapeType.WireCube) {
				Gizmos.DrawWireCube (transform.position + new Vector3 (0, gizmosSize.y / 2, 0), gizmosSize);
			} else if (shapeType == ShapeType.WireSphere) {
				Gizmos.DrawWireSphere (transform.position + new Vector3 (0, gizmosSize.x, 0), gizmosSize.x);
			}
		} else {
			if (shapeType == ShapeType.Cube) {
				Gizmos.DrawCube (transform.position , gizmosSize);
			} else if (shapeType == ShapeType.Sphere) {
				Gizmos.DrawSphere (transform.position , gizmosSize.x);
			} else if (shapeType == ShapeType.WireCube) {
				Gizmos.DrawWireCube (transform.position , gizmosSize);
			} else if (shapeType == ShapeType.WireSphere) {
				Gizmos.DrawWireSphere (transform.position , gizmosSize.x);
			}
		}

	}
}
