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

	private bool isJumping = false;
	//private bool isImmobile = false;


	// Use this for initialization
	void Start () {
		rb = body.GetComponent<Rigidbody>();
		agent = body.GetComponent<NavMeshAgent>();
		unitInfo = GetComponent<UnitInfo>();
		attackInfo = GetComponent<AttackInfo>();
		cam = GetComponentInChildren<Camera>(true);

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
		return agent != null &&
			agent.isOnNavMesh;
	}

	public void MoveTo(Vector3 destination) {
		if (IsReadyForNav())
			agent.destination = destination;
		else
			Debug.Log("MoveTo called, but Agent not ready for navigation.");
	}
	
	public Vector3 GetDestination() {
		return agent.destination;
	}

	public void DoAbility(AbilitySlots ability) {
		if (ability == AbilitySlots.ABILITY_1) { // shoot
			if (currentEnemyTarget != null) {
				AttackRoutine atk = Instantiate(attackRoutinePrefab) as AttackRoutine;
				atk.transform.parent = this.transform;
				atk.Initialize(this, currentEnemyTarget);
			}
		}
		else if (ability == AbilitySlots.ABILITY_3) { // jump
			StartCoroutine( Jump(1.95f) );
		}
	}

	public IEnumerator Jump(float end) {
		if (isJumping)
			yield break;

		isJumping = true;
		agent.enabled = false;
		rb.isKinematic = false;
		rb.AddForce(Vector3.up * 500.0f);
		rb.AddForce(rb.transform.forward * 300.0f);

		yield return new WaitForSeconds(end);

		isJumping = false;
		agent.enabled = true;
		rb.isKinematic = false;
	}

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
