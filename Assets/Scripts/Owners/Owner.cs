using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;

public class Owner : NetworkBehaviour {
	public UnitController unit;
	[SyncVar, SerializeField] protected int team;
	public NetworkHelper networkHelper;

	public void Awake() {
		networkHelper = GetComponent<NetworkHelper>();
	}

	public Teams GetTeam() {
		return (Teams) team;
	}

	public void SetTeam(Teams team) {
		this.team = (int) team;

		if (team == Teams.DWARVES) {
			networkHelper.SetUnitInfo("Dwarf");
		}
		else if (team == Teams.MONSTERS) {
			networkHelper.SetUnitInfo("Monster");
		}

	}

}
