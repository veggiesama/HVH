using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cast : Order {
	private UIController uiController;
	private float currentTimer = 0;
	protected bool failCast = false;
	
	public override void Initialize(GameObject obj, Ability ability, Vector3 targetLocation, UnitController allyTarget, UnitController enemyTarget, Tree tree) {
		base.Initialize(obj, ability, targetLocation, allyTarget, enemyTarget, tree);
		this.orderType = OrderTypes.NONE;
		uiController = unit.GetPlayer().uiController;
	}

	public override void Execute() {
		//CastResults.FAILURE_COOLDOWN_NOT_READY
		if (!ability.IsCooldownReady()) {
			failCast = true;
			return;
		}

		if (ability.castTime > 0) {
			unit.ForceStop();
			uiController.EnableCastbar(true);
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
			uiController.UpdateCastbar(currentTimer / ability.castTime);
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
			uiController.EnableCastbar(false);
			//ability.StartCooldown();
			failCast = true; // cancelling a spell
		}
	}

	private void FinishCasting() {
		uiController.EnableCastbar(false);
		unit.body.anim.SetBool("isShooting", false);

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

}
