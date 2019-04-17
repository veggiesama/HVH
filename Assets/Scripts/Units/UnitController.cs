using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Mirror;

public class UnitController : NetworkBehaviour {
	[HideInInspector] public BodyController body;
	[HideInInspector] public NavMeshAgent agent;
	[HideInInspector] public UnitInfo unitInfo;
	[HideInInspector] public AttackInfo attackInfo;
	[HideInInspector] public NetworkHelper networkHelper;
	[HideInInspector] public Player player;

	private Camera faceCam; 
	private UnitController currentFriendlyTarget, currentEnemyTarget;
	private OrderQueue orderQueue;
	private StatusEffectManager statusEffectManager;
	private AbilityManager abilityManager;
	private MeshRenderer targetFriendlyStand, targetEnemyStand;

	[Header("Abilities")]
	public Ability startingAttackAbility;
	public List<Ability> startingAbilitiesList;
	public List<Ability> startingItemsList;


	public static explicit operator UnitController(GameObject v)
	{
		throw new NotImplementedException();
	}

	private Quaternion wantedRotation;
	private bool orderRestricted = false;

	// Use this for initialization
	void Awake () {
		body = GetComponentInChildren<BodyController>();
		agent = body.GetComponent<NavMeshAgent>();
		unitInfo = GetComponent<UnitInfo>();
		attackInfo = GetComponent<AttackInfo>();
		faceCam = GetComponentInChildren<Camera>(true);
		player = GetComponentInParent<Player>();
		networkHelper = player.GetComponent<NetworkHelper>();

		abilityManager = GetComponentInChildren<AbilityManager>();
		orderQueue = GetComponentInChildren<OrderQueue>();
		statusEffectManager = GetComponentInChildren<StatusEffectManager>();

		targetFriendlyStand = transform.Find("Body/FX/Target friendly stand").GetComponent<MeshRenderer>();
		targetEnemyStand = transform.Find("Body/FX/Target enemy stand").GetComponent<MeshRenderer>();

		// sniper = 0.135s to turn 180 degrees, or 1350 degrees/sec
		// NS = 0.188s to turn 180 degrees, or 960 degrees/sec

		SetSpeed(unitInfo.movementSpeed);
		agent.angularSpeed = unitInfo.turnRate;
		SetTargetCamera(false, AbilityTargetTeams.ENEMY);
	}

	// UPDATE
	//private void Update () {}
	//private void FixedUpdate() {}

	// ORDERS

	public void MoveTo(Vector3 destination) {
		if (IsOrderRestricted()) return;
		MoveToPosition moveOrder = ScriptableObject.CreateInstance<MoveToPosition>();
		moveOrder.Initialize(gameObject, destination);
		orderQueue.Add(moveOrder);
	}

	public void Stop() {
		if (IsOrderRestricted()) return;
		Stop stopOrder = ScriptableObject.CreateInstance<Stop>();
		stopOrder.Initialize(gameObject); // not sure
		orderQueue.Add(stopOrder);
	}

	public void ForceStop() {
		if (IsReadyForNav())
			agent.ResetPath();
		else {}
			//Debug.Log("Can't force stop unit. Not ready for nav.");
	}

