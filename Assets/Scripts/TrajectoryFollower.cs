using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryFollower : MonoBehaviour
{
	public Trajectory trajectory;
	public float speed;
	public bool reverse;
	public bool loop;
	public bool disableOnCollision;

	private int nextTrajectoryPointIndex;
	private Vector3 lastPosition;
	private Quaternion lastDirection;
	private Vector3 nextPosition;
	private Quaternion nextDirection;
	private float nextPositionTravelDuration;
	private float transitionStartTime;

	void Awake ()
	{
		if (trajectory.points.Length == 0) {
			Debug.LogError ("The trajectory must contain at least one point");
			enabled = false;
			return;
		}

		nextPosition = transform.position;
		nextDirection = transform.rotation;

		nextTrajectoryPointIndex = GetClosestTrajectoryPointIndex ();
		nextTrajectoryPointIndex += reverse ? 1 : -1; // negate the modification done by SetUpPathToNextTrajectoryPoint
		SetUpPathToNextTrajectoryPoint ();
	}

	void FixedUpdate ()
	{
		float progress = (Time.time - transitionStartTime) / nextPositionTravelDuration;
		while (progress > 0) {
			transform.position = Vector3.Lerp (lastPosition, nextPosition, progress);
			transform.rotation = Quaternion.Lerp (lastDirection, nextDirection, progress);
			progress--;

			if (progress > 0) {
				if (!SetUpPathToNextTrajectoryPoint ()) {
					return;
				}
			}
		}
	}

	void OnCollisionEnter ()
	{
		if (disableOnCollision) {
			enabled = false;
		}
	}

	int GetClosestTrajectoryPointIndex ()
	{
		if (trajectory.points.Length == 0) {
			return -1;
		}

		float bestDistance = Vector3.Distance (trajectory.points [0].position, transform.position);
		int closestPointIndex = 0;
		for (int i = 1; i < trajectory.points.Length; i++) {
			float distance = Vector3.Distance (trajectory.points [i].position, transform.position);
			if (distance < bestDistance) {
				closestPointIndex = i;
				bestDistance = distance;
			}
		}
		return closestPointIndex;
	}

	bool SetUpPathToNextTrajectoryPoint ()
	{
		nextTrajectoryPointIndex += reverse ? -1 : 1;
		if (nextTrajectoryPointIndex < 0 || nextTrajectoryPointIndex >= trajectory.points.Length) {
			if (loop) {
				nextTrajectoryPointIndex = nextTrajectoryPointIndex == -1 ? trajectory.points.Length - 1 : 0;
			} else {
				enabled = false; // we have nothing to do anymore
				return false;
			}
		}

		lastPosition = nextPosition;
		lastDirection = nextDirection;
		nextPosition = trajectory.points [nextTrajectoryPointIndex].position;
		nextDirection = trajectory.points [nextTrajectoryPointIndex].direction;
		if (reverse) {
			// TODO: this doesn't work well, hand-crafted points might be neccessary. maybe remove altogether?
			nextDirection = Quaternion.Euler (-nextDirection.eulerAngles);
		}

		float nextPositionDistance = Vector3.Distance (lastPosition, nextPosition);
		nextPositionTravelDuration = nextPositionDistance / speed;
		transitionStartTime = Time.time;

		return true;
	}
}
