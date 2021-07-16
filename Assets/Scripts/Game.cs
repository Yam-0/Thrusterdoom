using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
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
	private int funds = 0;
	private int kills = 0;
	private float damage = 0;
	private int addFunds = 0;
	private bool gainMultiplier;

	private Animator gameAnimator;
	private CameraScript cameraScript;

	private GameState gameState;
	private MenuState menuState;
	private float timer;

	private bool killMission = false;
	private bool multiplierMission = false;
	private bool scoreMission = false;
	private bool timeMission = false;
	private bool spotMission = false;
	private bool thrusterdoomMission = false;
	private int missionCount = 0;
	public GameObject thrusterdoom;
	private GameObject thrusterdoomInstance;
	private bool thrusterdoomKilled = false;
	private bool thrusterdoomSpawned = false;
	private float thrusterDoomHealth;
	private float thrusterDoomMaxHealth;
	public TextMeshProUGUI timerText;
	private string formattedTime;
	public CanvasGroup[] missionGroups;
	public Image[] missionCheckmarks;
	public TextMeshProUGUI[] missionTexts;
	public GameObject killExplosion;
	public Sprite checkmarkSprite;
	public Sprite crossSprite;
	private float playerHealth = 0.0f;
	private float playerBoost = 0.0f;
	public RectTransform healthBar;
	public RectTransform healthBackground;
	public RectTransform boostBar;
	public RectTransform boostBackground;
	public GameObject thrusterDoomHealthBarContainer;
	public RectTransform thrusterDoomHealthBar;
	public RectTransform thrusterDoomHealthBackground;
	public Animation bossBarFadeIn;
	private bool paused;
	private float pauseSpeedSwap;
	public GameObject pauseMenu;
	private GameObject thrusterDoomPointer;
	private bool killable = true;
	private int playerEquippedWeapon;
	public TextMeshProUGUI[] hudWeaponDescriptions;
	public Image[] hudWeaponPanels;
	public Color weapodHudColor;
	public Color weapodHudEquippedColor;

	//Temp --------------------

	//TODO:
	//Main Menu buttons etc
	//pause menu buttons
	//Audio SFX/MUSIC
	//Proper graphics

	//Temp --------------------

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

					if (!thrusterdoomSpawned)
					{
						foreach (Enemy enemy in enemies)
						{
							enemy.Handle(playerInstance.transform.position, timer);
						}
					}
				}

				if (thrusterdoomInstance == null && !thrusterdoomKilled && thrusterdoomSpawned)
				{
					Debug.Log("Killed H.M.S Thrusterdoom");
					thrusterdoomKilled = true;
					gameAnimator.SetTrigger("Ingame_End");
					AudioManager.Instance.PlaySfx("title");
				}

				if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
				{
					if (paused)
					{
						Time.timeScale = pauseSpeedSwap;
					}
					else
					{
						pauseSpeedSwap = Time.timeScale;
						Time.timeScale = 0.0f;
					}

					paused = !paused;
					pauseMenu.SetActive(paused);
				}

				/*
				if (Input.GetKeyDown(KeyCode.K))
				{
					kills = 10;
					multiplier = 4;
					score = 3200;
					timer = 3.5f * 60;
				}
				*/

				if (!thrusterdoomKilled && thrusterdoomSpawned)
				{
					if (thrusterdoomInstance != null && playerInstance != null)
					{
						float dist = Vector3.Distance(thrusterdoomInstance.transform.position, playerInstance.transform.position);
						PixelPerfectCamera perfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();

						if (perfectCamera != null)
						{
							thrusterDoomPointer = playerInstance.transform.Find("pointer").gameObject;

							if (dist < 50.0f)
							{
								if (cameraScript.trackObject != thrusterdoomInstance)
								{
									cameraScript.SetTrackObject(thrusterdoomInstance);
									//cameraScript.smoothTrack = true;
									perfectCamera.assetsPPU = 8;
									cameraScript.shakeMult = 2;
								}

								thrusterDoomPointer.SetActive(false);
								Camera.main.transform.Find("arrow").GetComponent<ArrowPointerScript>().multiplier = 2.0f;
							}
							else
							{
								if (cameraScript.trackObject != playerInstance)
								{
									cameraScript.SetTrackObject(playerInstance);
									//cameraScript.smoothTrack = false;
									perfectCamera.assetsPPU = 16;
									cameraScript.shakeMult = 1;
								}

								thrusterDoomPointer.SetActive(true);
								Camera.main.transform.Find("arrow").GetComponent<ArrowPointerScript>().multiplier = 1.0f;

								Vector2 delta = thrusterdoomInstance.transform.position - playerInstance.transform.position;
								float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
								thrusterDoomPointer.transform.rotation = Quaternion.Euler(0, 0, angle);
							}
						}
						else
						{
							Debug.Log("Ca");
						}
					}
				}

				System.TimeSpan t = System.TimeSpan.FromSeconds(timer);
				formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
								t.Hours,
								t.Minutes,
								t.Seconds);

				UpdateMissions();
				UpdateWeaponsHud();
				UpdateTopHud();
				UpdateBottomHud();

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
		scoreText.SetText("Score: " + score);
		TestScore(score);
		highscoreText.SetText("Highscore: " + highscore);
		fundsText.SetText("Funds: " + funds + " (+" + addFunds + ")");
		missionText.SetText("Missions: (" + missionCount + "/6)");
		timeText.SetText("Time: " + formattedTime);
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

	public void UpdateTopHud()
	{
		float maxHealth = healthUpgradeValues[healthUpgradeLevel];
		float maxBoost = 1.2f;

		healthBackground.localScale = new Vector3(maxHealth / 300.0f, 1, 1);
		boostBackground.localScale = new Vector3(maxBoost / 1.2f, 1, 1);
		healthBar.localScale = new Vector3(playerHealth / maxHealth, 1, 1);
		boostBar.localScale = new Vector3(playerBoost / maxBoost, 1, 1);
	}

	public void UpdateBottomHud()
	{
		if (thrusterdoomSpawned)
		{
			if (!thrusterdoomKilled && thrusterdoomInstance != null)
			{
				EnemyShipScript enemyShipScript = thrusterdoomInstance.GetComponent<EnemyShipScript>();
				if (enemyShipScript != null)
				{
					thrusterDoomHealth = enemyShipScript.GetHealth();
					thrusterDoomMaxHealth = enemyShipScript.GetMaxHealth();
				}
				else
				{
					thrusterDoomHealth = 0.0f;
				}
			}
			else
			{
				thrusterDoomHealth = 0.0f;
			}

			thrusterDoomHealthBar.localScale = new Vector3(thrusterDoomHealth / thrusterDoomMaxHealth, 1, 1);
		}
	}

	public void UpdateMissions()
	{
		bool redraw = false;
		timerText.SetText(formattedTime);

		if (!killMission && kills >= 10)
		{
			killMission = true;
			redraw = true;
			missionCount++;
		}
		if (!multiplierMission && multiplier >= 4)
		{
			multiplierMission = true;
			redraw = true;
			missionCount++;
		}
		if (!scoreMission && score >= 3200)
		{
			scoreMission = true;
			redraw = true;
			missionCount++;
		}
		if (!timeMission && timer >= (3.5 * 60))
		{
			timeMission = true;
			redraw = true;
			missionCount++;
		}

		if (!thrusterdoomSpawned)
		{
			if (missionCount >= 4 && !thrusterdoomSpawned)
			{
				AudioManager.Instance.StopSource(0);
				AudioManager.Instance.PlaySfx("menace");
				SpawnThrusterdoom();
			}
		}

		bool spotted = false;

		if (thrusterdoomInstance != null && !spotMission && playerInstance != null)
		{
			float dist = Vector3.Distance(thrusterdoomInstance.transform.position, playerInstance.transform.position);
			if (dist < 50.0f)
			{
				thrusterDoomHealthBarContainer.SetActive(true);
				bossBarFadeIn.Play();

				spotted = true;
			}
		}

		if (!spotMission && spotted)
		{
			spotMission = true;
			redraw = true;
			missionCount++;
			AudioManager.Instance.PlaySfx("braam");
			AudioManager.Instance.StartSource(1);
		}
		if (!thrusterdoomMission && thrusterdoomKilled)
		{
			thrusterdoomMission = true;
			redraw = true;
			missionCount++;
		}

		string killText = Mathf.Clamp(kills, 0, 10).ToString();
		missionTexts[0].SetText("Kill 10 enemies (" + killText + "/10)");
		string multiplierText = Mathf.Clamp(multiplier, 0, 4).ToString();
		missionTexts[1].SetText("Get a 4x multiplier (" + multiplierText + "/4)");
		string scoreText = Mathf.Clamp(score, 0, 3200).ToString();
		missionTexts[2].SetText("Get 3200 score (" + scoreText + "/3200)");
		string surviveText = Mathf.Clamp((timer / 60.0f), 0.0f, 3.5f).ToString("0.0");
		missionTexts[3].SetText("Survive for 3.5 minutes (" + surviveText + "/3.5");

		string spotText = "";
		if (missionCount >= 4)
		{
			spotText = "Spot the ship";
		}
		else
		{
			spotText = "???";
		}
		missionTexts[4].SetText(spotText);

		string thrusterdoomText = "";
		if (missionCount >= 5)
		{
			thrusterdoomText = "Destroy H.M.S Thrusterdoom";
		}
		else
		{
			thrusterdoomText = "???";
		}
		missionTexts[5].SetText(thrusterdoomText);

		if (redraw)
		{
			float alpha = 0.3f;
			if (killMission)
			{
				missionGroups[0].alpha = alpha;
				missionCheckmarks[0].sprite = checkmarkSprite;
			}
			if (multiplierMission)
			{
				missionGroups[1].alpha = alpha;
				missionCheckmarks[1].sprite = checkmarkSprite;
			}
			if (scoreMission)
			{
				missionGroups[2].alpha = alpha;
				missionCheckmarks[2].sprite = checkmarkSprite;
			}
			if (timeMission)
			{
				missionGroups[3].alpha = alpha;
				missionCheckmarks[3].sprite = checkmarkSprite;
			}
			if (spotMission)
			{
				missionGroups[4].alpha = alpha;
				missionCheckmarks[4].sprite = checkmarkSprite;
			}
			if (thrusterdoomMission)
			{
				missionGroups[5].alpha = alpha;
				missionCheckmarks[5].sprite = checkmarkSprite;
			}
		}
	}

	public void UpdateWeaponsHud()
	{
		List<Weapon> weaponList = new List<Weapon>();
		for (int i = 0; i < weaponsEquipped.Length; i++)
		{
			if (weaponsEquipped[i])
			{
				weaponList.Add(weapons[i]);
			}
		}

		for (int i = 0; i < weapons.Count; i++)
		{
			bool enabled = i <= weaponList.Count - 1;
			hudWeaponPanels[i].enabled = enabled;
			hudWeaponDescriptions[i].enabled = enabled;
		}

		for (int i = 0; i < weaponList.Count; i++)
		{
			hudWeaponDescriptions[i].SetText(weaponList[i].weaponName);
			Color hudColor = weapodHudColor;
			if (playerEquippedWeapon == i)
			{
				hudColor = weapodHudEquippedColor;
			}
			hudWeaponPanels[i].color = hudColor;
		}
	}

	public void SpawnThrusterdoom()
	{
		Debug.Log("Spawning H.M.S Thrusterdoom");

		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		for (int i = 0; i < enemies.Length; i++)
		{
			Instantiate(killExplosion, enemies[i].transform.position, Quaternion.identity);
			Destroy(enemies[i], 0.0f);
		}

		Vector3 playerPos = playerInstance.transform.position;
		Vector3 spawnPos = new Vector3(playerPos.x - 100.0f, 16.0f, 0.0f);
		thrusterdoomInstance = Instantiate(thrusterdoom, spawnPos, Quaternion.identity);
		AudioManager.Instance.PlaySfx("sneaky");
		if (thrusterdoomInstance != null)
			print("Spawned H.M.S Thrusterdoom");
		thrusterdoomSpawned = true;
		thrusterdoomKilled = false;
	}

	public void SetWeaponIndex(int index)
	{
		playerEquippedWeapon = index;
	}

	public void WinGame()
	{
		Reload();
	}

	public void KilledThrusterdoom()
	{
		cameraScript.Shake(4.0f, 0.8f);
		thrusterdoomInstance.GetComponent<EnemyShipScript>().Hurt(1.0f);
	}

	public void EndGame()
	{
		Application.Quit();
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

	public void SetHealth(float health)
	{
		playerHealth = health;
	}

	public void SetBoost(float boost)
	{
		playerBoost = boost;
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
		AudioManager.Instance.PlaySfx("bong");

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
		AudioManager.Instance.PlaySfx("click");

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
		AudioManager.Instance.PlaySfx("bong");

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
		AudioManager.Instance.PlaySfx("click");

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
		AudioManager.Instance.PlaySfx("bong");

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
			AudioManager.Instance.PlaySfx("switch");
			weaponsEquipped[a] = !weaponsEquipped[a];
		}
		else
		{
			int cost = weaponPrices[a];
			if (cost <= funds)
			{
				AudioManager.Instance.PlaySfx("click");
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
		if (killable)
			TransitionTo(MenuState.Shop);
	}

	public void SetKillable(bool killable)
	{
		this.killable = killable;
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
			AudioManager.Instance.PlaySfx("magic");
			ResetMultiplier();
		}

		//Restart
		if (newState == MenuState.Ingame && menuState == MenuState.Shop)
		{
			menuState = MenuState.Shop_Ingame;
			gameAnimator.SetTrigger("Shop_Ingame");
			missionCount = 0;
			killMission = false;
			multiplierMission = false;
			scoreMission = false;
			timeMission = false;
			spotMission = false;
			thrusterdoomMission = false;
			thrusterdoomSpawned = false;
			score = 0;
			timer = 0;
			kills = 0;
			damage = 0;
			bestMultiplier = 0;
			thrusterdoomKilled = false;
			thrusterdoomSpawned = false;
			thrusterdoomInstance = null;
			thrusterDoomHealthBarContainer.SetActive(false);

			AudioManager.Instance.StopSource(1);
			AudioManager.Instance.StartSource(0);

			AudioManager.Instance.PlaySfx("magic");
			for (int i = 0; i < 6; i++)
			{
				missionGroups[i].alpha = 1.0f;
				missionCheckmarks[i].sprite = crossSprite;
			}

			ResetMultiplier();
		}

		//Die
		if (newState == MenuState.Shop && menuState == MenuState.Ingame)
		{
			menuState = MenuState.Ingame_Shop;
			gameAnimator.SetTrigger("Ingame_Shop");
			addFunds = score / 10;
			AddFunds(addFunds);

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
		Shop_Ingame,
		Ingame_End
	}
}

