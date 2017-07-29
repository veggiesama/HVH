using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityController : MonoBehaviour {

	[HideInInspector] public AbilityInfo abilityInfo;
	[HideInInspector] public UnitController caster, allyTarget, enemyTarget;
	[HideInInspector] public UnitInfo unitInfo;

	public float cooldownTimeRemaining = 0.0f;

	// Use this for initialization
	protected virtual void Start () {
		abilityInfo = GetComponent<AbilityInfo>();
		caster = GetComponentInParent<UnitController>();
		unitInfo = GetComponentInParent<UnitInfo>();
	}
	public virtual void Update() {
		if (cooldownTimeRemaining > 0) 
			cooldownTimeRemaining -= Time.deltaTime;
	}

	public virtual void Cast() {
		allyTarget = caster.GetTarget(AbilityTargetTeams.ALLY);
		enemyTarget = caster.GetTarget(AbilityTargetTeams.ENEMY);
		cooldownTimeRemaining = abilityInfo.cooldown;
	}

}
