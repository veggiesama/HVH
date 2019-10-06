using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "AI States/PursueNearestVisibleEnemy")]
public class PursueNearestVisibleEnemy : MoveTo {

	[Header("Pursue")]
	public float maxPursueDistance;
	private UnitController currentTarget;

	public override void Reset() {
		base.Reset();

		desire = (int) Desire.HIGH;
		maxPursueDistance = 10f;
		stoppingDistance = 1.0f;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);
		//forceRepeatMoveOrder = true;
	}

	public override void Evaluate() {
		destination = GetClosestTargetPosition();

		if (HasDestination() && !ReachedDestination() ) {
			desire = desireDefault;

			if (currentTarget != null && currentTarget.body.IsVisibleToUnit(this.unit))
				unit.SetCurrentTarget(currentTarget);

		}
		else
			desire = (int) Desire.NONE;
	}

	private Vector3 GetClosestTargetPosition() {
		Vector3 pos = unit.GetBodyPosition();
		Collider[] bodiesInPursuitRadius = Physics.OverlapSphere(pos, maxPursueDistance, (int) LayerMasks.BODY);

		Vector3 targetPosition = default;
		float targetDistance = 0;
		foreach (Collider col in bodiesInPursuitRadius) {
			UnitController target = col.gameObject.GetComponent<BodyController>().unit;
			if (!target.SharesTeamWith(unit) && target.IsAlive() && target.body.IsVisibleToUnit(this.unit)) {
				Vector3 targ = target.GetBodyPosition();
				float dist = Util.GetDistanceIn2D(pos, targ);
				if (targetDistance == 0 || dist < targetDistance) {
					currentTarget = target;
					targetDistance = dist;
					targetPosition = targ;
				}
			}
		}

		return targetPosition;
	}

}
