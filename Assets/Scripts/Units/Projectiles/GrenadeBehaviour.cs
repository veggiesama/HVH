using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GrenadeBehaviour : ProjectileBehaviour {
	public float rotationalSpeed;

	// should projectiles even have NetworkTransform?
	// are they deterministic? are they always?
	// should the AddForce be client-side and not localplayer-only? Move it to Start?

	public override void Initialize(Ability ability, Vector3 targetLocation) {
		base.Initialize(ability, targetLocation);

		if (grenadeLerpTimeByCastRange) {
			float targetDistance = Util.GetDistanceIn2D(attacker.GetBodyPosition(), targetLocation);
			grenadeTimeToHitTarget = Mathf.Lerp(grenadeTimeToHitTarget / 2, grenadeTimeToHitTarget, targetDistance / ability.castRange);
		}

		Vector3 throwVector = Util.CalculateBestLaunchSpeed(
			attacker.GetBodyPosition(),
			targetLocation,
			grenadeTimeToHitTarget);

		rb.AddForce(throwVector, ForceMode.VelocityChange);

		// prevent early triggering
		if (HasControllableAuthority()) {
			GetComponent<BoxCollider>().enabled = false;
			StartCoroutine(EnableCollider(grenadeTimeToHitTarget * 0.75f));
		}
	}

	private IEnumerator EnableCollider(float delay) {
		yield return new WaitForSeconds(delay);
		GetComponent<BoxCollider>().enabled = true;
	}

	protected override void FixedUpdate () {
		if (!initialized) return;
 		base.FixedUpdate();
		transform.Rotate(Vector3.up, rotationalSpeed * Time.fixedDeltaTime); // TODO: switch to Time.fixedDeltaTime?
	}

}