using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    protected Vector3 movementInput;

	public MoveState(CharacterMovement character) : base(character)
	{
	}

	public override void Enter()
	{
		Debug.Log("Entering Move State");
	}

	public override void HandleInput()
	{
		movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		if (Input.GetKeyDown(KeyCode.Space) && character.dashCooldownCur == 0)
		{
			character.TransitionToState(new DashState(character));
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			character.TransitionToState(new InteractState(character));
		}

		if (Input.GetMouseButtonDown(0))
		{
			character.TransitionToState(new SpecialState(character));
		}
	}

	public override void FixedUpdate()
	{
		if (movementInput.sqrMagnitude == 0)
		{
			character.rb.velocity = Vector3.zero;
			return;
		}
		movementInput = isometricMatrix.MultiplyPoint3x4(movementInput);
		movementInput.Normalize();
		character.rb.velocity = movementInput * character.movementSpeed;
		character.transform.forward = movementInput;
	}

	public override void Exit()
	{
		Debug.Log("Exiting Move State");
	}
}