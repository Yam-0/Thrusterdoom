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
	public TextMeshProUGUI timeText;
	public TextMeshProUGUI missionText;
	public TextMeshProUGUI bestMultiplierText;
	public TextMeshProUGUI damageText;
	public TextMeshProUGUI killsText;

	[Space(30)]
	public List<Weapon> weapons;

	public Color unlockedColor;
	public Color lockedColor;
	public Button[] healthUpgradeButtons;
	public float[] healthUpgradeValues;
	public string[] healthUpgradeEffects;
	public int[] healthUpgradeCosts;
	public int healthUpgradeLevel = 0;
	public Button[] damageUpgradeButtons;
	public float[] damageUpgradeValues;
	public string[] damageUpgradeEffects;
	public int damageUpgradeLevel = 0;
	public int[] damageUpgradeCosts;
	public bool[] weaponsUnlocked;
	public bool[] weaponsEquipped;
	public int[] weaponPrices;
	public TextMeshProUGUI weaponPriceText;
	public TextMeshProUGUI upgradePriceText;
	public TextMeshProUGUI upgradeEffectText;
	public TextMeshProUGUI[] weaponButtonTexts;

	private int score = 0;
	private int multiplier = 1;
	private int bestMultiplier = 1;
	private int highscore = 0;
	private int funds = 14000;
	private int kills = 0;
	private float damage = 0;
	private int addFunds = 0;
	private bool gainMultiplier;

	private Animator gameAnimator;
	private CameraScript cameraScript;

	private GameState gameState;
	private MenuState menuState;
	private float timer;

	//Temp --------------------
	//Missions:
	//Kill 10 enemies
	//Get a 4x multiplier
	//Get 2400 score
	//Survive for 3.5 minutes
	//Spot the ship
	//Destroy H.M.S Thrusterdoom

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

		for (int i = 0; i < weaponsUnlocked.Length; i++)
		{
			weaponsUnlocked[i] = false;
		}
		weaponsUnlocked[0] = true;
		for (int i = 0; i < weaponsEquipped.Length; i++)
		{
			weaponsEquipped[i] = false;
		}
		weaponsEquipped[0] = true;
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

		List<Weapon> equippedWeaponsList = new List<Weapon>();
		for (int i = 0; i < weaponsEquipped.Length; i++)
		{
			if (weaponsEquipped[i])
			{
				equippedWeaponsList.Add(weapons[i]);
			}
		}
		PlayerScript playerScript = playerInstance.GetComponent<PlayerScript>();
		playerScript.SetWeapons(equippedWeaponsList);
		playerScript.SetHealth(healthUpgradeValues[healthUpgradeLevel]);
		playerScript.damageMultiplier = damageUpgradeValues[damageUpgradeLevel];

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

	private void UpdateButtons()
	{
		for (int i = 0; i < 5; i++)
		{
			string buttonText = "";

			if (weaponsUnlocked[i])
			{
				if (weaponsEquipped[i])
				{
					buttonText = "Unequip";
				}
				else
				{
					buttonText = "Equip";
				}
			}
			else
			{
				buttonText = "Buy";
			}

			weaponButtonTexts[i].SetText(buttonText);
		}

		addFunds = score / 10;
		AddFunds(addFunds);
		scoreText.SetText("Score: " + score);
		TestScore(score);
		highscoreText.SetText("Highscore: " + highscore);
		fundsText.SetText("Funds: " + funds + " (+" + addFunds + ")");

		missionText.SetText("Missions: (4/6)");

		System.TimeSpan t = System.TimeSpan.FromSeconds(timer);
		string time = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
						t.Hours,
						t.Minutes,
						t.Seconds);

		timeText.SetText("Time: " + time);
		bestMultiplierText.SetText("Best Multiplier: " + bestMultiplier);
		damageText.SetText("Damage: " + (int)damage);
		killsText.SetText("Kills: " + kills);

		for (int i = 0; i < 10; i++)
		{
			if (healthUpgradeLevel >= i)
			{
				healthUpgradeButtons[i].interactable = false;
				healthUpgradeButtons[i].gameObject.GetComponent<Image>().color = unlockedColor;
			}
			else
			{
				healthUpgradeButtons[i].interactable = true;
				healthUpgradeButtons[i].gameObject.GetComponent<Image>().color = lockedColor;
			}

			if (damageUpgradeLevel >= i)
			{
				damageUpgradeButtons[i].interactable = false;
				damageUpgradeButtons[i].gameObject.GetComponent<Image>().color = unlockedColor;
			}
			else
			{
				damageUpgradeButtons[i].interactable = true;
				damageUpgradeButtons[i].gameObject.GetComponent<Image>().color = lockedColor;
			}
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
		kills++;

		if (gainMultiplier)
		{
			this.multiplier += 1;
			if (multiplier > bestMultiplier)
				bestMultiplier = multiplier;
		}
		else
		{
			gainMultiplier = true;
		}
	}

	public void DamagedEnemy(float damage)
	{
		this.damage += damage;
		AddScore(Mathf.RoundToInt(damage / 5));
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

	public void HealthUpgradeHover(int a)
	{
		upgradeEffectText.SetText("Effect: " + healthUpgradeEffects[a]);
		if (healthUpgradeLevel >= a)
		{
			upgradePriceText.SetText("Cost: Unlocked");
		}
		else
		{
			int cost = healthUpgradeCosts[a];
			upgradePriceText.SetText("Cost: " + cost);
		}
	}

	public void HealthButton(int a)
	{
		if (healthUpgradeLevel < a)
		{
			int cost = healthUpgradeCosts[a];
			if (cost <= funds)
			{
				funds -= cost;
				healthUpgradeLevel = a;
				HealthUpgradeHover(a);
			}
		}

		UpdateButtons();
	}

	public void DamageUpgradeHover(int a)
	{
		upgradeEffectText.SetText("Effect: " + damageUpgradeEffects[a]);
		if (damageUpgradeLevel >= a)
		{
			upgradePriceText.SetText("Cost: Unlocked");
		}
		else
		{
			int cost = damageUpgradeCosts[a];
			upgradePriceText.SetText("Cost: " + cost);
		}
	}

	public void DamageButton(int a)
	{
		if (damageUpgradeLevel < a)
		{
			int cost = damageUpgradeCosts[a];
			if (cost <= funds)
			{
				funds -= cost;
				damageUpgradeLevel = a;
				DamageUpgradeHover(a);
			}
		}

		UpdateButtons();
	}

	public void ShopHover(int a)
	{
		if (weaponsUnlocked[a])
		{
			weaponPriceText.SetText("Cost: Unlocked");
		}
		else
		{
			int cost = weaponPrices[a];
			weaponPriceText.SetText("Cost: " + cost);
		}
	}

	public void WeaponButton(int a)
	{
		if (weaponsUnlocked[a])
		{
			weaponsEquipped[a] = !weaponsEquipped[a];
		}
		else
		{
			int cost = weaponPrices[a];
			if (cost <= funds)
			{
				funds -= cost;
				weaponsUnlocked[a] = true;
				weaponsEquipped[a] = true;
			}
		}

		UpdateButtons();
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
			timer = 0;
			kills = 0;
			damage = 0;
			bestMultiplier = 0;
			ResetMultiplier();
		}

		//Die
		if (newState == MenuState.Shop && menuState == MenuState.Ingame)
		{
			menuState = MenuState.Ingame_Shop;
			gameAnimator.SetTrigger("Ingame_Shop");

			UpdateButtons();
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

