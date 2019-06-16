using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dwarf/FirePoint")]
public class FirePoint : Ability, IProjectileAbility {

	[Header("FirePoint")]
	public GameObject gunsmokePrefab;
	public GameObject muzzleFlashPrefab;
	public GameObject impactHitPrefab;

	public override void Reset()
	{
		abilityName = "FirePoint";
		targetType = AbilityTargetTypes.POINT;
		targetTeam = AbilityTargetTeams.NONE;
		damageType = DamageTypes.NORMAL;
		damage = 60;
		cooldown = 0.1f;
		castRange = 100f;
		castTime = 0f;
		duration = 0f;
		aoeRadius = 0f;
		quickCast = true;
		doNotCancelOrderQueue = true;

		projectilePrefab = null;
		projectileSpawner = BodyLocations.WEAPON;
		projectileBehaviour = ProjectileBehaviourTypes.BULLET;
		projectileSpeed = 5f;
		projectileTimeAlive = 10f;
		grenadeTimeToHitTarget = 0;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		networkHelper.CreateProjectile(this, castOrder);
		networkHelper.InstantiateParticle(gunsmokePrefab, caster, BodyLocations.WEAPON);
		networkHelper.InstantiateParticle(muzzleFlashPrefab, caster, BodyLocations.WEAPON);

		return CastResults.SUCCESS;
	}

	public bool OnHitEnemy(UnitController unit)
	{
		networkHelper.DealDamageTo(unit, damage);
		networkHelper.InstantiateParticle(impactHitPrefab, unit, BodyLocations.HEAD);
		return true;
	}

	public bool OnHitAlly(UnitController unit)
	{
		networkHelper.DealDamageTo(unit, damage / 2);
		networkHelper.InstantiateParticle(impactHitPrefab, unit, BodyLocations.HEAD);
		return true;
	}

	public bool OnHitTree(Tree tree)
	{
		return false;
	}

}
