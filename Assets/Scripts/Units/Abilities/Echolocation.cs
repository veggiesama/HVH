using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Monster/Echolocation")]
public class Echolocation : Ability, IProjectileAbility {

	[Header("Echolocation")]
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
		projectileSpawner = BodyLocations.MOUTH;
		projectileBehaviour = ProjectileBehaviourTypes.CONE;
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

		CreateProjectile(this, castOrder);

		return CastResults.SUCCESS;
	}

	public bool OnHitEnemy(UnitController enemy) { 
		float offset_y = enemy.body.GetComponent<Collider>().bounds.extents.y;
		Vector3 enemyPosition = enemy.body.transform.position;
		Vector3 casterForward = caster.body.transform.forward;
		Vector3 finalPosition = enemyPosition + (casterForward * knockbackForce);
		finalPosition.y = Terrain.activeTerrain.SampleHeight(finalPosition) + Terrain.activeTerrain.GetPosition().y + offset_y;
		Vector3 velocityVector = Util.CalculateBestLaunchSpeed(enemyPosition, finalPosition, duration);

		//Vector3 velocityVector = (enemy.GetBodyPosition() - caster.GetBodyPosition()).normalized * knockbackForce;
		networkHelper.ApplyKnockbackTo(enemy, velocityVector, this);
		networkHelper.DealDamageTo(enemy, damage);
		return false;
	}

	public bool OnHitAlly(UnitController ally) {
		return false;
	}

	public bool OnHitTree(Tree tree) {
		networkHelper.DestroyTree(tree, caster.GetBodyPosition(), 0);
		return false;
	}
}
