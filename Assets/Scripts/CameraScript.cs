using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	public GameObject trackObject;
	public Vector3 offset;
	public float preMoveDistance;

	void Start()
	{

	}

	void Update()
	{
		Vector3 preMovePositionOffset = trackObject.GetComponent<Rigidbody2D>().velocity * preMoveDistance;
		transform.position = trackObject.transform.position + offset + preMovePositionOffset;
	}
}
