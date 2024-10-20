using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Characters
{
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager Instance; // Singleton instance
        public GameObject customerPrefab;      // Prefab of the customer
        public List<Transform> queuePositions; // List of queue positions (back to front)
        private List<Transform> availableQueuePositions = new(); // List of available queue positions
        public Transform exitPoint;            // Exit point for customers
        public float timeBetweenSpawns = 15f;   // Time delay between each customer spawn
        private int customerCount = 1;
        // List of all chairs
        public List<Chair> availableChairs = new List<Chair>();

        // Start is called before the first frame update
        void Start()
        {
            availableQueuePositions.AddRange(queuePositions);
            // Find all chair objects at the start of the game and assign them to the chairs list
            foreach (GameObject chairObject in GameObject.FindGameObjectsWithTag("Chair"))
            {
                Chair chair = chairObject.GetComponent<Chair>();
                if (chair != null)
                {
                    availableChairs.Add(chair); // Add the chair component to the list
                }
            }

            Debug.Log($"Number of chairs: {availableChairs.Count}");

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
            if (availableQueuePositions.Count > 0) // Only spawn if there's space in the queue
            {
                Transform spawnPosition = availableQueuePositions.First();

                // Instantiate the customer at the spawn position
                GameObject newCustomer = Instantiate(customerPrefab, spawnPosition.position, Quaternion.identity);
                Customer customerScript = newCustomer.GetComponent<Customer>();

                // Store the original queue position in the customer's script
                customerScript.queuePosition = spawnPosition;

                // Remove the queue position from the list
                availableQueuePositions.RemoveAt(0);

                // Set properties (optional)
                customerScript.characterName = "Customer " + customerCount++;
            }
            else
            {
                Debug.Log("Queue is full!");
            }
        }

        // Called when a customer leaves the diner
        public void OnCustomerLeft(Customer customer)
        {
            Debug.Log($"{customer.characterName} is leaving.");

            // Free up any occupied chairs if the customer was sitting
            if (customer.occupiedChair != null)
            {
                availableChairs.Add(customer.occupiedChair);
                customer.occupiedChair.customer = null;
                customer.occupiedChair = null;
            }

            // Add the customer's original queue position back to the available queue positions
            if (customer.queuePosition != null && !availableQueuePositions.Contains(customer.queuePosition))
            {
                availableQueuePositions.Add(customer.queuePosition);
            }
        }
    }
}
