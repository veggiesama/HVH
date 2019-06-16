using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.SceneManagement;
using Mirror;

public class GameRules : Singleton<GameRules> {

	[HideInInspector]
	public NetworkGameRules networkGameRules;
	[HideInInspector]
	public TeamFieldOfView teamFov;

	public GameObject npcPrefab;
	private GameObject playerPrefab;
	private Player localPlayer;

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
		playerPrefab = NetworkManager.singleton.playerPrefab;
		networkGameRules = GetComponent<NetworkGameRules>();
		teamFov = GetComponentInChildren<TeamFieldOfView>();
	}


	// Use this for initialization
	/*
	void Start () {
		GameObject[] editorOnlyObjects = GameObject.FindGameObjectsWithTag("EditorOnly");
		foreach(GameObject obj in editorOnlyObjects) {
			foreach (MeshRenderer mesh in obj.GetComponentsInChildren<MeshRenderer>()) {
				//mesh.enabled = false;
			}
		}
	}*/

	public void SetupGame() {
		Debug.Log("Trying to spawn players.");
		SpawnUnassignedPlayers();

		Debug.Log("Trying to spawn NPCs.");
		SpawnNPCs();
		SpawnNPCs();
		SpawnNPCs();
	}

	public void SpawnNPCs() {
		Transform spawnLoc = GetRandomPointOfInterest();
		GameObject npc = Instantiate(npcPrefab, spawnLoc.position, spawnLoc.rotation);
		NetworkServer.Spawn(npc);
		//npc.GetComponent<NetworkIdentity>().AssignClientAuthority(NetworkServer.localConnection);
		//npc.SetTeam();
	}

	public void SpawnUnassignedPlayers() {

		// NOTE: Enum count determines number of team slots
		int n = 0;
		foreach (DwarfTeamSlots slot in System.Enum.GetValues(typeof(DwarfTeamSlots)))  {
			Transform spawnLoc = NetworkManager.singleton.GetStartPosition();
			GameObject unassignedPlayerGO = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation);
			Player unassignedPlayer = unassignedPlayerGO.GetComponent<Player>(); 
			unassignedPlayer.playerID = n;
			networkGameRules.dwarfDictionary.Add((int)slot, n);

			unassignedPlayer.MakeNPC();
			NetworkServer.Spawn(unassignedPlayer.gameObject);
			unassignedPlayer.SetTeam(Teams.DWARVES);
			n++;
		}

		foreach (MonsterTeamSlots slot in System.Enum.GetValues(typeof(MonsterTeamSlots)))  {
			Transform spawnLoc = NetworkManager.singleton.GetStartPosition();
			Player unassignedPlayer = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<Player>();
			unassignedPlayer.playerID = n;
			networkGameRules.monsterDictionary.Add((int)slot, n);

			unassignedPlayer.MakeNPC();
			NetworkServer.Spawn(unassignedPlayer.gameObject);
			unassignedPlayer.SetTeam(Teams.MONSTERS);
			n++;
		}

		//Debug.Log(dwarfDictionary.DebugToString());
		//Debug.Log(monsterDictionary.DebugToString());
	}

	public Player GetNextUnassignedPlayer() {
		foreach (KeyValuePair<int, int> kv in networkGameRules.monsterDictionary) {
			MonsterTeamSlots slot = (MonsterTeamSlots)kv.Key;
			Player player = GetPlayer(kv.Value);

			if (player.GetComponent<NetworkHelper>().isUnassigned)
				return player;
		}

		foreach (KeyValuePair<int, int> kv in networkGameRules.dwarfDictionary) {
			DwarfTeamSlots slot = (DwarfTeamSlots)kv.Key;
			Player player = GetPlayer(kv.Value);

			if (player.GetComponent<NetworkHelper>().isUnassigned)
				return player;
		}

		return null;
	}

	public Player GetPlayer(DwarfTeamSlots slot) {
		int id = networkGameRules.dwarfDictionary[(int)slot];
		return GetPlayer(id);
	}

	public Player GetPlayer(MonsterTeamSlots slot) {
		int id = networkGameRules.monsterDictionary[(int)slot];
		return GetPlayer(id);
	}

	public Player GetPlayer(int playerID) {
		foreach (Player p in GetAllPlayers()) {
			if (p.playerID == playerID)
				return p;
		}
		return null;
	}

	public static List<UnitController> GetEnemyUnitsOf(UnitController unit, bool visibleOnly) {
		UnitController[] units = FindObjectsOfType<UnitController>();
		List<UnitController> validUnitList = new List<UnitController>();

		foreach(UnitController u in units) {
			if (!unit.SharesTeamWith(u))
				if ((!visibleOnly) || (visibleOnly && u.body.IsVisible() ))
					validUnitList.Add(u);
		}

		return validUnitList;
	}

	public Player[] GetAllPlayers() {
		return FindObjectsOfType<Player>();
	}

	public static Transform GetRandomSpawnPoint() {
		GameObject[] allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		int rng = Random.Range(0, allSpawnPoints.Length);
		return allSpawnPoints[rng].transform;
	}

	public static Transform GetRandomPointOfInterest() {
		GameObject[] allPOIs = GameObject.FindGameObjectsWithTag("Point of Interest");
		int rng = Random.Range(0, allPOIs.Length);
		return allPOIs[rng].transform;
	}

	public void SetLocalPlayer(Player player) {
		foreach (Player p in GetAllPlayers()) {
			p.gameObject.tag = "Untagged";
		}

		player.gameObject.tag = "LocalPlayer";
		localPlayer = player;
	}

	public Player GetLocalPlayer() {
		return localPlayer;
	}

	public DwarfSlotsToPlayerID_SyncDictionary GetDwarfTeamDictionary() {
		return networkGameRules.dwarfDictionary;
	}

	public MonsterSlotsToPlayerID_SyncDictionary GetMonsterTeamDictionary() {
		return networkGameRules.monsterDictionary;
	}



}
