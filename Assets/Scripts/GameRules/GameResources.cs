using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.SceneManagement;
using Mirror;
using System.Linq;
using Tree = HVH.Tree;

public class GameResources : Singleton<GameResources> {

	// Singleton constructor
	public static GameResources Instance {
		get {
			return ((GameResources)mInstance);
		} set {
			mInstance = value;
		}
	}

	public NetworkGameResources networkGameResources;
	public Camera miniMapCamera;

	private Player localPlayer;
	//private Dictionary<int, Player> playerDictionary;
	//private List<UnitController> unitList;
	private List<TreeHandler> treeHandlers;
	private List<GameObject> spawnPoints;
	//private List<GameObject> pointsOfInterest;

	void Awake() {
		//playerDictionary = new Dictionary<int, Player>();
		//unitList = new List<UnitController>();
		treeHandlers = new List<TreeHandler>();
		spawnPoints = new List<GameObject>();
		//int numPlayers = System.Enum.GetValues(typeof(MonsterTeamSlots)).Length + 
		//				 System.Enum.GetValues(typeof(DwarfTeamSlots)).Length;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Player and unit references
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void AddUnitReference(UnitController unit) {
		networkGameResources.AddUnitReference(unit);
	}

	public void RemoveUnitReference(UnitController unit){
		networkGameResources.RemoveUnitReference(unit);
	}

	//public void AddPlayerReference(int playerID, Player p) {
	//	networkGameResources.AddPlayerReference(playerID, p);
	//}

	public void AddPlayerReference(Player p) {
		networkGameResources.AddPlayerReference(p);
	}

	//public void RemovePlayerReference(int playerID) {
	//	networkGameResources.RemovePlayerReference(playerID);
	//}

	public void RemovePlayerReference(Player p) {
		networkGameResources.RemovePlayerReference(p);
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Get player references
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Player GetPlayer(int playerID) {
		foreach (Player p in GetAllPlayers()) {
			if (p.playerID == playerID)
				return p;
		}
		return null;
	}

	public List<Player> GetAllPlayers() {
		return networkGameResources.GetAllPlayers();
	}

	public List<Player> GetAllPlayers(Teams team) {
		List<Player> players = GetAllPlayers().FindAll(x => x.GetTeam() == team);
		return players;
	}

	public List<UnitController> GetAllUnits() {
		return networkGameResources.GetAllUnits();
	}

	public Player GetNextUnassignedPlayer() {

		foreach(Player p in GetAllPlayers()) {
			if (p.networkHelper.isUnassigned)
				return p;
		}

		return null;
	}

	public Player GetNextPlayerOnTeam(Teams team) {
		var dict = GetTeamDictionary(team);
		foreach (KeyValuePair<int, int> kv in dict) {
			Player player = GetPlayer(kv.Value);
			if (player.GetComponent<NetworkHelper>().isUnassigned)
				return player;
		}
		return null;
	}

	public TeamSlotToPlayerIDSyncDictionary GetTeamDictionary(Teams team) {
		return networkGameResources.GetTeamDictionary(team);
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Get unit references
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public List<UnitController> GetEnemyUnitsOf(UnitController unit, bool visibleOnly) {
		//UnitController[] units = FindObjectsOfType<UnitController>();
		List<UnitController> validUnitList = new List<UnitController>();

		foreach(UnitController u in GetAllUnits()) {
			if (!unit.SharesTeamWith(u))
				if ((!visibleOnly) || (visibleOnly && u.body.IsVisibleToUnit(unit) ))
					validUnitList.Add(u);
		}

		return validUnitList;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Local player
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void SetLocalPlayer(Player player) {
		//foreach (Player p in GetAllPlayers()) {
		//	p.gameObject.tag = "Untagged";
		//}
		//
		//player.gameObject.tag = "LocalPlayer";
		localPlayer = player;
	}

	public Player GetLocalPlayer() {
		return localPlayer;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Trees
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void RegisterTreeHandler(TreeHandler treeHandler) {
		treeHandlers.Add(treeHandler);
	}

	public void DisableTreeHighlighting() {
		Debug.Log("Disable tree highlighting");
		foreach (TreeHandler treeHandler in treeHandlers) {
			foreach (Tree t in treeHandler.GetComponentsInChildren<Tree>()) {
				t.SetHighlighted(HighlightingState.NONE);
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Spawn points
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void RegisterSpawnPoint(GameObject spawnPoint) {
		spawnPoints.Add(spawnPoint);
	}

	//public void RegisterPointsOfInterest(GameObject poi) {
	//	pointsOfInterest.Add(poi);
	//}

	public Transform GetRandomSpawnPoint() {
		//GameObject[] allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		int rng = Random.Range(0, spawnPoints.Count - 1);
		Transform trans = spawnPoints[rng].transform;
		trans.position = Util.SnapVectorToTerrain(trans.position);
		return trans;
	}

	//public Transform GetRandomPointOfInterest() {
	//	//GameObject[] allPOIs = GameObject.FindGameObjectsWithTag("Point of Interest");
	//	int rng = Random.Range(0, pointsOfInterest.Count - 1);
	//	return pointsOfInterest[rng].transform;
	//}

}
