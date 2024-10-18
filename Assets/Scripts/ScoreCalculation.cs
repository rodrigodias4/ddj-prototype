using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Timer;

public class ScoreCalculation : MonoBehaviour
{
    // Score is presented at the end of the game
    public static int score;
    public static int dishCounter;
    public static int customerKilled;
    public static int tips;
    public TextMeshProUGUI scoreText;

    public static int highScore = 0;

    public Timer.Timer timer;



    void Start()
    {
        score = 0;
        dishCounter = 0;
        customerKilled = 0;
        tips = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Update()
    {
        // If the game is over, the score is calculated
        if (timer.timeremaining <= 0)
        {
            score = dishCounter * 50 - customerKilled * 100 + tips;
            scoreText.text = "Score: " + score;
            scoreText.color = Color.yellow;
        }

        if (score > highScore){
            highScore = score;
        }
    }

    public static void IncrementDishCounter()
    {
        dishCounter++;
    }

    public static void IncrementCustomerKilled()
    {
        customerKilled++;
    }

    public static void AddTips(int tipAmount)
    {
        tips += tipAmount;
    }
}
