using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileScript : MonoBehaviour
{
	public float speed = 10.0f;
	public float gravityScale = 0.0f;
	public float maxTime = 6.0f;
	public GameObject timeoutExplosion;
	public bool rotate;
	public bool inheritVelocity;
	public bool destroyOnHit = true;
	public bool homing;
	public float homingStrength;

	private Rigidbody2D rb;
	private Vector2 initialVelocity;
	private GameObject homingTarget;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = transform.right * speed;
		rb.gravityScale = gravityScale;
		Invoke("SelfDestruct", maxTime);
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

		if (homing && homingTarget != null)
		{
			Vector3 dir = homingTarget.transform.position - transform.position;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * homingStrength);
		}

		rb.velocity = transform.right * rb.velocity.magnitude;
	}

	void SelfDestruct()
	{
		if (timeoutExplosion != null)
			Instantiate(timeoutExplosion, transform.position, transform.rotation);
		Destroy(gameObject, 0.0f);
	}

	public void SetInitialVelocity(Vector2 velocity)
	{
		initialVelocity = velocity;
	}

	public void SetTarget(GameObject target)
	{
		homingTarget = target;
	}
}
