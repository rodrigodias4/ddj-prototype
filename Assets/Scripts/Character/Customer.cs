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

        private bool caught = false;

        // Reference to the speech bubble components
        private GameObject speechBubble;
        private TMP_Text speechBubbleText;
        private Camera mainCamera; // Reference to the main camera
        private CustomerManager customerManager;  // Cached reference to the CustomerManager
        public bool seated = false;
        private bool isLeaving = false; 
        public Chair occupiedChair; // Reference to the chair the customer is sitting on
        private Rigidbody rb;
        public Transform queuePosition;

        private InteractableCustomer interactableCustomer;

        protected override void Start()
        {
            base.Start();

            rb = GetComponent<Rigidbody>();

            customerManager = FindObjectOfType<CustomerManager>();
            // Select a random order for the customer
            customerOrder = (Order)Random.Range(0, System.Enum.GetValues(typeof(Order)).Length);

            // Get the InteractableCustomer component
            interactableCustomer = GetComponent<InteractableCustomer>();
            if (interactableCustomer != null)
            {
                interactableCustomer.enabled = false;
                interactableCustomer.customer = this;
            }

            Debug.Log($"{characterName} entered the diner and is waiting to place an order. Random Order: {customerOrder}");

            // Find the speech bubble GameObject and text component
            speechBubble = transform.Find("SpeechBubble").gameObject;
            speechBubbleText = speechBubble.GetComponentInChildren<TMP_Text>();

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
        public void Serve(Order order)
        {
            if (!isServed)
            {
                if (order != customerOrder)
                {
                    Debug.Log($"{characterName} has been served the wrong order.");
                    speechBubbleText.text = "Me No Likey!";
                    StartCoroutine(DefaultMessage(1f));
                    return;
                }

                interactableCustomer.enabled = false;
                isServed = true;
                Debug.Log($"{characterName} has been served {customerOrder}.");
                speechBubbleText.text = "Yummy!";
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

            // Check if customer is seated
            if (!seated)
            {
                foreach (Chair chair in customerManager.availableChairs)
                {
                    if (Vector3.Distance(transform.position, chair.chairPosition.position) < 2f)
                    {
                        Sit(chair);
                        break;
                    }
                }
            }

            if (caught) {
                Debug.Log($"{characterName} has been caught!");
                return;
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
                Debug.Log($"{characterName} has finished eating and wants to leave the diner.");
            }
            else
            {
                Debug.Log($"{characterName} got impatient and wants to leave the diner without being served.");
            }
            Destroy(gameObject);  // Destroy the customer object
            // Notify CustomerManager that the customer has left
            customerManager?.OnCustomerLeft(this);
        }

        public void Sit(Chair chair)
        {
            interactableCustomer.enabled = true;
            transform.position = chair.chairPosition.position + new Vector3(0, 1f, 0);
            patience *= 2;  // Double the patience when seated
            occupiedChair = chair;
            customerManager.availableChairs.Remove(chair);
            seated = true;
            rb.isKinematic = true;  // Disable physics when seated
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            ShowSpeechBubble();  // Show the speech bubble when seated
        }

        // Handle customer death/cleanup logic
        public override void Die()
        {
            Debug.Log($"{characterName} is dying.");

            customerManager?.OnCustomerLeft(this);

            Destroy(gameObject);
        }

        public void DisableCustomer(){
            caught = true;
        }

        public void EnableCustomer(){
            caught = false;
        }

        private IEnumerator EatFood(float eatingTime)
        {
            // Wait for the specified amount of time (in seconds)
            yield return new WaitForSeconds(eatingTime);

            // Call the Leave method after the wait
            Leave();
        }

        private IEnumerator DefaultMessage(float time)
        {
            // Wait for the specified amount of time (in seconds)
            yield return new WaitForSeconds(time);

            // Call the Leave method after the wait
            speechBubbleText.text = customerOrder.ToString();
        }
    }
}
