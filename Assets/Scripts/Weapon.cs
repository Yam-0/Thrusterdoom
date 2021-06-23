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

					if (weaponReference != null)
					{
						weaponReference.transform.position = firePoint.transform.position;
						weaponReference.transform.rotation = firePoint.transform.rotation;
					}
				}

				if (firing && weaponReference == null)
				{
					if (firePoint != null)
					{
						weaponReference = Instantiate(weapon, firePoint.position, firePoint.rotation);
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
					GameObject projectile = Instantiate(weapon, firePoint.position, firePoint.rotation);
					ProjectileScript projectileScript;
					if (TryGetComponent<ProjectileScript>(out projectileScript))
					{
						projectileScript.SetInitialVelocity(rb.velocity);
					}

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
