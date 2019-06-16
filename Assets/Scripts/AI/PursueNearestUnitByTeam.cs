using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "AI States/PursueNearestUnitByTeam")]
public class PursueNearestUnitByTeam : MoveTo {

	private DayNightController dayNightController;

	[Range(0,100), Header("Pursue")]
	public int desireDuringDay;
	public Teams pursuedTeam;
	public float stoppingDistance;

	public override void Reset() {
		base.Reset();

		desire = (int) Desire.NONE;
		desireDuringDay = (int) Desire.HIGH;
		pursuedTeam = Teams.MONSTERS;
		stoppingDistance = 1.0f;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);
		dayNightController = GameRules.Instance.GetComponent<DayNightController>();
		forceRepeatMoveOrder = true;
	}

	public override void Evaluate() {

		if (dayNightController.IsDay()) {

			destination = GetClosestTargetPosition(pursuedTeam);

			if (HasDestination() && (Util.GetDistanceIn2D(unit.GetBodyPosition(), destination) > stoppingDistance) )
				desire = desireDuringDay;
			else
				desire = (int) Desire.NONE;
		}

		else {
			desire = (int) Desire.NONE;
		}
	}

	public override void Execute() {
		base.Execute(); // does movement
	}

	private Vector3 GetClosestTargetPosition(Teams team) {

		List<UnitController> unitList = GameRules.GetEnemyUnitsOf(unit, false);
		Vector3 loc = unit.GetBodyPosition();
		Vector3 closestTargetPosition = default;

		if (unitList.Count > 0 ) {
			unitList.Sort((x, y) => Util.GetDistanceIn2D( loc, x.GetBodyPosition() )
				.CompareTo( Util.GetDistanceIn2D( loc, y.GetBodyPosition() ) )
				);
		
			for (int i = 0; i < unitList.Count; i++) {
				UnitController target = unitList[i];
				if (target.IsAlive()) {
					if (target.body.IsVisible())
						unit.SetCurrentTarget(target);

					closestTargetPosition = target.GetBodyPosition();
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
