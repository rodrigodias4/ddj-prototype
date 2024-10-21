using UnityEngine;
using TMPro;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Serialization;

namespace Assets.Scripts.Characters
{
    public class Customer : NPC
    {
        public enum Order { Burger, Ham, Stew };
        public float patience = 20f;       // How long the customer will wait before leaving
        public bool isServed = false;      // Whether the customer has been served
        private bool caught = false;
        [SerializeField] private float waitTime = 0f;       // Internal tracking of how long the customer has waited
        private float eatingTime = 3f;     // How long the customer takes to eat
        private bool growingImpatient = false;
        private float randomIdleRotationAngle;
        public Order customerOrder;        // Store the current order

        
        private GameObject orderVisual;
        private GameObject foodVisual;
        public GameObject burgerOrderPrefab;
        public GameObject hamOrderPrefab;
        public GameObject stewOrderPrefab;
        
        public GameObject burgerPrefab;
        public GameObject hamPrefab;
        public GameObject stewPrefab;
        
        [SerializeField] GameObject deathParticlesPrefab;

        [SerializeField] private TextMeshProUGUI waitingTMP;
        [SerializeField] private GameObject cross;
        
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
            
            StartCoroutine(RotateRandomly());
            transform.rotation = Quaternion.AngleAxis(randomIdleRotationAngle, Vector3.up);

            StartCoroutine(WaitingText());
        }

        // Show the speech bubble with the customer's order
        public void ShowSpeechBubble()
        {
            if (speechBubble != null && speechBubbleText != null)
            {
                speechBubble.SetActive(true);
                // //speechBubbleText.text = customerOrder.ToString();
            }
        }

        // Serve the customer
        public IEnumerator Serve(Order order)
        {
            if (!seated){
                Debug.Log($"{characterName} has not been seated yet.");
                ////speechBubbleText.text = "I need to sit!!";
                yield return StartCoroutine(DefaultMessage(1f));
            }
            else if (!isServed)
            {
                if (order != customerOrder)
                {
                    Debug.Log($"{characterName} has been served the wrong order.");
                    //speechBubbleText.text = "WRONG!!";
                    // StartCoroutine(DefaultMessage(1f));
                    //yield return StartCoroutine(DefaultMessage(1f));
                    //speechBubbleText.text = customerOrder.ToString();
                    ShowSpeechBubble();
                    StartCoroutine(WrongOrderVisual());
                }else{
                    interactableCustomer.enabled = false;
                    isServed = true;
                    Debug.Log($"{characterName} has been served {customerOrder}.");
                    //speechBubbleText.text = "Yummy!";
                    // Start the coroutine to wait before leaving
                    yield return StartCoroutine(EatFood(eatingTime)); 
                }
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

            if (!isServed && (seated || !caught))
            {
                waitTime += Time.deltaTime;

                if (waitTime >= patience * 0.75 && !isLeaving)
                {
                    if (growingImpatient == false)
                    {
                        Debug.Log($"{characterName} is losing patience.");
                        growingImpatient = true;
                    }
                }
                
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
                //Debug.Log($"{characterName} has been caught!");
                growingImpatient = false;
            }
            
            // Make sure the speech bubble is always facing the camera
            if (speechBubble != null && mainCamera != null)
            {
                // Make the speech bubble look at the camera
                speechBubble.transform.LookAt(speechBubble.transform.position + mainCamera.transform.rotation * Vector3.forward,
                    mainCamera.transform.rotation * Vector3.up);
            }
        }

        private void FixedUpdate()
        {
            if (growingImpatient) StartCoroutine(Tremble());
            if (!seated) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(randomIdleRotationAngle, Vector3.up), Time.fixedDeltaTime * 2f);
        }

