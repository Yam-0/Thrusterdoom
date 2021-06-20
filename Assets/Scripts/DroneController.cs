using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DroneController : MonoBehaviour
{
	[Header("Drone Specs")]
	public float maxHealth = 100.0f;
	public float maxBoost = 1.2f;
	public float thrust = 10.0f;
	public float drag = 0.4f;
	public float rotationSpeed = 4.5f;
	public float maxSpeed = 12.0f;
	public float sideThrust = 0.4f;
	public float nitroBoostMult = 2.0f;
	public float nitroTurnMult = 0.3f;
	public float glideTurnMult = 0.7f;

	[Space]
	[Header("Drone Effects")]
	public ParticleSystem rocket;
	public GameObject thrustPuff;

	[Space]
	[Header("Effect Colors")]
	public Color trailNormal;
	public Color goldingColor;
	public Color nitroColor;

	[Space]
	[Header("Other")]
	public GameObject laser;

	private ParticleSystem.MainModule rocketSettings;
	private ParticleSystem.EmissionModule rocketEmission;

	private Rigidbody2D rb;
	private Transform tr;
	private Transform objectTransform;

	private Vector2 moveInput = Vector2.zero;
	private Vector2 moveDirection = Vector2.zero;
	private Vector2 lookDirection = Vector2.zero;
	private float lookAngle = 0.0f;
	private bool boostInput = false;
	private float health;
	private float boost;

	private bool golding = false;
	private bool boosting = false;
	private bool gliding = false;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		objectTransform = transform.Find("object");

		rocketSettings = rocket.main;
		rocketEmission = rocket.emission;

		health = maxHealth;
		boost = maxBoost;
	}

	void Update()
	{
		Color lookColor = Color.blue;
		if (golding) { lookColor = Color.yellow; }

		Debug.DrawLine(transform.position, transform.position + new Vector3(moveDirection.x, moveDirection.y) * 3.0f, lookColor);
		Debug.DrawLine(transform.position, transform.position + new Vector3(lookDirection.x, lookDirection.y) * 4.0f, Color.red);

		rocketSettings.startColor = trailNormal;
		rocketSettings.startSpeedMultiplier = 1.0f;
		rocketEmission.rateOverTime = 100 * moveInput.y;

		if (!gliding)
		{
			if (boosting)
			{
				rocketSettings.startColor = nitroColor;
				rocketSettings.startSpeedMultiplier = 2.0f;
			}
			if (golding)
			{
				rocketSettings.startColor = goldingColor;
				rocketSettings.startSpeedMultiplier = 2.0f;
			}
		}

		boosting = (boost > 0 && boostInput && !gliding && moveInput.y > 0.1f);
		boost = boosting ? Mathf.Max(0, boost - Time.deltaTime) : boost;
		transform.Find("BoostMeter").localScale = new Vector3(0.6f, (boost / maxBoost) * 10, 1);
		if (gliding)
		{
			boost = Mathf.Min(maxBoost, boost + Time.deltaTime);
		}

		laser.SetActive(gliding);
	}

	void FixedUpdate()
	{
		HandleMovement();
	}

	public void HandleMovement()
	{
		rb.drag = drag;
		float rotateAmount = rotationSpeed * moveInput.x;

		if (gliding)
		{
			lookAngle -= rotateAmount * glideTurnMult;
		}
		else if (boosting)
		{
			lookAngle -= rotateAmount * nitroTurnMult;
		}
		else
		{
			lookAngle -= rotateAmount;
		}

		lookDirection = new Vector2(Mathf.Cos(lookAngle * Mathf.Deg2Rad), Mathf.Sin(lookAngle * Mathf.Deg2Rad));
		golding = false;
		Vector2 offset = Vector2.zero;

		for (int i = 0; i < objectTransform.childCount; i++)
		{
			GameObject child = objectTransform.GetChild(i).gameObject;
			child.transform.rotation = Quaternion.Euler(0, 0, lookAngle);
		}

		float dot = Vector2.Dot(lookDirection, moveDirection);
		float thrustMult = 1.0f;
		if (boosting)
		{
			thrustMult *= nitroBoostMult;
		}

		if (dot < 0)
		{
			golding = true;
			offset -= moveDirection * sideThrust * moveInput.y;
			thrustMult = 2.0f;
		}

		if (!gliding)
			rb.AddForce((lookDirection * thrust) * (moveInput.y * thrustMult) + offset);

		if (rb.velocity.magnitude > maxSpeed)
			rb.velocity = rb.velocity.normalized * maxSpeed;

		moveDirection = rb.velocity.normalized;
	}

	public void SetMoveInput(Vector2 axis)
	{
		if (gliding)
		{
			axis.y = 0;
		}

		if (moveInput.y < 0.1f && axis.y > 0.1f)
		{
			Instantiate(thrustPuff, transform.position, objectTransform.GetChild(0).rotation);
		}

		moveInput = axis;
	}

	public void SetBoostingInput(bool boostInput)
	{
		this.boostInput = boostInput;
	}

	public void SetGlideInput(bool gliding)
	{
		this.gliding = gliding;
	}

	public Vector2 GetMoveDirection()
	{
		return moveDirection;
	}

	public Vector2 GetLookDirection()
	{
		return lookDirection;
	}
}
