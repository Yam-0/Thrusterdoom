using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public PickupType pickupType = PickupType.ammo;
	public float amount = 10.0f;

	public enum PickupType
	{
		health,
		boost,
		ammo
	}
}
