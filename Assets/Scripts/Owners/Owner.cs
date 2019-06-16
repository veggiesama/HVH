using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;

public abstract class Owner : NetworkBehaviour {
	[HideInInspector] public UnitController unit;
	[HideInInspector] public NetworkHelper networkHelper;
	[SyncVar] protected int team;
	private Vector3 virtualPointerLocation;

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

	public void SetVirtualPointerLocation(Vector3 vec) {
		virtualPointerLocation = vec;
	}

	public Vector3 GetVirtualPointerLocation() {
		if (this is Player) {
			return ((Player)this).GetMouseLocationToGround();
		}

		else if (this is NPC) {
			return virtualPointerLocation;
		}

		else
			return default;
	}

}
