using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Monster/Swipe")]
public class Swipe : Ability, IProjectileAbility {

	public override void Reset()
	{
		abilityName = "Swipe";
		targetType = AbilityTargetTypes.POINT;
		targetTeam = AbilityTargetTeams.ENEMY;
		damageType = DamageTypes.NORMAL;
		damage = 300;
		cooldown = 1f;
		castRange = 0f;
		castTime = 0f;
		duration = 0.5f;
		doNotCancelOrderQueue = true;

		projectilePrefab = null; // set on scriptableobject
		projectileSpawner = BodyLocations.MOUTH;
		projectileBehaviour = ProjectileBehaviourTypes.CONE;
		projectileSpeed = 0f;
		projectileTimeAlive = 0.5f;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		CreateProjectile(this, castOrder);

		/*
		GameObject projectileObject = Instantiate(projectilePrefab,
			caster.attackInfo.spawnerObject.transform.position,
			caster.attackInfo.spawnerObject.transform.rotation,
			caster.transform);

		ConeBehaviour swipe = projectileObject.GetComponent<ConeBehaviour>();
		swipe.Initialize(this, castOrder.targetLocation);
		*/

		//TrackDuration();
		return CastResults.SUCCESS;
	}

	public override void FixedUpdate() {
		base.FixedUpdate();
		if (this.durationRemaining > 0 && caster.HasStatusEffect(StatusEffectTypes.AIRBORN)) {
			caster.SetMouseLook(true);
		}
		else {
			caster.SetMouseLook(false);
		}
	}

	protected override void OnDurationEnd()
	{
		base.OnDurationEnd();
		caster.SetMouseLook(false);
	}

	public bool OnHitEnemy(UnitController unit)
	{ 
		networkHelper.DealDamageTo(unit, damage);
		return false;
	}

	public bool OnHitAlly(UnitController unit)
	{
		networkHelper.DealDamageTo(unit, damage / 2);
		return false;
	}

	public bool OnHitTree(Tree tree)
	{
		networkHelper.DestroyTree(tree, caster.GetBodyPosition(), 0);
		return false;
	}
}
