using System.Collections;
using System.Collections.Generic;
using HVH;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dwarf/TossBone")]
public class TossBone : Ability, IProjectileAbility {

	[Header("TossBone")]
	public StatusEffect loyaltyStatusEffect;
	public StatusEffect wellFedStatusEffect;
	public UnitInfo targetUnitInfo;
	//public GameObject particlePrefab;

	public override void Reset() {
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

		projectilePrefab = null; // set in inspector
		projectileSpawner = BodyLocations.WEAPON;
		projectileBehaviour = ProjectileBehaviourTypes.HOMING;
		projectileSpeed = 4f;
		projectileTimeAlive = 12f;
		homingRotationalSpeed = 360f;
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
			CreateProjectile(this, castOrder);
			return CastResults.SUCCESS;
		}
		else {
			Debug.Log("Toss Bone: Invalid target.");
			return CastResults.FAILURE_INVALID_TARGET;
		}
	}

	public bool OnHitEnemy(UnitController enemy) {
		return false;
	}

	public bool OnHitAlly(UnitController ally) {
		if (ally.Equals(allyTarget)) {
			networkHelper.ApplyStatusEffectTo(allyTarget, loyaltyStatusEffect, this);
			networkHelper.ApplyStatusEffectTo(allyTarget, wellFedStatusEffect, this);
			return true;
		}

		return false;
	}

	public bool OnHitTree(HVH.Tree tree) {
		return false;
	}
}
