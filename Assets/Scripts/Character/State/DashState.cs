using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : State
{
	private float speedbonus = 2f;
	public DashState(CharacterMovement character) : base(character)
	{
	}

	public override void Enter()
	{
		Debug.Log("Entering Dash State");
		character.rb.velocity = speedbonus * character.transform.forward * (character.dashDistance / character.dashDuration);
		character.StartCoroutine(EndDash());
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
		character.rb.velocity = Vector3.zero;
		character.dashCooldownCur = character.dashCooldown;
	}

	private IEnumerator EndDash()
	{
		yield return new WaitForSeconds(character.dashDuration);
		character.TransitionToState(new MoveState(character));
	}

	public override void CustomerHit()
	{
		Debug.Log("Player hit customer with dash");
	}
}