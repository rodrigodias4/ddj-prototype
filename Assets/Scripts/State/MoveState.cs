using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    private Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    
    public MoveState(CharacterMovement character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public override void HandleInput()
    {
        character.movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.TransitionToState(character.dashState);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            character.TransitionToState(character.interactState);
        }
        if (Input.GetMouseButtonDown(0))
        {
            character.TransitionToState(character.specialState);
        }
    }

    public override void FixedUpdate()
    {
        character.movementInput = isometricMatrix.MultiplyPoint3x4(character.movementInput);
        character.movementInput.Normalize();
        character.rb.MovePosition(character.transform.position + character.movementInput * (character.movementSpeed * Time.deltaTime));
    }

    public override void Exit()
    {
        Debug.Log("Exiting Move State");
    }
}
