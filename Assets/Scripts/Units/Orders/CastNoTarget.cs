using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastNoTarget : Cast {
	public void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability, default, null, null, null);
		this.orderType = OrderTypes.CAST_NO_TARGET;
	}
}
