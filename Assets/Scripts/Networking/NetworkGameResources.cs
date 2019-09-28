using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Tree = HVH.Tree;

public class TeamSlotToPlayerIDSyncDictionary : SyncDictionary<int, int> {}

public class NetworkGameResources : NetworkBehaviour {

	//public class SyncDictionaryPlayerIdToPlayerObject : SyncDictionary<int, GameObject> {}
	public class SyncListPlayerObject : SyncList<GameObject> {}
	public class UnitOwnerSyncList : SyncList<GameObject> {}

	//public SyncDictionaryPlayerIdToPlayerObject playerDictionary = new SyncDictionaryPlayerIdToPlayerObject();
	public UnitOwnerSyncList unitOwnerList = new UnitOwnerSyncList();
	public SyncListPlayerObject playerObjectList = new SyncListPlayerObject();

	public TeamSlotToPlayerIDSyncDictionary dwarfDictionary = new TeamSlotToPlayerIDSyncDictionary();
	public TeamSlotToPlayerIDSyncDictionary monsterDictionary = new TeamSlotToPlayerIDSyncDictionary();

	public override void OnStartClient() {
		if (isServer) return;
		dwarfDictionary.Callback += OnDwarfTeamChange;
		monsterDictionary.Callback += OnMonsterTeamChange;
		//playerDictionary.Callback += OnPlayersChange;
	}

	/*private void Update()
	{
		//playerDictionary.TryGetValue(0, out GameObject value
		if (playerObjectList.Count > 0 && playerObjectList[0] != null)
			Debug.Log("PLAYER_DICT: " + (playerObjectList[0].name));
		else
			Debug.Log("PLAYER_DICT: null");
	}*/

	// callbacks
	//private void OnPlayersChange(SyncDictionaryPlayerIdToPlayerObject.Operation op, int playerId, GameObject go) {
	//	Debug.Log("OnPlayersChange called");
	//}

	private void OnDwarfTeamChange(TeamSlotToPlayerIDSyncDictionary.Operation op, int slot, int playerID) {
		Debug.Log("OnDwarfTeamChange called");
	}

	private void OnMonsterTeamChange(TeamSlotToPlayerIDSyncDictionary.Operation op, int slot, int playerID) {
		Debug.Log("OnMonsterTeamChange called");
	}

	public void AddPlayerReference(Player p) {
		if (!playerObjectList.Contains(p.gameObject))
			playerObjectList.Add(p.gameObject);
		else
			Debug.Log("Player object list already contains player.");
	}

	public void RemovePlayerReference(Player p) {
		playerObjectList.Remove(p.gameObject);
	}

	/*
	public void AddPlayerReference(int playerID, Player p) {
		if (!playerDictionary.ContainsKey(playerID))
			playerDictionary.Add(playerID, p.gameObject);
		else
			Debug.Log("Player dictionary already contains key.");
	}

	public void RemovePlayerReference(int playerID) {
		if (playerDictionary.ContainsKey(playerID))
			playerDictionary.Remove(playerID);
		else
			Debug.Log("Cannot find player ID in player dictionary.");
	}

	public void RemovePlayerReference(Player p) {
		GameObject playerGO = p.gameObject;
		var item = playerDictionary.First(kv => kv.Value == playerGO);
		playerDictionary.Remove(item.Key);
	}*/

	public void AddUnitReference(UnitController unit) {
		unitOwnerList.Add(unit.owner.gameObject);
	}

	public void RemoveUnitReference(UnitController unit){
		unitOwnerList.Remove(unit.owner.gameObject);
	}

	public List<Player> GetAllPlayers() {
		var playerList = new List<Player>();

		foreach (var kv in NetworkIdentity.spawned) {
			//var key = kv.Key;
			var value = kv.Value;
			var player = value.GetComponent<Player>();

			if (player != null) {
				playerList.Add(player);
			}
		}

		return playerList;
	}

	/*
	public List<Player> GetAllPlayers() {
		var playerList = new List<Player>();

		foreach (GameObject go in playerObjectList) {
			Player p = go.GetComponent<Player>();
			if (p != null)
				playerList.Add(p);
		}

		return playerList;
	}*/

	/*
	public List<Player> GetAllPlayers() {
		var playerList = new List<Player>();

		foreach (GameObject go in playerDictionary.Values) {
			Player p = go.GetComponent<Player>();
			if (p != null)
				playerList.Add(p);
		}

		return playerList;
	}*/

	public List<UnitController> GetAllUnits() {
		var unitList = new List<UnitController>();
		foreach (GameObject go in unitOwnerList) {
			Owner o = go.GetComponent<Owner>();
			if (o != null && o.unit != null)
				unitList.Add(o.unit);
		}
		return unitList;
	}

	public TeamSlotToPlayerIDSyncDictionary GetTeamDictionary(Teams team) {

		switch (team) {
			case Teams.DWARVES:
				return dwarfDictionary;
			case Teams.MONSTERS:
				return monsterDictionary;
			default:
				break;
		}

		return null;
	}

	/*
	public int GetPlayerCount() {
		return playerDictionary.Count;
	}*/

	public void AssignClient(NetworkConnection conn, int playerID) {
		Debug.Log("Server: Assigning client " + conn.connectionId + " to chosen player " + playerID);
		Player player = GameResources.Instance.GetPlayer(playerID);
		player.networkHelper.isUnassigned = false;

		//if (player.netIdentity.clientAuthorityOwner != null) {
		//	NetworkServer.SetClientNotReady(player.netIdentity.connectionToClient);
		//	//player.netIdentity.RemoveClientAuthority(NetworkServer.localConnection);
		//}

		//GameObject oldGO = conn.identity.gameObject;
		GameObject playerGO = player.gameObject;
		
		NetworkServer.ReplacePlayerForConnection(conn, playerGO);
		player.netIdentity.AssignClientAuthority(conn);
		//NetworkServer.Destroy(oldGO);

		//if (player.netIdentity.clientAuthorityOwner != null)
		//	NetworkServer.ReplacePlayerForConnection(conn, playerGO);
		//else
		//	NetworkServer.AddPlayerForConnection(conn, playerGO);

		// explicitly grant client authority to host (fixes Smooth Sync not syncing server-caused forced movement)
		//if (conn == NetworkServer.localConnection) {
		//	foreach (Player p in GameResources.Instance.GetAllPlayers()) {
		//		p.netIdentity.AssignClientAuthority(conn);
		//	}
		//}
	}

}
