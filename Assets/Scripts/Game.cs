using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	private static Game _instance;
	public static Game Instance { get { return _instance; } }


	//private int level;

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

		//level = 0;
	}

	void Update()
	{

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
