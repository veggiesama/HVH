using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBehaviour : MonoBehaviour {
	protected Ability ability;
	protected UnitController target, attacker;
	protected GameObject targetObject;
	protected Vector3 targetLocation;
	protected Vector3 growthFactor;

	protected Rigidbody rb;
	protected bool hasCollided = false;

	protected float projectileSpeed, immobileDuration, grenadeTimeToHitTarget;
	protected bool destroyOnHit;

	protected float currentTimer = 0;
	protected Vector3 originalScale;

	// Use this for initialization
	protected virtual void Start () {
		rb = GetComponent<Rigidbody>();
	}

	// target none
	public virtual void Initialize(Ability ability) {
		this.ability = ability;
		this.attacker = ability.caster;
		this.originalScale = transform.localScale;

		float timeAlive = ability.projectileTimeAlive;
		if (timeAlive <= 0) timeAlive = Constants.ProjectileSelfDestructTime; // fallback
		Invoke( "DestroySelf", timeAlive);
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
		this.projectileSpeed = ability.projectileSpeed;
	}

	// only useful for cones
	public virtual void SetGrowthFactor(Vector3 growthFactor) {
		this.growthFactor = growthFactor;
	}

	public void DestroySelf() {
		//Debug.Log("Projectile self-destructed due to time-out.");
		Destroy(this.gameObject);
	}

	private IProjectileAbility GetIProjectileAbility() {
		if (ability is IProjectileAbility)
			return (IProjectileAbility)ability;
		return null;
	}

	protected virtual void FixedUpdate () {
		currentTimer += Time.fixedDeltaTime;
		if (growthFactor != default) {
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
			//TreeHandler handler = other.GetComponentInParent<TreeHandler>();
			proj.OnHitTree(tree);
			return;
		}

		if (!Util.IsBody(other.gameObject))
			return; //Debug.Log("Collided with non-body.");

		if (attacker.body.gameObject == other.gameObject)
			return; //Debug.Log("Clipping self.");

		target = other.gameObject.GetComponentInParent<UnitController>();

		if (target.GetTeam() == attacker.GetTeam())
			destroyOnHit = proj.OnHitAlly(target); // collided with ally
		else
			destroyOnHit = proj.OnHitEnemy(target); // collided with enemy
		
		if (destroyOnHit) DestroySelf();
	}
}
