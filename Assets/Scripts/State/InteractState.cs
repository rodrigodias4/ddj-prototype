using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InteractState : State
{
    public InteractState(CharacterMovement character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("Entering Interact State");
        // Handle interaction logic
    }

    public override void Update()
    {
        // After interaction logic is done, return to Move state
        if (!Input.GetKey(KeyCode.E)) 
        {
            character.TransitionToState(character.moveState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Interact State");
    }
}
