using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
	Rigidbody rigidBody;

	void Start ()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate ()
	{
		rigidBody.AddForce (new Vector3 (0, 0, -1));
	}
}
