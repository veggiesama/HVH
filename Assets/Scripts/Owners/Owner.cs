using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;

public class Owner : NetworkBehaviour {
	[HideInInspector] public UnitController unit;
	[HideInInspector] public NetworkHelper networkHelper;
	[SyncVar] protected int team;

	public virtual void Awake() {
		unit = GetComponentInChildren<UnitController>();
		networkHelper = GetComponent<NetworkHelper>();
	}

	public Teams GetTeam() {
		return (Teams) team;
	}

	public void SetTeam(Teams team) {
		this.team = (int) team;

		if (!unit.IsPlayerOwned()) {
			networkHelper.SetUnitInfo("Dummy NPC");
		}

		else if (team == Teams.DWARVES) {
			networkHelper.SetUnitInfo("Dwarf");
		}
		else if (team == Teams.MONSTERS) {
			networkHelper.SetUnitInfo("Monster");
		}

	}

}
