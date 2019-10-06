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
	public int dwarfMooksToSpawn;
	public int monsterMooksToSpawn;

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

		int numPlayers = Constants.DwarvesTotal + Constants.MonstersTotal;
	}

	public void SetupGame() {
		DayNight.Instance.server.Initialize();

		Debug.Log("Trying to spawn players.");
		//SpawnUnassignedPlayers();
		SpawnUnassignedPlayers();

		Debug.Log("Trying to spawn NPCs.");
		SpawnHounds(houndsToSpawn);
		SpawnMooks("MookMonster", Teams.MONSTERS, monsterMooksToSpawn);
		SpawnMooks("MookDwarf", Teams.DWARVES, dwarfMooksToSpawn);
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

	public void SpawnMooks(string unitInfo, Teams team, int howMany) {
		for (int i = 0; i < howMany; i++) {
			SpawnMook(unitInfo, team);
		}
	}


	public void SpawnMook(string unitInfo, Teams team) {
		Transform spawnLoc = GameResources.Instance.GetRandomSpawnPoint();
		GameObject npc = Instantiate(ResourceLibrary.Instance.npcPrefab, spawnLoc.position, spawnLoc.rotation);
		NetworkServer.Spawn(npc);
		Owner npcOwner = npc.GetComponent<Owner>();
		npcOwner.SetTeam(team);
		npcOwner.SetUnitInfo(unitInfo);
	}

	public void SpawnUnassignedPlayers() {
		int totalPlayers = Constants.DwarvesTotal + Constants.MonstersTotal;
		for (int id = 0; id < totalPlayers; id++) {
			Transform spawnLoc = NetworkManager.singleton.GetStartPosition();
			GameObject unassignedPlayerGO = Instantiate(playerPrefab);
			Player unassignedPlayer = unassignedPlayerGO.GetComponent<Player>();

			unassignedPlayer.playerID = id;
			unassignedPlayer.MakeNPC();
			//NetworkServer.Spawn(unassignedPlayer.gameObject);
			NetworkServer.SpawnWithClientAuthority(unassignedPlayerGO, NetworkServer.localConnection);
			unassignedPlayer.unit.ServerRespawn(spawnLoc.position, spawnLoc.rotation);

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

}
