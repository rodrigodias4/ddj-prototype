using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteract : MonoBehaviour
{
    private IInteractable closestInteractable = null;
    public float interactRange = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] interactColliders = Physics.OverlapSphere(transform.position, interactRange);
        Collider closestInteractCollider = null;
        closestInteractable = null;
        float closestDistance = Mathf.Infinity;
        foreach (Collider interactCollider in interactColliders)
        {
            if (!interactCollider.TryGetComponent(out IInteractable interactable)) continue;

            float newDistance;
            if ((newDistance = Vector3.Distance(transform.position, interactable.GetTransform().position)) < closestDistance)
            {
                closestInteractable = interactable;
                closestDistance = newDistance;
            }
        }
        
        closestInteractable?.InteractRange();
    }
    
    public IInteractable GetClosestInteractable() { return closestInteractable; }
}
