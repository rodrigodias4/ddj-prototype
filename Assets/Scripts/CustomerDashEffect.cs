using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerDashEffect : MonoBehaviour
{
    private CharacterMovement characterMovement;
    public UnityEvent onCustomerHit;
    private bool canHit;
    private Collider physCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        physCollider = transform.Find("Capsule").GetComponent<Collider>();
        characterMovement = GameObject.Find("Character").GetComponent<CharacterMovement>();
        characterMovement.onCharacterDashStart.AddListener(OnDashStart);
        characterMovement.onCharacterDashEnd.AddListener(OnDashEnd);
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

    public void OnDashStart()
    {
        physCollider.enabled = false;
    }

    public void OnDashEnd()
    {
        physCollider.enabled = true;
    }
}
