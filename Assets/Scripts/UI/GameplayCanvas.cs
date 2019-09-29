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

		foreach (var kv in uiPortraits) {
			var slot = kv.Key;
			var port = kv.Value;

			port.Initialize(slot);
		}

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
		Teams playerTeam = player.GetTeam();
		List<Player> teammates = GameResources.Instance.GetAllPlayers(playerTeam);

		int teamCount;
		if (playerTeam == Teams.DWARVES)
			teamCount = Constants.DwarvesTotal;
		else
			teamCount = Constants.MonstersTotal;

		int enumCounter = 1;
		foreach (Player teammate in teammates) {
			if (teammate != player) {
				try {
					UiPortraitSlots slot = (UiPortraitSlots) System.Enum.Parse(typeof(UiPortraitSlots), "ALLY_" + enumCounter);
					uiPortraits[slot].RegisterPortrait(teammate.unit);
					enumCounter++;
				}
				catch(System.ArgumentException) {
					Debug.Log("Error: Too many players to see everyone's portrait. ALLY_" + enumCounter + " not valid portrait slot.");
				}
			}
			else {
				uiPortraits[UiPortraitSlots.SELF].RegisterPortrait(player.unit);

			}
		}
	}

}