﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private Vector2 input = Vector2.zero;
	private Transform objectTransform;
	private DroneController droneController;
	private float fireCooldown;
	private bool firing;
	private GameObject weaponReference;
	Transform firePoint;


	void Start()
	{
		droneController = GetComponent<DroneController>();
		objectTransform = transform.Find("object");
		firePoint = objectTransform.Find("Body").Find("firePoint");
		health = maxHealth;
		ammo = maxAmmo;
		boost = maxBoost;
	}

	void Update()
	{
		HandleFiring();

		boost = droneController.GetBoosting() ? Mathf.Max(0, boost - Time.deltaTime) : boost;
		boost = droneController.GetGliding() ? Mathf.Min(maxBoost, boost + Time.deltaTime) : boost;
		transform.Find("BoostMeter").localScale = new Vector3(0.6f, (boost / maxBoost) * 10, 1);
		transform.Find("AmmoMeter").localScale = new Vector3(0.6f, (ammo / maxAmmo) * 10, 1);

		input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		input.y = Mathf.Max(0, input.y);
		droneController.SetMoveInput(input);
		droneController.SetBoostingInput(Input.GetKey(KeyCode.LeftShift) && boost > 0);
		droneController.SetGlideInput(Input.GetKey(KeyCode.Space));
	}

	void HandleFiring()
	{
		switch (currentWeapon.weaponType)
		{
			case Weapon.WeaponType.active:
				firing = ammo > 0 && droneController.GetGliding();

				if (firing)
				{
					ammo = Mathf.Max(0, ammo - currentWeapon.ammoCost * Time.deltaTime);
				}

				if (firing && weaponReference == null)
				{
					if (firePoint != null)
						weaponReference = Instantiate(currentWeapon.weapon, firePoint.position, firePoint.rotation, firePoint);
				}

				if (!firing && weaponReference != null)
				{
					Destroy(weaponReference, 0);
				}
				break;

			case Weapon.WeaponType.summon:
				firing = ammo > 0 && droneController.GetGliding();

				if (fireCooldown == 0)
				{
					ammo = Mathf.Max(0, ammo - currentWeapon.ammoCost);
					Instantiate(currentWeapon.weapon, firePoint.position, firePoint.rotation);
				}

				fireCooldown = Mathf.Min(0, fireCooldown - Time.deltaTime);
				break;

			default:
				break;
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Pickup")
		{
			Pickup pickup;
			if (other.gameObject.TryGetComponent<Pickup>(out pickup))
			{
				switch (pickup.pickupType)
				{
					case Pickup.PickupType.health:
						health += pickup.amount;
						break;
					case Pickup.PickupType.boost:
						boost += pickup.amount;
						break;
					case Pickup.PickupType.ammo:
						break;
				}
			}
		}

		if (other.gameObject.tag == "Hitbox")
		{
			Hitbox hitbox;
			if (other.gameObject.TryGetComponent<Hitbox>(out hitbox))
			{
				if (hitbox.hitboxType != Hitbox.HitboxType.player)
				{
					health = Mathf.Max(0, health - hitbox.damage);
					Debug.Log("Player hit for " + hitbox.damage + " damage.");
				}
			}
		}
	}
}
