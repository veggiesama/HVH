using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cast : Order {
	private UIController uiController;
	private float currentTimer = 0;
	private bool failCast = false;
	
	public override void Initialize(GameObject obj, Ability ability, Vector3 targetLocation, UnitController allyTarget, UnitController enemyTarget, Tree tree) {
		base.Initialize(obj, ability, targetLocation, allyTarget, enemyTarget, tree);
		this.orderType = OrderTypes.NONE;
		uiController = unit.GetOwnerController().uiController;
	}

	public override void Execute()
	{
		//CastResults.FAILURE_TARGET_OUT_OF_RANGE;
		if (!ability.IsTargetLocationInRange(targetLocation)) {
			//Debug.Log("Cast failure: target out of range.");
			failCast = true;
			return;
		}

		//CastResults.FAILURE_NOT_FACING_TARGET;
		if (!unit.IsFacing(targetLocation))	//Debug.Log("Facing: " + caster.IsFacing(targetLocation));
		{
			//Debug.Log("Cast failure: not facing target. Adjusting.");
			unit.ForceStop();
			unit.TurnToFace(targetLocation);
			return;
		}
		
		//CastResults.FAILURE_TARGET_OUT_OF_RANGE;
		if ((targetLocation != default && !ability.IsTargetLocationInRange(targetLocation)) ||
		  (allyTarget  != null && !ability.IsTargetLocationInRange( allyTarget.GetBodyPosition())) ||
		  (enemyTarget != null && !ability.IsTargetLocationInRange(enemyTarget.GetBodyPosition())) )
		{
			failCast = true;
			return;
		}

		//CastResults.FAILURE_COOLDOWN_NOT_READY
		if (!ability.IsCooldownReady()) {
			failCast = true;
			return;
		}

		ability.allyTarget = unit.GetTarget(AbilityTargetTeams.ALLY);
		ability.enemyTarget = unit.GetTarget(AbilityTargetTeams.ENEMY);

		if (ability.castTime > 0) {
			unit.ForceStop();
			uiController.EnableCastbar(true);
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
			uiController.EnableCastbar(false);
		}		
	}

	public override void FixedUpdate() {}

	public override void Suspend(OrderTypes suspendedBy) {
		if (ability.castTime > 0) {
			if (suspendedBy == OrderTypes.TURN_TO_FACE) return;
			uiController.EnableCastbar(false);
			ability.StartCooldown();
			failCast = true; // cancelling a spell
		}
	}

	private void FinishCasting() {
		CastResults results = ability.Cast(this);
		if (results == CastResults.SUCCESS) 
			ability.StartCooldown();
		End();
	}

	public override void End() {
		base.End();
	}
}
