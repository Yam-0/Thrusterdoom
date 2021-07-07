using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGroup : MonoBehaviour
{
	void Start()
	{

	}

	void Update()
	{
		if (transform.childCount <= 0)
		{
			Game.LevelComplete();
			Destroy(gameObject, 0.0f);
		}
	}
}