        // Customer leaves after being served or when impatient
        public void Leave()
        {
            if (isServed)
            {
                Debug.Log($"{characterName} has finished eating and wants to leave the diner.");
                if (foodVisual is not null)
                    Destroy(foodVisual.gameObject);
                ScoreCalculation.IncrementDishCounter();
                int tipAmount = (int) (patience - waitTime);
                ScoreCalculation.IncrementTips(tipAmount);
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
            interactableCustomer.enableTooltip = true;
            transform.position = chair.chairPosition.position + new Vector3(0, 1f, 0);
            patience *= 2;  // Double the patience when seated
            occupiedChair = chair;
            customerManager.availableChairs.Remove(chair);
            StartCoroutine(customerManager.AddAvailableQueuePosition(queuePosition));
            seated = true;
            rb.isKinematic = true;  // Disable physics when seated
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            growingImpatient = false;
            CreateOrderVisual();
            StartCoroutine(WaitingText());
        }

        // Handle customer death/cleanup logic
        public override void Die()
        {
            Debug.Log($"{characterName} is dying.");

            customerManager?.OnCustomerLeft(this);
            ScoreCalculation.IncrementCustomerKilled();

            Destroy(gameObject);
            
            StartCoroutine(DeathParticles());
        }

        public void DisableCustomer(){
            caught = true;
        }

        public void EnableCustomer(){
            caught = false;
        }

        private IEnumerator EatFood(float eatingTime)
        {
            CreateFoodVisual();
            
            // Wait for the specified amount of time (in seconds)
            yield return new WaitForSeconds(eatingTime);

            // Call the Leave method after the wait
            Leave();
        }

        private IEnumerator DefaultMessage(float time)
        {
            ShowSpeechBubble(); 
            // Wait for the specified amount of time (in seconds)
            yield return new WaitForSeconds(time);

            // Call the Leave method after the wait
            // //speechBubbleText.text = customerOrder.ToString();
            HideSpeechBubble();

        }
        
        private IEnumerator Tremble() {
            transform.position += new Vector3(0.1f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            transform.position -= new Vector3(0.1f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            transform.position += new Vector3(0, 0, 0.1f);
            yield return new WaitForSeconds(0.01f);
            transform.position -= new Vector3(0, 0, 0.1f);
            yield return new WaitForSeconds(0.01f);
        }

        private IEnumerator RotateRandomly()
        {
            while (!seated) {
                randomIdleRotationAngle = Random.Range(0f, 360f);
                
                yield return new WaitForSeconds(Random.Range(2,5));
            }
        }

        private IEnumerator DeathParticles()
        {
            GameObject particles = Instantiate(deathParticlesPrefab, transform.position + Vector3.up, Quaternion.identity);
            
            foreach (ParticleSystem particle in particles.GetComponentsInChildren<ParticleSystem>())
            {
                particle.Play();
            }

            yield return new WaitForSeconds(10f);
            
            Destroy(particles);
        }

        private void CreateOrderVisual()
        {
            GameObject orderPrefab = null;
            if (customerOrder == Order.Burger)
                orderPrefab = burgerOrderPrefab;
            else if (customerOrder == Order.Ham)
                orderPrefab = hamOrderPrefab;
            else if (customerOrder == Order.Stew)
                orderPrefab = stewOrderPrefab;

            if (orderPrefab is not null)
            {
                orderVisual = Instantiate(orderPrefab, occupiedChair.tableTransform.position + new Vector3(0, 1.5f, 0),
                    Quaternion.identity,
                    occupiedChair.transform);
                orderVisual.transform.rotation = Quaternion.Euler(0, customerOrder == Order.Ham ? 180 : 0, 0);
            }
        }

        private void CreateFoodVisual()
        {
            if (orderVisual is not null)
                Destroy(orderVisual.gameObject);
            
            GameObject orderPrefab = null;
            if (customerOrder == Order.Burger)
                orderPrefab = burgerPrefab;
            else if (customerOrder == Order.Ham)
                orderPrefab = hamPrefab;
            else if (customerOrder == Order.Stew)
                orderPrefab = stewPrefab;

            if (orderPrefab is not null)
            {
                foodVisual = Instantiate(orderPrefab, occupiedChair.tableTransform.position + new Vector3(0, 1.5f, 0),
                    Quaternion.identity,
                    occupiedChair.transform);
                foodVisual.transform.rotation = Quaternion.Euler(0, customerOrder == Order.Ham ? 180 : 0, 0);
            }
        }

        public IEnumerator WaitingText()
        {
            yield return new WaitForSeconds(3f);

            while (true)
            {
                if (caught || isServed) break;
                if (waitingTMP.text.Length < 3)
                    waitingTMP.text += ".";
                else waitingTMP.text = "";
                yield return new WaitForSeconds(1f);
            }
            
            waitingTMP.text = "";
        }

        public IEnumerator WrongOrderVisual()
        {
            cross.SetActive(true);
            
            yield return new WaitForSeconds(2f);
            
            cross.SetActive(false);
        }
    }
}
