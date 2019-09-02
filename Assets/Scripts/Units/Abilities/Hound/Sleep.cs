using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Hound/Sleep")]
public class Sleep : Ability {

	[Header("Sleep")]
	public StatusEffect sleepBuff;

	public override void Reset() {
		abilityName = "Sleep";
		targetType = AbilityTargetTypes.NONE;
		targetTeam = AbilityTargetTeams.NONE;
		quickCast = true;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		caster.ApplyStatusEffect(sleepBuff);

		return CastResults.SUCCESS;
	}

	protected override void OnMoved() {
		if (caster.HasStatusEffect(sleepBuff))
			caster.RemoveStatusEffect(sleepBuff);
	}

}
