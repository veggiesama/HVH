using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast : Order {
	public override void Initialize(GameObject obj, Ability ability, Vector3 targetLocation, UnitController allyTarget, UnitController enemyTarget, Tree tree) {
		base.Initialize(obj, ability, targetLocation, allyTarget, enemyTarget, tree);
		this.orderType = OrderTypes.NONE;
	}

	public override void Execute()
	{
		//CastResults.FAILURE_TARGET_OUT_OF_RANGE;
		if (!ability.IsTargetLocationInRange(targetLocation)) {
			Debug.Log("Cast failure: target out of range.");
			End();
			return;
		}

		//CastResults.FAILURE_NOT_FACING_TARGET;
		if (!unit.IsFacing(targetLocation))	//Debug.Log("Facing: " + caster.IsFacing(targetLocation));
		{
			Debug.Log("Cast failure: not facing target. Adjusting.");
			unit.ForceStop();
			unit.TurnToFace(targetLocation);
			return;
		}
		
		//CastResults.FAILURE_TARGET_OUT_OF_RANGE;
		if ((targetLocation != default && !ability.IsTargetLocationInRange(targetLocation)) ||
		  (allyTarget  != null && !ability.IsTargetLocationInRange( allyTarget.GetBodyPosition())) ||
		  (enemyTarget != null && !ability.IsTargetLocationInRange(enemyTarget.GetBodyPosition())) )
		{
			End();
			return;
		}

		ability.allyTarget = unit.GetTarget(AbilityTargetTeams.ALLY);
		ability.enemyTarget = unit.GetTarget(AbilityTargetTeams.ENEMY);

		CastResults results = ability.Cast(this);
		if (results == CastResults.SUCCESS)
			ability.StartCooldown();
	}

	public override void Update()
	{
		End();
	}
	public override void FixedUpdate() {}

	public override void Suspend()
	{
	}

	public override void End() {
		base.End();
	}
}
