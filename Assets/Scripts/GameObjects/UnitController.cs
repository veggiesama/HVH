using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour {

	public BodyController body;
	public MeshRenderer targetFriendlyStand, targetEnemyStand;
	public AttackRoutine attackRoutinePrefab;
	private UnitController currentFriendlyTarget, currentEnemyTarget;

	private NavMeshAgent agent;
	private Rigidbody rb;
	[HideInInspector] public UnitInfo unitInfo;
	[HideInInspector] public AttackInfo attackInfo;
	private Camera cam; 

	public List<AbilityController> abilityControllerList;
	private Dictionary<AbilitySlots, AbilityController> abilities;

	private bool isJumping = false;
	//private bool isImmobile = false;
	private bool isDying = false;

	// Use this for initialization
	void Start () {
		rb = body.GetComponent<Rigidbody>();
		agent = body.GetComponent<NavMeshAgent>();
		unitInfo = GetComponent<UnitInfo>();
		attackInfo = GetComponent<AttackInfo>();
		cam = GetComponentInChildren<Camera>(true);
		abilities = Util.DictionaryBindAbilitySlotsToAbilityControllers(abilityControllerList);

		// sniper = 0.135s to turn 180 degrees, or 1350 degrees/sec
		// NS = 0.188s to turn 180 degrees, or 960 degrees/sec
		agent.speed = unitInfo.movementSpeed;
		agent.angularSpeed = unitInfo.turnRate;
		SetTargetCamera(false, false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool IsReadyForNav() {
		return !isJumping &&
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

		if (this.abilities.ContainsKey(ability))
			this.abilities[ability].Cast();
		else
			print("No ability in slot.");

		/*
		if (ability == AbilitySlots.ABILITY_1) { // shoot
			if (currentEnemyTarget != null) {
				AttackRoutine atk = (AttackRoutine) Instantiate(attackRoutinePrefab);
				atk.transform.parent = this.transform;
				atk.Initialize(this, currentEnemyTarget);
			}
		}
		else if (ability == AbilitySlots.ABILITY_3) { // jump
			StartCoroutine( Jump(1.95f) );
		}*/
	}

	// jumping

	public void StartJump(float forceUpwards, float forceForwards) {
		isJumping = true;
		agent.enabled = false;
		rb.isKinematic = false;
		rb.AddForce(Vector3.up * 500.0f);
		rb.AddForce(rb.transform.forward * 300.0f);
	}

	public void EndJump() {
		isJumping = false;
		agent.enabled = true;
		rb.isKinematic = false;
	}

	// targeting

	public void SetCurrentTarget(UnitController target, bool friendly) {
		if (friendly) {
			if (currentFriendlyTarget != null)
				currentFriendlyTarget.ShowTargetStand(false, true);

			currentFriendlyTarget = target;
			target.ShowTargetStand(true, true);

		}

		else {
			if (currentEnemyTarget != null)
				currentEnemyTarget.ShowTargetStand(false, false);

			currentEnemyTarget = target;

			if (target != null) {
				target.ShowTargetStand(true, false);
			}
		}
	}

	private void ShowTargetStand(bool enable, bool friendly) {
		if (friendly) {
			targetFriendlyStand.enabled = enable;
			SetTargetCamera(enable, true);
		}
		else {
			targetEnemyStand.enabled = enable;
			SetTargetCamera(enable, false);
		}
	}
	
	private void SetTargetCamera(bool enable, bool friendly) {
		cam.gameObject.SetActive(enable);

		if (enable) {
			if (friendly)
				cam.rect = CameraViewports.GetAllyViewport();
			else
				cam.rect = CameraViewports.GetEnemyViewport();
		}
	}

	public UnitController GetTarget(AbilityTargetTeams targetTeam) {
		if (targetTeam == AbilityTargetTeams.ALLY)
			return currentFriendlyTarget;
		else if (targetTeam == AbilityTargetTeams.ENEMY)
			return currentEnemyTarget;
		else
			return null;
	}

	public void ReceivesDamage(int damage, UnitController target) {
		this.unitInfo.currentHealth -= damage;
		
		if (this.unitInfo.currentHealth < 0) {
			StartCoroutine ( Die(target) );
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

	/*
	public void SetImmobile(float seconds) {
		isImmobile = true;
		agent.velocity = Vector3.zero;
		agent.destination = agent.transform.position;
		StartCoroutine( DisableImmobile(seconds) );
	}

	private IEnumerator DisableImmobile(float seconds) {
		yield return new WaitForSeconds(seconds);
		isImmobile = false;
	}
	*/

}
