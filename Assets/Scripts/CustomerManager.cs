using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Characters
{
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager Instance; // Singleton instance
        public GameObject customerPrefab;      // Prefab of the customer
        public Transform spawnPoint;           // Fixed spawn location
        public List<Transform> queuePositions; // List of queue positions (back to front)
        public float timeBetweenSpawns = 15f;   // Time delay between each customer spawn
        private int customerCount = 1;
        private Queue<Customer> customerQueue = new Queue<Customer>(); // Tracks customers in the queue

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

                // Add the customer to the queue
                customerQueue.Enqueue(customerScript);

                // Move the customer to the back of the queue using NavMesh
                StartCoroutine(MoveCustomerAfterInitialization(customerScript));

                // Set properties (optional)
                customerScript.characterName = "Customer " + customerCount++;
            }
            else
            {
                Debug.Log("Queue is full!");
            }
        }

        // Call this when a customer leaves the queue
        public void Sit()
        {
            if (customerQueue.Count > 0)
            {
                // Move the served customer to a random unoccupied chair
                GameObject selectedChair = FindUnoccupiedChair();
                if (selectedChair != null)
                {
                    // Mark the chair as occupied
                    occupiedChairs.Add(selectedChair);

                    // Remove the customer from the front of the queue
                    Customer servedCustomer = customerQueue.Dequeue();

                    // Move the customer to the selected chair using NavMesh
                    servedCustomer.isMovingToSeat = true;
                    servedCustomer.MoveCustomerToPosition(selectedChair.transform.position);

                    // Move all remaining customers forward in the queue
                    for (int i = 0; i < customerQueue.Count; i++)
                    {
                        Customer remainingCustomer = customerQueue.ToArray()[i];
                        remainingCustomer.MoveCustomerToPosition(queuePositions[i].position);
                    }
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

            return null;
        }

        // Called when a customer has been served
        public void OnCustomerServed(Customer customer)
        {
            Debug.Log($"{customer.characterName} has been served and is now sitting.");
        }

        // Called when a customer leaves the diner
        public void OnCustomerLeft(Customer customer)
        {
            Debug.Log($"{customer.characterName} is leaving.");

            // Free up any occupied chairs if the customer was sitting
            GameObject chair = FindOccupiedChairByCustomer(customer.gameObject);
            if (chair != null)
            {
                occupiedChairs.Remove(chair);
            }
            else if (customerQueue.Contains(customer))
            {
                customerQueue.Dequeue();
                // Move all remaining customers forward in the queue
                for (int i = 0; i < customerQueue.Count; i++)
                {
                    Customer remainingCustomer = customerQueue.ToArray()[i];
                    remainingCustomer.MoveCustomerToPosition(queuePositions[i].position);
                }
            }
        }

        // Find the occupied chair for a specific customer
        GameObject FindOccupiedChairByCustomer(GameObject customer)
        {
            foreach (var chair in occupiedChairs)
            {
                // Assuming the customer is directly sitting on the chair's position
                if (Vector3.Distance(chair.transform.position, customer.transform.position) < 0.5f)
                {
                    return chair;
                }
            }
            return null;
        }
        // Coroutine to move the customer after a short delay
        IEnumerator MoveCustomerAfterInitialization(Customer customerScript)
        {
            // Wait a frame or two for the NavMeshAgent to initialize
            yield return new WaitForSeconds(0.1f);

            // Move the customer to the back of the queue using NavMesh
            customerScript.MoveCustomerToPosition(queuePositions[customerQueue.Count - 1].position);
        }
    }
}
