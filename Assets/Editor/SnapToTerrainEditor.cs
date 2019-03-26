using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnapToTerrain))]
//[CanEditMultipleObjects]
public class SnapToTerrainEditor : Editor {

	public SerializedProperty lowerBoundForY; // = -2;
	public SerializedProperty upperBoundForY; // = 8;
	public SerializedProperty flatnessTolerance; // = 0.05f;

	public override void OnInspectorGUI() {
		SnapToTerrain snap = (SnapToTerrain) target;

		snap.flatnessTolerance = EditorGUILayout.FloatField("Flatness Tolerance", snap.flatnessTolerance);
		snap.lowerBoundForY = EditorGUILayout.FloatField("Lower bound for Y", snap.lowerBoundForY);
		snap.upperBoundForY = EditorGUILayout.FloatField("Upper bound for Y", snap.upperBoundForY);

		if (GUILayout.Button("Snap!"))
		{
			snap.Activate();
		}

	}

}
