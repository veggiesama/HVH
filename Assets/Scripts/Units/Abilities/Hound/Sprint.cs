using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Hound/Sprint")]
public class Sprint : Ability {

	public override void Reset()
	{
		abilityName = "Sprint";
		targetType = AbilityTargetTypes.NONE;
		targetTeam = AbilityTargetTeams.NONE;
		cooldown = 6f;
		duration = 0.5f;
		doNotCancelOrderQueue = true;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		

		return CastResults.SUCCESS;
	}

}
