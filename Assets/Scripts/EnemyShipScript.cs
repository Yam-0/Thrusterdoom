using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class EnemyShipScript : MonoBehaviour
{
	[Header("Enemy Specs")]
	public float maxHealth = 240.0f;
	public Weapon currentWeapon;
	public Weapon extraWeapon;
	public ShipAiType ai;
	public string scoreText;
	public int scoreWorth;
	public float scoreGibForce = 0.0f;

	private float health;

	[Space]
	public GameObject dieEffect;
	public GameObject wreckage;
	public GameObject engine;
	public GameObject scoreGib;
	public float killShakeDuration = 0.3f;
	public float killShakeIntensity = 0.4f;

	[Space]
	public Transform firePoint; //idfk temp lamo
	public TurretScript turretScript;
	public Transform extraFirePoint;
	public TurretScript extraTurretScript;

	[Space]
	public Material hurtMaterial;
	public List<SpriteRenderer> spriteRenderers;

	private Rigidbody2D rb;
	private Vector2 input = Vector2.zero;
	private ShipController shipController;
	private GameObject target;
	private Vector3 targetPos = Vector3.zero;
	private bool dir = false;
	private List<Material> materials;
	private float hurtTimer;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		shipController = GetComponent<ShipController>();
		target = GameObject.FindGameObjectWithTag("Player");
		materials = new List<Material>();

		for (int i = 0; i < spriteRenderers.Count; i++)
			materials.Add(spriteRenderers[i].material);

		currentWeapon = Weapon.MakeNewWeapon(currentWeapon);
		if (extraWeapon != null)
			extraWeapon = Weapon.MakeNewWeapon(extraWeapon);

		health = maxHealth;
	}

	void Update()
	{
		if (target != null)
			targetPos = target.transform.position;

		switch (ai)
		{
			case ShipAiType.enforcer:
				EnforcerAi();
				break;
			case ShipAiType.gunboat:
				GunboatAi();
				break;
			case ShipAiType.canonboat:
				CanonboatAi();
				break;
			case ShipAiType.carrier:
				CarrierAi();
				break;
		}

		Vector3 scale = transform.localScale;
		Vector3 engineScale = engine.transform.localScale;
		if (dir)
		{
			scale.x = -1;
			engineScale.x = -1;
			transform.localScale = scale;
			engine.transform.localScale = engineScale;
		}
		else
		{
			scale.x = 1;
			engineScale.x = 1;
			transform.localScale = scale;
			engine.transform.localScale = engineScale;
		}

		if (hurtMaterial)
		{
			if (hurtTimer > 0.0f)
			{
				if (spriteRenderers[0].material != hurtMaterial)
				{
					for (int i = 0; i < spriteRenderers.Count; i++)
					{
						spriteRenderers[i].material = hurtMaterial;
					}
				}
			}
			else
			{
				if (spriteRenderers[0].material != materials[0])
				{
					for (int i = 0; i < spriteRenderers.Count; i++)
					{
						spriteRenderers[i].material = materials[i];
					}
				}
			}

			hurtTimer = Mathf.Max(0, hurtTimer - Time.deltaTime);
		}

		if (health <= 0)
		{
			Camera.main.GetComponent<CameraScript>().Shake(killShakeDuration, killShakeIntensity);
			if (dieEffect != null)
				Instantiate(dieEffect, transform.position, Quaternion.identity);
			if (wreckage != null)
				Instantiate(wreckage, transform.position, transform.rotation);

			GameObject scoreGibInstance = Instantiate(scoreGib, transform.position, Quaternion.identity);
			scoreGibInstance.GetComponent<ScoreGibManager>().Set(scoreText, scoreGibForce);
			Game.Instance.AddScore(scoreWorth);
			Game.Instance.KilledEnemy();

			Destroy(gameObject, 0);
		}
	}

	void EnforcerAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, true, rb, turretScript);
		extraWeapon.Handle(ref ammo, extraFirePoint, true, rb, extraTurretScript);

		Vector2 deltaPosition = targetPos - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1.0f / Mathf.Sqrt(distance);
		input.x = (dir ? 1 : -1) / distance;
		if (Mathf.Abs(deltaPosition.x) < 20.0f)
		{
			input.x = 0.0f;
		}

		shipController.SetMoveInput(input.normalized);

		float swapRange = 40.0f;
		if (deltaPosition.x > swapRange)
		{
			dir = true;
		}
		if (deltaPosition.x < -swapRange)
		{
			dir = false;
		}
	}

	void GunboatAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, true, rb, turretScript);

		Vector2 deltaPosition = targetPos - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1.0f / Mathf.Sqrt(distance);
		input.x = (dir ? 1 : -1) / distance;
		shipController.SetMoveInput(input.normalized);

		float swapRange = 40.0f;
		if (deltaPosition.x > swapRange)
		{
			dir = true;
		}
		if (deltaPosition.x < -swapRange)
		{
			dir = false;
		}
	}

	void CanonboatAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, true, rb, turretScript);

		Vector2 deltaPosition = targetPos - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1.0f / Mathf.Sqrt(distance);
		input.x = (dir ? 1 : -1) / distance;
		shipController.SetMoveInput(input.normalized);

		float swapRange = 40.0f;
		if (deltaPosition.x > swapRange)
		{
			dir = true;
		}
		if (deltaPosition.x < -swapRange)
		{
			dir = false;
		}
	}

	void CarrierAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, true, rb, turretScript);
		extraWeapon.Handle(ref ammo, extraFirePoint, true, rb, extraTurretScript);

		Vector2 deltaPosition = target.transform.position - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1.0f / Mathf.Sqrt(distance);
		input.x = (dir ? 1 : -1) / distance;
		if (Mathf.Abs(deltaPosition.x) < 20.0f)
		{
			input.x = 0.0f;
		}

		shipController.SetMoveInput(input.normalized);

		float swapRange = 60.0f;
		if (deltaPosition.x > swapRange)
		{
			dir = true;
		}
		if (deltaPosition.x < -swapRange)
		{
			dir = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Hitbox")
		{
			Hitbox hitbox;
			if (other.gameObject.TryGetComponent<Hitbox>(out hitbox))
			{
				float _health = health;
				health = Mathf.Max(0, health - hitbox.Hit(Hitbox.HitboxSource.enemy, other.transform.position));
				if (_health != health && hurtTimer < hitbox.hurtTime)
					hurtTimer = hitbox.hurtTime;
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Hitbox")
		{
			Hitbox hitbox;
			if (other.gameObject.TryGetComponent<Hitbox>(out hitbox))
			{
				float _health = health;
				health = Mathf.Max(0, health - hitbox.Hitting(Hitbox.HitboxSource.enemy, transform.position));
				if (_health != health && hurtTimer < hitbox.hurtTime)
					hurtTimer = hitbox.hurtTime;
			}
		}
	}

	public enum ShipAiType
	{
		enforcer,
		gunboat,
		canonboat,
		carrier
	}
}
