using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGroup : MonoBehaviour
{
	public string spawnMessage1;
	public string spawnMessage2;
	public string spawnMessage3;

	void Update()
	{
		if (transform.childCount <= 0)
		{
			Game.Instance.LevelComplete();
			Destroy(gameObject, 0.0f);
		}
	}
}
