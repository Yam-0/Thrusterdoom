using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
	public List<GameObject> bullets;
	public Pattern firePattern;
	public float outerAngle = 45.0f;
	public float delay = 0.0f;
	public float fireTime = 2.0f;
	private Transform tr;
	private float timer;

	private float angleStepSize;
	private float timeStepSize;

	private float[] fireAngles;
	private float[] fireTimes;
	private bool[] fireFlags;

	void Awake()
	{
		tr = GetComponent<Transform>();
		timeStepSize = fireTime / bullets.Count;
		angleStepSize = outerAngle / (bullets.Count - 1);

		fireTimes = new float[bullets.Count];
		fireAngles = new float[bullets.Count];
		fireFlags = new bool[bullets.Count];

		switch (firePattern)
		{
			case Pattern.SprayRight:
				for (int i = 0; i < fireTimes.Length; i++)
				{
					fireTimes[i] = delay + timeStepSize * i;
				}
				for (int i = 0; i < fireAngles.Length; i++)
				{
					fireAngles[i] = tr.eulerAngles.z + (outerAngle / 2) - (angleStepSize * i);
				}
				for (int i = 0; i < fireFlags.Length; i++)
				{
					fireFlags[i] = false;
				}
				break;

			case Pattern.SprayLeft:
				for (int i = 0; i < fireTimes.Length; i++)
				{
					fireTimes[i] = delay + timeStepSize * i;
				}
				for (int i = 0; i < fireAngles.Length; i++)
				{
					fireAngles[i] = tr.eulerAngles.z - (outerAngle / 2) + (angleStepSize * i);
				}
				for (int i = 0; i < fireFlags.Length; i++)
				{
					fireFlags[i] = false;
				}
				break;

			case Pattern.SprayOut:
				for (int i = 0; i < fireTimes.Length / 2; i++)
				{
					fireTimes[i * 2] = delay + timeStepSize * i;
					fireTimes[i * 2 + 1] = delay + timeStepSize * i;
				}
				for (int i = 0; i < fireAngles.Length / 2; i++)
				{
					fireAngles[i * 2] = tr.eulerAngles.z + (angleStepSize * i);
					fireAngles[i * 2 + 1] = tr.eulerAngles.z - (angleStepSize * i);
				}
				for (int i = 0; i < fireFlags.Length; i++)
				{
					fireFlags[i] = false;
				}
				break;

			case Pattern.SprayIn:
				for (int i = 0; i < fireTimes.Length / 2; i++)
				{
					fireTimes[i * 2] = delay + timeStepSize * i;
					fireTimes[i * 2 + 1] = delay + timeStepSize * i;
				}
				for (int i = 0; i < fireAngles.Length / 2; i++)
				{
					fireAngles[i * 2] = tr.eulerAngles.z - (outerAngle / 2) + (angleStepSize * i);
					fireAngles[i * 2 + 1] = tr.eulerAngles.z + (outerAngle / 2) - (angleStepSize * i);
				}
				for (int i = 0; i < fireFlags.Length; i++)
				{
					fireFlags[i] = false;
				}
				break;
		}
	}

	void Update()
	{
		timer += Time.deltaTime;
		if (timer < delay) { return; }

		for (int i = 0; i < bullets.Count; i++)
		{
			if (fireTimes[i] < timer && !fireFlags[i])
			{
				fireFlags[i] = true;
				Instantiate(bullets[i], tr.position, Quaternion.AngleAxis(fireAngles[i], Vector3.forward));
			}
		}

		bool destroy = true;
		for (int i = 0; i < fireFlags.Length; i++)
		{
			if (!fireFlags[i]) { destroy = false; }
		}
		if (destroy) { Destroy(gameObject, 0); }
	}

	public enum Pattern
	{
		SprayRight,
		SprayLeft,
		SprayOut,
		SprayIn
	}

}
