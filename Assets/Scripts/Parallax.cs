using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	[Header("Parallax Settings")]
	[Range(-2.0f, 2.0f)]
	public float movementMultiplier = 1.0f;

	[Space]
	public bool wrap = false;
	public float wrapDistance = 50.0f;

	[Space]
	public bool stretching = false;
	public GameObject point1;
	public GameObject point2;

	private Vector3 initialPosition;
	private Vector3 initialScale;
	private float initialDistance = 0.0f;
	private GameObject trackObject;

	void Start()
	{
		initialPosition = transform.position;
		initialScale = transform.localScale;
		trackObject = GameObject.FindGameObjectWithTag("MainCamera");
	}

	void LateUpdate()
	{
		if (trackObject != null)
		{
			Vector3 moveOffset = (trackObject.transform.position * movementMultiplier);
			moveOffset.z = 0;
			transform.position = initialPosition + moveOffset;

			if (wrap)
			{
				if (transform.position.x - trackObject.transform.position.x > (wrapDistance / 2))
				{
					initialPosition.x -= wrapDistance;
				}
				if (transform.position.x - trackObject.transform.position.x < -(wrapDistance / 2))
				{
					initialPosition.x += wrapDistance;
				}
			}

			if (stretching)
			{
				float height = (point1.transform.position.y - point2.transform.position.y);
				if (height < 0) { height = 0; }
				transform.localScale = new Vector3(9, height, 1);

				Vector3 location = new Vector3();
				location.x = trackObject.transform.position.x;
				location.y = (point1.transform.position.y - point2.transform.position.y) / 2;
				location.z = 0;
				transform.position = location;
			}
		}
	}
}
