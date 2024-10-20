using UnityEngine;

public class Food : InteractableItem
{
    public enum Order { Burger, Ham, Stew };
    public Order order;
    [SerializeField] private GameObject foodPrefab;
    public override void Start()
    {
        base.Start();

        string objectName = gameObject.name;
        if (objectName.Contains("burger_dispenser"))
        {
            order = Order.Burger;
        }
        else if (objectName.Contains("ham_dispenser"))
        {
            order = Order.Ham;
        }
        else if (objectName.Contains("stew_dispenser"))
        {
            order = Order.Stew;
        }
    }
    
    public override string GetTooltip()
    {
        return $"Get {order}";
    }

    // Interaction logic
    public override void Interact(CharacterMovement character)
    {
        base.Interact(character);
        if (foodPrefab != null)
        {
            // Replace the currently held food with the new food prefab
            character.ReplaceHeldFood(foodPrefab, (CharacterMovement.Order)order);
        }
        else
        {
            Debug.LogWarning("Food prefab not assigned!");
        }
    }
}
