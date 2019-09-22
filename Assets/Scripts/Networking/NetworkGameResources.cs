using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Tree = HVH.Tree;


public class TeamSlotToPlayerID_SyncDictionary : SyncDictionary<int, int> {}
public class PlayerIdToPlayerObject_SyncDictionary : SyncDictionary<int, GameObject> {}
public class UnitOwner_SyncList : SyncList<GameObject> {}

public class NetworkGameResources : NetworkBehaviour {

	private PlayerIdToPlayerObject_SyncDictionary playerDictionary = new PlayerIdToPlayerObject_SyncDictionary();
	private UnitOwner_SyncList unitOwnerList = new UnitOwner_SyncList();

	private TeamSlotToPlayerID_SyncDictionary dwarfDictionary = new TeamSlotToPlayerID_SyncDictionary();
	private TeamSlotToPlayerID_SyncDictionary monsterDictionary = new TeamSlotToPlayerID_SyncDictionary();

	/*
	public override void OnStartClient() {
		if (isServer) return;
		dwarfDictionary.Callback += OnDwarfTeamChange;
		monsterDictionary.Callback += OnMonsterTeamChange;
		playerDictionary.Callback += OnPlayersChange;
	}

	// callbacks
	private void OnPlayersChange(PlayerIdToPlayerObject_SyncDictionary.Operation op, int playerId, GameObject go) {
		Debug.Log("OnPlayersChange called");
	}

	private void OnDwarfTeamChange(TeamSlotToPlayerID_SyncDictionary.Operation op, int slot, int playerID) {
		Debug.Log("OnDwarfTeamChange called");
	}

	private void OnMonsterTeamChange(TeamSlotToPlayerID_SyncDictionary.Operation op, int slot, int playerID) {
		Debug.Log("OnMonsterTeamChange called");
	}*/

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
	}

	public void AddUnitReference(UnitController unit) {
		unitOwnerList.Add(unit.owner.gameObject);
	}

	public void RemoveUnitReference(UnitController unit){
		unitOwnerList.Remove(unit.owner.gameObject);
	}

	public List<Player> GetAllPlayers() {
		var playerList = new List<Player>();

		foreach (GameObject go in playerDictionary.Values) {
			Player p = go.GetComponent<Player>();
			if (p != null)
				playerList.Add(p);
		}

		return playerList;
	}

	public List<UnitController> GetAllUnits() {
		var unitList = new List<UnitController>();
		foreach (GameObject go in unitOwnerList) {
			Owner o = go.GetComponent<Owner>();
			if (o != null && o.unit != null)
				unitList.Add(o.unit);
		}
		return unitList;
	}

	public TeamSlotToPlayerID_SyncDictionary GetTeamDictionary(Teams team) {

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

	public int GetPlayerCount() {
		return playerDictionary.Count;
	}

}
