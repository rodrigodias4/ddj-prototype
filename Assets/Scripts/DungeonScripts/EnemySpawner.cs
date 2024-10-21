using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Characters;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;           // The enemy prefab to instantiate
    public Transform[] spawnPoints;          // Array of spawn points
    public Transform player;                 // Reference to the player
    public float spawnInterval = 3f;         // Time interval between each spawn
    public float pushForce = 5f;             // Push force applied by enemies
    public float lifetime = 10f;             // Enemy lifetime before destruction
    public float activationDistance = 15f;   // Distance from player to activate spawning

    private void Start()
    {
        // Start the infinite spawning loop
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                // Check if the player is within range of the spawn point
                if (Vector3.Distance(player.position, spawnPoint.position) <= activationDistance)
                {
                    SpawnEnemy(spawnPoint);
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy(Transform spawnPoint)
    {
        // Instantiate enemy at the designated spawn point
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Set up the enemy AI to follow the player and have a lifetime
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        enemyAI.SetTarget(player);
        enemyAI.pushForce = pushForce;
        enemyAI.lifetime = lifetime;
    }
}
