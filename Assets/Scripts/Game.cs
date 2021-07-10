using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	private static Game _instance;
	public static Game Instance { get { return _instance; } }

	public GameObject player;
	private GameObject playerInstance;
	public Transform spawnPoint;

	[Space]
	public List<Enemy> enemies;
	public List<Menu> menus;

	[Space(30)]
	public TextMeshProUGUI ingameScore;
	public TextMeshProUGUI ingameMultiplier;

	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI highscoreText;
	public TextMeshProUGUI fundsText;

	[Space(30)]
	public List<Weapon> weapons;
	private List<Weapon> unlockedWeapons;

	private int score = 0;
	private int multiplier = 1;
	private int highscore = 0;
	private int funds = 0;
	private bool gainMultiplier;

	private Animator gameAnimator;
	private CameraScript cameraScript;

	private GameState gameState;
	private MenuState menuState;
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
			//DontDestroyOnLoad(gameObject);
		}

		gameState = GameState.StartScreen;
		menuState = MenuState.StartScreen;

		unlockedWeapons = new List<Weapon>();
		unlockedWeapons.Add(weapons[0]);
	}

	void Start()
	{
		gameAnimator = GetComponent<Animator>();
		UpdateCanvas();
	}

	void SetupScene()
	{
		Debug.Log("Setup");

		Camera.main.TryGetComponent<CameraScript>(out cameraScript);
		playerInstance = Instantiate(player, spawnPoint.position, spawnPoint.rotation);
		cameraScript.SetTrackObject(playerInstance);
		playerInstance.SetActive(false);
		timer = 0.0f;

		foreach (Enemy enemy in enemies)
		{
			enemy.timer = enemy.spawnDelay;
		}
	}

	void Update()
	{
		switch (gameState)
		{
			case GameState.StartScreen:
				if (Input.anyKeyDown)
				{
					if (Input.GetMouseButtonDown(0))
						return;

					Play();
				}
				break;
			case GameState.Ingame:

				if (score.ToString().Length > 6) { score = 999999; }

				string format = "000000";
				string textScore = score.ToString(format);

				ingameScore.SetText(textScore);
				ingameMultiplier.SetText(multiplier + "x");

				if (playerInstance != null)
				{
					timer += Time.deltaTime;

					foreach (Enemy enemy in enemies)
					{
						enemy.Handle(playerInstance.transform.position, timer);
					}
				}
				break;
			case GameState.Shop:
				if (Input.GetKeyDown(KeyCode.J))
				{
					PlayAgain();
				}
				break;
		}
	}

	private void UpdateCanvas()
	{
		foreach (Menu menu in menus)
		{
			bool active = false;
			bool interactable = false;

			for (int i = 0; i < menu.activeStates.Length; i++)
				if (menuState == menu.activeStates[i])
					active = true;
			for (int i = 0; i < menu.interactableStates.Length; i++)
				if (menuState == menu.interactableStates[i])
					interactable = true;

			menu.canvasGroup.gameObject.SetActive(active);
			menu.canvasGroup.interactable = interactable;
		}
	}

	public void AddScore(int addScore)
	{
		if (menuState == MenuState.Ingame)
			score += addScore * multiplier;
	}

	public void TestScore(int score)
	{
		if (score > highscore)
			highscore = score;
	}

	public void AddFunds(int funds)
	{
		this.funds += funds;
	}

	public void KilledEnemy()
	{
		if (gainMultiplier)
		{
			this.multiplier += 1;
		}
		else
		{
			gainMultiplier = true;
		}
	}

	public void ResetMultiplier()
	{
		gainMultiplier = false;
		multiplier = 1;
	}

	public int GetMultiplier()
	{
		return multiplier;
	}

	public void Play()
	{
		Debug.Log("Play");
		TransitionTo(MenuState.Ingame);
	}

	public void PlayAgain()
	{
		Debug.Log("Play again");
		TransitionTo(MenuState.Ingame);
	}

	public void PlayerDied()
	{
		Debug.Log("Player died, " + gameState.ToString());
		TransitionTo(MenuState.Shop);
	}

	public void TransitionDone()
	{
		switch (menuState)
		{
			case MenuState.StartScreen_Ingame:
				menuState = MenuState.Ingame;
				gameState = GameState.Ingame;
				StartCoroutine(Spawn());
				break;

			case MenuState.Ingame_Shop:
				menuState = MenuState.Shop;
				gameState = GameState.Shop;
				break;

			case MenuState.Shop_Ingame:
				menuState = MenuState.Ingame;
				gameState = GameState.Ingame;
				StartCoroutine(Spawn());
				break;

			default:
				break;
		}

		UpdateCanvas();
	}

	private void TransitionTo(MenuState newState)
	{
		//Start
		if (newState == MenuState.Ingame && menuState == MenuState.StartScreen)
		{
			menuState = MenuState.StartScreen_Ingame;
			gameAnimator.SetTrigger("StartScreen_Ingame");
			score = 0;
			ResetMultiplier();
		}

		//Restart
		if (newState == MenuState.Ingame && menuState == MenuState.Shop)
		{
			menuState = MenuState.Shop_Ingame;
			gameAnimator.SetTrigger("Shop_Ingame");
			score = 0;
			ResetMultiplier();
		}

		//Die
		if (newState == MenuState.Shop && menuState == MenuState.Ingame)
		{
			menuState = MenuState.Ingame_Shop;
			gameAnimator.SetTrigger("Ingame_Shop");

			int addFunds = score / 10;
			AddFunds(addFunds);
			scoreText.SetText("Score: " + score);
			highscoreText.SetText("Highscore: " + highscore);
			fundsText.SetText("Funds: " + funds + " (+" + addFunds + ")");
		}

		UpdateCanvas();
	}

	public void Reload()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private IEnumerator Spawn()
	{
		SetupScene();
		playerInstance.SetActive(true);
		playerInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(40.0f, 10.0f);
		yield return new WaitForSeconds(1.4f);
		cameraScript.LockOn();
		cameraScript.smoothTrack = true;
		yield return new WaitForSeconds(0.3f);
		cameraScript.smoothTrack = false;
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

	[System.Serializable]
	public class Menu
	{
		public string menuName;
		public CanvasGroup canvasGroup;

		[SerializeField]
		public MenuState[] activeStates;
		[SerializeField]
		public MenuState[] interactableStates;
	}

	public enum EnemyType
	{
		drone,
		ship
	}

	public enum GameState
	{
		StartScreen,
		Ingame,
		Shop
	}

	public enum MenuState
	{
		StartScreen,
		StartScreen_Ingame,
		Ingame,
		Ingame_Shop,
		Shop,
		Shop_Ingame
	}
}

