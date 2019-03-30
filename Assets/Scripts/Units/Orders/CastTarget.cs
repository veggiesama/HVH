using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastTarget : Cast {
	public void Initialize(GameObject obj, Ability ability, UnitController allyTarget, UnitController enemyTarget) {
		base.Initialize(obj, ability, default, allyTarget, enemyTarget, null);
		this.orderType = OrderTypes.CAST_TARGET;
	}
}
