using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour {
	[HideInInspector] public BodyController body;
	[HideInInspector] public NavMeshAgent agent;
	[HideInInspector] public NetworkHelper networkHelper;
	[HideInInspector] public Player player;

	private Camera faceCam; 
	private UnitController currentFriendlyTarget, currentEnemyTarget;
	private OrderQueue orderQueue;
	private StatusEffectManager statusEffectManager;
	private AbilityManager abilityManager;
	private MeshRenderer targetFriendlyStand, targetEnemyStand;
	private FieldOfView fov;

	public UnitInfo unitInfo;

	/*
	public static explicit operator UnitController(GameObject v)
	{
		throw new NotImplementedException();
	}*/

	private Quaternion wantedRotation;
	private bool orderRestricted = false;

	// Use this for initialization
	void Awake () {
		body = GetComponentInChildren<BodyController>();
		agent = body.GetComponent<NavMeshAgent>();
		faceCam = GetComponentInChildren<Camera>(true);
		player = GetComponentInParent<Player>();
		networkHelper = player.GetComponent<NetworkHelper>();

		abilityManager = GetComponentInChildren<AbilityManager>();
		orderQueue = GetComponentInChildren<OrderQueue>();
		statusEffectManager = GetComponentInChildren<StatusEffectManager>();
		fov = GetComponentInChildren<FieldOfView>(true);
		fov.OnTargetsVisibilityChange += OnTargetsVisibilityChange;

		targetFriendlyStand = transform.Find("Body/FX/Target friendly stand").GetComponent<MeshRenderer>();
		targetEnemyStand = transform.Find("Body/FX/Target enemy stand").GetComponent<MeshRenderer>();

		// sniper = 0.135s to turn 180 degrees, or 1350 degrees/sec
		// NS = 0.188s to turn 180 degrees, or 960 degrees/sec
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

		Order currentOrder = orderQueue.GetCurrentOrder();
		if (currentOrder != null && ability == currentOrder.ability) {
			//Debug.Log("Don't interrupt cast when same ability is used again.");
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
		if (target == null) return;

		if (SharesTeamWith(target)) {
			if (currentFriendlyTarget != null) {
				currentFriendlyTarget.ShowTargetStand(false, AbilityTargetTeams.ALLY);
				currentFriendlyTarget.SetTargetCamera(false, AbilityTargetTeams.ALLY);
			}

			currentFriendlyTarget = target;
			target.ShowTargetStand(true, AbilityTargetTeams.ALLY);
			target.SetTargetCamera(true, AbilityTargetTeams.ALLY);
		}
		else {
			if (currentEnemyTarget != null) {
				currentEnemyTarget.ShowTargetStand(false, AbilityTargetTeams.ENEMY);
				currentEnemyTarget.SetTargetCamera(false, AbilityTargetTeams.ENEMY);
			}

			currentEnemyTarget = target;
			target.ShowTargetStand(true, AbilityTargetTeams.ENEMY);
			target.SetTargetCamera(true, AbilityTargetTeams.ENEMY);
		}
	}

	public void RemoveCurrentTarget(AbilityTargetTeams targetTeam) {
		switch (targetTeam)	{
			case AbilityTargetTeams.ALLY:
				currentFriendlyTarget.ShowTargetStand(false, targetTeam);
				currentFriendlyTarget.SetTargetCamera(false, targetTeam);
				currentFriendlyTarget = null;
				break;
			case AbilityTargetTeams.ENEMY:
				currentEnemyTarget.ShowTargetStand(false, targetTeam);
				currentEnemyTarget.SetTargetCamera(false, targetTeam);
				currentEnemyTarget = null;
				break;
			default:
				Debug.Log("Get both targets not supported yet");
				return;
		}
	}

	private void ShowTargetStand(bool enable, AbilityTargetTeams targetTeam) {
		if (targetTeam == AbilityTargetTeams.ALLY)
			targetFriendlyStand.enabled = enable;
		else
			targetEnemyStand.enabled = enable;
	}
	
	private void SetTargetCamera(bool enable, AbilityTargetTeams targetTeam) {
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

	public void SetSize(int newSize) { // may change to enum
		unitInfo.size = newSize;
	}

	public void SetTurnRate(float newTurnRate) {
		unitInfo.turnRate = newTurnRate;
		agent.angularSpeed = newTurnRate;		
	}

	public void SetSpeed(float newSpeed) {
		unitInfo.movementSpeed = newSpeed;
		agent.speed = newSpeed;
	}

	public void ResetSpeed() {
		unitInfo.movementSpeed = unitInfo.movementSpeedOriginal;
		agent.speed = unitInfo.movementSpeedOriginal;
	}

	public void ResetTurnRate() {
		unitInfo.turnRate = unitInfo.turnRateOriginal;
		agent.speed = unitInfo.turnRateOriginal;
	}

	public void ResetSize() {
		unitInfo.size = unitInfo.sizeOriginal;
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
		//Debug.Log("Adding " + status.statusName + " to unit for " + status.duration + " seconds.");
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
	}

	public void EnableKnockbackCollider() {
		body.OnCollisionEventHandler += OnKnockbackCollision; // event sub
	}
	
	public void OnKnockbackCollision(Collision col) {
		//Debug.Log("Knockbacked unit collided with: " + col.collider.gameObject.name);
		GameObject o = col.collider.gameObject;

		if (Util.IsTree(o)) {
			Tree tree = o.GetComponent<Tree>();	
			networkHelper.DestroyTree(tree);
		}


		else if (Util.IsTerrain(o))
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

	public void ReloadAbilities() {
		abilityManager.LoadAbilities();
	}

	public void SetHealth(float newHealthValue) {
		networkHelper.currentHealth = newHealthValue;
	}

	public void SetUnitInfo(string unitInfoName) {
		if (ResourceLibrary.Instance.unitInfoDictionary.TryGetValue(unitInfoName, out UnitInfo scriptableObject)) {
			unitInfo = Instantiate(scriptableObject);
			unitInfo.Initialize();

			SetHealth(unitInfo.maxHealth);
			SetSpeed(unitInfo.movementSpeed);
			SetTurnRate(unitInfo.turnRate);
			abilityManager.Initialize();
			body.ResetAnimator();
			body.bodyMesh.material.color = unitInfo.bodyColor;
		}

		else
			Debug.Log("Invalid UnitInfo: " + unitInfoName);
	}

	// VISIBILITY
	// NOTE: No network code syncs vision. Purely based on shared transforms.
	public void EnableVision(bool enable) {
		fov.gameObject.SetActive(enable);
		body.SetVisibility(enable);
	}

	public void OnTargetsVisibilityChange(List<Transform> currentVisibleTargets, List<Transform> previousVisibleTargets) {
		if (!GameObject.Find("UI Canvas").GetComponent<UICanvas>().GetLocalPlayer().unit.SharesTeamWith(this)) return;

		// make newly visible bodies visible
		foreach (Transform t in currentVisibleTargets) {
			if (!previousVisibleTargets.Contains(t)) {
				BodyController b = t.gameObject.GetComponent<BodyController>();
				if (b != null && !b.unit.SharesTeamWith(this)) {
					b.SetVisibility(true);
					// TODO: if previous enemy target was lost and no new target was selected, restore previous enemy target
				}
			}
		}

		// make newly hidden bodies invisible
		foreach (Transform t in previousVisibleTargets) {
			if (!currentVisibleTargets.Contains(t)) {
				BodyController b = t.gameObject.GetComponent<BodyController>();
				if (b != null && !b.unit.SharesTeamWith(this)) {
					b.SetVisibility(false);
					if (b.unit == currentEnemyTarget)
						RemoveCurrentTarget(AbilityTargetTeams.ENEMY);
				}
			}
		}
	}

}
