using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject ownerPrefab;
	public Teams startingTeamForPlayer;

	public int numberOfAllies;
	public int numberOfEnemies;
	
	private GameObject player;

	private Dictionary<Allies, UnitController> allies;
	private Dictionary<Enemies, UnitController> enemies;

	//private List<GameObject> allies;
	//private GameObject[] enemies;

	// Use this for initialization
	void Start () {
		allies = new Dictionary<Allies, UnitController>();
		enemies = new Dictionary<Enemies, UnitController>();

		foreach (Allies ally in System.Enum.GetValues(typeof(Allies)))  {
			if (allies != null && allies.Count >= numberOfAllies)
				break;

			Transform spawnLoc = GetRandomSpawnPoint();
			OwnerController owner = Instantiate(ownerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<OwnerController>();
			allies.Add(ally, owner.unit);

			owner.MakeNPC();
			owner.SetTeam(Teams.GOODGUYS);
			owner.unit.body.GetComponent<Renderer>().material.color = Color.Lerp(Color.black, Color.cyan, Random.Range(0.2f, 1.0f));
		}

		foreach (Enemies enemy in System.Enum.GetValues(typeof(Enemies)))  {
			if (enemies != null && enemies.Count >= numberOfEnemies)
				break;

			Transform spawnLoc = GetRandomSpawnPoint();
			OwnerController owner = Instantiate(ownerPrefab, spawnLoc.position, spawnLoc.rotation).GetComponent<OwnerController>();
			enemies.Add(enemy, owner.unit);

			owner.MakeNPC();
			owner.SetTeam(Teams.BADGUYS);
			owner.unit.body.GetComponent<Renderer>().material.color = Color.Lerp(Color.black, Color.magenta, Random.Range(0.2f, 1.0f));;
		}
		/*
		for (int i = 1; i <= numberOfAllies; i++) {
			Transform spawnLoc = GetRandomSpawnPoint();
			allies.Add()
			allies.Add( Instantiate(ownerPrefab, spawnLoc.position, spawnLoc.rotation) );
			OwnerController owner = allies[i].GetComponent<OwnerController>();
			owner.MakeNPC();
		}*/
		/*
		for (int i = 1; i <= numberOfEnemies; i++) {
			Transform spawnLoc = GetRandomSpawnPoint();
			enemies[i] = (GameObject) Instantiate(ownerPrefab, spawnLoc.position, spawnLoc.rotation);

		}*/


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private Transform GetRandomSpawnPoint() {
		GameObject[] allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		int rng = Random.Range(0, allSpawnPoints.Length);

		return allSpawnPoints[rng].transform;
	}
}
