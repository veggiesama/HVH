using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Extensions;

// SyncDictionary: https://vis2k.github.io/Mirror/Classes/SyncDictionary
//public class DwarfSlotsToPlayerID_SyncDictionary : SyncDictionary<int, int> {} // SyncDictionary<(int)DwarfTeamSlots, (int)PlayerID> 
//public class MonsterSlotsToPlayerID_SyncDictionary: SyncDictionary<int, int> {} // SyncDictionary<(int)DwarfTeamSlots, (int)PlayerID>

public class NetworkGameRules : NetworkBehaviour {

	private DayNightController dayNightController;

	public void Awake() {
		//DontDestroyOnLoad(this.gameObject);
		dayNightController = GetComponent<DayNightController>();
	}

	private void Start() {
		//Cmd_SyncToServerDayNight();
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
