using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Assets.Scripts.Characters{
    public class EnemyAI : NPC
{
    private NavMeshAgent agent;
    private Transform player;
    private bool caught = false;
    public float pushForce = 5f;   // The force to push the player

    protected override void  Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    private void Update()
    {
        if (caught) {
            Debug.Log($"{characterName} has been caught!");
            return;
        }

        if (player != null)
        {
            // Move towards the player
            agent.SetDestination(player.position);

            // Check if enemy is close enough to push the player
            if (Vector3.Distance(transform.position, player.position) < 2f)
            {
                PushPlayer();
            }
        }
    }

    private void PushPlayer()
    {
        // Get direction to push the player
        Vector3 pushDirection = (player.position - transform.position).normalized;

        // Apply the push force to the player
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }


    public override void GetCaught(){
        caught = true;
    }

    public override void GetUnCaught(){
        caught = false;
    }

    public override bool CanBeCaught(){
        return !caught;
    }

    public override void Die()
    {
        Debug.Log($"{characterName} is dying.");

        Destroy(gameObject);
    }
}

}
