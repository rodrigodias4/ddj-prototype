using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterInteract : MonoBehaviour
{
    private IInteractable closestInteractable = null;
    public float interactRange = 2f;
    [SerializeField] private GameObject uiInteract;
    private UIInteractable uiInteractable;
    [SerializeField] private Camera mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(uiInteract);
        Assert.IsNotNull(mainCamera);
        uiInteractable = uiInteract.GetComponent<UIInteractable>();
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

        if (closestInteractable is not null)
        {
            closestInteractable.InteractRange();
            uiInteract.transform.position = mainCamera.WorldToScreenPoint(closestInteractable.GetTransform().position) + new Vector3(10f, 20f, 0f);
            uiInteractable.SetText(closestInteractable.GetTooltip());
            uiInteract.SetActive(true);
        }
        else
        {
            uiInteract.SetActive(false);
        }
    }
    
    public IInteractable GetClosestInteractable() { return closestInteractable; }
}
