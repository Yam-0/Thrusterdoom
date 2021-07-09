using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	private static Game _instance;
	public static Game Instance { get { return _instance; } }
	public List<GameObject> levels;
	private int level;

	public GameObject player;
	public Transform spawnPoint;

	private CameraScript cameraScript;
	public Animator canvasAnimator;

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
			Play();
		}
	}

	public void Play()
	{
		StartCoroutine(PlayEvent());
	}

	private IEnumerator PlayEvent()
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

	static public void LevelComplete()
	{

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
