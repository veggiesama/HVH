using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPosition : Order {
	public override void Initialize(GameObject obj, Ability ability, Vector3 targetLocation) {
		base.Initialize(obj, ability, targetLocation);
		this.orderType = OrderTypes.CAST_POSITION;
	}

	public override void Execute()
	{
		//CastResults.FAILURE_COOLDOWN_NOT_READY;
		if (!ability.IsCooldownReady()) {
			End();
			return;
		}

		//CastResults.FAILURE_TARGET_OUT_OF_RANGE;
		if (!ability.IsTargetLocationInRange(targetLocation)) {
			End();
			return;
		}

		//CastResults.FAILURE_NOT_FACING_TARGET;
		if (!unit.IsFacing(targetLocation))	//Debug.Log("Facing: " + caster.IsFacing(targetLocation));
		{
			unit.ForceStop();
			unit.TurnToFace(targetLocation);
			return;
		}
		
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
