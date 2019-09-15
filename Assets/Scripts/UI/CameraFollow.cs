using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


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
		cam = GetComponent<Camera>();
		player = GetComponentInParent<Player>();
	}

	public void Initialize() {
		camOffset = transform.position - body.transform.position;
		mouseOffset = Vector3.zero;
	}

	private void LateUpdate() {
		int minX = (int) (Screen.width * zoomRegionWidthPercentage);
		int minY = (int) (Screen.height * zoomRegionHeightPercentage);
		int maxX = Screen.width - minX;
		int maxY = Screen.height - minY;
			   
		Vector3 target = body.transform.position + camOffset;

		float t;

		if (player.IsCtrlZooming()) {
			Vector2 mousePos = Mouse.current.position.ReadValue();

			if (!wasZooming) {
				wasZooming = true;
				mouseOffset = Vector3.zero;
				if (mousePos.x > maxX) {
					mouseOffset += Vector3.left;
					mouseOffset += Vector3.down * 0.2f; // offset applied to counteract camera -15f Y rotation
				}
				else if (mousePos.x < minX) {
					mouseOffset += Vector3.right;
					mouseOffset += Vector3.up * 0.2f;
				}

				if (mousePos.y > maxY) {
					mouseOffset += Vector3.down;
					mouseOffset += Vector3.left * 0.2f;
				}
				else if (mousePos.y < minY) {
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
}