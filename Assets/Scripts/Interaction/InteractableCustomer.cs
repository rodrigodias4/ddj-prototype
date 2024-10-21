using Assets.Scripts.Characters;
using UnityEngine;

public class InteractableCustomer : InteractableItem
{
    public Customer customer;
    public bool enableTooltip = false;
    
    public override string GetTooltip()
    {
        return enableTooltip ? "Give Food" : "";
    }

    public override void LateUpdate()
	{
	}


    // Interaction logic
    public override void Interact(CharacterMovement character)
    {
        base.Interact(character);
        
        customer.StartCoroutine(customer.Serve((Customer.Order)character.order));

        character.RemoveHeldFood();
    }
}
