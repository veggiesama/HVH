using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Tree : MonoBehaviour {
    public Transform abilityAnchor;
	public Material highlightedTreeMaterial;
	public GameObject deadTreePrefab;

	private Material treeMaterial;
	private TreeHandler treeHandler;
	private Vector3 destroyedFromDirection;
	private Renderer rend;

	private void Start() {
		treeHandler = GetComponentInParent<TreeHandler>();
		rend = GetComponent<Renderer>();
		treeMaterial = rend.material;
	}

	public void SetHighlighted(bool enable) {
		if (enable)	
			rend.material = highlightedTreeMaterial;
		else
			rend.material = treeMaterial;
	}

	public Vector3 GetAnchorPoint() {
		return abilityAnchor.position;
	}

	public int GetSiblingIndex() {
		return transform.GetSiblingIndex();
	}

	public GameObject GetTreeHandlerGO() {
		return treeHandler.gameObject;
	}

}
