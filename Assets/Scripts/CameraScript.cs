using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	public GameObject trackObject;
	public Vector3 offset;
	public float preMoveDistance;

	private bool shaking;
	private float shakeTime;
	private float shakeIntensity;

	void Start()
	{

	}

	void Update()
	{
		Vector3 preMovePositionOffset = trackObject.GetComponent<Rigidbody2D>().velocity * preMoveDistance;
		transform.position = trackObject.transform.position + offset + preMovePositionOffset;

		if (Input.GetKeyDown(KeyCode.K))
		{
			Shake(0.5f, 1.0f);
		}
	}

	public void Shake(float length, float intensity)
	{
		shakeTime = length;
		shakeIntensity = intensity;
	}

	void LateUpdate()
	{
		shaking = (shakeTime > 0);

		if (shaking)
		{
			shakeTime = Mathf.Max(0, shakeTime - Time.deltaTime);
		}
	}
}
