using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    // Manages "overarching" stuff that should be saved between scenes (like music/sfx volume).
    public static MainManager instance;

    public float musicVolume = 0.2f;
    public float sfxVolume = 0.5f;
    public float blizzardVolumeMenu = 0.1f;
    public float blizzardVolumeLevel = 0.5f;

    public AudioClip blizzardSound;

    // Set up the main manager properly
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
