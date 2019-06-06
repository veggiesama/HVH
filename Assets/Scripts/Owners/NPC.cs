using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;


public class NPC : Owner {

	private void Start() {	
		//NetworkServer.Spawn(this.gameObject);
		//NetworkServer.SpawnWithClientAuthority(this.gameObject, NetworkServer.localConnection);
		//GetComponent<NetworkIdentity>().AssignClientAuthority(NetworkServer.localConnection);
		SetTeam(Teams.DWARVES);
		//StartCoroutine( DoAbil() );
	}

}
