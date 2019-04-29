using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class GameRules : Singleton<GameRules> {

	private NetworkGameRules networkGameRules;
	public GameObject sceneViewMask;
	public float cycleLength = 6f;

	// Singleton constructor
	public static GameRules Instance {
		get {
			return ((GameRules)mInstance);
		} set {
			mInstance = value;
		}
	}

	// Use this for initialization
	void Start () {
		networkGameRules = GetComponent<NetworkGameRules>();

		GameObject[] editorOnlyObjects = GameObject.FindGameObjectsWithTag("EditorOnly");
		foreach(GameObject obj in editorOnlyObjects) {
			foreach (MeshRenderer mesh in obj.GetComponentsInChildren<MeshRenderer>()) {
				//mesh.enabled = false;
			}
		}
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

	public void SpawnUnassignedPlayers() {
		networkGameRules.SpawnUnassignedPlayers();
	}

	public Player GetNextUnassignedPlayer() {
		return networkGameRules.GetNextUnassignedPlayer();
	}

	public Player GetPlayer(DwarfTeamSlots slot) {
		return networkGameRules.GetPlayer(slot);
	}

	public Player GetPlayer(MonsterTeamSlots slot) {
		return networkGameRules.GetPlayer(slot);
	}

	public Player GetPlayer(int playerID) {
		return networkGameRules.GetPlayer(playerID);
	}

	public Player[] GetAllPlayers() {
		return networkGameRules.GetAllPlayers();
	}

	public DwarfSlotsToPlayerID_SyncDictionary GetDwarfTeamDictionary() {
		return networkGameRules.dwarfDictionary;
	}

	public MonsterSlotsToPlayerID_SyncDictionary GetMonsterTeamDictionary() {
		return networkGameRules.monsterDictionary;
	}

}
