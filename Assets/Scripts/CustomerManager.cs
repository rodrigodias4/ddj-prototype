using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;  // Required for NavMesh

namespace Assets.Scripts.Characters
{
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager Instance; // Singleton instance
        public GameObject customerPrefab;      // Prefab of the customer
        public Transform spawnPoint;           // Fixed spawn location
        public List<Transform> queuePositions; // List of queue positions (back to front)
        public float timeBetweenSpawns = 15f;   // Time delay between each customer spawn
        public float moveSpeed = 2f;           // Not used anymore since NavMeshAgent will control the speed
        public int customerCount = 1;
        private Queue<GameObject> customerQueue = new Queue<GameObject>(); // Tracks customers in the queue

        // List of all chairs and a list of occupied chairs
        private List<GameObject> chairs = new List<GameObject>();
        private HashSet<GameObject> occupiedChairs = new HashSet<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            // Find all chairs at the start of the game
            chairs.AddRange(GameObject.FindGameObjectsWithTag("Chair"));

            // Start spawning customers over time
            StartCoroutine(SpawnCustomersOverTime());
        }

        // Coroutine to spawn customers at intervals
        IEnumerator SpawnCustomersOverTime()
        {
            while (true)  // Infinite loop to keep spawning customers
            {
                SpawnCustomer(); // Spawn a customer and add to queue
                yield return new WaitForSeconds(timeBetweenSpawns); // Wait before spawning the next customer
            }
        }

        // Method to spawn a customer
        void SpawnCustomer()
        {
            if (customerQueue.Count < queuePositions.Count) // Only spawn if there's space in the queue
            {
                // Instantiate the customer at the spawn point
                GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                Customer customerScript = newCustomer.GetComponent<Customer>();

                // Add a NavMeshAgent to the customer if it doesn't already exist
                NavMeshAgent agent = newCustomer.GetComponent<NavMeshAgent>();
                if (agent == null)
                {
                    agent = newCustomer.AddComponent<NavMeshAgent>();
                }

                // Set the agent's speed (optional if set in the prefab)
                agent.speed = moveSpeed;

                // Add the customer to the queue
                customerQueue.Enqueue(newCustomer);

                // Move the customer to the back of the queue using NavMesh
                MoveCustomerToPosition(newCustomer, queuePositions[customerQueue.Count - 1].position);

                // Set properties (optional)
                customerScript.characterName = "Customer " + customerCount++;
            }
            else
            {
                Debug.Log("Queue is full!");
            }
        }

        // Use NavMeshAgent to move the customer to the target position
        void MoveCustomerToPosition(GameObject customer, Vector3 targetPosition)
        {
            NavMeshAgent agent = customer.GetComponent<NavMeshAgent>();

            if (agent != null && agent.isOnNavMesh)
            {
                // Set the destination for the NavMeshAgent
                agent.SetDestination(targetPosition);
            }
        }

        // Call this when a customer leaves the queue
        public void Sit()
        {
            if (customerQueue.Count > 0)
            {
                // Remove the customer from the front of the queue
                GameObject servedCustomer = customerQueue.Dequeue();

                // Move all remaining customers forward in the queue
                for (int i = 0; i < customerQueue.Count; i++)
                {
                    GameObject remainingCustomer = customerQueue.ToArray()[i];
                    MoveCustomerToPosition(remainingCustomer, queuePositions[i].position);
                }

                // Now move the served customer to a random unoccupied chair
                GameObject selectedChair = FindUnoccupiedChair();
                if (selectedChair != null)
                {
                    // Mark the chair as occupied
                    occupiedChairs.Add(selectedChair);

                    // Move the customer to the selected chair using NavMesh
                    MoveCustomerToPosition(servedCustomer, selectedChair.transform.position);
                }
                else
                {
                    Debug.Log("No unoccupied chairs available!");
                }
            }
        }

        // Method to find a random unoccupied chair
        GameObject FindUnoccupiedChair()
        {
            List<GameObject> availableChairs = new List<GameObject>();

            // Filter out chairs that are already occupied
            foreach (var chair in chairs)
            {
                if (!occupiedChairs.Contains(chair))
                {
                    availableChairs.Add(chair);
                }
            }

            // If we have available chairs, select one at random
            if (availableChairs.Count > 0)
            {
                int randomIndex = Random.Range(0, availableChairs.Count);
                return availableChairs[randomIndex];
            }

            // Return null if no unoccupied chairs are found
            return null;
        }
    }
}
