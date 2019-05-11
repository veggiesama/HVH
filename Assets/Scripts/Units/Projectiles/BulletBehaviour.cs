using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : ProjectileBehaviour {
	protected bool hasRolledToMiss = false;
	protected bool missed = false;
	// bullets inherently miss 50% of the time when shooting through trees
	public static float treeMissChance = 50.0f;
	// prevents bullets from being aimed at the ground
	private float adjustTargetHeightBy = 1.0f;

	public override void Initialize(Ability ability, Vector3 targetLocation) {
		targetLocation = targetLocation + (Vector3.up * adjustTargetHeightBy);
		base.Initialize(ability, targetLocation);
		rb.transform.LookAt(targetLocation);
	}

	protected override void FixedUpdate () {
		if (!CanUpdate()) return;
		base.FixedUpdate();
		rb.velocity = transform.forward * projectileSpeed;
	}

	// if the calling ability has a tree miss chance, apply it here
	protected override void OnTriggerEnter(Collider col) {
		if (!hasAuthority) return;

		// collided with tree
		if (Util.IsTree(col.gameObject)) {
			if (treeMissChance > 0.0f && !hasRolledToMiss) {
				missed = RollMissChance();
				hasRolledToMiss = true;	
			}

			if (missed) {
				DestroySelf();
				return;
			}
		}

		base.OnTriggerEnter(col);
	}

	private bool RollMissChance() {
		float rng = Random.Range(0f, 1f); //Debug.Log("RNG: " + rng);
		if (rng <= treeMissChance) return true;
		return false;
	}
}
