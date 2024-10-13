using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterMovement : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;

	[SerializeField] private float movementSpeed = 5f;
	[SerializeField] private Vector3 movementInput;
	private Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

	private bool isDashing = false;
	private bool canDash = true;
	[SerializeField] private float dashDistance = 2f;
	[SerializeField] private float dashDuration = 0.2f;


	void Start()
	{
	}

	void Update()
	{
		if (!isDashing) GetMovementInput();

		if (Input.GetKeyDown(KeyCode.Space) && canDash)
		{
			isDashing = true;
			canDash = false;
		}
	}

	private void FixedUpdate()
	{
		Move();
	}

	private void GetMovementInput()
	{
		movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
	}

	private void Move()
	{
		if (isDashing) return;

		if (movementInput.magnitude == 0) return;
		
		movementInput = isometricMatrix.MultiplyPoint3x4(movementInput);
		movementInput.Normalize();
		rb.Move(transform.position + movementInput * (movementSpeed * Time.deltaTime), Quaternion.LookRotation(movementInput));
	}
}