using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Stores "global" stuff, like the player's score/money
    public int playerMoney { get; private set; } = 0;
    private int playerScore = 0;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI scoreText;

    public void UpdateScore(int scoreToAdd)
    {
        playerMoney += scoreToAdd;
        if(scoreToAdd > 0) // Any money *gained* should be added to score; however, subtractions from money do not subtract score.
            playerScore += scoreToAdd;
        moneyText.text = string.Format("${0}", playerMoney);
    }

    public void GameOver()
    {
        scoreText.text = string.Format("Score: {0}", playerScore);
        gameOverScreen.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
