using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DroneController))]
public class PlayerScript : MonoBehaviour
{
	[Header("Player Specs")]
	public float maxHealth = 100.0f;
	public float maxAmmo = 10.0f;
	public float maxBoost = 1.2f;
	public bool godmode = false;
	public bool infiniteAmmo = false;
	public bool infiniteBoost = false;
	public Weapon currentWeapon;

	private float health;
	private float ammo;
	private float boost;

	[Space]
	public GameObject dieEffect;
	public GameObject waterSplash;
	public GameObject wakeSplash;
	public float wakeSplashRate = 16.0f;

	[Space]
	public Transform firePoint;

	[Space]
	public Material hurtMaterial;
	public List<SpriteRenderer> spriteRenderers;

	private Rigidbody2D rb;
	private Vector2 input = Vector2.zero;
	private DroneController droneController;
	private float wakeSplashTimer = 0.0f;
	private List<Material> materials;
	private float hurtTimer;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		droneController = GetComponent<DroneController>();
		materials = new List<Material>();

		for (int i = 0; i < spriteRenderers.Count; i++)
			materials.Add(spriteRenderers[i].material);

		currentWeapon = Weapon.MakeNewWeapon(currentWeapon);

		health = maxHealth;
		ammo = maxAmmo;
		boost = maxBoost;
	}

	void Update()
	{
		if (godmode) { health = maxHealth; }
		if (infiniteAmmo) { ammo = maxAmmo; }
		if (infiniteBoost) { boost = maxBoost; }

		currentWeapon.Handle(ref ammo, firePoint, droneController.GetGliding(), rb, null);

		boost = droneController.GetBoosting() ? Mathf.Max(0, boost - Time.deltaTime) : boost;
		boost = droneController.GetGliding() ? Mathf.Min(maxBoost, boost + Time.deltaTime) : boost;

		transform.Find("BoostMeter").localScale = new Vector3(0.4f, (boost / maxBoost) * 10, 1);
		transform.Find("AmmoMeter").localScale = new Vector3(0.4f, (ammo / maxAmmo) * 10, 1);

		transform.Find("ReloadMeter").gameObject.SetActive(currentWeapon.weaponType == Weapon.WeaponType.summon);
		if (currentWeapon.GetLoadTime() > 0.0f && currentWeapon.GetFireCooldown() == 0)
		{
			transform.Find("ReloadMeter").localScale = new Vector3((1 - currentWeapon.GetLoadTime() / currentWeapon.loadTime) * 10, 0.4f, 1);
		}
		else
		{
			transform.Find("ReloadMeter").localScale = new Vector3((currentWeapon.GetFireCooldown() / (1.0f / currentWeapon.firerate)) * 10, 0.4f, 1);
		}

		transform.Find("Healthbar").gameObject.SetActive(health != maxHealth);
		transform.Find("Healthbar").localScale = new Vector3((health / maxHealth) * 10, 0.4f, 1);

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

		if (hurtMaterial)
		{
			if (hurtTimer > 0.0f)
			{
				if (spriteRenderers[0].material != hurtMaterial)
				{
					for (int i = 0; i < spriteRenderers.Count; i++)
					{
						spriteRenderers[i].material = hurtMaterial;
					}
				}
			}
			else
			{
				if (spriteRenderers[0].material != materials[0])
				{
					for (int i = 0; i < spriteRenderers.Count; i++)
					{
						spriteRenderers[i].material = materials[i];
					}
				}
			}

			hurtTimer = Mathf.Max(0, hurtTimer - Time.deltaTime);
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
				float _health = health;
				health = Mathf.Max(0, health - hitbox.Hit(Hitbox.HitboxSource.player, other.transform.position));
				if (_health != health && hurtTimer < hitbox.hurtTime)
					hurtTimer = hitbox.hurtTime;
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
