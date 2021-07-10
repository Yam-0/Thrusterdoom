using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	private static Game _instance;
	public static Game Instance { get { return _instance; } }

	public GameObject player;
	public Transform spawnPoint;

	[Space]
	public List<Enemy> enemies;

	[Space]
	public Animator canvasAnimator;

	private CameraScript cameraScript;
	private bool ingame = false;
	private float timer;

	void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			_instance = this;
			DontDestroyOnLoad(_instance);
		}
	}

	void Start()
	{
		Setup();
	}

	void Setup()
	{
		Camera.main.TryGetComponent<CameraScript>(out cameraScript);
		player = Instantiate(player, spawnPoint.position, spawnPoint.rotation);
		cameraScript.SetTrackObject(player);
		player.SetActive(false);
		timer = 0.0f;

		foreach (Enemy enemy in enemies)
		{
			enemy.timer = enemy.spawnDelay;
		}
	}

	void Update()
	{
		if (ingame)
		{
			timer += Time.deltaTime;

			foreach (Enemy enemy in enemies)
			{
				enemy.Handle(player.transform.position, timer);
			}
		}
		else
		{
			if (Input.anyKeyDown)
			{
				Play();
			}
		}
	}

	public void Play()
	{
		StartCoroutine(StartGame_Event());
	}

	private IEnumerator StartGame_Event()
	{
		ingame = true;
		canvasAnimator.SetTrigger("StartGame");

		yield return new WaitForSeconds(1.0f);
		player.SetActive(true);
		player.GetComponent<Rigidbody2D>().velocity = new Vector2(40.0f, 10.0f);
		yield return new WaitForSeconds(1.4f);
		cameraScript.LockOn();
		cameraScript.smoothTrack = true;
		yield return new WaitForSeconds(0.3f);
		cameraScript.smoothTrack = false;
	}

	public void PlayerDied()
	{
		Debug.Log("Player Died!!");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		ingame = false;
	}

	[System.Serializable]
	public class Enemy
	{
		public string enemyName;
		public EnemyType enemyType;
		public GameObject enemy;
		public float spawnTime;
		public float spawnDelay;
		[HideInInspector]
		public float timer;

		public void Handle(Vector3 playerPos, float timeAlive)
		{
			timer = Mathf.Max(0.0f, timer - Time.deltaTime);

			if (timer == 0)
			{
				timer = spawnTime;

				Vector3 spawnPosition = new Vector3();
				spawnPosition.y = enemyType == EnemyType.ship ? 0.0f : Random.Range(0.0f, 15.0f);
				spawnPosition.x = playerPos.x + ((Mathf.RoundToInt(Random.value) * 2 - 1) * Random.Range(70.0f, 100.0f));
				Instantiate(enemy, spawnPosition, Quaternion.identity);
			}
		}
	}

	public enum EnemyType
	{
		drone,
		ship
	}
}

