using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attack logic:
// 1. fire instantly at point target
// 2. check for trees in raytrace. if trees, apply 50% miss chance and alter point target
// 3. if projectile intersects foe, deal 100% dmg. if friendly, 50%. 
// 4. slow character during reload

[CreateAssetMenu(menuName = "Abilities/FirePoint")]
public class FirePoint : Ability, IProjectileAbility {

	[Header("Projectile (Bullet)")]
	public float treeMissChance = 0.5f; 

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
		doNotCancelOrderQueue = true;

		projectilePrefab = null;
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

		GameObject projectileObject = Instantiate(projectilePrefab,
			caster.attackInfo.spawnerObject.transform.position,
			caster.attackInfo.spawnerObject.transform.rotation,
			caster.transform);

		BulletBehaviour bullet = projectileObject.GetComponent<BulletBehaviour>();
		bullet.Initialize(this, castOrder.targetLocation, treeMissChance); // trees can cause the bullet to miss

		return CastResults.SUCCESS;
	}

	public bool OnHitEnemy(UnitController unit)
	{ 
		unit.ReceivesDamage(damage, caster);
		return true;
	}

	public bool OnHitAlly(UnitController unit)
	{
		unit.ReceivesDamage(damage / 2, caster);
		return true;
	}

	public bool OnHitTree(Tree tree)
	{
		return false;
	}
}
