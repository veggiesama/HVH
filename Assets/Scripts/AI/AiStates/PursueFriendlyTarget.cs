using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/PursueFriendlyTarget")]
public class PursueFriendlyTarget : MoveTo {

	[Header("PursueFriendlyTarget")]
	public StatusEffect loyaltyBuff;
	protected UnitController followedUnit;
	
	public override void Reset() {
		base.Reset();
		desire = (int) Desire.MEDIUM;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);
		forceRepeatMoveOrder = true;
	}

	public override void Evaluate() {
		if (unit.HasStatusEffect(loyaltyBuff)) {
			followedUnit = unit.GetTarget(AbilityTargetTeams.ALLY);
			if (IsValidFollowTarget(followedUnit))
				desire = desireDefault;
			else
				desire = (int) Desire.NONE;
		}
		else {
			desire = (int) Desire.NONE;
		}

		base.Evaluate();
	}

	public override void Execute() {
		base.Execute();

		if (IsValidFollowTarget(followedUnit))
			destination = followedUnit.GetBodyPosition();
	}

	public override void Update() {
		if (IsValidFollowTarget(followedUnit))
			destination = followedUnit.GetBodyPosition();
	
		base.Update();
	}

	public override void End() {
		base.End();
		followedUnit = null;
	}

	////////////////////////////////////////////////////////////
	private bool IsValidFollowTarget(UnitController u) {
		return (u != null && u.IsAlive());
	}

}
