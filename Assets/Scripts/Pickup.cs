using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pickup : MonoBehaviour
{
	public PickupType pickupType = PickupType.ammo;
	public float amount = 10.0f;
	public GameObject pickupEffect;

	public void Trigger()
	{
		Instantiate(pickupEffect, transform.position, Quaternion.identity);
		Destroy(gameObject, 0);
	}

	public enum PickupType
	{
		health,
		boost,
		ammo
	}
}
