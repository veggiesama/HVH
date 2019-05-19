using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		return unit.GetBodyPosition() + new Vector3(rng.x, 0, rng.y) * distance;
	}

	public static bool IsBody(GameObject gameObject) {
		return gameObject.layer == (int)LayerBits.BODY;
	}

	public static bool IsTree(GameObject gameObject) {
		return gameObject.layer == (int)LayerBits.TREE;
		//return (gameObject.GetComponent<Tree>() != null);
	}

	public static bool IsTerrain(GameObject gameObject) {
		return gameObject.layer == (int)LayerBits.TERRAIN;
	}

	public static bool IsLocalPlayer(Player player) {
		return (player.gameObject.CompareTag("LocalPlayer"));
	}

	public static void DebugDrawVector(Vector3 position, Color color, float dur = 1f) {
		Debug.DrawLine(position, position + (Vector3.up * 2f), color, dur);
	}
}
