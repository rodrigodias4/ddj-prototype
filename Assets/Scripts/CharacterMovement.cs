using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float movementSpeed = 5f;
    public Vector3 movementInput;
    private Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    
    void Start()
    {
    }

    void Update()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        movementInput = isometricMatrix.MultiplyPoint3x4(movementInput);
        movementInput.Normalize();
        rb.Move(transform.position + movementInput * (movementSpeed * Time.deltaTime), Quaternion.identity);
    }
}
