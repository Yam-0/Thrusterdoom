using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource[] bgm;
	public Sfx[] sfx;
	int currentLevel = -1;

	private static AudioManager _instance;
	bool allStopped;

	public static AudioManager Instance { get { return _instance; } }

	void Awake()
	{
		allStopped = false;

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

	void Update()
	{
		if (allStopped)
		{
			return;
		}
		GameObject player = GameObject.FindGameObjectWithTag("Player");
	}

	public void SetBgmLevelAndVolume(int level, float volume)
	{
		if (level == currentLevel) return;

		for (int i = 0; i < bgm.Length; i++)
		{
			AudioSource audioSource = bgm[i];
			if (i <= level)
			{
				audioSource.volume = volume;
			}
			else
			{
				audioSource.volume = 0f;
			}
		}
	}

	public void PlaySfx(string name)
	{
		sfx[0].PlaySound(name);
	}

	public void StopAll()
	{
		allStopped = true;
		for (int i = 0; i < bgm.Length; i++)
		{
			AudioSource audioSource = bgm[i];
			audioSource.volume = 0f;
		}
	}
}
