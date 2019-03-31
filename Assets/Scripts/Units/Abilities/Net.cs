using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Net")]
public class Net : Ability, IProjectileAbility {

	[Header("Net")]
	public Immobilized netImmobilizedStatus;

	public override void Reset()
	{
		abilityName = "Net";
		targetType = AbilityTargetTypes.POINT;
		targetTeam = AbilityTargetTeams.ENEMY;
		damageType = DamageTypes.NONE;
		damage = 0;
		cooldown = 0.1f;
		castRange = 30f;
		castTime = 0f;
		duration = 3f;
		doNotCancelOrderQueue = false;

		projectilePrefab = null;
		projectileSpeed = 0f;
		projectileTimeAlive = 0f;
		grenadeTimeToHitTarget = 2f;
	}

	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		GameObject projectileObject = Instantiate(projectilePrefab,
			caster.attackInfo.spawnerObject.transform.position,
			caster.attackInfo.spawnerObject.transform.rotation,
			caster.transform);

		GrenadeBehaviour projectile = projectileObject.GetComponent<GrenadeBehaviour>();
		projectile.Initialize(this, castOrder.targetLocation);

		return CastResults.SUCCESS;
	}

	public bool OnHitEnemy(UnitController unit) {
		Debug.Log("Hit enemy.");
		unit.ApplyStatusEffect(netImmobilizedStatus, this, unit);
		return true;
	}

	public bool OnHitAlly(UnitController unit) {
		Debug.Log("Hit ally.");
		unit.ApplyStatusEffect(netImmobilizedStatus, this, unit);
		return true;
	}

	public bool OnHitTree(Tree tree) {
		//Debug.Log("Hit tree.");
		//tree.DestroyThisTree();
		return false;
	}
}
