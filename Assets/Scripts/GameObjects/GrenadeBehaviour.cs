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
		target.SetImmobile(ability.duration);

		Destroy(this.gameObject);
	}
}