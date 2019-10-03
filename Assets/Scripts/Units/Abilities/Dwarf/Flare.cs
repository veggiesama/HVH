using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dwarf/Flare")]
public class Flare : Ability, IAoeGeneratorAbility {

	[Header("AOE Generator")]
	public GameObject aoeGeneratorPrefab;
	public GameObject aoeGeneratorParticlePrefab;
	public float reappliesEvery;
	public float delay = 2f;
	public bool destroysTrees = true;
	public StatusEffect[] statusEffects;

	[Header("Flare")]
	public GameObject particleLaunch;

	public override void Reset() {
		abilityName = "Flare";
		targetType = AbilityTargetTypes.AREA;
		targetTeam = AbilityTargetTeams.ENEMY;
		damageType = DamageTypes.NONE;
		damage = 0;
		cooldown = 1.0f;
		castRange = 30f;
		castTime = 0f;
		duration = 6f;
		aoeRadius = 4f;
		quickCast = false;
		doNotCancelOrderQueue = false;
	}

	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;

		Vector3 casterHead = Util.GetBodyLocationTransform(BodyLocations.HEAD, caster).position;
		InstantiateParticleAtLocation(particleLaunch, casterHead, particleLaunch.transform.rotation);
		//var aoeGenerator = Instantiate(aoeGeneratorPrefab, castOrder.targetLocation, default, caster.transform).GetComponent<AOEGenerator>();
		//aoeGenerator.Initialize(caster, this);

		CreateAOEGenerator(this, castOrder);

		return CastResults.SUCCESS;
	}

	public GameObject GetAoeGenParticlePrefab() {
		return aoeGeneratorParticlePrefab;
	}

	public bool GetDestroysTrees() {
		return destroysTrees;
	}

	public float GetReappliesEvery() {
		return reappliesEvery;
	}

	public StatusEffect[] GetStatusEffects() {
		return statusEffects;
	}

	public float GetDelay() {
		return delay;
	}
}
