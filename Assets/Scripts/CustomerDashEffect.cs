using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerDashEffect : MonoBehaviour
{
    public UnityEvent onCustomerHit;
    private bool canHit;
    
    // Start is called before the first frame update
    void Start()
    {
        canHit = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;
        if (!other.CompareTag("Player")) return;

        onCustomerHit.Invoke();
        
        canHit = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (canHit) return;
        if (!other.CompareTag("Player")) return;
        canHit = true;
    }
}
