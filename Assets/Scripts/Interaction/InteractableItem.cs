using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InteractableItem : MonoBehaviour, IInteractable
{
	[SerializeField] private Material defaultMaterial = null;
	[SerializeField] private Material highlightMaterial = null;
	protected bool inInteractRange = false;
	new Renderer renderer = null;

	public virtual void Start()
	{
		renderer = GetComponent<Renderer>();
		Assert.IsNotNull(defaultMaterial);
		Assert.IsNotNull(highlightMaterial);
		Assert.IsNotNull(renderer);
	}

	public virtual void Interact(CharacterMovement character)
	{
		Debug.Log($"{character.name} interacting with {gameObject.name}");
	}
	
	public void InteractRange()
	{
		if (inInteractRange == false) inInteractRange = true;
	}

	public virtual void LateUpdate()
	{
		// TODO: Setting material for all interactables in every frame might be very costly
		renderer.material = inInteractRange ? highlightMaterial : defaultMaterial;
		
		inInteractRange = false;
	}

	public Transform GetTransform() { return transform; }

	public virtual string GetTooltip()
	{
		return "Interact";
	}
}
