using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 10)]
public class Weapon : ScriptableObject
{
	public WeaponType weaponType;
	public GameObject weapon;
	public float ammoCost;
	public GameObject fireEffect;
	public float recoilForce = 0.0f;
	public float firerate;
	public float fireShakeLength;
	public float fireShakeIntensity;
	public float spread = 0.0f;
	public float loadTime = 0.0f;
	public string soundEffect;

	private GameObject weaponReference;
	private float fireCooldown;
	private float loadTimer;
	private bool loadHolding;
	private bool firing;

	public bool Handle(ref float ammo, Transform firePoint, bool tryFire, Rigidbody2D rb, TurretScript turretScript)
	{
		switch (weaponType)
		{
			case Weapon.WeaponType.active:
				firing = ammo > 0 && tryFire;

				if (firing)
				{
					Camera.main.GetComponent<CameraScript>().Shake(fireShakeLength, fireShakeIntensity);
					ammo = Mathf.Max(0, ammo - ammoCost * Time.deltaTime);

					float angle = firePoint.rotation.eulerAngles.z * Mathf.Deg2Rad;
					Vector2 fireDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
					Vector2 recoilDirection = -fireDirection;
					rb.AddForce(recoilDirection * recoilForce * Time.deltaTime);

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
				if (loadTimer <= 0 && fireCooldown == 0) { loadTimer = loadTime; }
				firing = ammo > 0 && tryFire;
				if (turretScript != null)
				{
					if (turretScript.limitFire)
					{
						firing = firing && turretScript.GetWithinRange();
					}
				}

				if (tryFire && loadHolding)
				{
					loadTimer = Mathf.Max(0, loadTimer - Time.deltaTime);
				}
				else
				{
					loadTimer = loadTime;
				}

				if (loadTimer > 0.0f)
				{
					firing = false;
				}
				loadHolding = tryFire;

				if (firing && fireCooldown == 0)
				{
					ammo = Mathf.Max(0, ammo - ammoCost);
					float fireRotation = firePoint.rotation.eulerAngles.z;
					float spreadDelta = Random.Range(-spread / 2, spread / 2);
					fireRotation += spreadDelta;

					Camera.main.GetComponent<CameraScript>().Shake(fireShakeLength, fireShakeIntensity);
					Instantiate(fireEffect, firePoint.position, firePoint.rotation);
					if (turretScript != null)
					{
						turretScript.Fire();
					}

					float angle = fireRotation * Mathf.Deg2Rad;
					Vector2 fireDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
					Vector2 recoilDirection = -fireDirection;
					rb.AddForce(recoilDirection * recoilForce);

					if (soundEffect != null)
						AudioManager.Instance.PlaySfx(soundEffect);

					GameObject projectile = Instantiate(weapon, firePoint.position, Quaternion.Euler(0, 0, fireRotation));
					ProjectileScript projectileScript;
					if (projectile.TryGetComponent<ProjectileScript>(out projectileScript))
					{
						projectileScript.SetInitialVelocity(rb.velocity);
						projectileScript.SetTarget(GameObject.FindGameObjectWithTag("Player"));
					}

					loadTimer = loadTime;

					fireCooldown = 1.0f / firerate;
				}

				fireCooldown = Mathf.Max(0, fireCooldown - Time.deltaTime);
				break;

			default:
				break;
		}

		return firing;
	}

	public static Weapon MakeNewWeapon(Weapon weapon)
	{
		Weapon newWeapon = Weapon.CreateInstance<Weapon>();
		newWeapon.weaponType = weapon.weaponType;
		newWeapon.weapon = weapon.weapon;
		newWeapon.ammoCost = weapon.ammoCost;
		newWeapon.fireEffect = weapon.fireEffect;
		newWeapon.recoilForce = weapon.recoilForce;
		newWeapon.firerate = weapon.firerate;
		newWeapon.fireShakeLength = weapon.fireShakeLength;
		newWeapon.fireShakeIntensity = weapon.fireShakeIntensity;
		newWeapon.spread = weapon.spread;
		newWeapon.loadTime = weapon.loadTime;
		newWeapon.soundEffect = weapon.soundEffect;
		return newWeapon;
	}

	public float GetFireCooldown()
	{
		return fireCooldown;
	}

	public float GetLoadTime()
	{
		return loadTimer;
	}

	public enum WeaponType
	{
		none,
		summon,
		active
	}
}
