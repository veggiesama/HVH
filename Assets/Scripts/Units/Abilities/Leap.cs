﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Leap")]
public class Leap : Ability {

	//public float forceUpwards;
	//public float forceForwards;

	public float leapDistance;
	public StatusEffect airbornStatusEffect;
	private float jumpingTimer = 0f;

	public override void Reset()
	{
		abilityName = "Leap";
		targetType = AbilityTargetTypes.NONE;
		targetTeam = AbilityTargetTeams.NONE;
		damageType = DamageTypes.NONE;
		damage = 0;
		cooldown = 2f;
		castRange = 0f;
		castTime = 0f;
		duration = 1.1f;
		doNotCancelOrderQueue = true;

		projectilePrefab = null;
		projectileSpeed = 0;
		projectileTimeAlive = 0;
		grenadeTimeToHitTarget = 0;

		leapDistance = 20f;
	}

	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;
		if (!caster.IsReadyForNav()) return CastResults.FAILURE_NAVIGATION_NOT_READY;
		if (caster.IsAirborn()) return CastResults.FAILURE_ALREADY_AIRBORN;

		float offset_y = caster.body.GetComponent<Collider>().bounds.extents.y;
		Vector3 casterPosition = caster.body.transform.position;
		Vector3 casterForward = caster.body.transform.forward;
		Vector3 finalPosition = casterPosition + (casterForward * leapDistance);
		finalPosition.y = Terrain.activeTerrain.SampleHeight(finalPosition) + Terrain.activeTerrain.GetPosition().y + offset_y;
		Vector3 velocityVector = Util.CalculateBestLaunchSpeed(casterPosition, finalPosition, duration);

		Debug.DrawRay(casterPosition, velocityVector, Color.blue, 10.0f);
		Debug.DrawLine(casterPosition, finalPosition, Color.yellow, 10.0f);
		Debug.DrawRay(finalPosition, Vector3.up, Color.green, 10.0f);

		caster.ApplyStatusEffect(airbornStatusEffect, this, caster);
		caster.body.PerformAirborn(velocityVector);
		caster.body.SetNoclip();

		TrackDuration();

		return CastResults.SUCCESS;
	}

	protected override void OnDurationEnd () {
		caster.RemoveStatusEffect(airbornStatusEffect.statusName);
		caster.body.ResetBody();
	}

}