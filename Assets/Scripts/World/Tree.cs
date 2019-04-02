using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {
    public Transform abilityAnchor;
	public Renderer meshRenderer;
	public GameObject deadTreePrefab;
	private Material treeMaterial;
	private Material highlightedTreeMaterial;
	private TreeHandler treeHandler;
	private Vector3 destroyedFromDirection;

	private void Start() {
		treeHandler = GetComponentInParent<TreeHandler>();
		MaterialsLibrary matLib = ReferenceManager.Instance.MaterialsLibrary;
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

	public void DestroyThisTree(Vector3 destroyedFromDirection = default, float delay = 0f) {
		this.destroyedFromDirection = destroyedFromDirection;
		if (delay > 0)
			Invoke("DestroyThisTree", delay);
		else
			DestroyThisTree();
	}

	public void DestroyThisTree() {
		GameObject deadTree = Instantiate(deadTreePrefab, transform.position, transform.rotation, treeHandler.transform);
		if (destroyedFromDirection != default) {
			Rigidbody rb = deadTree.GetComponent<Rigidbody>();
			float impactMagnitude = 400f;
			rb.AddForce((deadTree.transform.position - destroyedFromDirection).normalized * impactMagnitude);
		}

		Destroy(deadTree, 5.0f);
		treeHandler.DestroyTree(this.gameObject);
	}
}
