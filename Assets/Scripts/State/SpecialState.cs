using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpecialState : State
{
    public SpecialState(CharacterMovement character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("Entering Special State");
    }

    public override void Update()
    {
        // Special action, like charging, happens while the button is held
        if (!Input.GetMouseButton(0)) 
        {
            character.TransitionToState(character.moveState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Special State");
    }
}
