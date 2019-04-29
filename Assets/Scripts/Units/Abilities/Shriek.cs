using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Monster/Shriek")]
public class Shriek : Ability {

	[Header("Shriek")]
	public StatusEffect silenceStatusEffect;
	public GameObject particlePrefab;

	public override void Reset()
	{
		abilityName = "Shriek";
		targetType = AbilityTargetTypes.UNIT;
		targetTeam = AbilityTargetTeams.ENEMY;
		damageType = DamageTypes.NORMAL;
		damage = 60;
		cooldown = 0.5f;
		castRange = 20f;
		castTime = 0f;
		duration = 0f;
		aoeRadius = 0f;
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

		networkHelper.ApplyStatusEffectTo(enemyTarget, silenceStatusEffect, this);
		//networkHelper.CreateProjectile(this, castOrder);
		networkHelper.InstantiateParticle(particlePrefab, enemyTarget, BodyLocations.FEET, duration);
		return CastResults.SUCCESS;
	}
}
