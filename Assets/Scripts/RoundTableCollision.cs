using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RoundTableCollision : MonoBehaviour
{
	[SerializeField] private Collider[] colliders;
	void Start()
	{
		CharacterMovement characterMovement = GameObject.Find("Character")?.GetComponent<CharacterMovement>();
		Assert.IsNotNull(characterMovement);
		
		characterMovement.onCharacterDashStart.AddListener(DisableColliders);
		characterMovement.onCharacterDashEnd.AddListener(EnableColliders);
	}

	private void DisableColliders()
	{
		foreach (Collider collider in colliders)
		{
			collider.enabled = false;
		}
	}

	private void EnableColliders()
	{
		foreach (Collider collider in colliders)
		{
			collider.enabled = true;
		}
	}
}