using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public WeaponType weaponType;
	public GameObject weapon;
	public float ammoCost;
	public float firerate;

	public enum WeaponType
	{
		none,
		summon,
		active
	}
}
