using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dwarf/Net")]
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
		aoeRadius = 0f;
		quickCast = false;
		doNotCancelOrderQueue = false;

		projectilePrefab = null; // set on scriptableObject
		projectileSpawner = BodyLocations.WEAPON;
		projectileBehaviour = ProjectileBehaviourTypes.GRENADE;
		grenadeTimeToHitTarget = 2f;
	}

	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		CreateProjectile(this, castOrder);
		return CastResults.SUCCESS;
	}

	public bool OnHitEnemy(UnitController enemy) {
		//Debug.Log("Hit enemy.");
		networkHelper.ApplyStatusEffectTo(enemy, netImmobilizedStatus, this);
		return true;
	}

	public bool OnHitAlly(UnitController ally) {
		//Debug.Log("Hit ally.");
		networkHelper.ApplyStatusEffectTo(ally, netImmobilizedStatus, this);
		return true;
	}

	public bool OnHitTree(Tree tree) {
		//Debug.Log("Hit tree.");
		//tree.DestroyThisTree();
		return false;
	}
}
