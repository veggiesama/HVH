using UnityEngine;
using System.Collections;

public static class AiUtil {

	public static bool DetectNearbyEnemies(UnitController unit, float radius) {
		Vector3 pos = unit.GetBodyPosition();
		Collider[] bodiesInRadius = Physics.OverlapSphere(pos, radius, (int) LayerMasks.BODY);

		foreach (Collider col in bodiesInRadius) {
			UnitController target = col.gameObject.GetComponent<BodyController>().unit;
			if (!target.SharesTeamWith(unit) && target.IsAlive() && !target.body.IsVisibleToUnit(unit)) {
				return true;
			}
		}

		return false;
	}
}
