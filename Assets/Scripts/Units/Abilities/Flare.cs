using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Flare")]
public class Flare : Ability {

	[Header("Flare")]
	//public Immobilized netImmobilizedStatus;
	public GameObject aoeGeneratorPrefab;
	private AOEGenerator aoeGenerator;
	public float reappliesEvery;
	public StatusEffect[] statusEffects;

	public override void Reset()
	{
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

		aoeGenerator = Instantiate(aoeGeneratorPrefab, castOrder.targetLocation, Quaternion.identity, caster.transform).GetComponent<AOEGenerator>();
		aoeGenerator.Initialize(caster, this, statusEffects, reappliesEvery, true);

		return CastResults.SUCCESS;
	}

}
