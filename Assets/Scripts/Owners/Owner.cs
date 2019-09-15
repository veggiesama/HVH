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

	public override void OnStartClient() {
		if (!string.IsNullOrEmpty(networkHelper.unitInfo)) {
			unit.SetUnitInfo(networkHelper.unitInfo);
		}
	}

	public virtual void Awake() {
		unit = GetComponentInChildren<UnitController>();
		networkHelper = GetComponent<NetworkHelper>();
	}

	public Teams GetTeam() {
		return (Teams) team;
	}

	public void SetTeam(Teams team) {
		this.team = (int) team;
	}

	public void SetUnitInfo(string unitInfo) {
		networkHelper.SetUnitInfo(unitInfo);
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

