using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Util {

	// http://answers.unity3d.com/questions/248788/calculating-ball-trajectory-in-full-3d-world.html
	public static Vector3 CalculateBestLaunchSpeed(Vector3 origin, Vector3 destination, float timeToTarget) {
		// calculate vectors
		Vector3 toTarget = destination - origin;
		Vector3 toTargetXZ = toTarget;
		toTargetXZ.y = 0;
     
		// calculate xz and y
		float y = toTarget.y;
		float xz = toTargetXZ.magnitude;
     
		// calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
		// where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
		// so xz = v0xz * t => v0xz = xz / t
		// and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
		float t = timeToTarget;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;
     
		// create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;        // get direction of xz but with magnitude 1
		result *= v0xz;                                // set magnitude of xz to v0xz (starting speed in xz plane)
		result.y = v0y;                                // set y to v0y (starting speed of y plane)
     
		return result;
	}

	public static float GetDistanceIn2D(Vector3 pos1, Vector3 pos2) {
		pos1.y = 0;
		pos2.y = 0;
		return Vector3.Distance(pos1, pos2);
	}

	public static Vector3 GetNullVector() {
		return Vector3.zero;
	}

	public static bool IsNullVector(Vector3 vec) {
		return vec.Equals(Vector3.zero);
	}

	public static Vector3 GetRandomVectorAround(UnitController unit, float distance) {
		Vector2 rng = Random.insideUnitCircle;
		Vector3 newPosition = unit.GetBodyPosition() + new Vector3(rng.x, 0, rng.y) * distance;
		NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 100f, -1);
		newPosition = SnapVectorToTerrain(hit.position);

		return newPosition;
	}

	public static Vector3 SnapVectorToTerrain(Vector3 loc) {
		loc.y = loc.y + 20f;

		Physics.Raycast(loc, Vector3.down, out RaycastHit downHit, 100f, (int)LayerMasks.TERRAIN);
		Physics.Raycast(loc, Vector3.up, out RaycastHit upHit, 100f, (int)LayerMasks.TERRAIN);

		if (downHit.collider != null)
			return downHit.point;
		else if (upHit.collider != null)
			return upHit.point;
		else
			return loc;
	}

	public static bool IsBody(GameObject gameObject) {
		return (gameObject.layer == (int)LayerBits.BODY || gameObject.layer == (int)LayerBits.BODY_IGNORINGTREES);
	}

	public static bool IsTree(GameObject gameObject) {
		return gameObject.layer == (int)LayerBits.TREE;
		//return (gameObject.GetComponent<Tree>() != null);
	}

	public static bool IsTerrain(GameObject gameObject) {
		return gameObject.layer == (int)LayerBits.TERRAIN;
	}

	//public static bool IsLocalPlayer(Player player) {
	//	return (player.gameObject.CompareTag("LocalPlayer"));
	//}

	public static void DebugDrawVector(Vector3 position, Color color, float dur = 1f) {
		Debug.DrawLine(position, position + (Vector3.up * 2f), color, dur);
	}

	public static Transform GetBodyLocationTransform(BodyLocations bodyLoc, UnitController u) {
		Transform trans;
		switch (bodyLoc) {
			case BodyLocations.HEAD:
				trans = u.body.head.transform;
				break;
			case BodyLocations.MOUTH:
				trans = u.body.mouth.transform;
				break;
			case BodyLocations.WEAPON:
				trans = u.body.projectileSpawner.transform;
				break;
			case BodyLocations.FEET:
				trans = u.body.feet.transform;
				break;
			default: // case BodyLocations.NONE:
				trans = u.body.transform;
				break;
		}
		return trans;
	}

	public static Color CreateFadedColor(Color originalColor) {
		return new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f);
	}

	public static List<UnitController> FindUnitsInSphere(Vector3 center, float radius, Teams team) {
		Collider[] colliders = Physics.OverlapSphere(center, radius, (int) LayerMasks.BODY);

		List<UnitController> unitList = new List<UnitController>();
		foreach (Collider col in colliders) {
			UnitController unit = col.gameObject.GetComponent<BodyController>().unit;
			if (team.HasFlag(unit.GetTeam()))
				unitList.Add(unit);
		}

		return unitList;
	}

	public static AbilitySlotTypes GetSlotType(AbilitySlots slot) {
		switch (slot) {
			case AbilitySlots.ATTACK:
				return AbilitySlotTypes.ABILITY; 
			case AbilitySlots.ABILITY_1:
				return AbilitySlotTypes.ABILITY; 
			case AbilitySlots.ABILITY_2:
				return AbilitySlotTypes.ABILITY; 
			case AbilitySlots.ABILITY_3:
				return AbilitySlotTypes.ABILITY; 
			case AbilitySlots.ABILITY_4:
				return AbilitySlotTypes.ABILITY; 
			case AbilitySlots.ABILITY_5:
				return AbilitySlotTypes.ABILITY; 
			case AbilitySlots.ABILITY_6:
				return AbilitySlotTypes.ABILITY; 
			case AbilitySlots.ITEM_1:
				return AbilitySlotTypes.ITEM; 
			case AbilitySlots.ITEM_2:
				return AbilitySlotTypes.ITEM; 
			case AbilitySlots.ITEM_3:
				return AbilitySlotTypes.ITEM; 
			case AbilitySlots.ITEM_4:
				return AbilitySlotTypes.ITEM; 
			case AbilitySlots.ITEM_5:
				return AbilitySlotTypes.ITEM; 
			case AbilitySlots.ITEM_6:
				return AbilitySlotTypes.ITEM; 
			case AbilitySlots.NONE:
				return AbilitySlotTypes.NONE; 
		}

		return AbilitySlotTypes.NONE;
	}
}