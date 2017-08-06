using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNet : AbilityController {

	public GameObject projectilePrefab;
	public float timeToHitTarget;

	public override bool Cast() {
		if (!base.Cast()) return false;

		GameObject projectileObject = Instantiate(projectilePrefab,
			caster.attackInfo.spawnerObject.transform.position,
			caster.attackInfo.spawnerObject.transform.rotation,
			caster.transform);

		GrenadeBehaviour projectile = projectileObject.GetComponent<GrenadeBehaviour>();
		projectile.Initialize(this, targetLocation);

		return true;
	}

}
