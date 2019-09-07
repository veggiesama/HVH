using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tree = HVH.Tree;

[CustomEditor(typeof(RandomizeTreesButton))]
public class RandomizeTreesEditor : Editor {
	public SerializedProperty treePrefabs;

	public override void OnInspectorGUI() {
		RandomizeTreesButton script = (RandomizeTreesButton) target;

		if (GUILayout.Button("Randomize!"))
		{
			script.Activate();
		}

	}

}
