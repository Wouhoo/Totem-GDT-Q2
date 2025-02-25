using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXPlayer : MonoBehaviour
{
    // Script for managing the sound effect player, which is (currently) the central game object that plays all sound effects
    private AudioSource audioSource;
    [SerializeField] AudioClip buttonClickSound;
    private AudioSource blizzardAudioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = MainManager.instance.sfxVolume;
        blizzardAudioSource = gameObject.AddComponent<AudioSource>();
        blizzardAudioSource.clip = MainManager.instance.blizzardSound;
        blizzardAudioSource.loop = true;
        blizzardAudioSource.Play();
        UpdateBlizzardVolume();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateBlizzardVolume();
    }

    public void UpdateBlizzardVolume()
    {
        float baseVolume = SceneManager.GetActiveScene().name == "MainMenu" ? MainManager.instance.blizzardVolumeMenu : MainManager.instance.blizzardVolumeLevel;
        blizzardAudioSource.volume = baseVolume * MainManager.instance.sfxVolume;
    }
    public float GetButtonClickSoundLength()
    {
        return buttonClickSound.length;
    }
    public void ClickButtonSound()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }
}
