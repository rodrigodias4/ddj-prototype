using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject panel;
    public static int highScore = 0;

    public Timer.Timer timer;



    void Start()
    {
        score = 0;
        dishCounter = 0;
        customerKilled = 0;
        tips = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("Score: " + score);
        scoreText.gameObject.SetActive(true);
        finalScoreText.gameObject.SetActive(false);
        highScoreText.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    void Update()
    {

        score = dishCounter * 50 - customerKilled * 100 + tips;
        scoreText.text = "Score: " + score;
        // If the game is over, the score is calculated
        if (timer.timeremaining <= 0)
        {
            scoreText.text = "Score: " + score + "\nHigh Score: " + highScore;
            scoreText.gameObject.SetActive(false);
            finalScoreText.text = "Score: " + score;
            highScoreText.text = "Hi Score: " + highScore;
            finalScoreText.color = Color.yellow;
            highScoreText.color = Color.yellow;
            finalScoreText.gameObject.SetActive(true);
            Debug.Log("Score: " + score);
            highScoreText.gameObject.SetActive(true);
            panel.SetActive(true);
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
