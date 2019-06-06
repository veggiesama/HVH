using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cast : Order {
	private float currentTimer = 0;
	protected bool failCast = false;
	
	public override void Initialize(GameObject obj, Ability ability, Vector3 targetLocation, UnitController allyTarget, UnitController enemyTarget, Tree tree) {
		base.Initialize(obj, ability, targetLocation, allyTarget, enemyTarget, tree);
		this.orderType = OrderTypes.NONE;
	}

	public override void Execute() {
		if (ability.isPassive) {
			Debug.Log("Cannot cast a passive spell.");
			failCast = true;
			return;
		}

		//CastResults.FAILURE_COOLDOWN_NOT_READY
		if (!ability.IsCooldownReady()) {
			failCast = true;
			return;
		}

		if (ability.castTime > 0) {
			unit.ForceStop();
			EnableCastbar(true);
			unit.body.anim.SetBool("isShooting", true);
			currentTimer = ability.castTime;
		}
	}

	public override void Update() {
		if (failCast) {
			End();
			return;
		}

		if (currentTimer > 0) {
			currentTimer -= Time.deltaTime;
			UpdateCastbar(currentTimer / ability.castTime);
		}
		else {
			FinishCasting();
		}		
	}

	public override void FixedUpdate() {}

	public override void Suspend(OrderTypes suspendedBy) {
		if (ability.castTime > 0) {
			if (suspendedBy == OrderTypes.TURN_TO_FACE) return;
			unit.body.anim.SetBool("isShooting", false);
			EnableCastbar(false);
			//ability.StartCooldown();
			failCast = true; // cancelling a spell
		}
	}

	private void FinishCasting() {
		EnableCastbar(false);
		unit.body.anim.SetBool("isShooting", false);

		unit.onCastAbility.Invoke();
		CastResults results = ability.Cast(this);
		if (results == CastResults.SUCCESS) 
			ability.StartCooldown();
		End();
	}

	public override void End() {
		base.End();
	}

	// HELPERS
	protected bool IsTargetInRange(Vector3 location) {
		return ability.IsTargetLocationInRange(targetLocation);
	}

	protected bool IsTargetInRange(UnitController target) {
		return ability.IsTargetLocationInRange( target.GetBodyPosition() );
	}

	protected bool IsFacing(Vector3 location) {
		return unit.IsFacing(location);
	}
	
	protected bool IsFacing(UnitController target) {
		return unit.IsFacing(target.GetBodyPosition());
	}

	private void EnableCastbar(bool enable) {
		if (unit.IsPlayerOwned())
			unit.player.uiController.EnableCastbar(enable);
	}

	private void UpdateCastbar(float percentage) {
		if (unit.IsPlayerOwned())
			unit.player.uiController.UpdateCastbar(percentage);
	}

}
