using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;      // Prefab of the customer
    public Transform spawnPoint;           // Fixed spawn location
    public List<Transform> queuePositions; // List of queue positions (back to front)
    public float timeBetweenSpawns = 15f;   // Time delay between each customer spawn
    public float moveSpeed = 2f;           // Speed at which customers move to their position in the queue

    private Queue<GameObject> customerQueue = new Queue<GameObject>(); // Tracks customers in the queue

    // Start is called before the first frame update
    void Start()
    {
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
            customerQueue.Enqueue(newCustomer);

            // Move the customer to the back of the queue
            StartCoroutine(MoveCustomerToPosition(newCustomer, queuePositions[customerQueue.Count - 1].position));

            // Set properties (optional)
            customerScript.characterName = "Customer " + customerQueue.Count;
            customerScript.PlaceOrder("Dish " + customerQueue.Count);
        }
        else
        {
            Debug.Log("Queue is full!");
        }
    }

    // Coroutine to move customer to their queue position
    IEnumerator MoveCustomerToPosition(GameObject customer, Vector3 targetPosition)
    {
        while (Vector3.Distance(customer.transform.position, targetPosition) > 0.05f)
        {
            // Move towards the target position
            customer.transform.position = Vector3.MoveTowards(customer.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait until the next frame
        }
        
        // Ensure the customer is exactly at the position
        customer.transform.position = targetPosition;
    }

    // Call this when a customer leaves the queue (e.g., after being served)
    public void ServeCustomer()
    {
        if (customerQueue.Count > 0)
        {
            // Remove the customer from the front of the queue
            GameObject servedCustomer = customerQueue.Dequeue();

            // Move all remaining customers forward in the queue
            for (int i = 0; i < customerQueue.Count; i++)
            {
                GameObject remainingCustomer = customerQueue.ToArray()[i];
                StartCoroutine(MoveCustomerToPosition(remainingCustomer, queuePositions[i].position));
            }
        }
    }
}
