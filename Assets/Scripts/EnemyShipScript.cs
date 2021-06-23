using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class EnemyShipScript : MonoBehaviour
{
	[Header("Enemy Specs")]
	public float maxHealth = 240.0f;
	private float health;

	[Space]
	public ShipAiType ai;
	public Weapon currentWeapon;
	public GameObject dieEffect;
	public Transform firePoint; //idfk temp lamo
	public GameObject wreckage;
	public float killShakeDuration = 0.3f;
	public float killShakeIntensity = 0.4f;

	private Rigidbody2D rb;
	private Vector2 input = Vector2.zero;
	private ShipController shipController;
	private GameObject player;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		shipController = GetComponent<ShipController>();
		player = GameObject.FindGameObjectWithTag("Player");

		health = maxHealth;
	}

	void Update()
	{
		switch (ai)
		{
			case ShipAiType.enforcer:
				EnforcerAi();
				break;
		}

		if (health <= 0)
		{
			Camera.main.GetComponent<CameraScript>().Shake(killShakeDuration, killShakeIntensity);
			if (dieEffect != null)
				Instantiate(dieEffect, transform.position, Quaternion.identity);
			if (wreckage != null)
				Instantiate(wreckage, transform.position, transform.rotation);
			Destroy(gameObject, 0);
		}
	}

	void EnforcerAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, true, rb);

		Vector2 deltaPosition = player.transform.position - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1;
		input.x = (deltaPosition.x > 0 ? 1 : -1) / distance;

		shipController.SetMoveInput(input.normalized);
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

	public enum ShipAiType
	{
		enforcer
	}
}
