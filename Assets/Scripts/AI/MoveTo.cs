using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/MoveTo")]
public class MoveTo : AiState {

	protected Vector3 destination;

	public override void Evaluate() {
		if (HasDestination() && ReachedDestination()) {
			desire = (int) Desire.NONE;
		}
	}

	public override void Execute() {
		base.Execute();
		destination = GameRules.GetRandomPointOfInterest().position;
	}

	public override void Update() {
		base.Update();
		if (HasDestination() && !ReachedDestination() && unit.GetCurrentOrderType() != OrderTypes.MOVE_TO_POSITION) {
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
		return (Util.GetDistanceIn2D(unit.GetBodyPosition(), destination) < 1.0f);
	}
}
