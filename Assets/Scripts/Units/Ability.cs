using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject {

	public string abilityName;
	[HideInInspector] public UnitController caster, allyTarget, enemyTarget;
	[HideInInspector] public UnitInfo unitInfo;
	[HideInInspector] public AbilityManager abilityManager;
	//[HideInInspector] public Vector3 targetLocation;
	protected bool isEmptyAbility = false;
	protected Order castOrder;

	public AbilityTargetTypes targetType;
	public AbilityTargetTeams targetTeam;
	public DamageTypes damageType;
	public float damage = 0;
	public float cooldown = 0;
	public float castRange = 0;
	public float castTime = 0; 
	public float duration = 0;
	public float aoeRadius = 0;
	public bool quickCast = false;
	public bool doNotCancelOrderQueue = false;

	private float cooldownTimeRemaining = 0.0f;
	protected float durationRemaining = 0;

	[Header("Projectile")]
	public GameObject projectilePrefab = null;
	public float projectileSpeed = 0;
	public float projectileTimeAlive = 0;
	public float grenadeTimeToHitTarget = 0;

	public abstract void Reset();

	// Use this for initialization
	public virtual void Initialize(GameObject obj) {
		//this.Ability = obj.GetComponentInChildren<Ability>();
		this.abilityManager = obj.GetComponent<AbilityManager>();
		this.caster = obj.GetComponentInParent<UnitController>();
		this.unitInfo = obj.GetComponentInParent<UnitInfo>();
	}

	public virtual void Learn() {}
	public virtual void Unlearn() {}

	public virtual void Update() {
		if (cooldownTimeRemaining > 0) 
			cooldownTimeRemaining -= Time.deltaTime;
		if (durationRemaining > 0) {
			durationRemaining -= Time.deltaTime;
			if (durationRemaining <= 0)
				OnDurationEnd();
		}
	}

	public virtual void FixedUpdate() {}

	public virtual CastResults Cast(Order castOrder) { 
		this.castOrder = castOrder;
		return CastResults.SUCCESS;
	}

	public bool IsCooldownReady() {
		return cooldownTimeRemaining <= 0;
	}

	public bool IsProjectile() {
		return projectilePrefab != null;
	}

	public bool IsTargetLocationInRange(Vector3 targetLocation) {
		if (castRange == 0) return true;

		if (Util.IsNullVector(targetLocation)) {
			//print("Invalid target location");
			return false;
		}
		else if (Util.GetDistanceIn2D(caster.GetBodyPosition(), targetLocation) > castRange) {
			//print("Target location out of cast range.");
			return false;
		}

		return true;
	}

	public bool IsTargetInRange() {
		if (castRange == 0) return true;

		switch (targetType) {
			case AbilityTargetTypes.NONE:
				break;
			case AbilityTargetTypes.UNIT:
				switch (targetTeam)
				{
					case AbilityTargetTeams.ALLY:
						if (!allyTarget) {
							Debug.Log("No ally selected.");
							return false;
						}
						else if (Util.GetDistanceIn2D(caster.GetBodyPosition(), allyTarget.GetBodyPosition()) < castRange) {
							Debug.Log("Target ally out of cast range.");
							return false;
						}
						break;

					case AbilityTargetTeams.ENEMY:
						if (!enemyTarget) {
							Debug.Log("No enemy selected.");
							return false;
						}
						else if (Util.GetDistanceIn2D(caster.GetBodyPosition(), enemyTarget.GetBodyPosition()) > castRange) {
							Debug.Log("Target enemy out of cast range.");
							return false;
						}
						break;
					
					case AbilityTargetTeams.BOTH:
						if (!allyTarget && !enemyTarget) {
							Debug.Log("Neither ally nor enemy selected.");
							return false;
						}
						// nothing checks for range yet
						break;
	
					default:
						break;
				}
				break;
			/*
			case AbilityTargetTypes.POINT:
				if (Util.IsNullVector(targetLocation)) {
					print("Invalid target location");
					return false;
				}
				else if (Util.GetDistanceIn2D(caster.GetBodyPosition(), targetLocation) > castRange) {
					print("Target location out of cast range.");
					return false;
				}
				break;
			case AbilityTargetTypes.PASSIVE:
				break;
			case AbilityTargetTypes.CHANNELLED:
				break;
			case AbilityTargetTypes.SHAPE:
				break;*/
			default:
				Debug.Log("Unexpected AbilityTargetType value.");
				return false;
		}

		return true;
	}

	// enables OnDurationEnd()
	protected void TrackDuration() {
		durationRemaining = duration;
	}

	protected virtual void OnDurationEnd() {}

	public float GetCooldown() {
		return cooldownTimeRemaining;
	}

	public void StartCooldown() {
		cooldownTimeRemaining = cooldown;
	}

	public bool IsEmptyAbility() {
		return isEmptyAbility;
	}
}
 