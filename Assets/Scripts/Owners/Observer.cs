using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;

public class Observer : NetworkBehaviour {

	void Awake() {
		if (isLocalPlayer)
			ReferenceLibrary.Instance.networkHUD.SetLocalObserver(this);
	}

	public void RequestPlayerID(int playerID) {
		Cmd_RequestPlayerID(playerID);
	}

	[Command]
	public void Cmd_RequestPlayerID(int playerID) {
		if (GameResources.Instance.GetPlayer(playerID).networkHelper.isUnassigned) {
			GameResources.Instance.networkGameResources.AssignClient(connectionToClient, playerID);
			TargetRpc_GrantPlayerID(connectionToClient, playerID);
		}
		else {
			TargetRpc_DenyPlayerID(connectionToClient, playerID);
		}
	}

	[TargetRpc]
	public void TargetRpc_GrantPlayerID(NetworkConnection conn, int playerID) {
		var networkHUD = ReferenceLibrary.Instance.networkHUD;
		Player player = GameResources.Instance.GetPlayer(playerID);

		networkHUD.SetState(NetworkStates.GAME);
		player.Initialize();
		NetworkServer.Destroy(this.gameObject);
	}

	[TargetRpc]
	public void TargetRpc_DenyPlayerID(NetworkConnection conn, int playerID) {
		Debug.Log("Server denied character selection request.");
		ReferenceLibrary.Instance.networkHUD.SetState(NetworkStates.SELECTING_PLAYER);
	}

}
