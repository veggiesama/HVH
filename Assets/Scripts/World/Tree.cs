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

	public void FallOver(Vector3 fromDirection, float force) {
		GameObject deadTreeCopy = Instantiate(deadTreeGO);
		deadTreeCopy.SetActive(true);		
		deadTreeCopy.transform.position = meshGO.transform.position;
		deadTreeCopy.transform.rotation = meshGO.transform.rotation;
		deadTreeCopy.transform.localScale = meshGO.transform.localScale;
		MeshFilter deadTreeCopyMesh = deadTreeCopy.AddComponent<MeshFilter>();
		MeshRenderer deadTreeCopyRenderer = deadTreeCopy.AddComponent<MeshRenderer>();
		deadTreeCopy.GetComponent<MeshFilter>().mesh = Instantiate( meshGO.GetComponent<MeshFilter>().sharedMesh );
		deadTreeCopyRenderer.materials = originalTreeMaterials;

		if (fromDirection != default) {
			Rigidbody rb = deadTreeCopy.GetComponent<Rigidbody>();
			rb.AddForce((deadTreeCopy.transform.position - fromDirection).normalized * force);
		}

		Destroy(deadTreeCopy, 5.0f);
	}

	public void OnTriggerEnter(Collider col) {
		Debug.Log("Tree triggered by: " + col.gameObject.name);
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
