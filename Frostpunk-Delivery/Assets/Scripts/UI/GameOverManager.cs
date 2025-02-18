using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameOverManager : MonoBehaviour
{
    // Script for managing the in-game pause screen
    [SerializeField] GameObject gameOverScreen;
    private SFXPlayer sfxPlayer;

    private void Awake()
    {
        sfxPlayer = FindObjectOfType<SFXPlayer>();
    }

    public void Die()
    {
        sfxPlayer.ClickButtonSound();
        gameOverScreen.SetActive(true);
        // Time.timeScale = 0;
    }

    public void Retry()
    {
        sfxPlayer.ClickButtonSound();
        gameOverScreen.SetActive(false);
        // Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void BackToMenu()
    {
        sfxPlayer.ClickButtonSound();
        SceneManager.LoadScene(0);
    }
}
