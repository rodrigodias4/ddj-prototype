using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInteractable : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI uiInteractableTooltip;

	public void SetText(string text)
	{
		uiInteractableTooltip.text = text;
	}
	
	public string GetText() { return uiInteractableTooltip.text; }
}
