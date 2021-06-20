using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
	public float time = 5.0f;

	void Awake()
	{
		Destroy(gameObject, time);
	}
}
