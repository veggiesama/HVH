using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "AI States/MoveTo")]
public abstract class MoveTo : AiState {

	[Header("MoveTo")]
	public float stoppingDistance = 1f;
	private Vector3 destination;
	private bool forceRepeatMoveOrder;

	public override void Evaluate() {
		if (HasDestination() && ReachedDestination()) {
			desire = (int) Desire.NONE;
		}
	}

	public override void Execute() {
		base.Execute();
	}

	public override void Update() {
		base.Update();
		if ((forceRepeatMoveOrder) || (HasDestination() && !ReachedDestination() && !IsDoingMoveOrder())) {
			unit.MoveTo(destination);
		}
	}

	public override void End() {
		base.End();
		unit.ForceStop();
		destination = default;
	}

	// HELPERS

	protected bool HasDestination() {
		return (destination != default);
	}

	protected bool ReachedDestination() {
		return (Util.GetDistanceIn2D(unit.GetBodyPosition(), destination) < stoppingDistance);
	}

	protected void ForceRepeatMoveOrder(bool enable) {
		forceRepeatMoveOrder = enable;
	}

	protected bool IsDoingMoveOrder() {
		return unit.GetCurrentOrderType() == OrderTypes.MOVE_TO_POSITION;
	}

	protected void SetDestination(Vector3 destination) {
		this.destination = destination;
	}

	protected Vector3 GetDestination() {
		return destination;
	}

	protected static bool IsValidUnit(UnitController u) {
		return (u != null && u.IsAlive());
	}


	protected static List<UnitController> GetTargetsInRadiusSortedByDistance(UnitController unit, float radius, Teams teamFilter = Teams.NONE, bool mustBeVisibleToUnit = true) {
		Vector3 origin = unit.GetBodyPosition();
		Collider[] bodiesInPursuitRadius = Physics.OverlapSphere(origin, radius, Util.GetAllBodyLayerMasks());

		List<UnitController> targetList = new List<UnitController>();
		foreach (Collider col in bodiesInPursuitRadius) {
			UnitController target = col.gameObject.GetComponent<BodyController>().unit;
			if (!IsValidUnit(target)) continue;
			if (teamFilter != Teams.NONE && !teamFilter.HasFlag(target.GetTeam())) continue;
			if (mustBeVisibleToUnit && !target.body.IsVisibleToUnit(unit)) continue;

			targetList.Add(target);
		}
		
		targetList.OrderBy(x => Util.GetDistanceIn2D(origin, x.GetBodyPosition()));

		return targetList;
	}

	protected static List<UnitController> GetEnemyTargetsSortedByDistance(UnitController unit, Teams teamFilter = Teams.NONE, bool mustBeVisible = true) {
		Vector3 origin = unit.GetBodyPosition();
		List<UnitController> unitList = GameResources.Instance.GetEnemyUnitsOf(unit, mustBeVisible, true);

		unitList = unitList.Where(x => (teamFilter == Teams.NONE || teamFilter.HasFlag(x.GetTeam())))
						   .OrderBy(x => Util.GetDistanceIn2D(origin, x.GetBodyPosition()))	
						   .ToList();

		return unitList;
	}
}
