using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	public Rigidbody rb;
	public float movementSpeed = 10f;
	public float dashDistance = 7.5f;
	public float dashDuration = 0.2f;
	public float dashCooldown = 2f;
	public float dashCooldownCur = 0f; // Added explicit for possible visual in UI

	// States
	private State currentState;

	void Start()
	{
		// Set initial state
		rb = GetComponent<Rigidbody>();
		TransitionToState(new MoveState(this));
	}

	void Update()
	{
		currentState.HandleInput();
		currentState.Update();
		DecrementCooldowns();
	}

	private void FixedUpdate()
	{
		currentState.FixedUpdate();
	}

	public void TransitionToState(State newState)
	{
		if (currentState != null)
		{
			currentState.Exit();
		}

		currentState = newState;

		if (currentState != null)
		{
			currentState.Enter();
		}
	}

	private void DecrementCooldowns()
	{
		dashCooldownCur = Mathf.Clamp(dashCooldownCur - Time.deltaTime, 0f, dashCooldown);
	}
}