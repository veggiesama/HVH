using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeBehaviour : ProjectileBehaviour {

	public override void Initialize(Ability ability, Vector3 targetLocation) {
		base.Initialize(ability, targetLocation);
		rb.transform.LookAt(targetLocation);
	}

	protected override void FixedUpdate () {
		if (!CanUpdate()) return;
		base.FixedUpdate();
		rb.position = attacker.body.mouth.transform.position;
		rb.rotation = attacker.body.mouth.transform.rotation;
	}
}
