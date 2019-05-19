using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
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
			//Util.DebugDrawVector(targetLocation, Color.green, 1f);

			//NavMeshPath path = new NavMeshPath();
			//Vector3 newDest = targetLocation;
			/*
			unit.agent.CalculatePath(newDest, path);
			if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid) {
				float t = 1.0f; // add or subtract half of current value
				for (int i = 0; i < 10; i++) {
						t = t * 0.5f;
						newDest = Vector3.Lerp(unit.GetBodyPosition(), newDest, t);
						Util.DebugDrawVector(newDest, Color.yellow, 3f);
					}
				}
			}

			/*for (float t = 1.0f; t > 0; t -= 0.1f) {
				unit.agent.CalculatePath(newDest, path);
				if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid) {
					newDest = Vector3.Lerp(unit.GetBodyPosition(), newDest, t);
					Util.DebugDrawVector(newDest, Color.yellow, 3f);
				}
				else {
					break;
				}
			}*/
			

			//unit.agent.SetPath(path);
			unit.agent.SetDestination(targetLocation);

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

	public override void Suspend(OrderTypes suspendedBy) {
	}

	public override void End() {
		unit.ForceStop();
		base.End();
	}

}
