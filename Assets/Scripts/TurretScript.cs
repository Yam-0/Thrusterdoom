using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
	public bool limitFire = false;
	public float maxAngle = 30.0f;
	public float maxRange = 10.0f;
	public GameObject boat;
	public bool targetUnder = false;

	private float initialRotation;
	private GameObject target;
	private Vector3 targetPos = Vector3.zero;
	private float angleToTarget;
	private float targetRotation;
	private bool withinRange;
	private Animation an;

	void Start()
	{
		target = GameObject.FindGameObjectWithTag("Player");
		initialRotation = transform.rotation.eulerAngles.z;
		targetRotation = initialRotation;
		an = GetComponent<Animation>();
	}

	void Update()
	{
		if (target != null)
			targetPos = target.transform.position;

		transform.localScale = new Vector3(boat.transform.localScale.x, 1, 1);

		Vector2 deltaPosition = transform.position - targetPos;
		angleToTarget = Mathf.Atan2(deltaPosition.y, deltaPosition.x) * Mathf.Rad2Deg + 90;
		float distanceToTarget = Vector3.Distance(transform.position, targetPos);

		if (deltaPosition.y <= 0 || (targetUnder && deltaPosition.y > 0))
		{
			targetRotation = Mathf.Clamp(angleToTarget, initialRotation - maxAngle / 2, initialRotation + maxAngle / 2);
			transform.rotation = Quaternion.Euler(0, 0, targetRotation);
		}

		withinRange = (targetRotation > initialRotation - maxAngle / 2 && targetRotation < initialRotation + maxAngle / 2);
		withinRange = withinRange && distanceToTarget <= maxRange;
		withinRange = withinRange && ((deltaPosition.y > 0 && targetUnder) || (deltaPosition.y <= 0 && !targetUnder));
	}

	public void Fire()
	{
		if (an != null)
			an.Play();
	}

	public bool GetWithinRange()
	{
		return withinRange;
	}
}
