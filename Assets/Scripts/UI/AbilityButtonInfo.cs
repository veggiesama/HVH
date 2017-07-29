using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonInfo : MonoBehaviour {

	public AbilitySlots abilitySlot;
	private string originalText;
	private Text textComponent;
	private Button button;
	private AbilityController ability;

	private void Start()
	{
		button = this.gameObject.GetComponent<Button>();
		button.onClick.AddListener( delegate {
			GameController.GetLocalOwner().UI_ClickedAbilityButton(abilitySlot);
		});

		textComponent = GetComponentInChildren<Text>();
		originalText = textComponent.text;

		if (GameController.GetLocalOwner().unit.abilities.ContainsKey(abilitySlot))
			ability = GameController.GetLocalOwner().unit.abilities[abilitySlot];
		else
			ability = null;
	}

	// TODO: doesn't need to update every frame
	private void Update()
	{
		if (ability == null) return;

		float cdRemaining = ability.cooldownTimeRemaining;
		if (cdRemaining > 0) {
			textComponent.text = string.Format("{0:0.0}", cdRemaining);
			button.interactable = false;
		}
		else {
			textComponent.text = originalText;
			button.interactable = true;
		}

	}

}
