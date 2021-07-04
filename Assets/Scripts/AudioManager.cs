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

    void Awake() {
        allStopped = false;

        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
    }

    void Update() {
        if (allStopped) {
            return;
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        /*if (player != null) {
            if (player.transform.position.y > 216f) {
                SetBgmLevelAndVolume(5, 0.2f);
            } else if (player.transform.position.y > 168f) {
                SetBgmLevelAndVolume(4, 0.15f);
            } else if (player.transform.position.y > 120f) {
                SetBgmLevelAndVolume(3, 0.20f);
            } else if (player.transform.position.y > 72f) {
                SetBgmLevelAndVolume(2, 0.20f);
            } else if (player.transform.position.y > 24f) {
                SetBgmLevelAndVolume(1, 0.35f);
            } else {
                SetBgmLevelAndVolume(0, 1f);
            }
        } else {
            SetBgmLevelAndVolume(0, 1f);
        }

        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            SetBgmLevelAndVolume(0, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            SetBgmLevelAndVolume(1, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            SetBgmLevelAndVolume(2, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3)) {
            SetBgmLevelAndVolume(3, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)) {
            SetBgmLevelAndVolume(4, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5)) {
            SetBgmLevelAndVolume(5, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9)) {
            PlaySfx("click");
        }*/
    }

    public void SetBgmLevelAndVolume(int level, float volume) {
        if (level == currentLevel) return;

        for (int i = 0; i < bgm.Length; i++) {
            AudioSource audioSource = bgm[i];
            if (i <= level) {
                audioSource.volume = volume;
            } else {
                audioSource.volume = 0f;
            }
        }
    }

    public void PlaySfx(string name) {
        sfx[0].PlaySound(name);
    }

    public void StopAll() {
        allStopped = true;
        for (int i = 0; i < bgm.Length; i++) {
            AudioSource audioSource = bgm[i];
            audioSource.volume = 0f;
        }
    }
}
