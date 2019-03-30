using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {
    [SerializeField] private Transform abilityAnchor;
	[SerializeField] private Renderer meshRenderer;
	private Material treeMaterial;
	private Material highlightedTreeMaterial;
	private TreeHandler treeHandler;

	private void Awake() {
		treeHandler = GetComponentInParent<TreeHandler>();
		MaterialsLibrary matLib = GameObject.Find("MaterialsLibrary").GetComponent<MaterialsLibrary>();
		treeMaterial = matLib.treeMaterial;
		highlightedTreeMaterial = matLib.highlightedTreeMaterial;
	}

	public void SetHighlighted(bool enable) {
		if (enable)	
			meshRenderer.material = highlightedTreeMaterial;
		else
			meshRenderer.material = treeMaterial;
	}

	public Vector3 GetAnchorPoint() {
		return abilityAnchor.position;
	}

	public void DestroyThisTree() {
		treeHandler.DestroyTree(this.gameObject);
	}
}
