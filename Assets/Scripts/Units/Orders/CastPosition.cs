using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPosition : Cast {
	public void Initialize(GameObject obj, Ability ability, Vector3 targetLocation) {
		base.Initialize(obj, ability, targetLocation, null, null, null);
		this.orderType = OrderTypes.CAST_POSITION;
	}

	public override void Execute() {
		//CastResults.FAILURE_NOT_FACING_TARGET;
		if (!IsFacing(targetLocation)) { //Debug.Log("Facing: " + caster.IsFacing(targetLocation));
			//Debug.Log("Cast failure: not facing target. Adjusting.");
			unit.ForceStop();
			unit.TurnToFace(targetLocation);
			return;
		}
		
		//CastResults.FAILURE_TARGET_OUT_OF_RANGE;
		if (!IsTargetInRange(targetLocation)) {
			Debug.Log("Cast failure: Target location out of range.");
			failCast = true;
			return;
		}

		base.Execute();
	}
}
