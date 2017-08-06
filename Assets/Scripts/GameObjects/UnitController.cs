using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour {

	public BodyController body;
	public MeshRenderer targetFriendlyStand, targetEnemyStand;
	private UnitController currentFriendlyTarget, currentEnemyTarget;

	private NavMeshAgent agent;
	private Rigidbody rb;
	[HideInInspector] public UnitInfo unitInfo;
	[HideInInspector] public AttackInfo attackInfo;
	private Camera cam; 

	public List<AbilityController> abilityPrefabList;
	public Dictionary<AbilitySlots, AbilityController> abilities;

	private bool isJumping = false;
	private bool isImmobile = false;
	private bool isDying = false;

	// takes a public list of AbilityControllers and stitches them to enumerated AbilitySlots (ABILITY_1, etc.)
	private Dictionary<AbilitySlots, AbilityController> BindAbilitiesToAbilitySlots(List<AbilityController> abilityPrefabList) {

		Dictionary<AbilitySlots, AbilityController> abilityDictionary =
			new Dictionary<AbilitySlots, AbilityController>();
		
		Transform abilityFolder = this.transform.Find("Abilities");
		int n = 0;
		foreach (AbilityController ability in abilityPrefabList) {
			if (ability != null) {
				var abilityInstance = Instantiate(ability, abilityFolder);
				abilityDictionary.Add((AbilitySlots)n, abilityInstance);
			}
			else {}
			//	abilityDictionary.Add((AbilitySlots)n,  ) ;
			n++;
		}

		return abilityDictionary;
	}

	// Use this for initialization
	void Awake () {
		rb = body.GetComponent<Rigidbody>();
		agent = body.GetComponent<NavMeshAgent>();
		unitInfo = GetComponent<UnitInfo>();
		attackInfo = GetComponent<AttackInfo>();
		cam = GetComponentInChildren<Camera>(true);

		abilities = BindAbilitiesToAbilitySlots(abilityPrefabList);

		// sniper = 0.135s to turn 180 degrees, or 1350 degrees/sec
		// NS = 0.188s to turn 180 degrees, or 960 degrees/sec
		agent.speed = unitInfo.movementSpeed;
		agent.angularSpeed = unitInfo.turnRate;
		SetTargetCamera(false, AbilityTargetTeams.ENEMY);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool IsReadyForNav() {
		return !isJumping && !isImmobile && !isDying &&
			agent != null &&
			agent.isOnNavMesh;
	}

	public void MoveTo(Vector3 destination) {
		if (IsReadyForNav())
			agent.destination = destination;
		//else
			//Debug.Log("MoveTo called, but Agent not ready for navigation.");
	}
	
	public Vector3 GetDestination() {
		return agent.destination;
	}

	public void DoAbility(AbilitySlots ability) {
		if (this.abilities.ContainsKey(ability)) {
			this.abilities[ability].Cast();
		}
		else
			print("No ability in slot.");
	}

	// jumping
	public void StartJump() {
		isJumping = true;
		agent.enabled = false;
		rb.isKinematic = false;
		//rb.useGravity = false;

		Physics.IgnoreCollision(
			this.body.GetComponent<Collider>(),
			Terrain.activeTerrain.GetComponent<Collider>(),
			true);
	}

	public void EndJump() {
		isJumping = false;
		agent.enabled = true;
		rb.isKinematic = true;
		//rb.useGravity = true;

		Physics.IgnoreCollision(
			this.body.GetComponent<Collider>(),
			Terrain.activeTerrain.GetComponent<Collider>(),
			false);
	}

	// targeting
	public void SetCurrentTarget(UnitController target, AbilityTargetTeams targetTeam) {
		if (targetTeam == AbilityTargetTeams.ALLY) {
			if (currentFriendlyTarget != null)
				currentFriendlyTarget.ShowTargetStand(false, AbilityTargetTeams.ALLY);

			currentFriendlyTarget = target;
			target.ShowTargetStand(true, AbilityTargetTeams.ALLY);

		}

		else {
			if (currentEnemyTarget != null)
				currentEnemyTarget.ShowTargetStand(false, AbilityTargetTeams.ENEMY);

			currentEnemyTarget = target;

			if (target != null) {
				target.ShowTargetStand(true, AbilityTargetTeams.ENEMY);
			}
		}
	}

	private void ShowTargetStand(bool enable, AbilityTargetTeams targetTeam) {
		if (targetTeam == AbilityTargetTeams.ALLY) {
			targetFriendlyStand.enabled = enable;
			SetTargetCamera(enable, AbilityTargetTeams.ALLY);
		}
		else {
			targetEnemyStand.enabled = enable;
			SetTargetCamera(enable, AbilityTargetTeams.ENEMY);
		}
	}
	
	private void SetTargetCamera(bool enable, AbilityTargetTeams targetTeam) {
		cam.gameObject.SetActive(enable);

		if (enable) {
			if (targetTeam == AbilityTargetTeams.ALLY)
				cam.rect = CameraViewports.GetAllyViewport();
			else
				cam.rect = CameraViewports.GetEnemyViewport();
		}
	}

	public UnitController GetTarget(AbilityTargetTeams targetTeam) {
		switch (targetTeam)
		{
			case AbilityTargetTeams.ALLY:
				return currentFriendlyTarget;
			case AbilityTargetTeams.ENEMY:
				return currentEnemyTarget;
			default:
				return null;
		}
	}

	// damage
	public void ReceivesDamage(float damage, UnitController attacker) {
		this.unitInfo.currentHealth -= damage;
		
		if (this.unitInfo.currentHealth < 0) {
			StartCoroutine ( Die(attacker) );
		}
	}

	public IEnumerator Die(UnitController killer) {
		if (isDying)
			yield break;
		
		//print("IsDying!");
		isDying = true;
		agent.enabled = false;
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.None;
		rb.AddForce(Vector3.up * Random.Range(25.0f, 100.0f));
		rb.AddForce((this.GetBodyPosition() - killer.GetBodyPosition()) * 50.0f);

		yield return new WaitForSeconds(3.0f);

		Transform spawnLoc = GameController.GetRandomSpawnPoint();
		this.body.transform.SetPositionAndRotation(spawnLoc.position, spawnLoc.rotation);
		this.unitInfo.currentHealth = this.unitInfo.maxHealth;

		rb.constraints = RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY |
			RigidbodyConstraints.FreezeRotationZ;
		rb.isKinematic = true;
		agent.enabled = true;
		isDying = false;
	}

	// Misc.

	public Teams GetTeam() {
		return GetOwnerController().GetTeam();
	}

	public OwnerController GetOwnerController() {
		return transform.parent.GetComponent<OwnerController>();
	}

	public Vector3 GetBodyPosition() {
		return body.transform.position;
	}

	public void SetImmobile(float seconds) {
		isImmobile = true;
		agent.velocity = Vector3.zero;
		agent.destination = agent.transform.position;
		Invoke("DisableImmobile", seconds);
	}

	public void DisableImmobile() {
		isImmobile = false;
	}
}
