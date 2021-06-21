using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	[Header("Parallax Settings")]
	[Range(0.0f, 1.0f)]
	public float movementMultiplier = 1.0f;

	private Vector3 initialPosition;
	private GameObject trackObject;

	void Start()
	{
		initialPosition = transform.position;
		trackObject = GameObject.FindGameObjectWithTag("MainCamera");
	}

	void Update()
	{
		if (trackObject != null)
		{
			Vector3 offset = (trackObject.transform.position * movementMultiplier);
			offset.z = 0;
			transform.position = initialPosition + offset;

			if (transform.position.x - trackObject.transform.position.x > 25.0f)
			{
				initialPosition.x -= 50.0f;
			}
			if (transform.position.x - trackObject.transform.position.x < -25.0f)
			{
				initialPosition.x += 50.0f;
			}
		}
	}
}
