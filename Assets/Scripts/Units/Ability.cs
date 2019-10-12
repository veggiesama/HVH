using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ability : ScriptableObject {

	public string abilityName;
	public Sprite icon;
	public bool isItem = false;
	[HideInInspector] public UnitController caster, allyTarget, enemyTarget;
	[HideInInspector] public UnitInfo unitInfo;
	[HideInInspector] public AbilityManager abilityManager;
	[HideInInspector] public NetworkHelper networkHelper;
	//[HideInInspector] public Player player;
	[HideInInspector] public Owner owner;
	//[HideInInspector] public Vector3 targetLocation;
	protected bool isEmptyAbility = false;
	protected Order castOrder;

	public bool isPassive = false;
	public AbilityTargetTypes targetType = AbilityTargetTypes.NONE;
	public AbilityTargetTeams targetTeam = AbilityTargetTeams.NONE;
	public DamageTypes damageType = DamageTypes.NONE;
	public int damage = 0;
	public float cooldown = 0;
	public float castRange = 0;
	public float castTime = 0; 
	public float duration = 0;
	public float aoeRadius = 0;
	public bool quickCast = false;
	public bool doNotCancelOrderQueue = false;

	private float cooldownTimeRemaining = 0.0f;
	[HideInInspector] public float durationRemaining = 0;

	[Header("Projectile")]
	public GameObject projectilePrefab = null;
	public BodyLocations projectileSpawner = BodyLocations.NONE;
	public ProjectileBehaviourTypes projectileBehaviour = ProjectileBehaviourTypes.NONE;
	public float projectileSpeed = 0;
	public float projectileTimeAlive = 0;
	public float projectileTimeUntilFullGrowth = 0;
	public Vector3 projectileGrowthFactor = Vector3.one;
	public float grenadeTimeToHitTarget = 0;
	public bool grenadeLerpTimeByCastRange = false;
	public float homingRotationalSpeed = 0;

	public abstract void Reset();

	// Use this for initialization
	public virtual void Initialize(GameObject obj) {
		//this.Ability = obj.GetComponentInChildren<Ability>();
		this.abilityManager = obj.GetComponent<AbilityManager>();
		this.caster = obj.GetComponentInParent<UnitController>();
		this.owner = obj.GetComponentInParent<Owner>();
		//this.player = obj.GetComponentInParent<Player>();
		this.networkHelper = owner.GetComponent<NetworkHelper>();

		caster.onCastAbility.AddListener(OnCastAbility);
		caster.onMoved.AddListener(OnMoved);
		caster.onTakeDamage.AddListener(OnTakeDamage);
	}

	public virtual void OnDisable() {
		if (caster == null) return;
		caster.onCastAbility.RemoveListener(OnCastAbility);
		caster.onMoved.RemoveListener(OnMoved);
		caster.onTakeDamage.RemoveListener(OnTakeDamage);
	}

	public virtual void OnLearn() {}
	public virtual void OnUnlearn() {}

	public virtual void Update() {
		if (cooldownTimeRemaining > 0) 
			cooldownTimeRemaining -= Time.deltaTime;
		if (durationRemaining > 0) {
			//Debug.Log(durationRemaining + "/" + duration);
			durationRemaining -= Time.deltaTime;
			if (durationRemaining <= 0)
				OnDurationEnd();
		}
	}

	public virtual void FixedUpdate() {}

	public virtual CastResults Cast(Order castOrder) {
		durationRemaining = duration;
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

	protected virtual void OnDurationEnd() {}
	protected virtual void OnCastAbility() {}
	protected virtual void OnTakeDamage(float dmg) {}
	protected virtual void OnMoved() {}

	public void SetCooldown(float sec) {
		cooldownTimeRemaining = sec;
	}

	public float GetCooldown() {
		return cooldownTimeRemaining;
	}

	public void StartCooldown() {
		cooldownTimeRemaining = cooldown;
	}

	public bool IsEmptyAbility() {
		return isEmptyAbility;
	}

	public bool IsPassive() {
		return isPassive;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Network Helper functions
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public virtual void PlayAnimation(Animations anim) {
		networkHelper.PlayAnimation(anim);
	}

	public virtual void CreateProjectile(Ability ability, Order castOrder) {
		networkHelper.CreateProjectile(ability, castOrder);
	}

	public virtual void CreateAOEGenerator(Ability ability, Order castOrder) {
		networkHelper.CreateAOEGenerator(ability, castOrder);
	}

	public virtual void InstantiateParticleOnUnit(GameObject prefab, UnitController unit, BodyLocations loc, float duration = 0f, float radius = 0f) {
		NetworkParticle np = new NetworkParticle(prefab.name, unit.networkHelper.netId, (int) loc, duration, radius);
		networkHelper.InstantiateParticle(np);
	}

	public virtual void InstantiateParticleAtLocation(GameObject prefab, Vector3 location, Quaternion rotation, float duration = 0f, float radius = 0f) {
		NetworkParticle np = new NetworkParticle(prefab.name, location, rotation, duration, radius);
		networkHelper.InstantiateParticle(np);
	}



}
 