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
	public GameObject fireEffect;

	private Rigidbody2D rb;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		float angle = transform.rotation.eulerAngles.z;
		Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
		Vector2 velocity = direction * speed;
		velocity = inheritVelocity ? velocity + GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity : velocity;
		rb.velocity = velocity;
		rb.gravityScale = gravityScale;
		Instantiate(fireEffect, transform.position, transform.rotation);
		Destroy(gameObject, maxTime);
	}

	void Update()
	{
		if (rotate)
		{
			float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0, 0, angle);
		}
	}
}