	public void DoAbility(AbilitySlots slot) {
		if (IsOrderRestricted()) {
			//Debug.Log("Order restricted.");
			return;
		}
		if (!HasAbilityInSlot(slot)) {
			Debug.Log("No ability in slot.");
			return;
		}

		Ability ability = GetAbilityInSlot(slot);
		Player player = GetPlayer();

		if (!ability.IsCooldownReady()) {
			//Debug.Log("Cooldown not ready.");
			return;
		}

		if (!ability.quickCast && !player.IsMouseTargeting()) {
			player.SetMouseTargeting(true, ability, slot);
			return;
		}
		if (player.IsMouseTargeting())
			player.SetMouseTargeting(false);

		Order castOrder;
		switch (ability.targetType)
		{
			case AbilityTargetTypes.AREA:
				castOrder = ScriptableObject.CreateInstance<CastPosition>();
				((CastPosition)castOrder).Initialize(gameObject, ability, player.GetMouseLocationToGround());
				break;
			case AbilityTargetTypes.POINT:
				castOrder = ScriptableObject.CreateInstance<CastPosition>();
				((CastPosition)castOrder).Initialize(gameObject, ability, player.GetMouseLocationToGround());
				break;
			case AbilityTargetTypes.UNIT:
				castOrder = ScriptableObject.CreateInstance<CastTarget>();
				((CastTarget)castOrder).Initialize(gameObject, ability, currentFriendlyTarget, currentEnemyTarget);
				break;
			case AbilityTargetTypes.TREE:
				Tree tree = player.GetTreeAtMouseLocation();
				if (tree == null) {
					Debug.Log("Invalid target (not a tree).");
					return;
				}
				castOrder = ScriptableObject.CreateInstance<CastTree>();
				((CastTree)castOrder).Initialize(gameObject, ability, tree);
				break;
			default:
				castOrder = ScriptableObject.CreateInstance<CastNoTarget>();
				((CastNoTarget)castOrder).Initialize(gameObject, ability);
				break;
		}
		 
		orderQueue.Add(castOrder, ability.doNotCancelOrderQueue);
	}

