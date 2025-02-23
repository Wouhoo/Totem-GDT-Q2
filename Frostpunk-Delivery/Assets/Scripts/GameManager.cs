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

    public void UpdateScore(int scoreToAdd)
    {
        playerMoney += scoreToAdd;
        if(scoreToAdd > 0) // Any money *gained* should be added to score; however, subtractions from money do not subtract score.
            playerScore += scoreToAdd;
        moneyText.text = string.Format("${0}", playerMoney);
    }

    public void AddFailedQuest()
    {
        failedQuests++;
        if(failedQuests == 3) // Game over when 3rd quest is failed
        {
            GameOver(0);
        }
    }

    public void GameOver(int causeOfDeath)
    {
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
        Time.timeScale = 0.0f;
    }
}
