using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "AI States/TrackNearestTargetByTeam")]
public class TrackNearestTargetByTeam : MoveTo {

	// track invisible target across map

	[Header("Pursue")]
	public Teams pursuedTeam;
	protected UnitController currentTarget;

	public override void Reset() {
		base.Reset();

		desire = (int) Desire.HIGH;
		pursuedTeam = Teams.MONSTERS;
		stoppingDistance = 1.0f;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);
		ForceRepeatMoveOrder(true);
	}

	public override void Evaluate() {
		var targets = GetEnemyTargetsSortedByDistance(unit, pursuedTeam, false);

		if (targets.Count > 0) {
			unit.SetCurrentTarget(targets[0]);
			Vector3 dest = Vector3.MoveTowards(targets[0].GetBodyPosition(), unit.GetBodyPosition(), stoppingDistance);
			SetDestination(dest);
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

	public override void Execute() {
		base.Execute(); // does movement
	}

}
