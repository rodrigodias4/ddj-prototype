using Assets.Scripts.Characters;
using UnityEngine;

public class InteractableCustomer : InteractableItem
{
    public Customer customer;
    public bool enableTooltip;

    public override void Start()
    {
        enableTooltip = false;
    }
    
    public override string GetTooltip()
    {
        return enableTooltip ? $"Give held food" : "";
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
