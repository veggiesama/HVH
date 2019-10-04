using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBehaviour : ProjectileBehaviour {

	public override void Initialize(Ability ability, UnitController target) {
		base.Initialize(ability, target);
		rb.isKinematic = true;
	}

	protected override void FixedUpdate () {
		//if (!CanUpdate()) return;
		if (!initialized) return;

		base.FixedUpdate();

		// Homing and looking at
		//rb.transform.LookAt(target.body.transform);
		//rb.velocity = transform.forward * projectileSpeed;

		transform.Rotate(0, homingRotationalSpeed * Time.fixedDeltaTime, 0);
		transform.position = Vector3.MoveTowards(transform.position, target.body.transform.position, projectileSpeed * Time.fixedDeltaTime);
	}

}
