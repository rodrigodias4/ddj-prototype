using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractState : State
{
	public InteractState(CharacterMovement character) : base(character)
	{
	}

	public override void Enter()
	{
		Debug.Log("Entering Interact State");
		character.characterInteract.GetClosestInteractable()?.Interact(character);
	}
	public override void Update()
	{
		// After interaction logic is done, return to Move state
		if (!Input.GetKey(KeyCode.E))
		{
			character.TransitionToState(new MoveState(character));
		}
	}

	public override void Exit()
	{
		Debug.Log("Exiting Interact State");
	}
}