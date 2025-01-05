using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Script for managing the main menu.
    [SerializeField] GameObject helpScreen;
    private SFXPlayer sfxPlayer;

    void Start()
    {
        sfxPlayer = FindObjectOfType<SFXPlayer>();
    }

    public void StartGame()
    {
        sfxPlayer.ClickButtonSound();
        SceneManager.LoadScene(1); // Make sure the level has index 1 in the build settings !!!
    }

    public void QuitGame()
    {
        sfxPlayer.ClickButtonSound();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void HelpScreen()
    {
        sfxPlayer.ClickButtonSound();
        Time.timeScale = 0;
        helpScreen.SetActive(true);
    }

    public void LeaveHelpScreen()
    {
        sfxPlayer.ClickButtonSound();
        Time.timeScale = 1;
        helpScreen.SetActive(false);
    }
}
