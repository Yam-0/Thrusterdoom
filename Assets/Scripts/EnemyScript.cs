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

	private Vector2 input = Vector2.zero;
	private Transform objectTransform;
	private DroneController droneController;
	private float fireCooldown;
	private bool firing;
	private GameObject weaponReference;
	Transform firePoint;

	private GameObject player;

	void Start()
	{
		droneController = GetComponent<DroneController>();
		objectTransform = transform.Find("object");
		firePoint = objectTransform.Find("Body").Find("firePoint");
		health = maxHealth;
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update()
	{
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

		if (droneController.GetGliding())
		{
			if (fireCooldown == 0)
			{
				Instantiate(currentWeapon.weapon, firePoint.position, firePoint.rotation);
				fireCooldown = 1.0f / currentWeapon.firerate;
			}

			fireCooldown = Mathf.Max(0, fireCooldown - Time.deltaTime);
		}

		if (health <= 0)
		{
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
				if (hitbox.hitboxType != Hitbox.HitboxType.enemy)
				{
					health = Mathf.Max(0, health - hitbox.damage);
					Destroy(other.gameObject, 0);
					Debug.Log("Enemy hit for " + hitbox.damage + " damage.");
				}
			}
		}
	}
}
