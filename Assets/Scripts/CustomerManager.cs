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

        // List of all chairs
        private List<Chair> chairs = new List<Chair>();

        // Start is called before the first frame update
        void Start()
        {
            // Find all chair objects at the start of the game and assign them to the chairs list
            foreach (GameObject chairObject in GameObject.FindGameObjectsWithTag("Chair"))
            {
                Chair chair = chairObject.GetComponent<Chair>();
                if (chair != null)
                {
                    chairs.Add(chair); // Add the chair component to the list
                }
            }

            Debug.Log($"Number of chairs: {chairs.Count}");

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
                Chair selectedChair = FindUnoccupiedChair();
                Debug.Log($"Selected chair: {selectedChair}");
                if (selectedChair != null)
                {
                    // Remove the customer from the front of the queue
                    Customer servedCustomer = customerQueue.Dequeue();

                    // Mark the chair as occupied
                    selectedChair.customer = servedCustomer;
                    servedCustomer.occupiedChair = selectedChair;

                    // Move the customer to the selected chair using NavMesh
                    servedCustomer.isMovingToSeat = true;
                    servedCustomer.MoveCustomerToPosition(selectedChair.chairPosition.position);

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
        Chair FindUnoccupiedChair()
        {
            List<Chair> availableChairs = new List<Chair>();

            // Filter out chairs that are already occupied
            foreach (var chair in chairs)
            {
                if (chair.customer == null)
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

        // Called when a customer leaves the diner
        public void OnCustomerLeft(Customer customer)
        {
            Debug.Log($"{customer.characterName} is leaving.");

            // Free up any occupied chairs if the customer was sitting
            if (customer.occupiedChair != null)
            {
                customer.occupiedChair.customer = null;
                customer.occupiedChair = null;
            }
            else if (customerQueue.Contains(customer))
            {
                DequeueSpecificCustomer(customer);
                // Move all remaining customers forward in the queue
                for (int i = 0; i < customerQueue.Count; i++)
                {
                    Customer remainingCustomer = customerQueue.ToArray()[i];
                    remainingCustomer.MoveCustomerToPosition(queuePositions[i].position);
                }
            }
        }

        // Coroutine to move the customer after a short delay
        IEnumerator MoveCustomerAfterInitialization(Customer customerScript)
        {
            // Wait a frame or two for the NavMeshAgent to initialize
            yield return new WaitForSeconds(0.1f);

            // Move the customer to the back of the queue using NavMesh
            customerScript.MoveCustomerToPosition(queuePositions[customerQueue.Count - 1].position);
        }

        public void DequeueSpecificCustomer(Customer targetCustomer)
        {
            Queue<Customer> newQueue = new Queue<Customer>();

            // Iterate through the existing queue
            while (customerQueue.Count > 0)
            {
                // Dequeue each customer
                Customer customer = customerQueue.Dequeue();

                // If it's not the customer we want to remove, enqueue it to the new queue
                if (customer != targetCustomer)
                {
                    newQueue.Enqueue(customer);
                }
                else
                {
                    Debug.Log($"Removed {customer.characterName} from the queue.");
                }
            }

            // Replace the old queue with the new one
            customerQueue = newQueue;
        }

    }
}
