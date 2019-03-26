using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : ProjectileBehaviour {
	public float rotationalSpeed;
	private Transform childTransform;

	protected override void Start() {
		base.Start();

		childTransform = transform.GetChild(0);

		Vector3 throwVector = Util.CalculateBestLaunchSpeed(
			attacker.GetBodyPosition(),
			targetLocation,
			grenadeTimeToHitTarget);

		rb.AddForce(throwVector, ForceMode.VelocityChange);

		// prevent early triggering
		GetComponent<BoxCollider>().enabled = false;
		Invoke("EnableCollider", grenadeTimeToHitTarget * 0.75f);
	}

	private void EnableCollider() {
		GetComponent<BoxCollider>().enabled = true;
	}

	protected override void FixedUpdate () {
		childTransform.Rotate(Vector3.up, rotationalSpeed * Time.deltaTime); // TODO: switch to Time.fixedDeltaTime?

	}

}