using Assets.Scripts.Characters;
using UnityEngine;

public class InteractableCustomer : InteractableItem
{
    public Customer customer;

    public override void Start()
    {
    }
    
    public override string GetTooltip()
    {
        return $"Give held food";
    }

    public override void LateUpdate()
	{	
		inInteractRange = false;
	}


    // Interaction logic
    public override void Interact(CharacterMovement character)
    {
        base.Interact(character);
        
        customer.StartCoroutine(customer.Serve((Customer.Order)character.order));

        character.RemoveHeldFood();
    }
}
