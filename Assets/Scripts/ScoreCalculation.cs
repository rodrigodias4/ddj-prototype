using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Timer;
using UnityEngine.Assertions;

public class ScoreCalculation : MonoBehaviour
{
    // Score is presented at the end of the game
    private static int score;
    private static int dishCounter;
    private static int customerKilled;
    private static int tips;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreSubText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject panel;

    [SerializeField] private Timer.Timer timer;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Assert.IsNotNull(gameManager);
        
        score = 0;
        dishCounter = 0;
        customerKilled = 0;
        tips = 0;
        gameManager.gameOver = false;
        gameManager.highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("Score: " + score);
        scoreText.gameObject.SetActive(true);
        finalScoreText.gameObject.SetActive(false);
        highScoreText.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    void Update()
    {
        if (gameManager.gameOver) return;
        score = (int)Mathf.Clamp(dishCounter * 50 - customerKilled * 100 + tips, 0, Mathf.Infinity);
        scoreText.text = score.ToString();
        // If the game is over, the score is calculated
        if (timer.timeremaining <= 0)
        {
            gameManager.gameOver = true;
            
            scoreText.gameObject.SetActive(false);
            scoreSubText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            
            finalScoreText.text = "Score: " + score;
            highScoreText.text = "High Score: " + gameManager.highScore;
            
            finalScoreText.gameObject.SetActive(true);
            highScoreText.gameObject.SetActive(true);
            panel.SetActive(true);
            
            Debug.Log("Score: " + score);
            EndGameStopTime();
        }

        if (score > gameManager.highScore){
            gameManager.highScore = score;
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

    public static void IncrementTips(int tipAmount)
    {
        tips += tipAmount;
    }

    // ik this should be in gamemanager but. 
    // this is probably the only place where we'll need it. get silly
    void EndGameStopTime ()
    {
        Time.timeScale = 0;
    }
}
