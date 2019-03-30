using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPosition : Cast {
	public void Initialize(GameObject obj, Ability ability, Vector3 targetLocation) {
		base.Initialize(obj, ability, targetLocation, null, null, null);
		this.orderType = OrderTypes.CAST_POSITION;
	}
}
