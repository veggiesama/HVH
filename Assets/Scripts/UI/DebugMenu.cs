﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;
using TMPro;

public class DebugMenu : MonoBehaviour {

	public Dropdown playerSwapDropdown;
	public Button playerSwapButton;
	public Dropdown statusDropdown;
	public Button statusButton;
	public Button reloadAbilitiesButton;
	public TMP_Text dayNightText;

	public void Initialize() {
		BuildPlayerSwapper();
		BuildStatusApplier();
		BuildAbilityReloader();
		StartCoroutine( UpdateDayNightText() );
	}

	IEnumerator UpdateDayNightText() {
		while (true) {
			string s;

			if (DayNight.Instance.IsDay())
				s = "Day";
			else
				s = "Night";

			s += " ";

			float timeRemaining = DayNight.Instance.GetTimeRemaining();
			s += System.Convert.ToString(System.TimeSpan.FromSeconds( timeRemaining ));
			dayNightText.SetText(s);
			yield return new WaitForSeconds(1.0f);
		}
	}

	// PLAYER SWAPPER
	private void BuildPlayerSwapper() {
		List<string> playerList = new List<string>();
		foreach (Player p in GameResources.Instance.GetAllPlayers()) {
			playerList.Add(p.playerID+"");
		}

		playerSwapDropdown.AddOptions(playerList);

		playerSwapButton.onClick.AddListener( delegate {
			SwapPlayer();
		});
	}

	private void SwapPlayer() {
		Player localPlayer = GameResources.Instance.GetLocalPlayer();

		int playerID = int.Parse(playerSwapDropdown.captionText.text);
		foreach (Player p in GameResources.Instance.GetAllPlayers()) {
			if (p.playerID == playerID) {
				localPlayer.EnableLocalPlayerOnlyObjects(false);
				localPlayer.RefreshInputActions();

				NetworkServer.ReplacePlayerForConnection(localPlayer.connectionToClient, p.gameObject);
				p.OnStartLocalPlayer();
				ReloadAbilities();
				break;
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
		UnitController localUnit = GameResources.Instance.GetLocalPlayer().unit;

		var dict = ResourceLibrary.Instance.statusEffectDictionary;
		string selectedOption = statusDropdown.captionText.text;
		if (dict.TryGetValue(selectedOption, out StatusEffect status)) {
			StatusEffect s = Instantiate(status);
			localUnit.networkHelper.ApplyStatusEffectTo(s);
		} 

	}

	// ABILITY RELOADER

	private void BuildAbilityReloader() {
		reloadAbilitiesButton.onClick.AddListener( delegate {
			ReloadAbilities();
		});
	}

	private void ReloadAbilities() {
		UnitController localUnit = GameResources.Instance.GetLocalPlayer().unit;
		localUnit.ReloadAbilities();
		Debug.Log("Reloaded scriptable object abilities");

	}

}
