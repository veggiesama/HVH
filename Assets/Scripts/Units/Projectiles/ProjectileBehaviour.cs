using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class ProjectileBehaviour : NetworkBehaviour {
	protected bool initialized = false;
	protected Ability ability;
	protected UnitController target, attacker;
	protected NetworkHelper networkHelper;
	protected GameObject targetObject;
	protected Vector3 targetLocation;
	protected Vector3 growthFactor;

	protected Rigidbody rb;
	protected bool hasCollided = false;

	protected float projectileSpeed, immobileDuration, grenadeTimeToHitTarget;
	protected bool destroyOnHit;
	protected bool grenadeLerpTimeByCastRange;

	protected float currentTimer = 0;
	protected Vector3 originalScale;

	protected List<UnitController> alreadyTriggeredList = new List<UnitController>();

	// target none
	public virtual void Initialize(Ability ability) {
		this.rb = GetComponent<Rigidbody>();
		this.ability = ability;
		this.attacker = ability.caster;
		this.originalScale = transform.localScale;
		this.growthFactor = ability.projectileGrowthFactor;
		this.networkHelper = attacker.networkHelper;

		float timeAlive = ability.projectileTimeAlive;
		if (timeAlive <= 0) timeAlive = Constants.ProjectileSelfDestructTime; // fallback
		Invoke( "DestroySelf", timeAlive);
		this.initialized = true;
	}

	// target unit
	public virtual void Initialize(Ability ability, UnitController target) {
		Initialize(ability);
		this.target = target;
		this.targetObject = target.body.gameObject;
		this.projectileSpeed = ability.projectileSpeed;
	}

	// target location
	public virtual void Initialize(Ability ability, Vector3 targetLocation) {
		Initialize(ability);
		this.targetLocation = targetLocation;
		this.grenadeTimeToHitTarget = ability.grenadeTimeToHitTarget;
		this.grenadeLerpTimeByCastRange = ability.grenadeLerpTimeByCastRange;
		this.projectileSpeed = ability.projectileSpeed;
	}

	public void DestroySelf() {
		//Debug.Log("Projectile self-destructed due to time-out.");
		networkHelper.DestroyProjectile(this.gameObject);
	}

	private IProjectileAbility GetIProjectileAbility() {
		if (ability is IProjectileAbility)
			return (IProjectileAbility)ability;
		return null;
	}

	protected virtual void FixedUpdate () {
		currentTimer += Time.fixedDeltaTime;
		if (growthFactor != Vector3.one) {
			float t = Mathf.Min(currentTimer / ability.projectileTimeUntilFullGrowth, 1.0f);
			transform.localScale = new Vector3(
				Mathf.Lerp(originalScale.x, growthFactor.x, t),
				Mathf.Lerp(originalScale.y, growthFactor.y, t),
				Mathf.Lerp(originalScale.z, growthFactor.z, t)
			);
		}
	}

	protected virtual void OnTriggerEnter(Collider other) {
		IProjectileAbility proj = GetIProjectileAbility();
		if (proj == null) return; // not a projectile ability

		// collided with tree
		if (Util.IsTree(other.gameObject)) {
			Tree tree = other.gameObject.GetComponent<Tree>();
			proj.OnHitTree(tree);
			return;
		}

		if (!Util.IsBody(other.gameObject))
			return; //Debug.Log("Collided with non-body.");

		if (attacker.body.gameObject == other.gameObject)
			return; //Debug.Log("Clipping self.");

		target = other.gameObject.GetComponentInParent<UnitController>();
		if (alreadyTriggeredList.Contains(target))
			return; // Debug.Log("Already triggered against this target.");

		if (target.SharesTeamWith(attacker))
			destroyOnHit = proj.OnHitAlly(target); // collided with ally
		else
			destroyOnHit = proj.OnHitEnemy(target); // collided with enemy

		alreadyTriggeredList.Add(target);
		if (destroyOnHit) DestroySelf();
	}

	protected bool CanUpdate() {
		// only update if the player has authority to
		if (!hasAuthority) return false;
		// don't start update until player receives RPC from server and initializes the object
		if (!initialized) return false;
		return true;
	}
}
