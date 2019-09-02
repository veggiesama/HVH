using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour {

	public BodyController body;
	private Vector3 camOffset;
	private Vector3 camVelocity;
	private bool wasZooming;
	private Vector3 mouseOffset;

	//public bool followTightly = false;

	//public float innerOffsetRingRadius;
	//public float outerOffsetRingRadius;
	//public float effectiveVerticalSpace;
	
	[Range(0, 3)] public float smoothDampTime = 0.3f;
	[Range(0, 3)] public float zoomedSmoothDampTime = 0.1f;
	[Range(0, 20)] public float zoomedDistanceMultiplier = 10f;
	[Range(0, 0.5f)] public float zoomRegionWidthPercentage = 0.4f;
	[Range(0, 0.5f)] public float zoomRegionHeightPercentage = 0.4f;

	private Camera cam;
	private Player player;

	//private string debugStr = "";

	// TODO: recalculations needed whenever resolution changes
	void Start () {
		camOffset = transform.position - body.transform.position;
		cam = GetComponent<Camera>();
		player = GetComponentInParent<Player>();

		camVelocity = Vector3.zero;
		mouseOffset = Vector3.zero;
	}



	private void LateUpdate() {
		int halfScreenHeight = Screen.height/2;
		int halfScreenWidth = Screen.width/2;

		int mouseX = (int) Mathf.Clamp(Input.mousePosition.x, 0, Screen.width);
		int mouseY = (int) Mathf.Clamp(Input.mousePosition.y, 0, Screen.height);

		int minX = (int) (Screen.width * zoomRegionWidthPercentage);
		int minY = (int) (Screen.height * zoomRegionHeightPercentage);
		int maxX = Screen.width - minX;
		int maxY = Screen.height - minY;
			   
		Vector3 target = body.transform.position + camOffset;

		float t;

		if (player.IsCtrlZooming()) {
			if (!wasZooming) {
				wasZooming = true;
				mouseOffset = Vector3.zero;
				if (Input.mousePosition.x > maxX) {
					mouseOffset += Vector3.left;
					mouseOffset += Vector3.down * 0.2f; // offset applied to counteract camera -15f Y rotation
				}
				else if (Input.mousePosition.x < minX) {
					mouseOffset += Vector3.right;
					mouseOffset += Vector3.up * 0.2f;
				}

				if (Input.mousePosition.y > maxY) {
					mouseOffset += Vector3.down;
					mouseOffset += Vector3.left * 0.2f;
				}
				else if (Input.mousePosition.y < minY) {
					mouseOffset += Vector3.up;
					mouseOffset += Vector3.left * 0.2f;
				}

				// switch from x,y to x,z grid
				mouseOffset.z = mouseOffset.y;
				mouseOffset.y = 0f;
			}

			t = zoomedSmoothDampTime;
			target = target - (mouseOffset * zoomedDistanceMultiplier);
		}

		else {
			t = smoothDampTime;
			wasZooming = false;
		}

		Vector3 vel = cam.velocity;
		transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, t);

	}


	/*
	
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

		Debug.Log(debugStr);

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

	*/
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