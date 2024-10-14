using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float movementSpeed = 5f;
    public Vector3 movementInput;

    // States
    public State currentState;
    public MoveState moveState;
    public DashState dashState;
    public InteractState interactState;
    public SpecialState specialState;

    void Start()
    {
        // Initialize states
        moveState = new MoveState(this);
        dashState = new DashState(this);
        interactState = new InteractState(this);
        specialState = new SpecialState(this);

        // Set initial state
        TransitionToState(moveState);
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