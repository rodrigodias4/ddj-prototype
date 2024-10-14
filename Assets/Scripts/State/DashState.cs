using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DashState : State
{
    private float dashSpeed = 15f;
    private float dashDuration = 0.3f;
    private float dashTime = 0f;

    public DashState(CharacterMovement character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("Entering Dash State");
        dashTime = dashDuration;
    }

    public override void Update()
    {
        dashTime -= Time.deltaTime;
        if (dashTime <= 0)
        {
            character.TransitionToState(character.moveState);
        }
    }

    public override void FixedUpdate()
    {
        character.rb.MovePosition(character.transform.position + character.movementInput * (dashSpeed * Time.deltaTime));
    }

    public override void Exit()
    {
        Debug.Log("Exiting Dash State");
    }
}
