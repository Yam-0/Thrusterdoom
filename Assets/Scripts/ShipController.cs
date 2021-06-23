using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	[Header("Ship Specs")]
	public float speed = 2.0f;
	public float maxSpeed = 12.0f;

	[Space]
	[Header("Ship Effects")]
	public ParticleSystem engine;
	public GameObject enginePuff;
	public Transform wakePoint;

	private Rigidbody2D rb;
	private Vector2 moveInput = Vector2.zero;
	private Vector2 velocity = Vector2.zero;

	private bool moving;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{

	}

	void FixedUpdate()
	{
		HandleMovement();
	}

	void HandleMovement()
	{
		velocity.x = Mathf.Clamp(velocity.x + (moveInput.x * speed), -maxSpeed, maxSpeed);
		rb.velocity = velocity;
	}

	public void SetMoveInput(Vector2 axis)
	{
		if (Mathf.Abs(moveInput.x) < 0.1f && Mathf.Abs(axis.x) > 0.1f)
		{
			Instantiate(enginePuff, wakePoint.position, wakePoint.rotation);
		}

		moveInput = axis;
	}

	public Vector2 GetMoveInput()
	{
		return moveInput;
	}

	public Vector2 GetVelocity()
	{
		return velocity;
	}

	public Vector2 GetMoveDirection()
	{
		return velocity.normalized;
	}

	public bool GetMoving()
	{
		return moving;
	}
}
