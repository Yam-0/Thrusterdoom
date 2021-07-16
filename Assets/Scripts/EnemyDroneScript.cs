using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneController))]
public class EnemyDroneScript : MonoBehaviour
{
	[Header("Enemy Specs")]
	public float maxHealth = 100.0f;
	public Weapon currentWeapon;
	public DroneAiType ai;
	public string scoreText;
	public int scoreWorth;
	public float scoreGibForce = 0.0f;

	private float health;

	[Space]
	public GameObject dieEffect;
	public GameObject wreckage;
	public GameObject scoreGib;
	public float killShakeDuration = 0.3f;
	public float killShakeIntensity = 0.4f;

	[Space]
	public Transform firePoint;
	public TurretScript turretScript;

	[Space]
	public Material hurtMaterial;
	public List<SpriteRenderer> spriteRenderers;
	public string killSoundEffect;

	private Rigidbody2D rb;
	private Vector2 input = Vector2.zero;
	private DroneController droneController;
	private GameObject target;
	private Vector3 targetPos = Vector3.zero;
	private List<Material> materials;
	private float hurtTimer;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		droneController = GetComponent<DroneController>();
		target = GameObject.FindGameObjectWithTag("Player");
		materials = new List<Material>();

		for (int i = 0; i < spriteRenderers.Count; i++)
			materials.Add(spriteRenderers[i].material);

		currentWeapon = Weapon.MakeNewWeapon(currentWeapon);

		health = maxHealth;
	}

	void Update()
	{
		if (target != null)
			targetPos = target.transform.position;

		switch (ai)
		{
			case DroneAiType.chaser:
				ChaserAi();
				break;
			case DroneAiType.cruiser:
				CruiserAi();
				break;
			case DroneAiType.dart:
				DartAi();
				break;
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
				Instantiate(wreckage, transform.position, transform.rotation).GetComponent<ProjectileScript>().SetInitialVelocity(rb.velocity);

			GameObject scoreGibInstance = Instantiate(scoreGib, transform.position, Quaternion.identity);
			scoreGibInstance.GetComponent<ScoreGibManager>().Set(scoreText, scoreGibForce);
			Game.Instance.AddScore(scoreWorth);
			Game.Instance.KilledEnemy();

			//Funky
			AudioManager.Instance.PlaySfx("die1");
			AudioManager.Instance.PlaySfx("die2");

			Destroy(gameObject, 0);
		}
	}

	void ChaserAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, (droneController.GetGliding() && (transform.position.y > 0)), rb, null);

		Vector2 deltaPosition = targetPos - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		Vector2 direction = droneController.GetLookDirection();
		float lookAngle = Mathf.Atan2(direction.y, direction.x);
		float deltaAngle = lookAngle - toPlayerAngle;
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1;
		input.x = deltaAngle;

		droneController.SetGlideInput(Mathf.Abs(deltaAngle) < 30 * Mathf.Rad2Deg && distance < 10.0f);
		droneController.SetMoveInput(input.normalized);
	}

	void CruiserAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, (droneController.GetGliding() && (transform.position.y > 0)), rb, turretScript);

		Vector2 deltaPosition = targetPos - transform.position;
		Vector2 playerOffset = new Vector2(0.0f, 10.0f);
		deltaPosition += playerOffset;

		float toTargetAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		Vector2 direction = droneController.GetLookDirection();
		float lookAngle = Mathf.Atan2(direction.y, direction.x);
		float deltaAngle = lookAngle - toTargetAngle;
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1;
		input.x = deltaAngle > 0 ? 1 : -1;

		droneController.SetGlideInput(Mathf.Abs(deltaAngle) < 30 * Mathf.Rad2Deg && distance < 10.0f);
		droneController.SetMoveInput(input.normalized);

		//Rotate towards x move direction
		float angle = droneController.GetMoveDirection().x * -30.0f;
		transform.rotation = Quaternion.Euler(0, 0, angle);

		GameObject hull = transform.Find("Hull").gameObject;
		Vector3 scale = hull.transform.localScale;
		if (rb.velocity.x > 0)
		{
			scale.x = -1;
		}
		else
		{
			scale.x = 1;
		}
		hull.transform.localScale = scale;
	}

	void DartAi()
	{
		float ammo = 1.0f; //Infinite ammo temp fix
		currentWeapon.Handle(ref ammo, firePoint, (droneController.GetGliding() && (transform.position.y > 0)), rb, null);

		Vector2 deltaPosition = targetPos - transform.position;
		float toPlayerAngle = Mathf.Atan2(deltaPosition.y, deltaPosition.x);
		Vector2 direction = droneController.GetLookDirection();
		float lookAngle = Mathf.Atan2(direction.y, direction.x);
		float deltaAngle = lookAngle - toPlayerAngle;
		float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

		input.y = 1;
		input.x = deltaAngle > 0 ? 1 : -1;

		droneController.SetGlideInput(Mathf.Abs(deltaAngle) < 30 * Mathf.Rad2Deg && distance < 10.0f);
		droneController.SetMoveInput(input.normalized);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Hitbox")
		{
			Hitbox hitbox;
			if (other.gameObject.TryGetComponent<Hitbox>(out hitbox))
			{
				float _health = health;
				float damage = hitbox.Hit(Hitbox.HitboxSource.enemy, other.transform.position);
				damage = DamageMask() ? damage : 0;
				health = Mathf.Max(0, health - hitbox.Hit(Hitbox.HitboxSource.enemy, other.transform.position));
				if (_health != health && hurtTimer < hitbox.hurtTime)
				{
					Game.Instance.DamagedEnemy(damage);
					hurtTimer = hitbox.hurtTime;
				}
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
				float damage = hitbox.Hitting(Hitbox.HitboxSource.enemy, transform.position);
				damage = DamageMask() ? damage : 0;
				health = Mathf.Max(0, health - hitbox.Hitting(Hitbox.HitboxSource.enemy, transform.position));
				if (_health != health && hurtTimer < hitbox.hurtTime)
				{
					Game.Instance.DamagedEnemy(damage);
					hurtTimer = hitbox.hurtTime;
				}
			}
		}
	}

	private bool DamageMask()
	{
		bool takeDamage = true;

		return takeDamage;
	}

	public enum DroneAiType
	{
		chaser,
		cruiser,
		dart
	}
}
