using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AbilityButtonInfo : MonoBehaviour {

	public AbilitySlots abilitySlot;
	private string originalText;
	private TextMeshProUGUI textComponent;
	private Button button;
	private Ability ability;
	private Player player;

	private float updateEvery = 0.1f;

	private void Awake() {
		button = GetComponentInChildren<Button>();
		textComponent = GetComponentInChildren<TextMeshProUGUI>();
		originalText = textComponent.text;

		StartCoroutine ( SlowUpdate() );
	}

	public void Initialize() {

		if (player != null) {
			button.onClick.RemoveListener( delegate {
				player.UI_ClickedAbilityButton(abilitySlot);
			});
			ability = null;
			button.enabled = false;
		}

		player = GameRules.Instance.GetLocalPlayer();

		button.onClick.AddListener( delegate {
			player.UI_ClickedAbilityButton(abilitySlot);
		});

		if (player.unit.HasAbilityInSlot(abilitySlot)) {
			ability = player.unit.GetAbilityInSlot(abilitySlot);
			button.enabled = true;
		}
		else {
			ability = null;
			button.enabled = false;
		}


	}

	IEnumerator SlowUpdate() {
		while (true) {
			if (ability == null) {
				yield return new WaitForSeconds(updateEvery);
				continue;
			}

			float cdRemaining = ability.GetCooldown();
			if (cdRemaining > 0) {
				textComponent.text = string.Format("{0:0.0}", cdRemaining);
				button.interactable = false;
			}
			else {
				textComponent.text = originalText;
				button.interactable = true;
			}

			yield return new WaitForSeconds(updateEvery);
		}
	}

}
