using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFire : AbilityController {

	public AttackRoutine attackRoutinePrefab;

	public override void Cast() {
		//base.Cast();

		UnitController target = caster.GetTarget(AbilityTargetTeams.ENEMY);

		if (target != null) {
			AttackRoutine atk = (AttackRoutine) Instantiate(attackRoutinePrefab, this.transform);
			atk.Initialize(caster, target);
		}
	}
}
