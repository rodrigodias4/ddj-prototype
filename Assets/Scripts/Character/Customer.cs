using UnityEngine;
using TMPro;
using UnityEngine.AI;
using System.Collections;

namespace Assets.Scripts.Characters
{
    public class Customer : NPC
    {
        public enum Order { Burger, Ham, Stew };
        public float patience = 60f;       // How long the customer will wait before leaving
        public bool isServed = false;      // Whether the customer has been served
        private float waitTime = 0f;       // Internal tracking of how long the customer has waited
        private float eatingTime = 3f;     // How long the customer takes to eat
        public Order customerOrder;        // Store the current order

        // Reference to the speech bubble components
        private GameObject speechBubble;
        private TMP_Text speechBubbleText;
        private UnityEngine.AI.NavMeshAgent agent; // Reference to the NavMeshAgent component
        private Camera mainCamera; // Reference to the main camera
        private CustomerManager customerManager;  // Cached reference to the CustomerManager
        private Vector3 initialPosition;          // Initial position of the customer
        public bool isMovingToSeat = false;
        private bool isLeaving = false; 

        protected override void Start()
        {
            base.Start();

            initialPosition = transform.position;

            customerManager = FindObjectOfType<CustomerManager>();
            // Select a random order for the customer
            customerOrder = (Order)Random.Range(0, System.Enum.GetValues(typeof(Order)).Length);

            Debug.Log($"{characterName} entered the diner and is waiting to place an order. Random Order: {customerOrder}");

            // Find the speech bubble GameObject and text component
            speechBubble = transform.Find("SpeechBubble").gameObject;
            speechBubbleText = speechBubble.GetComponentInChildren<TMP_Text>();

            // Add a NavMeshAgent to the customer if it doesn't already exist
            agent = gameObject.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
            }
            Debug.Log(agent);
            
            // Set the agent's speed (optional if set in the prefab)
            agent.speed = speed;

            // Cache a reference to the main camera
            mainCamera = Camera.main;
        }

        // Show the speech bubble with the customer's order
        public void ShowSpeechBubble()
        {
            if (speechBubble != null && speechBubbleText != null)
            {
                speechBubble.SetActive(true);
                speechBubbleText.text = customerOrder.ToString();
            }
        }

        // Serve the customer
        public void Serve()
        {
            if (!isServed)
            {
                isServed = true;
                Debug.Log($"{characterName} has been served {customerOrder}.");
                HideSpeechBubble();
                // Start the coroutine to wait before leaving
                StartCoroutine(EatFood(eatingTime)); 

            }
            else
            {
                Debug.Log($"{characterName} has already been served.");
            }
        }

        // Hide the speech bubble after being served
        private void HideSpeechBubble()
        {
            if (speechBubble != null)
            {
                speechBubble.SetActive(false);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isServed)
            {
                waitTime += Time.deltaTime;
                if (waitTime >= patience && !isLeaving)
                {
                    Leave();  // Customer leaves after losing patience
                }
            }
            // Check if customer is moving to a seat and has reached the destination
            if (isMovingToSeat && agent != null && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                isMovingToSeat = false;  // Stop checking once the seat is reached
                ShowSpeechBubble();  // Show the speech bubble when seated
            }
            
            // Check if customer is leaving and has reached the initial position
            if (isLeaving && agent != null && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                isLeaving = false;  // Customer has finished leaving
                Debug.Log($"{characterName} has left the diner.");
                // Destroy the customer GameObject
                Destroy(gameObject);
            }

            // Make sure the speech bubble is always facing the camera
            if (speechBubble != null && mainCamera != null)
            {
                // Make the speech bubble look at the camera
                speechBubble.transform.LookAt(speechBubble.transform.position + mainCamera.transform.rotation * Vector3.forward,
                    mainCamera.transform.rotation * Vector3.up);
            }
        }

        // Customer leaves after being served or when impatient
        public void Leave()
        {
            if (isServed)
            {
                Debug.Log($"{characterName} has finished eating and left the diner.");
            }
            else
            {
                Debug.Log($"{characterName} got impatient and left the diner without being served.");
            }

            isLeaving = true;  // Set the leaving flag to true
            
            HideSpeechBubble();

            MoveCustomerToPosition(initialPosition);

            // Notify CustomerManager that the customer has left
            customerManager?.OnCustomerLeft(this);
        }

        // Handle customer death/cleanup logic
        protected override void Die()
        {
            Debug.Log($"{characterName} is dying.");

            customerManager?.OnCustomerLeft(this);

            Destroy(gameObject);
        }

        // Use NavMeshAgent to move the customer to the target position
        public void MoveCustomerToPosition(Vector3 targetPosition)
        {
            Debug.Log($"{characterName} is moving to position: {targetPosition}");
            if (agent != null && agent.isOnNavMesh)
            {
                // Set the destination for the NavMeshAgent
                agent.SetDestination(targetPosition);
            }
        }
        private IEnumerator EatFood(float eatingTime)
        {
            // Wait for the specified amount of time (in seconds)
            yield return new WaitForSeconds(eatingTime);

            // Call the Leave method after the wait
            Leave();
        }
    }
}
