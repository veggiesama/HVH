using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TreeHandler : NetworkBehaviour {

	private void Awake() {
		GameResources.Instance.RegisterTreeHandler(this);
	}

	[ClientRpc]
	public void Rpc_DestroyTree(int treeSiblingIndex, Vector3 destroyedFromDirection, float delay) {
		GameObject treeGO = GetTreeGOFromSiblingIndex(treeSiblingIndex);
		DestroyTree(treeGO, destroyedFromDirection);
		StartCoroutine( RespawnTree(treeGO) );
	}

	private void DestroyTree(GameObject treeGO, Vector3 destroyedFromDirection) {
		Tree tree = treeGO.GetComponent<Tree>();
		tree.EnableTree(false);
		tree.FallOver(destroyedFromDirection, 400f);
	}

	private IEnumerator RespawnTree(GameObject treeGO) {
		yield return new WaitForSeconds(Constants.TreeRespawnTime);
		Tree tree = treeGO.GetComponent<Tree>();
		//tree.EnableTree(true);
		tree.Grow();
	}

	private GameObject GetTreeGOFromSiblingIndex(int siblingIndex) {
		return transform.GetChild(siblingIndex).gameObject;
	}
}
