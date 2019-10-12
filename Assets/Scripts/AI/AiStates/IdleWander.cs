using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/IdleWander")]
public class IdleWander : MoveTo {

	[Header("IdleWander")]
	public float wanderDistance = 2f;

	public override void Evaluate() {
		if (currentTimer > 0)
			desire = (int) Desire.MAX;
		else if (HasDestination() && ReachedDestination() && currentTimer <= 0) {
			desire = (int) Desire.NONE;
		}
	}

	public override void Execute() {
		base.Execute();
		SetDestination(Util.GetRandomVectorAround(unit, wanderDistance));
	}


}
