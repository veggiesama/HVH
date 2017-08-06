using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLeap : AbilityController {

	//public float forceUpwards;
	//public float forceForwards;

	public float leapDistance;
	private Rigidbody rb;

	public override bool Cast() {
		if (!base.Cast()) return false;

		if (!caster.IsReadyForNav()) return false;

		// stop current cmds

		float offset_y = caster.body.GetComponent<Collider>().bounds.extents.y;

		Vector3 casterPosition = caster.body.transform.position;
		Vector3 casterForward = caster.body.transform.forward;
		Vector3 finalPosition = casterPosition + (casterForward * leapDistance);
		finalPosition.y = Terrain.activeTerrain.SampleHeight(finalPosition) + Terrain.activeTerrain.GetPosition().y + offset_y;
		Vector3 velocityVector = Util.CalculateBestLaunchSpeed(casterPosition, finalPosition, duration);

		Debug.DrawRay(casterPosition, velocityVector, Color.blue, 10.0f);
		Debug.DrawLine(casterPosition, finalPosition, Color.yellow, 10.0f);

		Debug.DrawRay(finalPosition, Vector3.up, Color.green, 10.0f);

		rb = caster.body.GetComponent<Rigidbody>();
		caster.StartJump();
		rb.AddForce(velocityVector, ForceMode.VelocityChange);
		//anim, disjoint, immunity

		Invoke("Land", duration);
		return true;
	}

	private void Land() {
		//rb.velocity = Vector3.zero;
		//rb.angularVelocity = Vector3.zero;
		caster.EndJump();
	}
}