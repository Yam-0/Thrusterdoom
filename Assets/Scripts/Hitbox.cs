using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
	public HitboxType hitboxType = HitboxType.player;
	public float damage;
	public bool instaKill;

	public enum HitboxType
	{
		player,
		enemy,
		both
	}
}
