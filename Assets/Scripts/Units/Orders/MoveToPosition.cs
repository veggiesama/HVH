using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPosition : Order {
	public void Initialize(GameObject obj, Vector3 targetLocation) {
		base.Initialize(obj, null, targetLocation);
		this.orderType = OrderTypes.MOVE_TO_POSITION;
	}

	public override void Execute()
	{
		if (unit.HasStatusEffect(StatusEffectTypes.AIRBORN) || unit.HasStatusEffect(StatusEffectTypes.IMMOBILIZED)) {
			unit.TurnToFace(targetLocation);
			return;
		}

		if (unit.IsReadyForNav()) {
			unit.agent.destination = targetLocation;
			return;
		}
	}

	public override void Update()
	{
		if (!unit.agent.pathPending && !unit.agent.hasPath) {
			End();
		}
	}

	public override void FixedUpdate() {}

	public override void Suspend()
	{
	}

	public override void End() {
		unit.ForceStop();
		base.End();
	}

}
