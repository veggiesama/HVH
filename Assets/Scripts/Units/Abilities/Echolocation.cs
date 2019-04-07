using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Echolocation")]
public class Echolocation : Ability, IProjectileAbility {

	public float knockbackForce;

	public override void Reset()
	{
		abilityName = "Echolocation";
		targetType = AbilityTargetTypes.POINT;
		targetTeam = AbilityTargetTeams.ENEMY;
		damageType = DamageTypes.NORMAL;
		damage = 50;
		cooldown = 3f;
		castRange = 0f;
		castTime = 0f;
		duration = 3.0f;
		aoeRadius = 0f;
		quickCast = false;
		doNotCancelOrderQueue = true;

		projectilePrefab = null;
		projectileSpeed = 0f;
		projectileTimeAlive = 0.5f;
		projectileGrowthFactor = new Vector3(1,1,1);
		grenadeTimeToHitTarget = 0;

		knockbackForce = 20f;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		GameObject projectileObject = Instantiate(projectilePrefab,
			caster.attackInfo.spawnerObject.transform.position,
			caster.attackInfo.spawnerObject.transform.rotation,
			caster.transform);

		ConeBehaviour echolocation = projectileObject.GetComponent<ConeBehaviour>();
		echolocation.Initialize(this, castOrder.targetLocation);
		echolocation.SetGrowthFactor(projectileGrowthFactor);

		TrackDuration();
		return CastResults.SUCCESS;
	}

	public bool OnHitEnemy(UnitController unit)
	{ 
		Debug.Log("Cone hit enemy");
		Vector3 velocityVector = (unit.GetBodyPosition() - caster.GetBodyPosition()).normalized * knockbackForce;
		unit.Knockback(caster.GetBodyPosition(), this, caster);
		unit.ReceivesDamage(damage, caster);
		return false;
	}

	public bool OnHitAlly(UnitController unit)
	{
		return false;
	}

	public bool OnHitTree(Tree tree)
	{
		caster.GetPlayer().DestroyTree(tree, caster.GetBodyPosition(), 0);
		return false;
	}
}