	// targeting
	public void SetCurrentTarget(UnitController target) {
		if (target != null && SharesTeamWith(target)) {
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
		//faceCam.gameObject.SetActive(enable);
		faceCam.enabled = enable;

		if (enable) {
			if (targetTeam == AbilityTargetTeams.ALLY)
				faceCam.rect = CameraViewports.GetAllyViewport();
			else
				faceCam.rect = CameraViewports.GetEnemyViewport();
		}
	}

	public UnitController GetTarget(AbilityTargetTeams targetTeam) {
		switch (targetTeam)	{
			case AbilityTargetTeams.ALLY:
				return currentFriendlyTarget;
			case AbilityTargetTeams.ENEMY:
				return currentEnemyTarget;
			default:
				Debug.Log("Get both targets not supported yet");
				return null;
		}
	}

	/*
	// damage
	public void ReceivesDamage(float damage, UnitController attacker) {

		if (!HasStatusEffect(StatusEffectTypes.INVULNERABLE))
			networkHelper.currentHealth -= damage;
		
		if (networkHelper.currentHealth < 0) {
			ApplyStatusEffect(unitInfo.onDeathStatusEffect, null, null);
		}
	}*/

	// Misc.

	public Teams GetTeam() {
		return GetPlayer().GetTeam();
	}

	public Player GetPlayer() {
		return player;
		//return transform.parent.GetComponent<Player>();
	}

	public Vector3 GetBodyPosition() {
		return body.transform.position;
	}

	public Vector3 GetBodyForward() {
		return body.transform.forward;
	}

	public bool IsFacing(Vector3 targetPosition) {
		return body.IsFacing(targetPosition);
	}

	public void TurnToFace(Vector3 targetPosition) {
		TurnToFace turnOrder = ScriptableObject.CreateInstance<TurnToFace>();
		turnOrder.Initialize(gameObject, targetPosition);
		orderQueue.Add(turnOrder, true);
	}

	public void SetSpeed(float newSpeed) {
		unitInfo.movementSpeed = newSpeed;
		agent.speed = newSpeed;
	}

	public void ResetSpeed() {
		unitInfo.movementSpeed = unitInfo.movementSpeedOriginal;
		agent.speed = unitInfo.movementSpeedOriginal;
	}

	public void SetOrderRestricted(bool cmd) {
		orderRestricted = cmd;
	}

	public bool IsOrderRestricted() {
		return orderRestricted;
	}

	public void EnableNav(bool enable) {
		agent.enabled = enable;
	}

	public void DetachFromNav() {
		agent.updatePosition = false;
		agent.updateRotation = false;
	}

	public void AttachToNav() {
		agent.Warp(GetBodyPosition());
		agent.updatePosition = true;
		agent.updateRotation = true;

		if (!agent.isOnNavMesh) {
		//	agent.FindClosestEdge(out NavMeshHit hit);
		//	Debug.DrawLine(hit.position, Vector3.up, Color.cyan, 3.0f, false);

			ApplyStatusEffect(unitInfo.onDeathStatusEffect);
		}
	}

	public bool IsReadyForNav() {
		return !HasStatusEffect(StatusEffectTypes.AIRBORN) &&
			!HasStatusEffect(StatusEffectTypes.DEAD) &&
			agent != null &&
			agent.isOnNavMesh;
	}

	public Vector3 GetDestination() {
		return agent.destination;
	}

	public bool IsAirborn() {
		return HasStatusEffect(StatusEffectTypes.AIRBORN);
	}

	public bool HasStatusEffect(StatusEffectTypes status) {
		return statusEffectManager.HasStatusEffect(status);
	}

	public bool HasStatusEffect(string name) {
		return statusEffectManager.HasStatusEffect(name);
	}

	public void ApplyStatusEffect(StatusEffect status) {
		ApplyStatusEffect(status, null);
	}

	public void ApplyStatusEffect(StatusEffect status, Ability ability) {
		StatusEffect cloneStatus = Instantiate(status);
		cloneStatus.Initialize(this.gameObject, ability);
		statusEffectManager.Add(cloneStatus);
		Debug.Log("Adding " + status.statusName + " to unit for " + status.duration + " seconds.");
	}

	public void RemoveStatusEffect(StatusEffectTypes statusType) {
		statusEffectManager.Remove(statusType);
	}

	public void RemoveStatusEffect(string statusName) {
		statusEffectManager.Remove(statusName);
	}

	public float GetStatusEffectDuration(StatusEffectTypes statusType) {
		return statusEffectManager.GetStatusEffect(statusType).duration;
	}
	
	public void CancelAllOrders() {
		orderQueue.CancelAllOrders();
	}

	public Order GetCurrentOrder() {
		return orderQueue.GetCurrentOrder();
	}
	
	public bool HasAbilityInSlot(AbilitySlots slot) {
		if (abilityManager.HasAbilityInSlot(slot) && !abilityManager.GetAbilityInSlot(slot).IsEmptyAbility())
			return true;
		else
			return false;
	}

	public Ability GetAbilityInSlot(AbilitySlots slot) {
		return abilityManager.GetAbilityInSlot(slot);
	}

	public AbilitySlots GetAbilitySlot(Ability ability) {
		return abilityManager.GetAbilitySlot(ability);
	}

	private bool lockFacingToMouse = false; // TODO: cleanup and separate helper functions

	public void SetMouseLook(bool enable) {
		lockFacingToMouse = enable;
	}

	public bool IsMouseLooking() {
		return lockFacingToMouse;
	}
	
	public bool SharesTeamWith(UnitController unit) {
		return (this.GetTeam() == unit.GetTeam());
	}

	public void Knockback(Vector3 velocityVector, Ability ability) {
		ApplyStatusEffect(unitInfo.onAirbornStatusEffect, ability);
		body.PerformAirborn(velocityVector);
		Invoke("EnableKnockbackCollider", 0.25f); // prevent early triggering
		//EnableNav(false);
	}

	public void EnableKnockbackCollider() {
		body.OnCollisionEventHandler += OnKnockbackCollision; // event sub
	}
	
	public void OnKnockbackCollision(Collision col) {
		Debug.Log("Knockbacked unit collided with: " + col.collider.gameObject.name);

		GameObject o = col.collider.gameObject;

		if (o.layer == (int) LayerBits.TREE) {
			Tree tree = o.GetComponent<Tree>();	
			networkHelper.DestroyTree(tree);
		}


		else if (o.layer == (int) LayerBits.TERRAIN)
			EndKnockback();
	}

	public void EndKnockback() {
		RemoveStatusEffect(unitInfo.onAirbornStatusEffect.statusName);
		body.OnCollisionEventHandler -= OnKnockbackCollision; // event unsub
		//EnableNav(true);
	}

	public List<StatusEffect> GetStatusEffectList() {
		return statusEffectManager.GetStatusEffectList();
	}

}
