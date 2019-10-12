using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MoveToPosition : Order {

	public void Initialize(GameObject obj, Vector3 targetLocation) {
		base.Initialize(obj, null, targetLocation);
		this.orderType = OrderTypes.MOVE_TO_POSITION;
	}

	public override void Execute() {
		if (unit.HasStatusEffect(StatusEffectTypes.AIRBORN) || unit.HasStatusEffect(StatusEffectTypes.IMMOBILIZED)) {
			unit.TurnToFace(targetLocation);
			return;
		}

		if (unit.IsReadyForNav()) {

			NavMeshPath path = CalculatePath(targetLocation);

			if (path.status != NavMeshPathStatus.PathComplete) {
				path = CalculateDirectNearestPath(targetLocation, 6);
			}

			if (path != null) {
				unit.agent.SetPath(path);
				unit.ShowMovementProjector();
			}

			return;
		}
	}

	// this speeds up the nav agent's decision time
	private NavMeshPath CalculateDirectNearestPath(Vector3 destination, int tryHowManyTimes) {
		Util.DebugDrawVector(destination, Color.red, 3f);

		Vector3 newDestination = destination;
		NavMeshPath path;
		NavMeshPath lastValidPath = null;

		float lerpT = 0.5f;
		float lerpInc = lerpT;
		for (int i = 0; i < tryHowManyTimes; i++) {
			newDestination = Vector3.Lerp(unit.GetBodyPosition(), destination, lerpT);
			string debug = "Before: " + newDestination.y;
			newDestination = Util.SnapVectorToTerrain(newDestination);
			debug += (", After: " + newDestination.y);
			lerpInc = lerpInc * 0.5f;
			path = CalculatePath(newDestination);

			if (path.status != NavMeshPathStatus.PathComplete) {
				lerpT -= lerpInc;
				//Util.DebugDrawVector(newDestination, Color.yellow, 3f);
			}
			else {
				lastValidPath = path;
				lerpT += lerpInc;
				//Util.DebugDrawVector(newDestination, Color.green, 3f);
			}
	
		}

		return lastValidPath;
	}

	private NavMeshPath CalculatePath(Vector3 destination) {
		NavMeshPath path = new NavMeshPath();
		unit.agent.CalculatePath(destination, path);
		return path;
	}

	public override void Update() {
		if (!unit.agent.pathPending && !unit.agent.hasPath)
			End();
		else
			unit.onMoved.Invoke();
	}

	public override void FixedUpdate() {}

	public override void Suspend(OrderTypes suspendedBy) {
	}

	public override void End() {
		unit.ForceStop();
		unit.HideMovementProjector();
		base.End();
	}

}
