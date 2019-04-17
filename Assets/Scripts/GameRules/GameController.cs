﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Extensions;

// SyncDictionary: https://vis2k.github.io/Mirror/Classes/SyncDictionary
public class DwarfSlotsToPlayerID_SyncDictionary : SyncDictionary<int, int> {} // SyncDictionary<(int)DwarfTeamSlots, (int)PlayerID> 
public class MonsterSlotsToPlayerID_SyncDictionary: SyncDictionary<int, int> {} // SyncDictionary<(int)DwarfTeamSlots, (int)PlayerID>

public class GameController : NetworkBehaviour {

	[SerializeField] private NetworkManagerHVH networkManager;
	public GameObject playerPrefab;
	public GameObject sceneViewMask;
  
	public DwarfSlotsToPlayerID_SyncDictionary dwarfDictionary = new DwarfSlotsToPlayerID_SyncDictionary();
	public MonsterSlotsToPlayerID_SyncDictionary monsterDictionary = new MonsterSlotsToPlayerID_SyncDictionary();

	// Use this for initialization
	void Start () {
		GameObject[] editorOnlyObjects = GameObject.FindGameObjectsWithTag("EditorOnly");
		foreach(GameObject obj in editorOnlyObjects) {
			foreach (MeshRenderer mesh in obj.GetComponentsInChildren<MeshRenderer>()) {
				//mesh.enabled = false;
			}
		}
	}

	public override void OnStartServer() {
		SpawnUnassignedPlayers();
	}

	public override void OnStartClient()
	{
		if (isServer) return;
		dwarfDictionary.Callback += OnDwarfTeamChange;
		monsterDictionary.Callback += OnMonsterTeamChange;
	}

	private void OnDwarfTeamChange(DwarfSlotsToPlayerID_SyncDictionary.Operation op, int slot, int playerID) {
		Debug.Log("OnDwarfTeamChange called");
	}

	private void OnMonsterTeamChange(MonsterSlotsToPlayerID_SyncDictionary.Operation op, int slot, int playerID) {
		Debug.Log("OnMonsterTeamChange called");
	}

	public void SpawnUnassignedPlayers() {
		// NOTE: Enum count determines number of team slots
		int n = 0;
		foreach (DwarfTeamSlots slot in System.Enum.GetValues(typeof(DwarfTeamSlots)))  {
			Transform spawnLoc = networkManager.GetStartPosition();
			Player unassignedPlayer = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<Player>();
			unassignedPlayer.playerID = n;
			dwarfDictionary.Add((int)slot, n);

			unassignedPlayer.MakeNPC();
			unassignedPlayer.SetTeam(Teams.DWARVES);
			NetworkServer.Spawn(unassignedPlayer.gameObject);
			n++;
		}

		foreach (MonsterTeamSlots slot in System.Enum.GetValues(typeof(MonsterTeamSlots)))  {
			Transform spawnLoc = networkManager.GetStartPosition();
			Player unassignedPlayer = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<Player>();
			unassignedPlayer.playerID = n;
			monsterDictionary.Add((int)slot, n);

			unassignedPlayer.MakeNPC();
			unassignedPlayer.SetTeam(Teams.MONSTERS);
			NetworkServer.Spawn(unassignedPlayer.gameObject);
			n++;
		}

		Debug.Log(dwarfDictionary.DebugToString());
		Debug.Log(monsterDictionary.DebugToString());
	}

	public Player GetNextUnassignedPlayer() {
		foreach (KeyValuePair<int, int> kv in dwarfDictionary) {
			DwarfTeamSlots slot = (DwarfTeamSlots)kv.Key;
			Player player = GetPlayer(kv.Value);

			if (player.GetComponent<NetworkHelper>().isUnassigned)
				return player;
		}

		foreach (KeyValuePair<int, int> kv in monsterDictionary) {
			MonsterTeamSlots slot = (MonsterTeamSlots)kv.Key;
			Player player = GetPlayer(kv.Value);

			if (player.GetComponent<NetworkHelper>().isUnassigned)
				return player;
		}

		return null;
	}

	public Player GetPlayer(DwarfTeamSlots slot) {
		int id = dwarfDictionary[(int)slot];
		return GetPlayer(id);
	}

	public Player GetPlayer(MonsterTeamSlots slot) {
		int id = monsterDictionary[(int)slot];
		return GetPlayer(id);
	}

	public Player GetPlayer(int playerID) {
		Player[] playerArray = FindObjectsOfType<Player>();
		foreach (Player p in playerArray) {
			if (p.playerID == playerID)
				return p;
		}

		return null;
	}

	public static GameObject GetSceneMask() {
		return GameObject.Find("SceneViewMask");
	}

	//private List<GameObject> allies;
	//private GameObject[] enemies;

	public static Transform GetRandomSpawnPoint() {
		GameObject[] allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		int rng = Random.Range(0, allSpawnPoints.Length);

		return allSpawnPoints[rng].transform;
	}


}
