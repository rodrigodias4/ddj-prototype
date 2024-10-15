using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using Assets.Scripts.Characters;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private float nextUpdateTime = 0.0f;
    private bool gameEnded = false;
    public float timeElapsed = 0.0f;
    public CustomerSpawner customerSpawner {get; private set; }
    public static class GameConstants
    {
        public const float UPDATE_INTERVAL = 2.0f;
        public const int TIME_LIMIT = 500;
    }

    void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        if (!this.gameEnded)
        {
            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + GameConstants.UPDATE_INTERVAL;
                this.timeElapsed += GameConstants.UPDATE_INTERVAL;
            }

            if (this.timeElapsed >= GameConstants.TIME_LIMIT)
            {
                this.gameEnded = true;
            }
        }
    }
}