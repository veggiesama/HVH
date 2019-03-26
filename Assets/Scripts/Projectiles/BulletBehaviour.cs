using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : ProjectileBehaviour {

	protected float treeMissChance = 0.0f;
	protected bool hasRolledToMiss = false;
	protected bool missed = false;

	private float adjustTargetHeightBy = 1.0f;

	protected override void Start() {
		base.Start();
		targetLocation = targetLocation + (Vector3.up * adjustTargetHeightBy);
		rb.transform.LookAt(targetLocation);
	}

	protected override void FixedUpdate () {
		rb.velocity = transform.forward * projectileSpeed;
	}

	public virtual void Initialize(Ability ability, Vector3 targetLocation, float treeMissChance) {
		base.Initialize(ability, targetLocation);
		this.treeMissChance = treeMissChance;
	}

	// if the calling ability has a tree miss chance, apply it here
	protected override void OnTriggerEnter(Collider other) {
		// collided with tree
		if (other.gameObject.CompareTag("Tree")) {
			if (treeMissChance > 0.0f && !hasRolledToMiss) {
				missed = RollMissChance();
				hasRolledToMiss = true;	
			}

			if (missed) {
				DestroySelf();
				return;
			}
		}

		base.OnTriggerEnter(other);
	}

	private bool RollMissChance() {
		float rng = Random.Range(0f, 1f); //Debug.Log("RNG: " + rng);
		if (rng <= treeMissChance) return true;
		return false;
	}
}
