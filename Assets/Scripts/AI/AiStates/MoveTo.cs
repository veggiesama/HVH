using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/MoveTo")]
public abstract class MoveTo : AiState {

	[Header("MoveTo")]
	public float stoppingDistance = 1f;
	protected Vector3 destination;
	protected bool forceRepeatMoveOrder;

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
		if ((forceRepeatMoveOrder) || (HasDestination() && !ReachedDestination() && unit.GetCurrentOrderType() != OrderTypes.MOVE_TO_POSITION)) {
			unit.MoveTo(destination);
		}
	}

	public override void End() {
		base.End();
		unit.ForceStop();
		destination = default;
	}

	protected bool HasDestination() {
		return (destination != default);
	}

	protected bool ReachedDestination() {
		return (Util.GetDistanceIn2D(unit.GetBodyPosition(), destination) < stoppingDistance);
	}
}
