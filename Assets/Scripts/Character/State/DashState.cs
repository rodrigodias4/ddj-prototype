using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : State
{
	private float dashDuration = 0.3f;

	public DashState(CharacterMovement character) : base(character)
	{
	}

	public override void Enter()
	{
		Debug.Log("Entering Dash State");
	}

	public override void Update()
	{
	}

	public override void FixedUpdate()
	{
	}

	public override void Exit()
	{
		Debug.Log("Exiting Dash State");
	}
}