using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "AI States/PursueNearestUnitByTeam")]
public class PursueNearestUnitByTeam : MoveTo {

	[Header("Pursue")]
	public Teams pursuedTeam;
	private UnitController currentTarget;

	public override void Reset() {
		base.Reset();

		desire = (int) Desire.HIGH;
		pursuedTeam = Teams.MONSTERS;
		stoppingDistance = 1.0f;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);
		forceRepeatMoveOrder = true;
	}

	public override void Evaluate() {
		destination = GetClosestTargetPosition(pursuedTeam);

		if (HasDestination() && !ReachedDestination() ) {
			desire = desireDefault;

			if (currentTarget != null && currentTarget.body.IsVisible())
				unit.SetCurrentTarget(currentTarget);

		}
		else
			desire = (int) Desire.NONE;
	}

	public override void Execute() {
		base.Execute(); // does movement
	}

	private Vector3 GetClosestTargetPosition(Teams team) {

		List<UnitController> unitList = GameResources.Instance.GetEnemyUnitsOf(unit, false);
		Vector3 loc = unit.GetBodyPosition();
		Vector3 closestTargetPosition = default;

		if (unitList.Count > 0 ) {
			unitList.Sort((x, y) => Util.GetDistanceIn2D( loc, x.GetBodyPosition() )
				.CompareTo( Util.GetDistanceIn2D( loc, y.GetBodyPosition() ) )
				);
		
			for (int i = 0; i < unitList.Count; i++) {
				UnitController target = unitList[i];
				if (target.IsAlive()) {
					closestTargetPosition = target.GetBodyPosition();
					currentTarget = target;
					break;
				}
			}
		}

		if (closestTargetPosition != default) {
			closestTargetPosition = Vector3.MoveTowards(closestTargetPosition, loc, stoppingDistance);
		}

		return closestTargetPosition;
	}

}
