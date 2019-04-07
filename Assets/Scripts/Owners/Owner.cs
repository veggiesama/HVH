using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;

public class Owner : NetworkBehaviour {
	public UnitController unit;
	[SerializeField] private Teams team;

	public Teams GetTeam() {
		return team;
	} 

	public void SetTeam(Teams team) {
		this.team = team;
	}

}
