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

	public float Hit(HitboxSource self, Vector2 hitPoint)
	{
		if (hitboxSource != self && hitboxType == HitboxType.summon)
		{
			Instantiate(hitEffect, hitPoint, Quaternion.identity);
			Destroy(projectile, 0);
			Debug.Log(self.ToString() + " hit for " + damage + " damage.");
			return damage;
		}

		return 0;
	}

	public float Hitting(HitboxSource self, Vector2 hitPoint)
	{
		if (hitboxSource != self && hitboxType == HitboxType.active)
		{
			Instantiate(hitEffect, hitPoint, Quaternion.identity);
			float frameDamage = (damage * Time.deltaTime);
			Debug.Log(self.ToString() + " hit for " + frameDamage + " damage. ");
			return frameDamage;
		}

		return 0;
	}

	public enum HitboxSource
	{
		player,
		enemy,
		both
	}

	public enum HitboxType
	{
		summon,
		active
	}
}
