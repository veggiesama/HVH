using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBehaviour : ProjectileBehaviour {

	public override void Initialize(Ability ability, Vector3 targetLocation) {
		base.Initialize(ability, targetLocation);
		rb.velocity = transform.forward * projectileSpeed;
	}

	protected override void FixedUpdate () {
		//if (!CanUpdate()) return;
		if (!initialized) return;

		base.FixedUpdate();
		rb.transform.LookAt(target.body.transform);
	}

	protected override void OnTriggerEnter(Collider other) {
		if (!other.gameObject.Equals(targetObject)) { 
			return; //Debug.Log("Passed through wrong target.");
		}

		if (hasCollided) {
			return; //Debug.Log("Double-tap!");
		}

		hasCollided = true;
		networkHelper.DealDamageTo(target, ability.damage);
		Destroy(this.gameObject);
	}
}
