using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	[Header("Parallax Settings")]
	public bool parallax = true;
	[Range(-2.0f, 2.0f)]
	public float movementMultiplierX = 1.0f;
	[Range(-2.0f, 2.0f)]
	public float movementMultiplierY = 1.0f;

	[Space]
	public bool wrap = false;
	public float wrapDistance = 50.0f;

	[Space]
	public bool stretching = false;
	public GameObject point1;
	public GameObject point2;

	[Space]
	public bool track = false;
	public bool trackX = false;
	public bool trackY = false;

	[Space]
	public bool move = false;
	public float speedX = 0.0f;
	public float speedY = 0.0f;

	private Vector2 movementMultiplier;
	private Vector3 initialPosition;
	private Vector3 initialScale;
	private GameObject trackObject;

	void Start()
	{
		initialPosition = transform.position;
		initialScale = transform.localScale;
		trackObject = GameObject.FindGameObjectWithTag("MainCamera");
		movementMultiplier = new Vector2(movementMultiplierX, movementMultiplierY);
	}

	void LateUpdate()
	{
		if (trackObject != null)
		{
			if (parallax)
			{
				Vector3 moveOffset = (trackObject.transform.position * movementMultiplier);
				moveOffset.z = 0;
				transform.position = initialPosition + moveOffset;
			}

			if (wrap)
			{
				Vector3 deltaPos = transform.position - trackObject.transform.position;
				float offset = (wrapDistance * 2);
				if (deltaPos.x > wrapDistance)
				{
					initialPosition.x -= offset;
				}
				if (deltaPos.x < -wrapDistance)
				{
					initialPosition.x += offset;
				}
			}

			Vector3 location = new Vector3();
			if (stretching)
			{
				float height = (point1.transform.position.y - point2.transform.position.y);
				if (height < 0) { height = 0; }
				transform.localScale = new Vector3(40, height, 1);

				location.x = trackObject.transform.position.x;
				location.y = (point1.transform.position.y - point2.transform.position.y) / 2;
				location.z = 0;
				transform.position = location;
			}

			if (track)
			{
				if (trackX)
				{
					location = transform.position;
					location.x = trackObject.transform.position.x + initialPosition.x;
					transform.position = location;
				}
				if (trackY)
				{
					location = transform.position;
					location.y = trackObject.transform.position.y + initialPosition.y;
					transform.position = location;

				}
			}

			if (move)
			{
				Vector2 delta = new Vector2(speedX, speedY);
				delta *= Time.deltaTime;
				initialPosition += (Vector3)delta;
			}
		}
	}
}
