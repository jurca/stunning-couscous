using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float minSpeed;
	public float maxSpeed;
	public float mouseWheelAccelarationFactor;
	public float speedSmoothingTime;
	public ParticleSystem mainEngineExhaust;
	public float minExhaustEmissionRate;
	public float maxExhaustEmissionRate;
	public float minExhaustParticleLifetime;
	public float maxExhaustParticleLifetime;

	Rigidbody rigidBody;
	float speed = 0;
	float targetSpeed = 0;
	float speedSmoothingVelocity = 0;

	void Awake ()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	void Update ()
	{
		float speedDelta = Input.mouseScrollDelta.y * mouseWheelAccelarationFactor;
		targetSpeed = Mathf.Clamp (targetSpeed + speedDelta, minSpeed, maxSpeed);
		speed = Mathf.SmoothDamp (speed, targetSpeed, ref speedSmoothingVelocity, speedSmoothingTime);

		ParticleSystem.EmissionModule emission = mainEngineExhaust.emission;
		emission.rateOverTime = Mathf.Max (0, speed) / (maxSpeed - Mathf.Max (0, minSpeed)) * (maxExhaustEmissionRate - minExhaustEmissionRate) + minExhaustEmissionRate;
		ParticleSystem.MainModule mainParticleModule = mainEngineExhaust.main;
		mainParticleModule.startLifetimeMultiplier = Mathf.Max (0, speed) / (maxSpeed - Mathf.Max (0, minSpeed)) * (maxExhaustParticleLifetime - minExhaustParticleLifetime) + minExhaustParticleLifetime;
	}

	void FixedUpdate ()
	{
		Quaternion orientation = transform.rotation;
		rigidBody.velocity = orientation * Vector3.forward * speed;
	}
}
