using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;

public class Owner : NetworkBehaviour {
	public UnitController unit;
	[SerializeField] protected Teams team;

	[SyncVar] protected Color bodyColor;

	public Teams GetTeam() {
		return team;
	} 

	public void SetTeam(Teams team) {
		this.team = team;

		if (team == Teams.DWARVES) {
			bodyColor = Color.Lerp(Color.blue, Color.cyan, Random.Range(0.2f, 1.0f));
		}
		else if (team == Teams.MONSTERS) {
			bodyColor = Color.Lerp(Color.red, Color.magenta, Random.Range(0.2f, 1.0f));
		}

		unit.body.GetComponent<Renderer>().material.color = bodyColor;
	}

}
