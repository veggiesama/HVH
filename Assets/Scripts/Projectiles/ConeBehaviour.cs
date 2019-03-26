using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeBehaviour : ProjectileBehaviour {
	protected override void Start() {
		base.Start();
		rb.transform.LookAt(targetLocation);
	}

	protected override void FixedUpdate () {
		rb.position = attacker.attackInfo.spawnerObject.transform.position;
		rb.rotation = attacker.attackInfo.spawnerObject.transform.rotation;
	}
}
