using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Stores "global" stuff, like the player's score/money

    public int playerScore { get; private set; } = 0;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameOverText;

    public void UpdateScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = string.Format("Score: {0}", playerScore);
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }
}
