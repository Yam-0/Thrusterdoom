using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	private static Game _instance;
	public static Game Instance { get { return _instance; } }
	public List<GameObject> levels;
	private int level = 0;
	private int world = 0;

	public GameObject player;
	public Transform spawnPoint;

	private CameraScript cameraScript;

	[Space]
	public Animator canvasAnimator;
	public Animator popupAnimator;
	public Text levelPopup1;
	public Text levelPopup2;
	public Text levelPopup3;

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

		level = 0;
	}

	void Start()
	{
		Camera.main.TryGetComponent<CameraScript>(out cameraScript);
		player = Instantiate(player, spawnPoint.position, spawnPoint.rotation);
		cameraScript.SetTrackObject(player);
		player.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.J))
		{
			StartCoroutine(StartLevel_Event());
		}
	}

	public void Play()
	{
		StartCoroutine(StartGame_Event());
	}

	private IEnumerator StartGame_Event()
	{
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

	private IEnumerator StartLevel_Event()
	{
		GameObject levelObject = levels[level];
		LevelGroup levelGroup = levelObject.GetComponent<LevelGroup>();
		levelPopup1.text = levelGroup.spawnMessage1;
		levelPopup2.text = levelGroup.spawnMessage2;
		levelPopup3.text = levelGroup.spawnMessage3;
		popupAnimator.SetTrigger("NewLevel");

		Instantiate(levelObject, new Vector3(player.transform.position.x + 75, 0.0f, 0.0f), Quaternion.identity);
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(3.0f);
		Time.timeScale = 1;
	}

	static public void LoadScene(int index)
	{
		SceneManager.LoadScene(index);
	}

	static public int GetCurrentSceneIndex()
	{
		return SceneManager.GetActiveScene().buildIndex;
	}

	static public void ReloadScene()
	{
		LoadScene(GetCurrentSceneIndex());
	}

	public void LevelComplete()
	{
		Debug.Log("Level complete!!");
		level++;
	}

	public static class Events
	{
		public static void PlayerDied()
		{
			Debug.Log("Player Died!!");
			Game.ReloadScene();
		}
	}
}
