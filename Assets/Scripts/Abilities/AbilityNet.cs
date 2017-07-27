using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNet : AbilityController {

	public GameObject projectilePrefab;

	public override void Cast() {
		RaycastHit hit;
		Ray ray = (Ray)Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 100)) {
			GameObject projectileObject = Instantiate(projectilePrefab,
				caster.attackInfo.spawnerObject.transform.position,
				caster.attackInfo.spawnerObject.transform.rotation,
				caster.transform);

			GrenadeBehaviour projectile = projectileObject.GetComponent<GrenadeBehaviour>();
			projectile.Initialize(caster, hit.point);
		}
	}

}
