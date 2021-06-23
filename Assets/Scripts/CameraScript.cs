using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	public GameObject trackObject;
	public Vector3 offset;
	public float preMoveDistance;

	private float shakeIntensity;
	private float shakeTime;
	private Vector3 shakeOffset;

	void Start()
	{
		Time.timeScale = 1.0f;
	}

	void Update()
	{
		Vector3 preMovePositionOffset = trackObject.GetComponent<Rigidbody2D>().velocity * preMoveDistance;
		transform.position = trackObject.transform.position + offset + preMovePositionOffset + shakeOffset;

		if (Input.GetKeyDown(KeyCode.K))
		{
			Shake(0.5f, 1.0f);
		}

		shakeTime = Mathf.Max(0, shakeTime - Time.deltaTime);
		if (shakeTime > 0)
		{
			shakeOffset.x = Random.value * shakeIntensity * 2 - shakeIntensity;
			shakeOffset.y = Random.value * shakeIntensity * 2 - shakeIntensity;
		}
		else
		{
			shakeIntensity = 0.0f;
		}
	}

	public void Shake(float length, float intensity)
	{
		if (intensity > shakeIntensity)
		{
			shakeIntensity = intensity;
			shakeTime = length;
		}
	}

	public void Freeze(float length)
	{
		StartCoroutine(FreezeEvent(length));
	}

	IEnumerator FreezeEvent(float length)
	{
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(length);
		Time.timeScale = 1.0f;
	}
}
