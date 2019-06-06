using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Extensions;

// SyncDictionary: https://vis2k.github.io/Mirror/Classes/SyncDictionary
public class DwarfSlotsToPlayerID_SyncDictionary : SyncDictionary<int, int> {} // SyncDictionary<(int)DwarfTeamSlots, (int)PlayerID> 
public class MonsterSlotsToPlayerID_SyncDictionary: SyncDictionary<int, int> {} // SyncDictionary<(int)DwarfTeamSlots, (int)PlayerID>

public class NetworkGameRules : NetworkBehaviour {

	public DwarfSlotsToPlayerID_SyncDictionary dwarfDictionary = new DwarfSlotsToPlayerID_SyncDictionary();
	public MonsterSlotsToPlayerID_SyncDictionary monsterDictionary = new MonsterSlotsToPlayerID_SyncDictionary();
	private DayNightController dayNightController;

	public void Awake() {
		//DontDestroyOnLoad(this.gameObject);
		dayNightController = GetComponent<DayNightController>();
	}

	public override void OnStartClient() {
		if (isServer) return;
		dwarfDictionary.Callback += OnDwarfTeamChange;
		monsterDictionary.Callback += OnMonsterTeamChange;
	}

	private void Start() {
		//Cmd_SyncToServerDayNight();
	}


	// callbacks
	private void OnDwarfTeamChange(DwarfSlotsToPlayerID_SyncDictionary.Operation op, int slot, int playerID) {
		Debug.Log("OnDwarfTeamChange called");
	}

	private void OnMonsterTeamChange(MonsterSlotsToPlayerID_SyncDictionary.Operation op, int slot, int playerID) {
		Debug.Log("OnMonsterTeamChange called");
	}

	// day night controller helpers

	[Command]
	public void Cmd_SyncToServerDayNight() {
		TargetRpc_SyncToServerDayNight(connectionToClient, dayNightController.IsDay());

	}

	[TargetRpc]
	public void TargetRpc_SyncToServerDayNight(NetworkConnection conn, bool isDay) {
		if (isDay) {
			dayNightController.TransitionToDay();
		}
		else {
			dayNightController.TransitionToNight();
		}
		
	}

	public void CheckServerDayNightTransition(bool isDay, float currentTimer) {
		if (!isServer) return;

		if (currentTimer <= 0) {
			if (isDay)
				Rpc_StartNight();
			else
				Rpc_StartDay();
		}
	}

	[ClientRpc]
	public void Rpc_StartDay() {
		dayNightController.StartDay();
	}

	[ClientRpc]
	public void Rpc_StartNight() {
		dayNightController.StartNight();
	}

}
