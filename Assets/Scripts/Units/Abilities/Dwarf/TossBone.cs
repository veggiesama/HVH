using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dwarf/TossBone")]
public class TossBone : Ability {

	[Header("TossBone")]
	public StatusEffect loyaltyStatusEffect;
	public StatusEffect wellFedStatusEffect;
	public UnitInfo targetUnitInfo;
	//public GameObject particlePrefab;

	public override void Reset()
	{
		abilityName = "Toss bone";
		targetType = AbilityTargetTypes.UNIT;
		targetTeam = AbilityTargetTeams.ALLY;
		damage = -50;
		cooldown = 45f; // should match duration
		castRange = 50f;
		castTime = 0.3f;
		duration = 45f;
		quickCast = true;
		doNotCancelOrderQueue = false;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		allyTarget = castOrder.allyTarget;
		if (allyTarget.SharesUnitInfoWith(targetUnitInfo)) {
			networkHelper.ApplyStatusEffectTo(allyTarget, loyaltyStatusEffect, this);
			networkHelper.ApplyStatusEffectTo(allyTarget, wellFedStatusEffect, this);
			//networkHelper.CreateProjectile(this, castOrder);
			//networkHelper.InstantiateParticle(particlePrefab, allyTarget, BodyLocations.FEET, duration);
		}

		else {
			Debug.Log("Toss Bone: Invalid target.");
			return CastResults.FAILURE_INVALID_TARGET;
		}

		return CastResults.SUCCESS;
	}
}
