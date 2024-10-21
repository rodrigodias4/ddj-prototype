using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceMoveState : MoveState
{
    public float movementForce = 200f;  // Adjust the force applied for movement
    public float maxSpeed = 6f;        // Cap the maximum speed the character can reach

    public AddForceMoveState(CharacterMovement character) : base(character) {}

    public override void FixedUpdate()
    {
        if (movementInput.sqrMagnitude == 0)
        {
            character.rb.velocity = Vector3.zero;
            return;
        }

        movementInput = isometricMatrix.MultiplyPoint3x4(movementInput);
        movementInput.Normalize();  // Normalize to get direction

        // Apply force for movement
        Vector3 force = movementInput * movementForce;

        // Add force to the rigidbody
        character.rb.AddForce(force, ForceMode.Acceleration);

        // Cap the speed to avoid infinite acceleration
        LimitSpeed();

        // Update character's facing direction
        if (character.rb.velocity.sqrMagnitude > 0.1f)  // Avoid jittery movement when idle
        {
            character.transform.forward = new Vector3(character.rb.velocity.x, 0, character.rb.velocity.z).normalized;
        }
    }

    // Limit the speed of the character so that it doesn't exceed maxSpeed
    private void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(character.rb.velocity.x, 0, character.rb.velocity.z);

        // Check if the current speed exceeds maxSpeed
        if (flatVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            // Scale the velocity down to maxSpeed
            flatVelocity = flatVelocity.normalized * maxSpeed;
            character.rb.velocity = new Vector3(flatVelocity.x, character.rb.velocity.y, flatVelocity.z);
        }
    }
}
