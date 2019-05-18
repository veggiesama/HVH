using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Tree : MonoBehaviour {

	public GameObject meshGO;
	public GameObject deadTreeGO;
    public Transform abilityAnchor;
	public Material[] highlightedTreeMaterials;

	private Material[] originalTreeMaterials;
	private TreeHandler treeHandler;
	private Renderer meshRenderer;
	private FadeMaterial fadeScript;

	private void Start() {
		meshRenderer = meshGO.GetComponent<Renderer>();
		treeHandler = GetComponentInParent<TreeHandler>();
		//rend = GetComponent<Renderer>();
		originalTreeMaterials = meshRenderer.materials;

		if (originalTreeMaterials.Length != highlightedTreeMaterials.Length) {
			Debug.Log("Number of highlighted tree materials must match number of tree materials. Fix prefab.");
		}

		// Fill in empty material slots
		for (int n = 0; n < highlightedTreeMaterials.Length; n++) {
			if (highlightedTreeMaterials[n] == null) {
				highlightedTreeMaterials[n] = originalTreeMaterials[n];
			}
		} 

		// dead tree mesh referenced from regular tree mesh
		Mesh sm = meshGO.GetComponent<MeshFilter>().sharedMesh;
		var deadTreeMeshFilter = deadTreeGO.AddComponent<MeshFilter>();
		var deadTreeRenderer = deadTreeGO.AddComponent<MeshRenderer>();
		deadTreeMeshFilter.mesh = sm;
		deadTreeRenderer.materials = originalTreeMaterials;
		fadeScript = deadTreeGO.GetComponent<FadeMaterial>();
		deadTreeGO.SetActive(false);
	}

	public void SetHighlighted(bool enable) {
		// https://docs.unity3d.com/ScriptReference/Renderer-materials.html
		//Note that like all arrays returned by Unity, this returns a copy of materials array. If you want to change some materials in it, get the value, change an entry and set materials back.
		Material[] currentMats = meshRenderer.materials;

		for (int n = 0; n < currentMats.Length; n++) {
			if (enable)
				currentMats[n] = highlightedTreeMaterials[n];
			else
				currentMats[n] = originalTreeMaterials[n];
		}

		meshRenderer.materials = currentMats;
	}

	// create a dead tree from the prefab that falls over and destroys itself
	public void FallOver(Vector3 fromDirection, float force) {
		GameObject deadTreeClone = Instantiate(deadTreeGO);
		deadTreeClone.SetActive(true);
		deadTreeClone.transform.position = meshGO.transform.position;
		deadTreeClone.transform.rotation = meshGO.transform.rotation;
		deadTreeClone.transform.localScale = meshGO.transform.localScale;

		if (fromDirection != default) {
			Rigidbody rb = deadTreeClone.GetComponent<Rigidbody>();
			rb.AddForce((deadTreeClone.transform.position - fromDirection).normalized * force);
		}

		fadeScript.enabled = true;
		Destroy(deadTreeClone, 5.0f);
	}

	/*
	public void FallOver(Vector3 fromDirection, float force) {
		GameObject deadTreeCopy = Instantiate(deadTreeGO);
		deadTreeCopy.SetActive(true);		
		deadTreeCopy.transform.position = meshGO.transform.position;
		deadTreeCopy.transform.rotation = meshGO.transform.rotation;
		deadTreeCopy.transform.localScale = meshGO.transform.localScale;
		MeshFilter deadTreeCopyMesh = deadTreeCopy.AddComponent<MeshFilter>();
		MeshRenderer deadTreeCopyRenderer = deadTreeCopy.AddComponent<MeshRenderer>();
		deadTreeCopyMesh.mesh = Instantiate( meshGO.GetComponent<MeshFilter>().sharedMesh );
		deadTreeCopyRenderer.materials = originalTreeMaterials;

		if (fromDirection != default) {
			Rigidbody rb = deadTreeCopy.GetComponent<Rigidbody>();
			rb.AddForce((deadTreeCopy.transform.position - fromDirection).normalized * force);
		}

		Destroy(deadTreeCopy, 5.0f);
	}*/

	public void OnTriggerEnter(Collider col) {
		//Debug.Log("Tree triggered by: " + col.gameObject.name);
		GameObject go = col.gameObject;

		if (Util.IsBody(go)) {
			go.GetComponent<BodyController>().OnCollidedTree(this);
		}	
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
