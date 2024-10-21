using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Timer
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] float levelTime = 120f;
        public float timeremaining { get; set; }

        // Update is called once per frame

        void Start()
        {
            timeremaining = levelTime;
        }
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

}


