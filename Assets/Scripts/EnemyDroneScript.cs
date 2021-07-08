using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneController))]
public class EnemyDroneScript : MonoBehaviour
{
	[Header("Enemy Specs")]
	public float maxHealth = 100.0f;
	private float health;

	[Space]
	public DroneAiType ai;
	public Weapon currentWeapon;
	public GameObject dieEffect;
	public Transform firePoint;
	public GameObject wreckage;
	public float killShakeDuration = 0.3f;
	public float killShakeIntensity = 0.4f;
	public TurretScript turretScript;

	private Rigidbody2D rb;
	private Vector2 input = Vector2.zero;
	private DroneController droneController;
	private GameObject player;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		droneController = GetComponent<DroneController>();
		player = GameObject.FindGameObjectWithTag("Player");

		currentWeapon = Weapon.MakeNewWeapon(currentWeapon);

		health = maxHealth;
	}

	void Update()
	{
		switch (ai)
		{
			case DroneAiType.chaser:
				ChaserAi();
				break;
			case DroneAiType.cruiser:
				CruiserAi();
				break;
			case DroneAiType.dart:
				DartAi();
				break;
		}

		if (health <= 0)
		{
			Camera.main.GetComponent<CameraScript>().Shake(killShakeDuration, killShakeIntensity);
			if (dieEffect != null)
				Instantiate(dieEffect, transform.position, Quaternion.identity);
			if (wreckage != null)
				Instantiate(wreckage, transform.position, transform.rotation).GetComponent<ProjectileScript>().SetInitialVelocity(rb.velocity);
			Destroy(gameObject, 0);
		}
	}

	void ChaserAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, droneController.GetGliding(), rb, null);

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
	}

	void CruiserAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, droneController.GetGliding(), rb, turretScript);

		Vector2 deltaPosition = player.transform.position - transform.position;
		Vector2 playerOffset = new Vector2(0.0f, 10.0f);
		deltaPosition += playerOffset;

		float toTargetAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		Vector2 direction = droneController.GetLookDirection();
		float lookAngle = Mathf.Atan2(direction.y, direction.x);
		float deltaAngle = lookAngle - toTargetAngle;
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1;
		input.x = deltaAngle > 0 ? 1 : -1;

		droneController.SetGlideInput(Mathf.Abs(deltaAngle) < 30 * Mathf.Rad2Deg && distance < 10.0f);
		droneController.SetMoveInput(input.normalized);

		//Rotate towards x move direction
		float angle = droneController.GetMoveDirection().x * -30.0f;
		transform.rotation = Quaternion.Euler(0, 0, angle);
	}

	void DartAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, droneController.GetGliding(), rb, null);

		Vector2 deltaPosition = player.transform.position - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		Vector2 direction = droneController.GetLookDirection();
		float lookAngle = Mathf.Atan2(direction.y, direction.x);
		float deltaAngle = lookAngle - toPlayerAngle;
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1;
		input.x = deltaAngle > 0 ? 1 : -1;

		droneController.SetGlideInput(Mathf.Abs(deltaAngle) < 30 * Mathf.Rad2Deg && distance < 10.0f);
		droneController.SetMoveInput(input.normalized);
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

	public enum DroneAiType
	{
		chaser,
		cruiser,
		dart
	}
}
