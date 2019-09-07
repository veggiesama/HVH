#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Tree = HVH.Tree;

[InitializeOnLoad]
public class TreeLoader {

    static TreeLoader() {
		ReloadTrees();
		EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
		Debug.Log("TreeLoader: Tree colors set.");
	}

	private static void EditorApplication_playModeStateChanged(PlayModeStateChange state) {
		if (state == PlayModeStateChange.EnteredEditMode) {
			ReloadTrees();
		}

	}

	private static void ReloadTrees() {
		Tree[] trees = Object.FindObjectsOfType<Tree>();

		foreach (Tree tree in trees) {
			tree.SetColors();
		}

		Debug.Log("TreeLoader: Tree colors fixed after play state end.");
	}
}
#endif