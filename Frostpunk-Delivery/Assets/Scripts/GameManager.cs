using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Stores "global" stuff, like the player's score/money

    private int playerScore = 0;
    [SerializeField] TextMeshProUGUI scoreText;

    public void UpdateScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = string.Format("Score: {0}", playerScore);
    }
}
