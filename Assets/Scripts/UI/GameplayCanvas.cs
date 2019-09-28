using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.Events;

public class GameplayCanvas : Singleton<GameplayCanvas> {

	// Singleton constructor
	public static GameplayCanvas Instance {
		get {
			return ((GameplayCanvas)mInstance);
		} set {
			mInstance = value;
		}
	}

	[System.Serializable]
	public class UiPortaitDictionary : SerializableDictionaryBase<UiPortraitSlots, UiPortrait> {}
	public UiPortaitDictionary uiPortraits;

	[Header("Self")]
    public Slider castbar;
	private AbilityButtonInfo[] abilityButtons;

	[Header("Prefab references")]
	public GameObject statusEffectPrefab;

	[Header("Debug")]
	public DebugMenu debugMenu;

    // Start is called before the first frame update
    void Awake() {

		// show each hud container
		for (int n = 0; n < transform.childCount; n++) {
			transform.GetChild(n).gameObject.SetActive(true);
		}

		abilityButtons = GetComponentsInChildren<AbilityButtonInfo>();
		//ResetButtons();

		//EnableSliders(uiPortraits[UiPortraitSlots.ALLY_TARGET], false);
		//EnableSliders(uiPortraits[UiPortraitSlots.ENEMY_TARGET], false);
	}

	// Start() not getting run because LocalPlayerOnly

	public void ResetButtons() {
		foreach (AbilityButtonInfo button in abilityButtons) {
			button.Initialize();
		}
	}

	public void EnableCastbar(bool enable) {
		castbar.gameObject.SetActive(enable);
		//Debug.Log("Castbar " + enable);

		if (enable) {
			castbar.value = 1;
		}
	}

	public void UpdateCastbar(float percentage) {
		castbar.value = percentage;
	}

	public void RegisterAllyPortraits(Player player) {
		List<Player> teammates = GameResources.Instance.GetAllPlayers(player.GetTeam());

		int n = 1; // counts from 1 to 3
		foreach (Player teammate in teammates) {
			if (teammate != player) {
				UiPortraitSlots slot = (UiPortraitSlots) System.Enum.Parse(typeof(UiPortraitSlots), "ALLY_" + n);
				uiPortraits[slot].RegisterPortrait(teammate.unit);
				n++;

				if (n > 3)
					break;
			}
			else {
				uiPortraits[UiPortraitSlots.SELF].RegisterPortrait(player.unit);
			}
		}
	}

}