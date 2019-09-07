using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugStateVisualizer : MonoBehaviour
{
	public BodyController body;

	private void Reset() {
		body = GetComponentInParent<BodyController>();
	}
	
	#if UNITY_EDITOR
	private void OnDrawGizmos() {
		Vector3 offset = Vector3.up * 1.1f;
		string str = "VisibilityState: " + body.GetVisibilityState();

		if (body.unit.aiManager != null) 
			str += "\n" + "AIState: " + body.unit.aiManager.GetCurrentState();

		Handles.Label(transform.position + offset, str);
	}
	#endif
}
