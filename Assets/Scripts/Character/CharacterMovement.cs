using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	public Rigidbody rb;
	public float movementSpeed = 5f;

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
}