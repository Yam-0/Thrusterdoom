using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DroneController))]
public class PlayerScript : MonoBehaviour
{
	[Header("Player Specs")]
	public float maxHealth = 100.0f;
	private float health;
	public float maxAmmo = 10.0f;
	private float ammo;
	public float maxBoost = 1.2f;
	private float boost;

	[Space]
	public Weapon currentWeapon;
	public GameObject dieEffect;
	public Transform firePoint;
	public GameObject waterSplash;
	public GameObject wakeSplash;
	public float wakeSplashRate = 16.0f;

	private Rigidbody2D rb;
	private Vector2 input = Vector2.zero;
	private DroneController droneController;
	private float wakeSplashTimer = 0.0f;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		droneController = GetComponent<DroneController>();

		currentWeapon = Weapon.MakeNewWeapon(currentWeapon);

		health = maxHealth;
		ammo = maxAmmo;
		boost = maxBoost;
	}

	void Update()
	{
		currentWeapon.Handle(ref ammo, firePoint, droneController.GetGliding(), rb, null);

		boost = droneController.GetBoosting() ? Mathf.Max(0, boost - Time.deltaTime) : boost;
		boost = droneController.GetGliding() ? Mathf.Min(maxBoost, boost + Time.deltaTime) : boost;
		transform.Find("BoostMeter").localScale = new Vector3(0.4f, (boost / maxBoost) * 10, 1);
		transform.Find("AmmoMeter").localScale = new Vector3(0.4f, (ammo / maxAmmo) * 10, 1);
		transform.Find("ReloadMeter").gameObject.SetActive(currentWeapon.weaponType == Weapon.WeaponType.summon);
		transform.Find("ReloadMeter").localScale = new Vector3((currentWeapon.GetFireCooldown() / (1.0f / currentWeapon.firerate)) * 10, 0.4f, 1);

		input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		input.y = Mathf.Max(0, input.y);
		droneController.SetMoveInput(input);
		droneController.SetBoostingInput(Input.GetKey(KeyCode.LeftShift) && boost > 0);
		droneController.SetGlideInput(Input.GetKey(KeyCode.Space));

		float shakeIntensity = droneController.GetMoveInput().y / 20.0f;
		shakeIntensity = droneController.GetBoosting() ? shakeIntensity * 2 : 0;
		Camera.main.GetComponent<CameraScript>().Shake(0.1f, shakeIntensity);

		wakeSplashTimer = Mathf.Max(0, wakeSplashTimer - Time.deltaTime);
		if (transform.position.y <= 2.5f && transform.position.y > 0 && wakeSplashTimer == 0 && input.y > 0.1f && !droneController.GetGliding())
		{
			wakeSplashTimer = 1.0f / wakeSplashRate;
			Vector2 spawnLocation = new Vector2(transform.position.x - droneController.GetLookDirection().x * transform.position.y, 0.0f);
			Instantiate(wakeSplash, spawnLocation, Quaternion.identity);
		}

		if (health <= 0)
		{
			Game.Events.PlayerDied();
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Pickup")
		{
			Debug.Log("Collided with pickup");
			Pickup pickup;
			if (other.gameObject.TryGetComponent<Pickup>(out pickup))
			{
				bool pickedUp = false;

				switch (pickup.pickupType)
				{
					case Pickup.PickupType.health:
						if (health < maxHealth)
						{
							health = Mathf.Min(maxHealth, health + pickup.amount);
							pickedUp = true;
						}
						break;
					case Pickup.PickupType.boost:
						if (boost < maxBoost)
						{
							boost = Mathf.Min(maxBoost, boost + pickup.amount);
							pickedUp = true;
						}
						break;
					case Pickup.PickupType.ammo:
						if (ammo < maxAmmo)
						{
							ammo = Mathf.Min(maxAmmo, ammo + pickup.amount);
							pickedUp = true;
						}
						break;
				}

				if (pickedUp)
				{
					pickup.Trigger();
				}
			}
		}

		if (other.gameObject.tag == "Ocean")
		{
			Instantiate(waterSplash, transform.position, Quaternion.identity);
		}

		if (other.gameObject.tag == "Hitbox")
		{
			Hitbox hitbox;
			if (other.gameObject.TryGetComponent<Hitbox>(out hitbox))
			{
				health = Mathf.Max(0, health - hitbox.Hit(Hitbox.HitboxSource.player, other.transform.position));
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Ocean")
		{
			health -= 10 * Time.deltaTime;
		}
	}
}
