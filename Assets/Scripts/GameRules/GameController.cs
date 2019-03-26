﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject ownerPrefab;
	public GameObject sceneViewMask;
	public Teams startingTeamForPlayer;

	public OwnerController player;

	public Dictionary<Allies, UnitController> allies;
	public Dictionary<Enemies, UnitController> enemies;

	public UnitController GetAlly(Allies position) {
		return this.allies[position];
	}

	public UnitController GetEnemy(Enemies position) {
		return this.enemies[position];
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

		if (Constants.SpawnNPCs) SpawnNPCs();
	}
	
	private void SpawnNPCs() {
		allies = new Dictionary<Allies, UnitController>();
		enemies = new Dictionary<Enemies, UnitController>();

		allies.Add(Allies.ALLY_1, player.unit);
		foreach (Allies ally in System.Enum.GetValues(typeof(Allies)))  {
			// skip the first iteration; that's the player's healthbar
			if (ally == Allies.ALLY_1)
				continue;

			Transform spawnLoc = GetRandomSpawnPoint();
			OwnerController owner = Instantiate(ownerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<OwnerController>();
			allies.Add(ally, owner.unit);

			owner.MakeNPC();
			owner.SetTeam(Teams.GOODGUYS);
			owner.unit.body.GetComponent<Renderer>().material.color =
				Color.Lerp(Color.blue, Color.cyan, Random.Range(0.2f, 1.0f));
		}

		foreach (Enemies enemy in System.Enum.GetValues(typeof(Enemies)))  {
			Transform spawnLoc = GetRandomSpawnPoint();
			OwnerController owner = Instantiate(ownerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<OwnerController>();
			enemies.Add(enemy, owner.unit);

			owner.MakeNPC();
			owner.SetTeam(Teams.BADGUYS);
			owner.unit.body.GetComponent<Renderer>().material.color =
				Color.Lerp(Color.red, Color.magenta, Random.Range(0.2f, 1.0f));;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	public static Transform GetRandomSpawnPoint() {
		GameObject[] allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		int rng = Random.Range(0, allSpawnPoints.Length);

		return allSpawnPoints[rng].transform;
	}

	// TODO: update when MP becomes a thing
	public static OwnerController GetLocalOwner() {
		return GameObject.FindGameObjectWithTag("Player").GetComponent<OwnerController>();
	}
}
