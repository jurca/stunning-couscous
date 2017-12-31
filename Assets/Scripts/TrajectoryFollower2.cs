using UnityEngine;
using System.Collections;

public class TrajectoryFollower2 : MonoBehaviour
{
	public Transform trajectory;
	public bool reverse;
	public bool loop;
	public float speed;
	public bool startAtClosestPoint;
	public bool disableOnCollision;

	private Vector3 lastPosition;
	private Quaternion lastRotation;
	private Vector3 nextPosition;
	private Quaternion nextRotation;
	private int nextPointIndex;
	private float nextPointTravelDuration;
	private float transitionStartTime;

	void Awake ()
	{
		if (trajectory.childCount < 1) {
			Debug.LogError ("The trajectory must contain at least one point");
			enabled = false;
			return;
		}

		if (startAtClosestPoint) {
			nextPointIndex = GetClosestTrajectoryPointIndex ();
		} else {
			nextPointIndex = reverse ? trajectory.childCount - 1 : 0;
		}

		lastPosition = transform.position;
		lastRotation = transform.rotation;
		Tuple<Vector3, Quaternion> nextPoint = GetTowardsNextPointTransform ();
		nextPosition = nextPoint._1;
		nextRotation = nextPoint._2;

		float nextPointDistance = Vector3.Distance (lastPosition, nextPosition);
		nextPointTravelDuration = nextPointDistance / speed;
		transitionStartTime = Time.time;
	}

	void FixedUpdate ()
	{
		float progress = (Time.time - transitionStartTime) / nextPointTravelDuration;
		while (progress > 0) {
			transform.position = Vector3.Lerp (lastPosition, nextPosition, progress);
			transform.rotation = Quaternion.Lerp (lastRotation, nextRotation, progress);
			progress--;

			if (progress > 0) {
				if (!PrepareNextTransition ()) {
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

	private Tuple<Vector3, Quaternion> GetTowardsNextPointTransform ()
	{
		Transform upOrientation = trajectory.GetChild (nextPointIndex);
		Vector3 towardsNextPoint = upOrientation.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation (towardsNextPoint, upOrientation.rotation * Vector3.up);
		return new Tuple<Vector3, Quaternion> (upOrientation.position, rotation);
	}

	private bool PrepareNextTransition ()
	{
		nextPointIndex += reverse ? -1 : 1;
		if (nextPointIndex < 0 || nextPointIndex >= trajectory.childCount) {
			if (!loop) {
				enabled = false;
				return false;
			}

			nextPointIndex = reverse ? trajectory.childCount - 1 : 0;
		}

		lastPosition = nextPosition;
		lastRotation = nextRotation;
		Tuple<Vector3, Quaternion> nextPoint = GetTowardsNextPointTransform ();
		nextPosition = nextPoint._1;
		nextRotation = nextPoint._2;

		float nextPointDistance = Vector3.Distance (lastPosition, nextPosition);
		nextPointTravelDuration = nextPointDistance / speed;
		transitionStartTime = Time.time;

		return true;
	}

	int GetClosestTrajectoryPointIndex ()
	{

		float bestDistance = Vector3.Distance (trajectory.GetChild (0).position, transform.position);
		int closestPointIndex = 0;
		for (int i = 1; i < trajectory.childCount; i++) {
			float distance = Vector3.Distance (trajectory.GetChild (i).position, transform.position);
			if (distance < bestDistance) {
				closestPointIndex = i;
				bestDistance = distance;
			}
		}
		return closestPointIndex;
	}
}

class Tuple<T1, T2>
{
	public T1 _1;
	public T2 _2;

	public Tuple (T1 first, T2 second)
	{
		_1 = first;
		_2 = second;
	}
}
