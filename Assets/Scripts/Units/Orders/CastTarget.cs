using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastTarget : Cast {
	public void Initialize(GameObject obj, Ability ability, UnitController allyTarget, UnitController enemyTarget) {
		base.Initialize(obj, ability, default, allyTarget, enemyTarget, null);
		this.orderType = OrderTypes.CAST_TARGET;
	}

	public override void Execute() {

		switch (ability.targetTeam)
		{
			case AbilityTargetTeams.ALLY:
				if (allyTarget == null) {
					Debug.Log("Cast failure: No ally target.");
					failCast = true;
					return;
				}
				else if (!IsTargetInRange(allyTarget)) {
					Debug.Log("Cast failure: Ally target out of range.");
					failCast = true;
					return;
				}
				else if (!IsFacing(allyTarget)) {
					//Debug.Log("Cast failure: not facing target. Adjusting.");
					unit.ForceStop();
					unit.TurnToFace(allyTarget.GetBodyPosition());
					return;
				}
				break;

			case AbilityTargetTeams.ENEMY:
				if (enemyTarget == null) {
					Debug.Log("Cast failure: No enemy target.");
					failCast = true;
					return;
				}
				else if (!IsTargetInRange(enemyTarget)) {
					Debug.Log("Cast failure: Enemy target out of range.");
					failCast = true;
					return;
				}
				else if (!IsFacing(enemyTarget)) {
					//Debug.Log("Cast failure: not facing target. Adjusting.");
					unit.ForceStop();
					unit.TurnToFace(enemyTarget.GetBodyPosition());
					return;
				}
				break;

			case AbilityTargetTeams.BOTH:
				if (allyTarget == null || enemyTarget == null) {
					Debug.Log("Cast failure: Missing target (requires both ally and enemy).");
					failCast = true;
					return;
				}
				else if (!IsTargetInRange(allyTarget) || !IsTargetInRange(enemyTarget)) {
					Debug.Log("Cast failure: Ally and/or enemy target out of range.");
					failCast = true;
					return;
				}
				// doesn't make sense to turn to face
				break;

			default:
				break;
		}

		//ability.allyTarget = unit.GetTarget(AbilityTargetTeams.ALLY);
		//ability.enemyTarget = unit.GetTarget(AbilityTargetTeams.ENEMY);

		base.Execute();
	}
}
