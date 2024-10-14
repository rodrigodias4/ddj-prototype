using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer: MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timeremaining;

    // Update is called once per frame
    void Update()
    {
        if (timeremaining > 0)
        {
            timeremaining -= Time.deltaTime;
        }

        if (timeremaining <= 0)
        {
            timerText.text = "00:00";
            timerText.color = Color.red;
            return;
        }
    
        int minutes = Mathf.FloorToInt(timeremaining / 60);
        int seconds = Mathf.FloorToInt(timeremaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
