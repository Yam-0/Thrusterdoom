using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanScript : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Ocean collided with " + other.gameObject.tag);
		Hitbox hitbox;
		if (other.gameObject.TryGetComponent<Hitbox>(out hitbox))
		{
			Debug.Log("Hitbox hit water");
			hitbox.HitWater();
		}
	}
}
