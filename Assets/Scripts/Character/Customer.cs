using UnityEngine;

public class Customer : NPC
{
    // Additional properties for Customer
    public string order;         // What the customer orders
    public float patience;       // How long the customer will wait before leaving
    public bool isServed;        // Whether the customer has been served

    private float waitTime;      // Internal tracking of how long the customer has waited

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        patience = 60f;          // Default patience time in seconds
        isServed = false;
        Debug.Log($"{characterName} entered the diner and is waiting to place an order.");
    }

    // Customer places an order
    public void PlaceOrder(string newOrder)
    {
        order = newOrder;
        Debug.Log($"{characterName} ordered {order}.");
    }

    // Method to serve the customer
    public void Serve()
    {
        if (!isServed)
        {
            isServed = true;
            Debug.Log($"{characterName} has been served {order}.");
        }
        else
        {
            Debug.Log($"{characterName} has already been served.");
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
        
        // Here you would add logic to remove the customer from the scene, like disabling the GameObject
        Destroy(gameObject);
    }
}
