using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculation : MonoBehaviour
{
    // Score is presented at the end of the game
    public static int score;
    public static int dishCounter
    public static int customerKilled;
    public static int tips;



    void Start()
    {
        score = 0;
        dishCounter = 0;
        customerKilled = 0;
        tips = 0;
    }

    void Update()
    {
        // If the game is over, the score is calculated
        if (Serve){
            dishCounter++;
        }
        if (Kill){
            customerKilled++;
        }
        if (Tip){
            tips += tips.amount;
        }
        if (Timer.timeremaining <= 0)
        {
            score = dishCounter * 50 - customerKilled * 100 + tips;
        }
    }
}
