using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneController))]
public class EnemyScript : MonoBehaviour
{
	[Header("Enemy Specs")]
	public float maxHealth = 100.0f;
	private float health;

	[Space]
	public Weapon currentWeapon;
	public GameObject dieEffect;
	public Transform firePoint;
	public GameObject wreckage;

	private Rigidbody2D rb;
	private Vector2 input = Vector2.zero;
	private DroneController droneController;
	private GameObject player;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		droneController = GetComponent<DroneController>();
		player = GameObject.FindGameObjectWithTag("Player");

		health = maxHealth;
	}

	void Update()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, droneController.GetGliding(), rb);

		Vector2 deltaPosition = player.transform.position - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		Vector2 direction = droneController.GetLookDirection();
		float lookAngle = Mathf.Atan2(direction.y, direction.x);
		float deltaAngle = lookAngle - toPlayerAngle;
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1;
		input.x = deltaAngle;

		droneController.SetGlideInput(Mathf.Abs(deltaAngle) < 30 * Mathf.Rad2Deg && distance < 10.0f);
		droneController.SetMoveInput(input.normalized);

		if (health <= 0)
		{
			Camera.main.GetComponent<CameraScript>().Shake(0.3f, 0.4f);
			Instantiate(dieEffect, transform.position, Quaternion.identity);
			Instantiate(wreckage, transform.position, transform.rotation).GetComponent<ProjectileScript>().SetInitialVelocity(rb.velocity);
			Destroy(gameObject, 0);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Hitbox")
		{
			Hitbox hitbox;
			if (other.gameObject.TryGetComponent<Hitbox>(out hitbox))
			{
				health = Mathf.Max(0, health - hitbox.Hit(Hitbox.HitboxSource.enemy, other.transform.position));
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Hitbox")
		{
			Hitbox hitbox;
			if (other.gameObject.TryGetComponent<Hitbox>(out hitbox))
			{
				health = Mathf.Max(0, health - hitbox.Hitting(Hitbox.HitboxSource.enemy, transform.position));
			}
		}
	}
}
