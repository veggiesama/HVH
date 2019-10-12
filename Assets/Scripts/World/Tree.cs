using Tree = HVH.Tree;
using Outline = cakeslice.Outline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

namespace HVH {

public class Tree : MonoBehaviour {

	public GameObject visionBlockerGO;
	public GameObject meshGO;
	public GameObject anchorGO;
	public GameObject deadTreeGO;

	private TreeHandler treeHandler;
	[HideInInspector] public Renderer meshRenderer;
	private DeadTree deadTree;
	private Outline outline;
	private NavMeshObstacle navMeshObstacle;
	private Collider capsuleCollider;

	private MaterialPropertyBlock block;
	public Color colorTint;
	public Color emissionColor;

	private void Awake() {
		SetColors(); // colors need to be set in both editor/play modes due to use of MaterialPropertyBlock

		meshRenderer = meshGO.GetComponent<Renderer>();
		treeHandler = GetComponentInParent<TreeHandler>();
		outline = meshGO.GetComponent<Outline>();
		navMeshObstacle = GetComponent<NavMeshObstacle>();
		capsuleCollider = GetComponent<CapsuleCollider>();

		// dead tree mesh referenced from regular tree mesh
		Mesh sm = meshGO.GetComponent<MeshFilter>().sharedMesh;
		var deadTreeMeshFilter = deadTreeGO.GetComponent<MeshFilter>();
		var deadTreeRenderer = deadTreeGO.GetComponent<MeshRenderer>();
		deadTreeMeshFilter.mesh = sm;
		
		deadTreeRenderer.sharedMaterials = meshRenderer.sharedMaterials;

		if (deadTreeRenderer.sharedMaterials.Length >= 2) { 
			deadTreeRenderer.SetPropertyBlock(block, 1); // bark is material 0, leaves/pines are material 1.
		}

		//deadTreeRenderer.materials = originalTreeMaterials;
		deadTree = deadTreeGO.GetComponent<DeadTree>();
	}

	public void EnableTree(bool enable) {
		visionBlockerGO.SetActive(enable);
		meshGO.SetActive(enable);
		anchorGO.SetActive(enable);
		navMeshObstacle.enabled = enable;
		capsuleCollider.enabled = enable;
		
		if (enable) {
			deadTreeGO.SetActive(false);
		}
		else {
			deadTree.ResetTransform(meshGO.transform.position, 
								meshGO.transform.rotation,
								meshGO.transform.localScale);
			deadTreeGO.SetActive(true);
			deadTree.StartSinking();
		}
	}


	public void SetHighlighted(HighlightingState state) {
		switch (state) {
			case HighlightingState.NONE:
				outline.enabled = false;
				break;
			case HighlightingState.NORMAL:
				outline.enabled = true;
				outline.color = 0;
				break;
			case HighlightingState.INTEREST:
				outline.enabled = true;
				outline.color = 1;
				break;
			case HighlightingState.ENEMY:
				outline.enabled = true;
				outline.color = 2;
				break;
		}
	}

	public void FallOver(Vector3 fromDirection, float force) {
		if (fromDirection != default) {
			Rigidbody rb = deadTreeGO.GetComponent<Rigidbody>();
			rb.AddForce((deadTreeGO.transform.position - fromDirection).normalized * force);
		}
	}

	public void Grow() {
		StartCoroutine(
			deadTree.StartRising(meshGO.transform.position, 
									meshGO.transform.rotation,
									meshGO.transform.localScale)
		);
	}

	public Vector3 GetAnchorPoint() {
		return anchorGO.transform.position;
	}

	public int GetSiblingIndex() {
		return transform.GetSiblingIndex();
	}

	public GameObject GetTreeHandlerGO() {
		return treeHandler.gameObject;
	}

	// usable in edit mode
	public void SaveColors(Color colorTint, Color emissionColor) {
		this.colorTint = colorTint;
		this.emissionColor = emissionColor;
	}

	// usable in edit mode
	public void SetColors() {
		Renderer rend = meshGO.GetComponent<Renderer>();
		if (rend.sharedMaterials.Length < 2) return;

		if (block != null)
			block.Clear();
	
		block = new MaterialPropertyBlock();
		block.SetColor("_ColorTint", colorTint);
		block.SetColor("_EmissionColor", emissionColor);

		meshGO.GetComponent<Renderer>().SetPropertyBlock(block, 1); // bark is material 0, leaves/pines are material 1.
	}

}

} // HVH namespace