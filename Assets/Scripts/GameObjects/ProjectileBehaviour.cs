using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBehaviour : MonoBehaviour {
	protected AbilityController ability;
	protected UnitController target, attacker;
	protected GameObject targetObject;
	protected Vector3 targetLocation;

	protected Rigidbody rb;
	protected AttackInfo attackInfo;
	protected bool hasCollided = false;

	protected float projectileSpeed, timeToHitTarget, immobileDuration;

	// Use this for initialization
	protected virtual void Start () {
		rb = GetComponent<Rigidbody>();
		Invoke( "DestroySelf", Constants.ProjectileSelfDestructTime );
	}

	protected abstract void FixedUpdate();

	// target unit
	public virtual void Initialize(AbilityController ability, UnitController target) {
		this.ability = ability;
		this.attacker = ability.caster;
		this.target = target;
		this.targetObject = target.body.gameObject;
		this.projectileSpeed =  ((AbilityFire)ability).projectileSpeed;
	}

	// target location
	public virtual void Initialize(AbilityController ability, Vector3 targetLocation) {
		this.ability = ability;
		this.attacker = ability.caster;
		this.targetLocation = targetLocation;
		this.timeToHitTarget = ((AbilityNet)ability).timeToHitTarget;
	}

	public void DestroySelf() {
		Debug.Log("Projectile self-destructed due to time-out.");
		Destroy(this.gameObject);
	}
}
