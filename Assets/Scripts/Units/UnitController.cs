﻿using Tree = HVH.Tree;
using Outline = cakeslice.Outline;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class UnitController : MonoBehaviour {
	[HideInInspector] public BodyController body;
	[HideInInspector] public NavMeshAgent agent;
	[HideInInspector] public NetworkHelper networkHelper;
	[HideInInspector] public Player player;
	[HideInInspector] public Owner owner;

	private UnitController currentFriendlyTarget, currentEnemyTarget, forgottenEnemyTarget;
	private OrderQueue orderQueue;
	private StatusEffectManager statusEffectManager;
	private AbilityManager abilityManager;
	//private Projector targetProjector;
	private UnitMaterials unitMaterials;
	private FieldOfView fov;

	public UnitInfo unitInfo;
	public AiManager aiManager;

	[HideInInspector] public UnityEventHealth onHealthChanged;
	[HideInInspector] public UnityEventDamage onTakeDamage;
	[HideInInspector] public UnityEventDamage onTakeHealing;
	[HideInInspector] public UnityEvent onMoved;
	[HideInInspector] public UnityEvent onCastAbility;
	[HideInInspector] public UnityEvent onDeath;
	[HideInInspector] public UnityEvent onRespawn;

	/*
	public static explicit operator UnitController(GameObject v)
	{
		throw new NotImplementedException();
	}*/

	private Quaternion wantedRotation;
	private bool orderRestricted = false;

	private WorldProjector projectorMoveTo;
	private WorldProjector projectorAllyTarget;
	private WorldProjector projectorEnemyTarget;

	// Use this for initialization
	void Awake () {
		body = GetComponentInChildren<BodyController>();
		agent = body.GetComponent<NavMeshAgent>();
		owner = GetComponentInParent<Owner>();
		networkHelper = owner.GetComponent<NetworkHelper>();
		abilityManager = GetComponent<AbilityManager>();
		orderQueue = GetComponent<OrderQueue>();
		statusEffectManager = GetComponent<StatusEffectManager>();
		fov = GetComponentInChildren<FieldOfView>(true);
		unitMaterials = GetComponent<UnitMaterials>();
		//targetProjector = transform.Find("Body/FX/Single Targeting projector").GetComponent<Projector>();
		projectorAllyTarget = ReferenceLibrary.Instance.allyTargetProjector.GetComponent<WorldProjector>();
		projectorEnemyTarget = ReferenceLibrary.Instance.enemyTargetProjector.GetComponent<WorldProjector>();
		projectorMoveTo = ReferenceLibrary.Instance.movementProjector.GetComponent<WorldProjector>();


		if (owner is Player) {
			player = (Player) owner;
		}
		//SetTargetPortrait(false, AbilityTargetTeams.ENEMY);
	}

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
		//if (!HasAbilityInSlot(slot)) {
		//	Debug.Log("No ability in slot.");
		//	return;
		//}

		Ability ability = GetAbilityInSlot(slot);

		if (ability.IsPassive()) {
			return;
		}

		if (!ability.IsCooldownReady()) {
			//Debug.Log("Cooldown not ready.");
			return;
		}

		Order currentOrder = orderQueue.GetCurrentOrder();
		if (currentOrder != null && ability == currentOrder.ability) {
			//Debug.Log("Don't interrupt cast when same ability is used again.");
			return;
		}

		if (/*IsPlayerOwned() && */!ability.quickCast && !player.IsMouseTargeting() && ability.targetType != AbilityTargetTypes.UNIT) {
			player.SetMouseTargeting(true, ability, slot);
			return;
		}

		Order castOrder;
		switch (ability.targetType)	{
			case AbilityTargetTypes.AREA:
				castOrder = ScriptableObject.CreateInstance<CastPosition>();
				((CastPosition)castOrder).Initialize(gameObject, ability, owner.GetVirtualPointerLocation());
				break;
			case AbilityTargetTypes.POINT:
				castOrder = ScriptableObject.CreateInstance<CastPosition>();
				((CastPosition)castOrder).Initialize(gameObject, ability, owner.GetVirtualPointerLocation());
				break;
			case AbilityTargetTypes.UNIT:
				castOrder = DoAbilityUnitTarget(slot);
				break;
			case AbilityTargetTypes.TREE:
				Tree tree = player.GetTreeAtMouseLocation(); // TODO: fix for non-players
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

		if (castOrder != null)
			orderQueue.Add(castOrder, ability.doNotCancelOrderQueue);
	}

	private Order DoAbilityUnitTarget(AbilitySlots slot) {
		UnitController target;
		Ability ability = GetAbilityInSlot(slot);
		Order castOrder = null;

		switch (ability.targetTeam) {

			case AbilityTargetTeams.NONE:
				castOrder = ScriptableObject.CreateInstance<CastNoTarget>();
				((CastNoTarget)castOrder).Initialize(gameObject, ability);
				break;

			case AbilityTargetTeams.ALLY:
				if (currentFriendlyTarget != null)
					target = currentFriendlyTarget;
				else if (player.IsMouseTargeting()) {
					target = player.GetUnitAtMouseLocation();
					if (target == null || !this.SharesTeamWith(target))	{ // TODO: Overload GetUnitAtMouseLocation() to filter for teams?
						Debug.Log("No ally at mouse location.");
						return null;
					}
				}
				else {
					player.SetMouseTargeting(true, ability, slot);
					return null;
				}
				castOrder = ScriptableObject.CreateInstance<CastTarget>();
				((CastTarget)castOrder).Initialize(gameObject, ability, target, null);
				break;

			case AbilityTargetTeams.ENEMY:
				if (currentEnemyTarget != null)
					target = currentEnemyTarget;
				else if (player.IsMouseTargeting()) {
					target = player.GetUnitAtMouseLocation();
					if (target == null || this.SharesTeamWith(target)) { // TODO: Overload GetUnitAtMouseLocation() to filter for teams?
						Debug.Log("No enemy at mouse location.");
						return null;
					}
				} 
				else {
					player.SetMouseTargeting(true, ability, slot);
					return null;
				}
				castOrder = ScriptableObject.CreateInstance<CastTarget>();
				((CastTarget)castOrder).Initialize(gameObject, ability, null, target);
				break;

			case AbilityTargetTeams.BOTH:
				//? Not sure what this will look like yet.
				castOrder = ScriptableObject.CreateInstance<CastNoTarget>();
				((CastNoTarget)castOrder).Initialize(gameObject, ability);
				break;
		}
	
		return castOrder;

	}

	public bool IsLocalPlayer() {
		return (IsPlayerOwned() && GameResources.Instance.GetLocalPlayer() == player);
	}

	// targeting

	public void SetCurrentTarget(UnitController target) {
		if (target == null) return;

		bool isLocalPlayer = IsLocalPlayer();

		if (SharesTeamWith(target)) {
			if (currentFriendlyTarget != null) {

				if (isLocalPlayer) {
					UnregisterPortrait(currentFriendlyTarget, UiPortraitSlots.ALLY_TARGET);
				}
			}

			currentFriendlyTarget = target;

			if (isLocalPlayer) {
				RegisterPortrait(target, UiPortraitSlots.ALLY_TARGET);
			}
		}
		else {
			if (currentEnemyTarget != null && isLocalPlayer) {
				UnregisterPortrait(currentEnemyTarget, UiPortraitSlots.ENEMY_TARGET);
			}

			currentEnemyTarget = target;
			
			if (isLocalPlayer) {
				RegisterPortrait(target, UiPortraitSlots.ENEMY_TARGET);
			}
		}
	}

	private void RegisterPortrait(UnitController target, UiPortraitSlots slot) {
		SetPortraitRegistration(target, slot, true);

	}

	private void UnregisterPortrait(UnitController target, UiPortraitSlots slot) {
		SetPortraitRegistration(target, slot, false);
	}

	private void SetPortraitRegistration(UnitController target, UiPortraitSlots slot, bool enabled) {
		if (enabled) {
			GameplayCanvas.Instance.uiPortraits[slot].RegisterPortrait(target);
			ShowTargetStand(target);
		}
		else {
			GameplayCanvas.Instance.uiPortraits[slot].UnregisterPortrait(target);
			HideTargetStand(target);
		}
	}

	public void RemoveCurrentTarget(AbilityTargetTeams targetTeam, bool doNotRememberLater=false) {
		switch (targetTeam)	{
			case AbilityTargetTeams.ALLY:
				if (currentFriendlyTarget == null) return;
				UnregisterPortrait(currentFriendlyTarget, UiPortraitSlots.ALLY_TARGET);
				currentFriendlyTarget = null;
				break;
			case AbilityTargetTeams.ENEMY:
				if (currentEnemyTarget == null) return;
				UnregisterPortrait(currentEnemyTarget, UiPortraitSlots.ENEMY_TARGET);
				currentEnemyTarget = null;

				if (doNotRememberLater) {
					forgottenEnemyTarget = null;
				}

				break;
			default:
				Debug.Log("Get both targets not supported yet");
				return;
		}
	}

	public bool IsForgottenTarget(UnitController target) {
		return (forgottenEnemyTarget == target);
	}

	// units lost to FOW become "forgotten" and stored in temporary memory
	public void ForgetTarget() {
		forgottenEnemyTarget = currentEnemyTarget;
		RemoveCurrentTarget(AbilityTargetTeams.ENEMY);
	}
	 
	// newly revealed units may be "remembered" (automatically targeted) if a new enemy target wasn't chosen since forgetting
	public void RememberTarget() {
		if (forgottenEnemyTarget != null && currentEnemyTarget == null) {
			SetCurrentTarget(forgottenEnemyTarget);
			forgottenEnemyTarget = null;
		}
	}

	private void ShowTargetStand(UnitController target) {
		if (this.SharesTeamWith(target)) {
			projectorAllyTarget.gameObject.transform.SetParent(target.body.transform, false);
			projectorAllyTarget.gameObject.SetActive(true);
		}
		else {
			projectorEnemyTarget.gameObject.transform.SetParent(target.body.transform, false);
			projectorEnemyTarget.gameObject.SetActive(true);
		}

	}

	private void HideTargetStand(UnitController target) {
		if (this.SharesTeamWith(target))
			projectorAllyTarget.gameObject.SetActive(false);
		else
			projectorEnemyTarget.gameObject.SetActive(false);
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
		return owner.GetTeam();
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
		if (HasStatusEffect(StatusEffectTypes.DEAD)) return;

		NavMesh.SamplePosition(GetBodyPosition(), out NavMeshHit hit, 100f, -1);
		body.transform.position = hit.position;
		Debug.DrawLine(GetBodyPosition(), hit.position, Color.cyan, 3.0f);

		agent.Warp(GetBodyPosition());
		agent.updatePosition = true;
		agent.updateRotation = true;

		//networkHelper.SyncTransform();
		if (networkHelper.hasAuthority)
			networkHelper.SyncTeleport();

		if (!agent.isOnNavMesh) {
			Debug.Log("Unit fell off map.");
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

	public bool IsAlive() {
		return !HasStatusEffect(StatusEffectTypes.DEAD);
	}

	public bool HasStatusEffect(StatusEffectTypes statusType) {
		return statusEffectManager.HasStatusEffect(statusType);
	}

	public bool HasStatusEffect(StatusEffect status) {
		return HasStatusEffect(status.statusName);
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

	public void RemoveStatusEffect(StatusEffect status) {
		RemoveStatusEffect(status.statusName);
	}

	public void RemoveAllStatusEffects(bool force = false) {
		statusEffectManager.RemoveAll(force);
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

	public OrderTypes GetCurrentOrderType() {
		Order order = GetCurrentOrder();
		if (order != null)
			return order.orderType;
		else
			return OrderTypes.NONE;
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
		return (IsPlayerOwned() && lockFacingToMouse);
	}
	
	public bool SharesTeamWith(UnitController unit) {
		return (this.GetTeam() == unit.GetTeam());
	}

	public bool SharesUnitInfoWith(UnitController unit) {
		return (this.unitInfo.unitName == unit.unitInfo.unitName);
	}

	public bool SharesUnitInfoWith(UnitInfo unitInfo) {
		return (this.unitInfo.unitName == unitInfo.unitName);
	}

	public void Knockback(Vector3 velocityVector, Ability ability) {
		ApplyStatusEffect(unitInfo.onAirbornStatusEffect, ability);
		body.PerformAirborn(velocityVector);
		Invoke("EnableKnockbackCollider", 0.25f); // prevent early triggering
	}

	public void EnableKnockbackCollider() {
		body.onCollidedTerrain.AddListener(OnKnockbackCollidedTerrain); // subs
		//body.onCollideWithTree.AddListener(OnKnockbackCollideWithTree);
	}

	private void OnKnockbackCollidedTerrain(Collision col) {
		EndKnockback();
	}

	public void EndKnockback() {
		RemoveStatusEffect(unitInfo.onAirbornStatusEffect.statusName);

		body.onCollidedTerrain.RemoveListener(OnKnockbackCollidedTerrain); // unsubs
		//body.onCollideWithTree.RemoveListener(OnKnockbackCollideWithTree);
	}

	public List<StatusEffect> GetStatusEffectList() {
		return statusEffectManager.GetStatusEffectList();
	}

	public void ReloadAbilities() {
		abilityManager.LoadAbilities();

		if (IsPlayerOwned())
			GameplayCanvas.Instance.ResetButtons();
	}

	private void SetHealth(float newHealthValue) {
		networkHelper.currentHealth = newHealthValue;
	}

	public float GetHealthPercentage() {
		return networkHelper.currentHealth / unitInfo.maxHealth;
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
			body.SetColor(unitInfo.bodyColor);

			fov.dayViewRadius = unitInfo.daySightRange;
			fov.nightViewRadius = unitInfo.nightSightRange;
			fov.viewAngle = unitInfo.fovViewAngle;

			if (networkHelper.isServer && unitInfo.hasAI) {
				aiManager = gameObject.AddComponent<AiManager>();
				aiManager.Initialize(unitInfo);
			}

		}

		else
			Debug.Log("Invalid UnitInfo: " + unitInfoName);
	}

	// VISIBILITY
	// NOTE: No network code syncs vision. Purely based on shared transforms.

	public FieldOfView GetFieldOfView() {
		return body.fov;
	}

	public void EnableVision(bool enable) {
		fov.gameObject.SetActive(enable);
		body.AppearInFOV(enable);
	}

	public void SetVision(VisionType vision) {
		fov.SetObstacleMask(vision);
	}

	public void Die(Vector3 killerLocation, float killingDmg) {
		networkHelper.Die(killerLocation, killingDmg);
	}

	public void ServerDie(Vector3 killerLocation, float killingDmg) {
		if (networkHelper.HasControllableAuthority())
			ApplyStatusEffect(unitInfo.onDeathStatusEffect, null);


		DetachFromNav();
		EnableNav(false);
		body.PerformDeath(killerLocation, killingDmg);
		body.EnableClickableHitbox(false);
		onDeath.Invoke();
	}

	public void Respawn() {
		networkHelper.Respawn();
	}

	public void Respawn(Vector3 position, Quaternion rotation) {
		StartCoroutine( DelayedRespawn(position, rotation, 0.1f) );
	}

	public void ServerRespawn(Vector3 position, Quaternion rotation) {
		DoRespawn(position, rotation);
	}

	private IEnumerator DelayedRespawn(Vector3 position, Quaternion rotation, float delay) {
		yield return new WaitForSeconds(delay);
		DoRespawn(position, rotation);
	}

	private void DoRespawn(Vector3 position, Quaternion rotation) {

		body.transform.SetPositionAndRotation(position, rotation);
		body.ResetBody();
		EnableNav(true);
		AttachToNav();
		onRespawn.Invoke();

		if (networkHelper.HasControllableAuthority())
			networkHelper.PlayAnimation(Animations.RESPAWN);
	}

	public void SetHighlighted(HighlightingState state) {
		for (int i = 0; i < body.bodyMeshes.Length; i++) {
			Renderer mesh = body.bodyMeshes[i];
			Outline outline = mesh.GetComponent<Outline>();
			switch (state) {
				case HighlightingState.NONE:
					outline.enabled = false;
					break;
				case HighlightingState.NORMAL:
					outline.enabled = true;
					outline.color = 0;
					break;
				case HighlightingState.INTEREST:
					outline.enabled = true;
					outline.color = 1;
					break;
				case HighlightingState.ENEMY:
					outline.enabled = true;
					outline.color = 2;
					break;
			}
		}
	}

	public void DealDamageTo(UnitController targetUnit, float dmg) {
		networkHelper.DealDamageTo(targetUnit, dmg);
	}

	public void OnHealthChanged(float newPercentage) {
		onHealthChanged.Invoke(newPercentage);
	}

	public void OnTakeDamage(float dmg) {
		onTakeDamage.Invoke(dmg);
	}

	public void OnTakeHealing(float healing) {
		onTakeHealing.Invoke(healing);
	}

	public void SetVisibilityState(VisibilityState state) {
		networkHelper.SetVisibilityState(state);
	}

	public bool IsPlayerOwned() {
		return (player != null);
	}

	private bool enteringWater;

	public void OnEnterWater() {
		if (enteringWater) return;
		enteringWater = true;
		body.PerformSink();
		StartCoroutine( OnEnterWater(1.0f) );
	}

	private IEnumerator OnEnterWater(float delay) {
		yield return new WaitForSeconds(delay);

		AttachToNav();
		//NavMesh.SamplePosition(GetBodyPosition(), out NavMeshHit hit, 100f, -1);
		//body.transform.position = hit.position;
		enteringWater = false;		
	}


	// PROJECTORS

	public void ShowMovementProjector() {
		if (!IsLocalPlayer()) return;
		Vector3 pos = agent.destination;
		pos.y += projectorMoveTo.yOffset;
		projectorMoveTo.transform.position = pos;
		projectorMoveTo.gameObject.SetActive(true);
	}

	public void HideMovementProjector() {
		if (!IsLocalPlayer()) return;
		projectorMoveTo.gameObject.SetActive(false);
	}

	public void ShowAoeProjector() {
		if (!IsLocalPlayer()) return;
		Vector3 pos = agent.destination;
		pos.y += projectorMoveTo.yOffset;
		projectorMoveTo.transform.position = pos;
		projectorMoveTo.gameObject.SetActive(true);
	}

	public void HideAoeProjector() {
		if (!IsLocalPlayer()) return;
		projectorMoveTo.gameObject.SetActive(false);
	}

}
