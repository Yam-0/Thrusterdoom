using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileScript : MonoBehaviour
{
	public float speed = 10.0f;
	public float gravityScale = 0.0f;
	public bool rotate;
	public bool inheritVelocity;
	public float maxTime = 6.0f;
	public bool destroyOnHit = true;
	public GameObject fireEffect;

	private Rigidbody2D rb;
	private Vector2 initialVelocity;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		float angle = transform.rotation.eulerAngles.z;
		Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
		rb.velocity = direction * speed;
		rb.gravityScale = gravityScale;
		Instantiate(fireEffect, transform.position, transform.rotation);
		Destroy(gameObject, maxTime);
	}

	void Update()
	{
		if (inheritVelocity)
		{
			rb.velocity += initialVelocity;
			inheritVelocity = false;
		}
		if (rotate)
		{
			float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0, 0, angle);
		}
	}

	public void SetInitialVelocity(Vector2 velocity)
	{
		initialVelocity = velocity;
	}
}
