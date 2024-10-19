using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact()
    {
        Debug.Log("Interaction not implemented");
    }

    public void InteractRange();

    public Transform GetTransform();
}
