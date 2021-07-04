﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	public GameObject trackObject;
	public Vector3 offset;
	public CameraTrackingType cameraTrackingType;
	public float preMoveDistance;
	public bool smoothTrack = false;

	private bool lockedOn = true;
	private float shakeIntensity;
	private float shakeTime;
	private Vector3 shakeOffset;

	private Vector2 smoothMoveFromPos, smoothMoveToPos;
	private float smoothMoveFromRot, smoothMoveToRot;
	private bool smoothMoving = false;
	private float smoothMoveTime;

	void Start()
	{
		Time.timeScale = 1.0f;
	}

	void Update()
	{
		if (lockedOn)
		{
			Vector3 preMovePositionOffset = Vector3.zero;
			Vector2 moveDirection;

			switch (cameraTrackingType)
			{
				case CameraTrackingType.velocity:
					preMovePositionOffset = trackObject.GetComponent<Rigidbody2D>().velocity * preMoveDistance;
					break;
				case CameraTrackingType.direction:
					moveDirection = trackObject.GetComponent<DroneController>().GetLookDirection();
					preMovePositionOffset = moveDirection * preMoveDistance * trackObject.GetComponent<Rigidbody2D>().velocity.magnitude;
					break;
				case CameraTrackingType.combined:
					moveDirection = trackObject.GetComponent<DroneController>().GetLookDirection();
					Vector3 preMovePositionOffsetVelocity = trackObject.GetComponent<Rigidbody2D>().velocity * preMoveDistance;
					Vector3 preMovePositionOffsetDirection = moveDirection * preMoveDistance * trackObject.GetComponent<Rigidbody2D>().velocity.magnitude;
					preMovePositionOffset = (preMovePositionOffsetVelocity + preMovePositionOffsetDirection * 2) / 2;
					break;
			}

			Vector3 targetPos = trackObject.transform.position + offset + preMovePositionOffset + shakeOffset;
			Vector3 moveDelta = (targetPos - transform.position) * Time.deltaTime * 3;
			Vector3 newPos = transform.position + moveDelta;
			if (smoothTrack)
			{
				transform.position = newPos;
			}
			else
			{
				transform.position = targetPos;
			}

			Debug.DrawLine(transform.position, targetPos, new Color(255, 0, 0, 255));
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			LockOff();
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

		if (smoothMoving && smoothMoveTime > 0)
		{
			smoothMoveTime = Mathf.Max(0, smoothMoveTime - Time.deltaTime);
			//transform.position = 
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

	public void LockOn()
	{
		lockedOn = true;
		smoothMoving = false;
	}

	public void LockOff()
	{
		lockedOn = false;
	}

	public void SmoothMoveTo(Vector2 position, float rotation, float time)
	{
		LockOff();
		smoothMoveFromPos = transform.position;
		smoothMoveFromRot = transform.rotation.eulerAngles.z;
		smoothMoveToPos = position;
		smoothMoveToRot = rotation;
		smoothMoveTime = time;
		smoothMoving = true;
	}

	public enum CameraTrackingType
	{
		direction,
		velocity,
		combined
	}
}
