using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{
    // Script for managing the in-game pause screen & tutorial pop-up
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject helpScreen;
    [SerializeField] GameObject tutorialScreen;
    private SFXPlayer sfxPlayer;
    private bool paused;

    private void Awake()
    {
        sfxPlayer = FindObjectOfType<SFXPlayer>();
        tutorialScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
                Pause();
            else
                Unpause();
        }
    }

    public void Pause()
    {
        sfxPlayer.ClickButtonSound();
        pauseScreen.SetActive(true);
        paused = true;
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        sfxPlayer.ClickButtonSound();
        helpScreen.SetActive(false);
        pauseScreen.SetActive(false);
        paused = false;
        Time.timeScale = 1;
    }

    public void BackToMenu()
    {
        sfxPlayer.ClickButtonSound();
        SceneManager.LoadScene(0);
    }

    public void HelpScreen()
    {
        sfxPlayer.ClickButtonSound();
        helpScreen.SetActive(true);
    }

    public void LeaveHelpScreen()
    {
        sfxPlayer.ClickButtonSound();
        helpScreen.SetActive(false);
    }

    public void LeaveTutorial()
    {
        sfxPlayer.ClickButtonSound();
        tutorialScreen.SetActive(false);
        Time.timeScale = 1f;
    }
}
