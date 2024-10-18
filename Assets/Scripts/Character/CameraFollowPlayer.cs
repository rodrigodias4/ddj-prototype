using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15F;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = target.position;
    }

    void FixedUpdate()
    {
        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
    }
}
