using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Hound/Sprint")]
public class Sprint : Ability {

	[Header("Sprint")]
	public StatusEffect houndSprintBuff;
	public StatusEffect dwarfSprintBuff;
	public float houndSpeedBonus;
	public float dwarfSpeedBonus;

	public override void Reset()
	{
		abilityName = "Sprint";
		targetType = AbilityTargetTypes.NONE;
		targetTeam = AbilityTargetTeams.NONE;
		cooldown = 6f;
		duration = 6f;
		aoeRadius = 1800f;
		doNotCancelOrderQueue = true;

		houndSpeedBonus = 3f;
		dwarfSpeedBonus = 1f;
		quickCast = true;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		var unitsToBuff = Util.FindUnitsInSphere(caster.GetBodyPosition(), aoeRadius, caster.GetTeam());

		foreach (var unit in unitsToBuff) {
			if (caster.SharesUnitInfoWith(unit))
				networkHelper.ApplyStatusEffectTo(unit, houndSprintBuff, this);
			else 
				networkHelper.ApplyStatusEffectTo(unit, dwarfSprintBuff, this);
		}

		return CastResults.SUCCESS;
	}

}
