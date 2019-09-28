using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.SceneManagement;
using Mirror;
using System.Linq;

public class GameRules : Singleton<GameRules> {

	[HideInInspector]
	public TeamFieldOfView teamFov;

	public int houndsToSpawn;
	public int testMonstersToSpawn;
	public GameObject playerPrefab;

	// Singleton constructor
	public static GameRules Instance {
		get {
			return ((GameRules)mInstance);
		} set {
			mInstance = value;
		}
	}

	void Awake() {
		//DontDestroyOnLoad(this.gameObject);
		//playerPrefab = NetworkManager.singleton.playerPrefab;
		teamFov = GetComponentInChildren<TeamFieldOfView>();

		int numPlayers = System.Enum.GetValues(typeof(MonsterTeamSlots)).Length + 
						 System.Enum.GetValues(typeof(DwarfTeamSlots)).Length;
	}

	public void SetupGame() {
		DayNight.Instance.server.Initialize();

		Debug.Log("Trying to spawn players.");
		//SpawnUnassignedPlayers();
		SpawnUnassignedPlayers();

		Debug.Log("Trying to spawn NPCs.");
		SpawnHounds(houndsToSpawn);
		//SpawnTestMonsters(testMonstersToSpawn);
	}

	public void SpawnHounds(int howMany) {
		for (int i = 0; i < howMany; i++) {
			SpawnHound();
		}
	}

	public void SpawnHound() {
		Transform spawnLoc = GameResources.Instance.GetRandomSpawnPoint();
		GameObject npc = Instantiate(ResourceLibrary.Instance.npcPrefab, spawnLoc.position, spawnLoc.rotation);
		NetworkServer.Spawn(npc);
		Owner npcOwner = npc.GetComponent<Owner>();
		npcOwner.SetTeam(Teams.DWARVES);
		npcOwner.SetUnitInfo("Hound");

		//npc.GetComponent<NetworkIdentity>().AssignClientAuthority(NetworkServer.localConnection);
		//npc.SetTeam();
	}

	public void SpawnTestMonsters(int howMany) {
		for (int i = 0; i < howMany; i++) {
			SpawnTestMonster();
		}
	}


	public void SpawnTestMonster() {
		Transform spawnLoc = GameResources.Instance.GetRandomSpawnPoint();
		GameObject npc = Instantiate(ResourceLibrary.Instance.npcPrefab, spawnLoc.position, spawnLoc.rotation);
		NetworkServer.Spawn(npc);
		Owner npcOwner = npc.GetComponent<Owner>();
		npcOwner.SetTeam(Teams.MONSTERS);
		npcOwner.SetUnitInfo("MonsterTest");
	}

	public void SpawnUnassignedPlayers() {
		int totalPlayers = Constants.DwarvesTotal + Constants.MonstersTotal;
		for (int id = 0; id < totalPlayers; id++) {
			Transform spawnLoc = NetworkManager.singleton.GetStartPosition();
			GameObject unassignedPlayerGO = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation);
			Player unassignedPlayer = unassignedPlayerGO.GetComponent<Player>(); 

			unassignedPlayer.playerID = id;
			unassignedPlayer.MakeNPC();
			//NetworkServer.Spawn(unassignedPlayer.gameObject);
			NetworkServer.SpawnWithClientAuthority(unassignedPlayer.gameObject, NetworkServer.localConnection);

			//GameResources.Instance.AddPlayerReference(id, unassignedPlayer);
			GameResources.Instance.AddPlayerReference(unassignedPlayer);
			GameResources.Instance.AddUnitReference(unassignedPlayer.unit);
			//networkGameRules.dwarfDictionary.Add(playerIdCounter, n);

			if (id < Constants.DwarvesTotal) {
				unassignedPlayer.SetTeam(Teams.DWARVES);
				unassignedPlayer.SetUnitInfo("Dwarf");
			}
			else {
				unassignedPlayer.SetTeam(Teams.MONSTERS);
				unassignedPlayer.SetUnitInfo("Monster");
			}
		}
	}
	/*
	public void SpawnUnassignedPlayers() {

		// NOTE: Enum count determines number of team slots
		int n = 0;
		foreach (DwarfTeamSlots slot in System.Enum.GetValues(typeof(DwarfTeamSlots)))  {
			Transform spawnLoc = NetworkManager.singleton.GetStartPosition();
			GameObject unassignedPlayerGO = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation);
			Player unassignedPlayer = unassignedPlayerGO.GetComponent<Player>(); 

			unassignedPlayer.playerID = n;
			GameResources.Instance.AddPlayerReference(n, unassignedPlayer);
			networkGameRules.dwarfDictionary.Add((int)slot, n);

			unassignedPlayer.MakeNPC();
			NetworkServer.Spawn(unassignedPlayer.gameObject);
			unassignedPlayer.SetTeam(Teams.DWARVES);
			unassignedPlayer.SetUnitInfo("Dwarf");
			n++;
		}

		foreach (MonsterTeamSlots slot in System.Enum.GetValues(typeof(MonsterTeamSlots)))  {
			Transform spawnLoc = NetworkManager.singleton.GetStartPosition();
			GameObject unassignedPlayerGO = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation);
			Player unassignedPlayer = unassignedPlayerGO.GetComponent<Player>(); 

			unassignedPlayer.playerID = n;
			GameResources.Instance.AddPlayerReference(n, unassignedPlayer);
			networkGameRules.monsterDictionary.Add((int)slot, n);

			unassignedPlayer.MakeNPC();
			NetworkServer.Spawn(unassignedPlayer.gameObject);
			unassignedPlayer.SetTeam(Teams.MONSTERS);
			unassignedPlayer.SetUnitInfo("Monster");
			n++;
		}

		//Debug.Log(dwarfDictionary.DebugToString());
		//Debug.Log(monsterDictionary.DebugToString());
	}
	*/

	/*
	public DwarfSlotsToPlayerID_SyncDictionary GetDwarfTeamDictionary() {
		return networkGameRules.dwarfDictionary;
	}

	public MonsterSlotsToPlayerID_SyncDictionary GetMonsterTeamDictionary() {
		return networkGameRules.monsterDictionary;
	}*/


}
