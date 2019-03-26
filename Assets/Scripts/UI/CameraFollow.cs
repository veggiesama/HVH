using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour {

	public UnitController unit;
	private Transform targetTransform;
	private Vector3 camOffset;
	private Vector3 smoothingVelocity = Vector3.zero;

	public bool followTightly = false;

	public float innerOffsetRingRadius;
	public float outerOffsetRingRadius;
	//public float effectiveVerticalSpace;
	
	public float smoothDampTime = 0.1f;
	public float distanceMultiplier = 0.02f;

	private string debugStr = "";

	// TODO: recalculations needed whenever resolution changes
	void Start () {
		targetTransform = unit.body.transform;
		camOffset = transform.position - targetTransform.position;
	}

	private void FollowTightly() {
		transform.position = targetTransform.position + camOffset;
	}

	void LateUpdate () {
		if (followTightly) {
			FollowTightly();
			return;
		}

		debugStr = "Camera mouse boundary. ";
		float halfScreenHeight = Screen.height/2;
		float halfScreenWidth = Screen.width/2;

		Vector3 screenMiddle = new Vector3(halfScreenWidth, halfScreenHeight);
		Vector3 midOffset = screenMiddle - Input.mousePosition; // controllable by player

		// switch from x,y to x,z grid
		midOffset.z = midOffset.y;
		midOffset.y = 0f;

		// clamp to screen boundaries
		midOffset.x = Mathf.Clamp(midOffset.x, -halfScreenWidth, halfScreenWidth);
		midOffset.z = Mathf.Clamp(midOffset.z, -halfScreenHeight, halfScreenHeight);

		// sets anything inside eclipse to no movement
		if (IsPointInsideEllipse(midOffset.x, midOffset.z, innerOffsetRingRadius, innerOffsetRingRadius)) {
			midOffset.x = 0;
			midOffset.z = 0;
			debugStr = debugStr + "INSIDE: true. ";
		}

		// clamps to rectangle
		if (!IsPointInsideEllipse(midOffset.x, midOffset.z, outerOffsetRingRadius, outerOffsetRingRadius)) {
			midOffset.x = Mathf.Clamp(midOffset.x, -outerOffsetRingRadius, outerOffsetRingRadius);
			midOffset.z = Mathf.Clamp(midOffset.z, -outerOffsetRingRadius, outerOffsetRingRadius);
			debugStr = debugStr + "OUTSIDE: true. ";
		}

		//Debug.Log(debugStr);

		Vector3 target = targetTransform.position + camOffset - (midOffset * distanceMultiplier);
		transform.position = Vector3.SmoothDamp(transform.position, target, ref smoothingVelocity, smoothDampTime);

		//transform.position = Vector3.Lerp(transform.position, target, 2.0f * Time.deltaTime);

		//Debug.DrawRay(targetTransform.position, Vector3.up * 5, Color.red);
		//Debug.DrawRay(target, Vector3.up * 5, Color.red);
		//Debug.DrawLine(target, targetTransform.position, Color.cyan);

	}

	// https://www.mathopenref.com/coordgeneralellipse.html
	// https://www.geeksforgeeks.org/check-if-a-point-is-inside-outside-or-on-the-ellipse/
	private bool IsPointInsideEllipse(float a, float b, float radiusX, float radiusZ) {
		if ( Mathf.Pow(a, 2) / Mathf.Pow(radiusX, 2) + Mathf.Pow(b, 2) / Mathf.Pow(radiusZ, 2) <= 1 )
			return true;
		return false;
	}
}


/*
		// inner ring of no middle offset
		midOffset.x = (Mathf.Abs(midOffset.x) < innerOffsetRingRadius) ? 0f : midOffset.x;
		midOffset.z = (Mathf.Abs(midOffset.z) < innerOffsetRingRadius*effectiveVerticalSpace) ? 0f : midOffset.z;

		// clamp max values to effective screen space
		midOffset.x = Mathf.Clamp(midOffset.x, -outerOffsetRingRadius, outerOffsetRingRadius);
		midOffset.z = Mathf.Clamp(midOffset.z, -outerOffsetRingRadius*effectiveVerticalSpace, outerOffsetRingRadius*effectiveVerticalSpace);
		midOffset.z = midOffset.z * (2 - effectiveVerticalSpace);
		*/