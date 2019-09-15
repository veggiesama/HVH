using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class AbilityButtonInfo : MonoBehaviour {

	public AbilitySlots abilitySlot;

	public TextMeshProUGUI hotkeyText;
	public TextMeshProUGUI cooldownText;
	public Image iconImage;

	private string originalText;
	private Button button;
	private Ability ability;
	private Player player;

	private float updateEvery = 0.1f;

	private void Awake() {
		button = GetComponentInChildren<Button>();
		originalText = cooldownText.text;

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

		player = GameResources.Instance.GetLocalPlayer();

		button.onClick.AddListener( delegate {
			player.UI_ClickedAbilityButton(abilitySlot);
		});

		//if (player.unit.HasAbilityInSlot(abilitySlot)) {
		ability = player.unit.GetAbilityInSlot(abilitySlot);
		iconImage.sprite = ability.iconImage;
		hotkeyText.text = AbilitySlotToInputActionString(abilitySlot);
		button.enabled = true;
		//}
		//else {
		//	ability = null;
		//	iconImage.sprite = null;
		//	hotkeyText.text = "";
		//	cooldownText.text = "";
		//	button.enabled = false;
		//}
	}

	IEnumerator SlowUpdate() {
		while (true) {
			if (ability == null) {
				yield return new WaitForSeconds(updateEvery);
				continue;
			}

			float cdRemaining = ability.GetCooldown();
			if (cdRemaining > 0) {
				cooldownText.text = string.Format("{0:0.0}", cdRemaining);
				button.interactable = false;
			}
			else {
				cooldownText.text = originalText;
				button.interactable = true;
			}

			yield return new WaitForSeconds(updateEvery);
		}
	}

	private string AbilitySlotToInputActionString(AbilitySlots slot) {

		InputAction act = null;

		switch (slot) {
			case AbilitySlots.ATTACK:
				act = player.hvhInputs.Player.Attack;
				break;
			case AbilitySlots.ABILITY_1:
				act = player.hvhInputs.Player.Ability1;
				break;
			case AbilitySlots.ABILITY_2:
				act = player.hvhInputs.Player.Ability2;
				break;
			case AbilitySlots.ABILITY_3:
				act = player.hvhInputs.Player.Ability3;
				break;
			case AbilitySlots.ABILITY_4:
				act = player.hvhInputs.Player.Ability4;
				break;
			case AbilitySlots.ABILITY_5:
				act = player.hvhInputs.Player.Ability5;
				break;
			case AbilitySlots.ABILITY_6:
				act = player.hvhInputs.Player.Ability6;
				break;
			case AbilitySlots.ITEM_1:
				act = player.hvhInputs.Player.Item1;
				break;
			case AbilitySlots.ITEM_2:
				act = player.hvhInputs.Player.Item2;
				break;
			case AbilitySlots.ITEM_3:
				act = player.hvhInputs.Player.Item3;
				break;
			case AbilitySlots.ITEM_4:
				act = player.hvhInputs.Player.Item4;
				break;
			case AbilitySlots.ITEM_5:
				act = player.hvhInputs.Player.Item5;
				break;
			case AbilitySlots.ITEM_6:
				act = player.hvhInputs.Player.Item6;
				break;
			case AbilitySlots.NONE:
				break;
		}

		if (act != null)
			return InputControlPath.ToHumanReadableString(act.bindings[0].path, InputControlPath.HumanReadableStringOptions.OmitDevice);
		
		return "";
	}

}
