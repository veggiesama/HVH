using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "AI States/PursueNearestVisibleEnemy")]
public class PursueNearestVisibleEnemy : MoveTo {

	[Header("Pursue")]
	public Teams pursuedTeams;
	public float maxPursueDistance;
	protected UnitController currentTarget;

	public override void Reset() {
		base.Reset();

		desire = (int) Desire.HIGH;
		pursuedTeams = Teams.MONSTERS;
		maxPursueDistance = 10f;
		stoppingDistance = 1.0f;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);
		//ForceRepeatMoveOrder(true);
	}

	public override void Evaluate() {
		var targets = GetTargetsInRadiusSortedByDistance(unit, maxPursueDistance, pursuedTeams, true);

		if (targets.Count > 0) {
			unit.SetCurrentTarget(targets[0]);
			SetDestination( targets[0].GetBodyPosition() );
		}
		else
			SetDestination(default);

		if (HasDestination() && !ReachedDestination() ) {
			desire = desireDefault;

			//	unit.SetCurrentTarget(currentTarget);
		}
		else
			desire = (int) Desire.NONE;

	}

}
