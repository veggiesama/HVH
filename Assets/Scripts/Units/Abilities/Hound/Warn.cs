using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Hound/Warn")]
public class Warn : Ability {

	//[Header("Warn")]

	public override void Reset() {
		abilityName = "Warn";
		targetType = AbilityTargetTypes.NONE;
		targetTeam = AbilityTargetTeams.NONE;
		cooldown = 5f;
		quickCast = true;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		caster.body.PlayAnimation(Animations.CAST_B);

		return CastResults.SUCCESS;
	}

}
