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

	public void Swap()
	{
		AudioSource source1 = bgm[0];
		AudioSource source2 = bgm[1];

		if (source1.isPlaying && !source2.isPlaying)
		{
			source2.Stop();
			return;
		}
		if (source2.isPlaying && !source1.isPlaying)
		{
			source1.Stop();
			return;
		}
	}

	public void StartSource(int a)
	{
		AudioSource source = bgm[a];
		source.Play();
	}
	public void StopSource(int a)
	{
		AudioSource source = bgm[a];
		source.Stop();
	}

	public void PlaySfx(string name)
	{
		if (name != null)
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
