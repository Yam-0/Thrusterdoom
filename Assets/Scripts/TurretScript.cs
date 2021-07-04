using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
	public bool limitFire = false;
	public float maxAngle = 30.0f;
	public float maxRange = 10.0f;
	public GameObject boat;

	private float initialRotation;
	private GameObject target;
	private float angleToTarget;
	private bool withinRange;

	void Start()
	{
		target = GameObject.FindGameObjectWithTag("Player");
		initialRotation = transform.rotation.eulerAngles.z;
	}

	void Update()
	{
		if (boat.transform.localScale.x == -1)
		{
			transform.localScale = new Vector3(-1, 1, 1);
		}
		else
		{
			transform.localScale = new Vector3(1, 1, 1);
		}
		Vector2 deltaPosition = transform.position - target.transform.position;
		angleToTarget = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		float targetRotation = angleToTarget * Mathf.Rad2Deg + 90;
		float distanceToTarget = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		if (deltaPosition.y < 0)
		{
			targetRotation = Mathf.Clamp(targetRotation, initialRotation - maxAngle / 2, initialRotation + maxAngle / 2);
			transform.rotation = Quaternion.Euler(0, 0, targetRotation);
		}

		withinRange = (deltaPosition.y < 0 && targetRotation > initialRotation - maxAngle / 2 && targetRotation < initialRotation + maxAngle / 2);
		withinRange = withinRange && distanceToTarget <= maxRange;
	}

	public bool GetWithinRange()
	{
		return withinRange;
	}
}
