using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Stores "global" stuff, like the player's score/money and the number of quests they've failed
    public int playerMoney = 0;
    private int playerScore = 0;
    public int failedQuests; // This should probably be displayed on the HUD somewhere...

    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI causeOfDeathText;
    [SerializeField] AudioSource gameOverAudioSource;
    [SerializeField] AudioSource levelThemeAudioSource;
    [SerializeField] AudioSource miscAudioSource;
    [SerializeField] AudioClip gameOverClip;
    [SerializeField] AudioClip successfulDeliveryClip;
    [SerializeField] AudioClip unsuccessfulDeliveryClip;
    [SerializeField] AudioClip refuelClip;
    [SerializeField] AudioClip collisionClip;
    [SerializeField] AudioClip alarmClip;
    [SerializeField] AudioClip upgradeMenuOpenClip;
    [SerializeField] AudioClip upgradeMenuCloseClip;
    [SerializeField] AudioClip upgradeButtonClickClip;
    [SerializeField] AudioClip unsuccessfulUpgradeClip;

    private bool isGameOver = false;

    public void UpdateScore(int scoreToAdd)
    {
        playerMoney += scoreToAdd;
        if (scoreToAdd > 0) // Any money *gained* should be added to score; however, subtractions from money do not subtract score.
            playerScore += scoreToAdd;
        moneyText.text = string.Format("${0}", playerMoney);
    }

    public void AddFailedQuest()
    {
        failedQuests++;
        if (failedQuests == 3) // Game over when 3rd quest is failed
        {
            GameOver(0);
        }
    }

    public void GameOver(int causeOfDeath)
    {
        if (isGameOver) return; // Add this line to prevent multiple executions
        isGameOver = true; // Add this line to set the flag

        Debug.Log("GameOver method called"); // Add this line

        // causeOfDeath is an integer code indicating the player's cause of death:
        // 0 = too many failed deliveries
        // 1 = took too much damage
        // 2 = ran out of fuel
        scoreText.text = string.Format("Score: {0}", playerScore);
        switch (causeOfDeath) // Change cause of death text to reflect cause of death
        {
            case 0: causeOfDeathText.text = "Failed too many deliveries..."; break;
            case 1: causeOfDeathText.text = "Busted up your car..."; break;
            case 2: causeOfDeathText.text = "Ran out of fuel..."; break;
            default: causeOfDeathText.text = "Congratulations, you found an unforeseen way too fail!"; break;
        }
        gameOverScreen.gameObject.SetActive(true);
        levelThemeAudioSource.Stop();
        gameOverAudioSource.Stop();
        gameOverAudioSource.PlayOneShot(gameOverClip);
        Time.timeScale = 0.0f;
    }

    public void PlaySuccessfulDeliverySound()
    {
        miscAudioSource.PlayOneShot(successfulDeliveryClip);
    }

    public void PlayUnsuccessfulDeliverySound()
    {
        miscAudioSource.PlayOneShot(unsuccessfulDeliveryClip);
    }

    public void PlayRefuelSound()
    {
        miscAudioSource.PlayOneShot(refuelClip);
    }

    public void PlayCollisionSound()
    {
        miscAudioSource.PlayOneShot(collisionClip);
    }

    public void PlayAlarmSound()
    {
        miscAudioSource.PlayOneShot(alarmClip);
    }

    public void PlayUpgradeMenuOpenSound()
    {
        miscAudioSource.PlayOneShot(upgradeMenuOpenClip);
    }

    public void PlayUpgradeMenuCloseSound()
    {
        miscAudioSource.PlayOneShot(upgradeMenuCloseClip);
    }

    public void PlayUpgradeButtonClickSound()
    {
        miscAudioSource.PlayOneShot(upgradeButtonClickClip);
    }

    public void PlayUnsuccessfulUpgradeSound()
    {
        miscAudioSource.PlayOneShot(unsuccessfulUpgradeClip);
    }
}