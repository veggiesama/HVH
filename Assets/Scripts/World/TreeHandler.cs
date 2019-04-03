using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TreeHandler : NetworkBehaviour {

	[ClientRpc]
	public void Rpc_DestroyTree(int treeSiblingIndex, Vector3 destroyedFromDirection, float delay) {
		Debug.Log("Rpc_DestroyTree");
		GameObject tree = GetTreeFromSiblingIndex(treeSiblingIndex);
		GameObject deadTreePrefab = tree.GetComponent<Tree>().deadTreePrefab;
		GameObject deadTree = Instantiate(deadTreePrefab, tree.transform.position, tree.transform.rotation, transform);

		if (!tree.CompareTag("Tree")) {
			print("Tried to destroy tree that wasn't a tree.");
			return;
		}
	
		Debug.Log("1");
		if (destroyedFromDirection != default) {
			Rigidbody rb = deadTree.GetComponent<Rigidbody>();
			float impactMagnitude = 400f;
			Debug.Log("2");
			rb.AddForce((deadTree.transform.position - destroyedFromDirection).normalized * impactMagnitude);
		}

		Destroy(deadTree, 5.0f);
		tree.SetActive(false);
		Debug.Log("3");
		StartCoroutine( RespawnTree(tree) );
	}

	private IEnumerator RespawnTree(GameObject tree) {
		yield return new WaitForSeconds(Constants.TreeRespawnTime);
		Debug.Log("Tree respawns");
		tree.SetActive(true);
	}

	private GameObject GetTreeFromSiblingIndex(int siblingIndex) {
		return transform.GetChild(siblingIndex).gameObject;
	}
}
