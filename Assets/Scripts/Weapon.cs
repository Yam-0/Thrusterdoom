using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public WeaponType weaponType;
	public GameObject weapon;
	public float ammoCost;
	public float firerate;
	public float fireShakeLength;
	public float fireShakeIntensity;

	private GameObject weaponReference;
	private float fireCooldown;
	private bool firing;

	public bool Handle(ref float ammo, Transform firePoint, bool tryFire, Rigidbody2D rb)
	{
		switch (weaponType)
		{
			case Weapon.WeaponType.active:
				firing = ammo > 0 && tryFire;

				if (firing)
				{
					Camera.main.GetComponent<CameraScript>().Shake(fireShakeLength, fireShakeIntensity);
					ammo = Mathf.Max(0, ammo - ammoCost * Time.deltaTime);
				}

				if (firing && weaponReference == null)
				{
					if (firePoint != null)
					{
						weaponReference = Instantiate(weapon, firePoint.position, firePoint.rotation, firePoint);
					}
				}

				if (!firing && weaponReference != null)
				{
					Destroy(weaponReference, 0);
				}
				break;

			case Weapon.WeaponType.summon:
				firing = ammo > 0 && tryFire;

				if (firing && fireCooldown == 0)
				{
					ammo = Mathf.Max(0, ammo - ammoCost);
					Camera.main.GetComponent<CameraScript>().Shake(fireShakeLength, fireShakeIntensity);
					Instantiate(weapon, firePoint.position, firePoint.rotation).GetComponent<ProjectileScript>().SetInitialVelocity(rb.velocity);
					fireCooldown = 1.0f / firerate;
				}

				fireCooldown = Mathf.Max(0, fireCooldown - Time.deltaTime);
				break;

			default:
				break;
		}

		return firing;
	}

	public float GetFireCooldown()
	{
		return fireCooldown;
	}

	public enum WeaponType
	{
		none,
		summon,
		active
	}
}
