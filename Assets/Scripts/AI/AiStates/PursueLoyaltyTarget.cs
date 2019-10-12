using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/PursueLoyaltyTarget")]
public class PursueLoyaltyTarget : MoveTo {

	[Header("PursueLoyaltyTarget")]
	public Loyalty loyaltyStatusEffect;
	//public StatusEffect loyaltyBuff;
	protected UnitController followedUnit;
	
	public override void Reset() {
		base.Reset();
		desire = (int) Desire.MEDIUM;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);
		ForceRepeatMoveOrder(true);
	}

	public override void Evaluate() {
		if (unit.HasStatusEffect(loyaltyStatusEffect)) {
			followedUnit = loyaltyStatusEffect.GetLoyaltyTarget();
			//followedUnit = unit.GetTarget(AbilityTargetTeams.ALLY);
			if (IsValidUnit(followedUnit))
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
		FollowTarget();
	}

	public override void Update() {
		FollowTarget();
		base.Update();
	}

	public override void End() {
		base.End();
		followedUnit = null;
	}

	////////////////////////////////////////////////////////////
	private void FollowTarget() {
		if (IsValidUnit(followedUnit))
			SetDestination(followedUnit.GetBodyPosition());
	}

}
