using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Flare")]
public class Flare : Ability {

	//[Header("Flare")]
	//public Immobilized netImmobilizedStatus;

	public override void Reset()
	{
		abilityName = "Flare";
		targetType = AbilityTargetTypes.AREA;
		targetTeam = AbilityTargetTeams.ENEMY;
		damageType = DamageTypes.NONE;
		damage = 0;
		cooldown = 1.0f;
		castRange = 30f;
		castTime = 0f;
		duration = 6f;
		aoeRadius = 4f;
		doNotCancelOrderQueue = false;

		projectilePrefab = null;
		projectileSpeed = 0f;
		projectileTimeAlive = 0f;
		grenadeTimeToHitTarget = 0f;
	}

	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		Debug.Log("Cast flare");

		TrackDuration();

		return CastResults.SUCCESS;
	}

	protected override void OnDurationEnd () {
		
	}

}
