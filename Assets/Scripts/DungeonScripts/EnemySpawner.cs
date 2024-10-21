using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Characters;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;     // The enemy prefab to instantiate
    public Transform spawnPoint;       // Where enemies will spawn
    public Transform player;           // Reference to the player
    public float spawnInterval = 3f;   // How often enemies will spawn
    public float pushForce = 5f;       // How hard enemies will push the player

    private void Start()
    {
        // Start the infinite spawning loop
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        enemyAI.SetTarget(player);
    }
}

