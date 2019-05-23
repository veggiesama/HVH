using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;

public class DebugMenu : MonoBehaviour {

	public Dropdown playerSwapDropdown;
	public Button playerSwapButton;
	public Dropdown statusDropdown;
	public Button statusButton;
	public Button reloadAbilitiesButton;
	private UnitController unit; 

	private void Start() {
		unit = GameRules.Instance.GetLocalPlayer().unit;

		BuildPlayerSwapper();
		BuildStatusApplier();
		BuildAbilityReloader();
	}

	// PLAYER SWAPPER
	private void BuildPlayerSwapper() {
		List<string> playerList = new List<string>();
		foreach (Player p in GameRules.Instance.GetAllPlayers()) {
			playerList.Add(p.playerID+"");
		}

		playerSwapDropdown.AddOptions(playerList);

		playerSwapButton.onClick.AddListener( delegate {
			SwapPlayer();
		});
	}

	private void SwapPlayer() {
		int playerID = int.Parse(playerSwapDropdown.captionText.text);
		foreach (Player p in GameRules.Instance.GetAllPlayers()) {
			if (p.playerID == playerID) {
				NetworkServer.ReplacePlayerForConnection(unit.player.connectionToClient, p.gameObject);
			}
		}
	}

	// STATUS APPLIER

	private void BuildStatusApplier() {
		var dict = ResourceLibrary.Instance.statusEffectDictionary;
		statusDropdown.AddOptions(dict.Keys.ToList());

		statusButton.onClick.AddListener( delegate {
			ApplyStatus();
		});
	}

	private void ApplyStatus() {
		var dict = ResourceLibrary.Instance.statusEffectDictionary;
		string selectedOption = statusDropdown.captionText.text;
		if (dict.TryGetValue(selectedOption, out StatusEffect status)) {
			StatusEffect s = Instantiate(status);
			unit.networkHelper.ApplyStatusEffectTo(s);
		} 

	}

	// ABILITY RELOADER

	private void BuildAbilityReloader() {
		reloadAbilitiesButton.onClick.AddListener( delegate {
			ReloadAbilities();
		});
	}

	private void ReloadAbilities() {
		Debug.Log("Reloaded scriptable object abilities");
		unit.ReloadAbilities();
	}

}
