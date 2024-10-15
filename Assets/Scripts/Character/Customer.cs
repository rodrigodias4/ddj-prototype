using UnityEngine;
using TMPro;  // If using TextMeshPro

namespace Assets.Scripts.Characters {
    public class Customer : NPC
    {
        // Additional properties for Customer
        public enum Order { Burger, Ham, Stew };
        public float patience;       // How long the customer will wait before leaving
        public bool isServed;        // Whether the customer has been served

        private float waitTime;      // Internal tracking of how long the customer has waited
        public Order customerOrder;  // Store the current order

        // Reference to the speech bubble components
        private GameObject speechBubble; // The speech bubble GameObject
        private TMP_Text speechBubbleText; // Text component to display the order

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            patience = 60f;          // Default patience time in seconds
            isServed = false;

            // Select a random order for the customer
            customerOrder = (Order)Random.Range(0, System.Enum.GetValues(typeof(Order)).Length);

            Debug.Log($"{characterName} entered the diner and is waiting to place an order. Random Order: {customerOrder}");

            // Find the speech bubble GameObject and text component
            speechBubble = transform.Find("SpeechBubble").gameObject; // Adjust the name if necessary
            speechBubbleText = speechBubble.GetComponentInChildren<TMP_Text>(); // Adjust if using TextMeshPro

            // Show the speech bubble with the customer's order
            ShowSpeechBubble();
        }

        // Show the speech bubble with the customer's order
        private void ShowSpeechBubble()
        {
            if (speechBubble != null && speechBubbleText != null)
            {
                // Set the speech bubble active and update the text
                speechBubble.SetActive(true);
                speechBubbleText.text = customerOrder.ToString();
            }
        }

        // Method to serve the customer
        public void Serve()
        {
            if (!isServed)
            {
                isServed = true;
                Debug.Log($"{characterName} has been served {customerOrder}.");
                HideSpeechBubble();
            }
            else
            {
                Debug.Log($"{characterName} has already been served.");
            }
        }

        // Hide the speech bubble after the customer has been served
        private void HideSpeechBubble()
        {
            if (speechBubble != null)
            {
                speechBubble.SetActive(false);
            }
        }

        // Method to handle customer waiting
        private void Update()
        {
            if (!isServed)
            {
                waitTime += Time.deltaTime;
                if (waitTime >= patience)
                {
                    Leave();
                }
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

            // Remove the speech bubble and the customer GameObject
            Destroy(gameObject);
        }
    }
}
