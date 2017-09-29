using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPoint : MonoBehaviour
{
	public Transform positionAndOrientation;
	public float radius;

	public Vector3 position {
		get { return this.positionAndOrientation.position; }
	}

	public Quaternion direction {
		get { return this.positionAndOrientation.rotation; }
	}
}
