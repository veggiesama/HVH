using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : ProjectileBehaviour {

	public float projectileSpeed;

	//public override void Initialize(UnitController attacker, UnitController target) {
	//	base.Initialize(attacker, target);
	//}

	protected override void FixedUpdate () {
		rb.transform.LookAt(target.body.transform);
		rb.velocity = transform.forward * projectileSpeed;
	}

	private void OnTriggerEnter(Collider other)
	{
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
