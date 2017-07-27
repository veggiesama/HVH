using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBehaviour : MonoBehaviour {
	protected UnitController target, attacker;
	protected GameObject targetObject;
	protected Vector3 targetLocation;

	protected Rigidbody rb;
	protected AttackInfo attackInfo;
	protected bool hasCollided = false;

	// Use this for initialization
	protected virtual void Start () {
		rb = GetComponent<Rigidbody>();
		Invoke( "DestroySelf", Constants.ProjectileSelfDestructTime );
	}

	protected abstract void FixedUpdate();

	// target unit
	public virtual void Initialize(UnitController attacker, UnitController target) {
		this.attacker = attacker;
		this.target = target;
		this.targetObject = target.body.gameObject;
	}

	// target location
	public virtual void Initialize(UnitController attacker, Vector3 targetLocation) {
		this.attacker = attacker;
		this.targetLocation = targetLocation;
	}

	public void DestroySelf() {
		Debug.Log("Projectile self-destructed due to time-out.");
		Destroy(this.gameObject);
	}
}
