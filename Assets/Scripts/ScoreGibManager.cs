using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreGibManager : MonoBehaviour
{
	private TextMeshProUGUI scoreText;
	private Rigidbody2D rb;

	void Start()
	{

	}

	public void Set(string text, float force)
	{
		scoreText = transform.GetComponentInChildren<TextMeshProUGUI>();
		bool simulate = force != 0.0f;
		scoreText.SetText(Game.Instance.GetMultiplier() + "x " + text);

		rb = GetComponent<Rigidbody2D>();

		transform.rotation = Quaternion.Euler(0, 0, 0);

		if (simulate)
		{
			Vector2 forceVector = new Vector2(Random.value, 0.5f + (Random.value / 2));
			rb.velocity = (forceVector * force);
		}
		else
		{
			rb.gravityScale = 0.0f;
		}
	}

	public void DestroyNow()
	{
		Destroy(gameObject);
	}
}
