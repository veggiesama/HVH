using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Monster/Spit")]
public class Spit : Ability {

	[Header("Spit")]
	public StatusEffect slowStatusEffect;
	public GameObject particlePrefab;

	public override void Reset()
	{
		abilityName = "Spit";
		targetType = AbilityTargetTypes.UNIT;
		targetTeam = AbilityTargetTeams.ENEMY;
		damageType = DamageTypes.NORMAL;
		damage = 30;
		cooldown = 1.0f;
		castRange = 10f;
		castTime = 0.3f;
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

		enemyTarget = castOrder.enemyTarget;

		networkHelper.ApplyStatusEffectTo(enemyTarget, slowStatusEffect, this);
		networkHelper.DealDamageTo(enemyTarget, damage);
		InstantiateParticleOnUnit(particlePrefab, enemyTarget, BodyLocations.HEAD);

		return CastResults.SUCCESS;
	}
}
