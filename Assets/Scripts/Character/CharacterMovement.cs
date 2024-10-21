using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
	public UnityEvent onCharacterDashStart;
	public UnityEvent onCharacterDashEnd;
	
	public Rigidbody rb;
	public float movementSpeed = 10f;
	public float dashDistance = 7.5f;
	public float dashDuration = 0.2f;
	public float dashCooldown = 2f;
	public float dashCooldownCur = 0f; // Added explicit for possible visual in UI
	public CharacterInteract characterInteract;
    public Transform handTransform;	// Add a Transform to represent the hand where food will be held
	public GameObject currentFood;
	// States
	private State currentState;
	public enum Order { Burger, Ham, Stew, None };
	public Order order = Order.None;
	
	private GameManager gameManager;

	void Start()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		characterInteract = GetComponent<CharacterInteract>();
		Assert.IsNotNull(characterInteract);
		
		// Set initial state
		rb = GetComponent<Rigidbody>();
		TransitionToState(new MoveState(this));

		// Add a hand transform to the character
		handTransform = transform.Find("Hand");
	}

	void Update()
	{
		if (gameManager.gameOver) return;
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

	public void CustomerHit()
	{
		currentState.CustomerHit();
	}
	public void ReplaceHeldFood(GameObject newFoodPrefab, Order order)
    {
		this.order = order;
        // If there is already food, remove it
        if (currentFood != null)
        {
            Destroy(currentFood);  // Remove the old food
        }

        // Instantiate the new food prefab and attach it to the hand
        currentFood = Instantiate(newFoodPrefab, handTransform);

        // Reset the position and rotation for correct placement
        currentFood.transform.localPosition = Vector3.zero;
        currentFood.transform.localRotation = Quaternion.identity;
    }

	public void RemoveHeldFood()
	{
		this.order = Order.None;
		if (currentFood != null)
		{
			Destroy(currentFood);
		}
	}
}