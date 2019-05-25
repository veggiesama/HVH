using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TreeHandler : NetworkBehaviour {

	[ClientRpc]
	public void Rpc_DestroyTree(int treeSiblingIndex, Vector3 destroyedFromDirection, float delay) {
		//Debug.Log("Rpc_DestroyTree");
		GameObject treeGO = GetTreeGOFromSiblingIndex(treeSiblingIndex);
		Tree tree = treeGO.GetComponent<Tree>();
		//GameObject deadTreePrefab = tree.deadTreePrefab;
		//GameObject deadTree = Instantiate(deadTreePrefab, tree.transform.position, tree.transform.rotation, transform);

		tree.FallOver(destroyedFromDirection, 400f);
		treeGO.SetActive(false);
		StartCoroutine( RespawnTree(treeGO) );
	}

	private IEnumerator RespawnTree(GameObject treeGO) {
		yield return new WaitForSeconds(Constants.TreeRespawnTime);
		treeGO.SetActive(true);
	}

	private GameObject GetTreeGOFromSiblingIndex(int siblingIndex) {
		return transform.GetChild(siblingIndex).gameObject;
	}
}
