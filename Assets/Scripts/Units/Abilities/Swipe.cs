using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Swipe")]
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
		bypassOrderQueue = true;

		projectilePrefab = null;
		projectileSpeed = 0f;
		projectileTimeAlive = 0.5f;
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

		ConeBehaviour swipe = projectileObject.GetComponent<ConeBehaviour>();
		swipe.Initialize(this, castOrder.targetLocation);
		
		TrackDuration();
		return CastResults.SUCCESS;
	}

	public override void FixedUpdate() {
		base.FixedUpdate();
		if (this.durationRemaining > 0 && caster.HasStatusEffect(StatusEffectTypes.AIRBORN)) {
			Vector3 mousePosition = caster.GetOwnerController().GetMouseLocationToGround();
			caster.body.FixedUpdate_ForceTurn(mousePosition);
			/*Quaternion wantedRotation = Quaternion.LookRotation(caster.GetOwnerController().GetMouseLocationToGround() - caster.GetBodyPosition());
			float step = caster.unitInfo.turnRate * Time.fixedDeltaTime;
			caster.body.transform.rotation = Quaternion.RotateTowards(caster.body.transform.rotation, wantedRotation, step);*/
		}
	}

	public bool OnHitEnemy(UnitController unit)
	{ 
		Debug.Log("Cone hit enemy");
		unit.ReceivesDamage(damage, caster);
		return false;
	}

	public bool OnHitAlly(UnitController unit)
	{
		Debug.Log("Cone hit ally");
		unit.ReceivesDamage(damage / 2, caster);
		return false;
	}

	public bool OnHitTree(TreeHandler treeHandler, GameObject tree)
	{
		treeHandler.DestroyTree(tree);
		return false;
	}
}
