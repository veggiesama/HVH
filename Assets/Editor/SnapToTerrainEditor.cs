using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnapToTerrain))]
public class SnapToTerrainEditor : Editor {

	public float lowerBoundForY = -2;
	public float upperBoundForY = 8;

	public override void OnInspectorGUI() {
		SnapToTerrain snap = (SnapToTerrain) target;

		if (GUILayout.Button("Snap!"))
		{
			snap.Activate(lowerBoundForY, upperBoundForY);
		}

	}

}
