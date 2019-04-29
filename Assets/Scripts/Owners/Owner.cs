using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;

public class Owner : NetworkBehaviour {
	public UnitController unit;
	[SyncVar, SerializeField] protected int team;
	[SyncVar] protected Color bodyColor;

	public Teams GetTeam() {
		return (Teams) team;
	}

	public void SetTeam(Teams team) {
		this.team = (int) team;

		if (team == Teams.DWARVES) {
			bodyColor = Color.Lerp(Color.white, Color.cyan, Random.Range(0.2f, 0.6f));
			unit.SetUnitInfo("Dwarf");
		}
		else if (team == Teams.MONSTERS) {
			bodyColor = Color.Lerp(Color.white, Color.red, Random.Range(0.2f, 0.6f));
			unit.SetUnitInfo("Monster");
		}

		unit.body.bodyMesh.material.color = bodyColor;
	}

}
