using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	[HideInInspector] public Player localPlayer;
	public GameObject playerPrefab;
	public GameObject sceneViewMask;
	//public Teams startingTeamForPlayer;

	public Dictionary<DwarfTeamSlots, Player> dwarfTeamPlayers;
	public Dictionary<MonsterTeamSlots, Player> monsterTeamPlayers;

	public Player GetDwarf(DwarfTeamSlots position) {
		return this.dwarfTeamPlayers[position];
	}

	public Player GetMonster(MonsterTeamSlots position) {
		return this.monsterTeamPlayers[position];
	}

	public static GameObject GetSceneMask() {
		return GameObject.Find("SceneViewMask");
	}

	//private List<GameObject> allies;
	//private GameObject[] enemies;

	// Use this for initialization
	void Start () {
		GameObject[] editorOnlyObjects = GameObject.FindGameObjectsWithTag("EditorOnly");
		foreach(GameObject obj in editorOnlyObjects) {
			foreach (MeshRenderer mesh in obj.GetComponentsInChildren<MeshRenderer>()) {
				mesh.enabled = false;
			}
		}
		//Invoke("SpawnUnassignedPlayers", 3.0f);
	}

	public void SpawnUnassignedPlayers() {
		dwarfTeamPlayers = new Dictionary<DwarfTeamSlots, Player>();
		monsterTeamPlayers = new Dictionary<MonsterTeamSlots, Player>();

		foreach (DwarfTeamSlots slot in System.Enum.GetValues(typeof(DwarfTeamSlots)))  {
			Transform spawnLoc = GetRandomSpawnPoint();
			Player unassignedPlayer = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<Player>();
			dwarfTeamPlayers.Add(slot, unassignedPlayer);

			unassignedPlayer.MakeNPC();
			unassignedPlayer.SetTeam(Teams.DWARVES);
			Debug.Log("Unable to change color");
			//unassignedPlayer.unit.body.GetComponent<Renderer>().material.color =
			//	Color.Lerp(Color.blue, Color.cyan, Random.Range(0.2f, 1.0f));
		}

		foreach (MonsterTeamSlots slot in System.Enum.GetValues(typeof(MonsterTeamSlots)))  {
			Transform spawnLoc = GetRandomSpawnPoint();
			Player unassignedPlayer = Instantiate(playerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<Player>();
			monsterTeamPlayers.Add(slot, unassignedPlayer);

			//unassignedPlayer.MakeNPC();
			unassignedPlayer.MakeNPC();
			unassignedPlayer.SetTeam(Teams.MONSTERS);
			unassignedPlayer.SetTeam(Teams.MONSTERS);
			//unassignedPlayer.unit.body.GetComponent<Renderer>().material.color =
			//	Color.Lerp(Color.red, Color.magenta, Random.Range(0.2f, 1.0f));;

		}		
	}
	
	// Update is called once per frame
	//void Update () {}

	public static Transform GetRandomSpawnPoint() {
		GameObject[] allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		int rng = Random.Range(0, allSpawnPoints.Length);

		return allSpawnPoints[rng].transform;
	}

	//public static OwnerController GetLocalOwner() {
	//	return GameObject.FindGameObjectWithTag("Player").GetComponent<OwnerController>();
	//}

	public Player GetNextUnassignedPlayer() {
		foreach (KeyValuePair<DwarfTeamSlots, Player> kv in dwarfTeamPlayers) {
			DwarfTeamSlots slot = kv.Key;
			Player player = kv.Value;

			if (dwarfTeamPlayers[slot].isUnassigned)
				return dwarfTeamPlayers[slot];
		}

		foreach (KeyValuePair<MonsterTeamSlots, Player> kv in monsterTeamPlayers) {
			MonsterTeamSlots slot = kv.Key;
			Player player = kv.Value;

			if (monsterTeamPlayers[slot].isUnassigned)
				return monsterTeamPlayers[slot];
		}

		return null;
	}
}
