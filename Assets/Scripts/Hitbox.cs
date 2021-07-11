using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
	public HitboxSource hitboxSource = HitboxSource.player;
	public HitboxType hitboxType = HitboxType.summon;
	public float damage;
	public GameObject projectile;
	//public bool instaKill;
	public GameObject hitEffect;
	public GameObject hitWaterEffect;
	public float hitFreezeLength = 0.06f;
	public float hitShakeLength;
	public float hitShakeIntensity;
	public float hurtTime = 0.1f;

	public float Hit(HitboxSource self, Vector2 hitPoint)
	{
		if (hitboxSource != self && hitboxType == HitboxType.summon && hitboxSource != HitboxSource.none)
		{
			if (projectile.GetComponent<ProjectileScript>().destroyOnHit)
				Destroy(projectile, 0);
			Instantiate(hitEffect, hitPoint, Quaternion.identity);
			Camera.main.GetComponent<CameraScript>().Shake(hitShakeLength, hitShakeIntensity);
			Camera.main.GetComponent<CameraScript>().Freeze(hitFreezeLength);
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			float damageMultiplier = 1;
			if (player != null)
				damageMultiplier = player.GetComponent<PlayerScript>().damageMultiplier;
			Debug.Log(self.ToString() + " hit for " + damage + " x " + damageMultiplier + " damage.");
			float totalDamage = hitboxSource != HitboxSource.player ? damage : damage * damageMultiplier;
			Game.Instance.DamagedEnemy(totalDamage);
			return totalDamage;
		}

		return 0;
	}

	public float Hitting(HitboxSource self, Vector2 hitPoint)
	{
		if (hitboxSource != self && hitboxType == HitboxType.active && hitboxSource != HitboxSource.none)
		{
			Camera.main.GetComponent<CameraScript>().Shake(hitShakeLength, hitShakeIntensity);
			Instantiate(hitEffect, hitPoint, Quaternion.identity);
			float frameDamage = (damage * Time.deltaTime);
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			float damageMultiplier = 1;
			if (player != null)
				damageMultiplier = player.GetComponent<PlayerScript>().damageMultiplier;
			Debug.Log(self.ToString() + " hit for " + frameDamage + " x " + damageMultiplier + " frame damage.");
			float totalDamage = hitboxSource != HitboxSource.player ? frameDamage : frameDamage * damageMultiplier;
			Game.Instance.DamagedEnemy(totalDamage);
			return totalDamage;
		}

		return 0;
	}

	public void HitWater()
	{
		if (hitboxType == HitboxType.summon)
		{
			Camera.main.GetComponent<CameraScript>().Shake(hitShakeLength, hitShakeIntensity / 2);
			Instantiate(hitWaterEffect, transform.position, Quaternion.identity);
			Destroy(projectile, 0);
		}
	}

	public enum HitboxSource
	{
		player,
		enemy,
		both,
		none
	}

	public enum HitboxType
	{
		summon,
		active
	}
}
