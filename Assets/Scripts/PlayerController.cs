using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float minSpeed;
	public float maxSpeed;
	public float mouseWheelAccelarationFactor;
	public float speedSmoothingTime;
	public float turningSpeed;
	public ShipComponents.Engine engine;

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

		Vector2 mousePosition = GetNormalizedMousePosition ();
		float rotation = Input.GetKey (KeyCode.Q) ? 1f : Input.GetKey (KeyCode.E) ? -1f : 0;
		Vector3 turning = new Vector3 (
			                  -mousePosition.y * turningSpeed,
			                  mousePosition.x * turningSpeed,
			                  rotation * turningSpeed
		                  );
		rigidBody.angularVelocity = transform.rotation * turning;

		engine.power = (speed - minSpeed) / (maxSpeed - minSpeed);
	}

	void FixedUpdate ()
	{
		Quaternion orientation = transform.rotation;
		rigidBody.velocity = orientation * Vector3.forward * speed;
	}

	Vector2 GetNormalizedMousePosition ()
	{
		Vector2 absolutePosition = Input.mousePosition;
		float scale = (Screen.width > Screen.height ? Screen.height : Screen.width) / 2f;
		Vector2 positionToCenter = new Vector2 (
			                           absolutePosition.x - Screen.width / 2,
			                           absolutePosition.y - Screen.height / 2
		                           );
		Vector2 normalizedPosition = new Vector2 (
			                             Mathf.Clamp (positionToCenter.x / scale, -1f, 1f),
			                             Mathf.Clamp (positionToCenter.y / scale, -1f, 1f)
		                             );
		return normalizedPosition.magnitude > 1 ? normalizedPosition.normalized : normalizedPosition;
	}
}
