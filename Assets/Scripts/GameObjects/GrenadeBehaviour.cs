using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : ProjectileBehaviour {
	public float rotationalSpeed;
	private Transform childTransform;

	protected override void Start() {
		base.Start();

		childTransform = transform.GetChild(0);

		Vector3 throwVector = calculateBestThrowSpeed(
			attacker.GetBodyPosition(),
			targetLocation,
			timeToHitTarget);

		rb.AddForce(throwVector, ForceMode.VelocityChange);

		// prevent early triggering
		GetComponent<BoxCollider>().enabled = false;
		Invoke("EnableCollider", timeToHitTarget * 0.75f);
	}

	private void EnableCollider() {
		GetComponent<BoxCollider>().enabled = true;
	}

	protected override void FixedUpdate () {
		childTransform.Rotate(Vector3.up, rotationalSpeed * Time.deltaTime);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Body"))
			return; //Debug.Log("Not a body.");

		if (attacker.body.gameObject == other.gameObject)
			return; //Debug.Log("Clipping self.");

		if (hasCollided)
			return; //Debug.Log("Double-tap!");

		hasCollided = true;
		target = other.gameObject.GetComponentInParent<UnitController>();
		target.SetImmobile(ability.abilityInfo.duration);

		Destroy(this.gameObject);
	}

	// http://answers.unity3d.com/questions/248788/calculating-ball-trajectory-in-full-3d-world.html
	private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 destination, float timeToTarget) {
		// calculate vectors
		Vector3 toTarget = destination - origin;
		Vector3 toTargetXZ = toTarget;
		toTargetXZ.y = 0;
     
		// calculate xz and y
		float y = toTarget.y;
		float xz = toTargetXZ.magnitude;
     
		// calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
		// where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
		// so xz = v0xz * t => v0xz = xz / t
		// and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
		float t = timeToTarget;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;
     
		// create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;        // get direction of xz but with magnitude 1
		result *= v0xz;                                // set magnitude of xz to v0xz (starting speed in xz plane)
		result.y = v0y;                                // set y to v0y (starting speed of y plane)
     
		return result;
	}
}