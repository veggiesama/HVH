using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBehaviour : ProjectileBehaviour {

	protected override void FixedUpdate () {
		base.FixedUpdate();
		rb.transform.LookAt(target.body.transform);
		rb.velocity = transform.forward * projectileSpeed;
	}

	protected override void OnTriggerEnter(Collider other) {
		if (!other.gameObject.Equals(targetObject)) { 
			return; //Debug.Log("Passed through wrong target.");
		}

		if (hasCollided) {
			return; //Debug.Log("Double-tap!");
		}

		hasCollided = true;
		target.ReceivesDamage(attacker.attackInfo.damage, attacker);
		Destroy(this.gameObject);
	}
}
