using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InteractableItem : MonoBehaviour, IInteractable
{
	[SerializeField] private Material defaultMaterial = null;
	[SerializeField] private Material highlightMaterial = null;
	private bool inInteractRange = false;
	new Renderer renderer = null;

	public void Start()
	{
		renderer = GetComponent<Renderer>();
		Assert.IsNotNull(defaultMaterial);
		Assert.IsNotNull(highlightMaterial);
		Assert.IsNotNull(renderer);
	}
	
	public void InteractRange()
	{
		if (inInteractRange == false) inInteractRange = true;
	}

	public void LateUpdate()
	{
		// TODO: Setting material for all interactables in every frame might be very costly
		renderer.material = inInteractRange ? highlightMaterial : defaultMaterial;
		
		inInteractRange = false;
	}

	public Transform GetTransform() { return transform; }

	public string GetTooltip()
	{
		return "Interact";
	}
}
