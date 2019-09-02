#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DebugStateVisualizer : MonoBehaviour
{
	public BodyController body;

	private void Reset() {
		body = GetComponentInParent<BodyController>();
	}

	private void OnDrawGizmos() {
		Vector3 offset = Vector3.up * 1.1f;
		string str = "VisibilityState: " + body.GetVisibilityState();
		Handles.Label(transform.position + offset, str);
	}
}
#endif