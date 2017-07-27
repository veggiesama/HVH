using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonInfo : MonoBehaviour {

	public AbilitySlots abilitySlot;

	private void Start()
	{
		Button button = this.gameObject.GetComponent<Button>();
		button.onClick.AddListener( delegate {
			GameController.GetLocalOwner().UI_ClickedAbilityButton(abilitySlot);
		});

	}

}
