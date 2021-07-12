using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointerScript : MonoBehaviour
{
	private GameObject target;
	private SpriteRenderer spriteRenderer;
	public GameObject top;
	public GameObject right;
	public GameObject bottom;
	public GameObject left;
	public float multiplier = 1.0f;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void LateUpdate()
	{
		if (target != null)
		{
			Vector3 cameraPos = Camera.main.transform.position;


			float topLimit = (top.transform.position.y - cameraPos.y) * multiplier;
			float rightLimit = (right.transform.position.x - cameraPos.x) * multiplier;
			float bottomLimit = (bottom.transform.position.y - cameraPos.y) * multiplier;
			float leftLimit = (left.transform.position.x - cameraPos.x) * multiplier;

			bool draw = false;
			draw = draw || (target.transform.position.y > topLimit + cameraPos.y);
			draw = draw || (target.transform.position.x > rightLimit + cameraPos.x);
			draw = draw || (target.transform.position.y < bottomLimit + cameraPos.y);
			draw = draw || (target.transform.position.x < leftLimit + cameraPos.x);
			spriteRenderer.enabled = draw;

			if (draw)
			{
				Vector3 newPos = new Vector3();
				newPos.x = Mathf.Clamp(target.transform.position.x, leftLimit + cameraPos.x, rightLimit + cameraPos.x);
				newPos.y = Mathf.Clamp(target.transform.position.y, bottomLimit + cameraPos.y, topLimit + cameraPos.y);
				transform.position = newPos;

				Vector2 deltaPosition = transform.position - target.transform.position;
				float angleToTarget = Mathf.Atan2(deltaPosition.y, deltaPosition.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler(0, 0, angleToTarget);
			}
		}
		else
		{
			target = GameObject.FindGameObjectWithTag("Player");
		}
	}
}
