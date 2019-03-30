using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHandler : MonoBehaviour {

	public float treeRespawnTime = 6.0f;

	public void DestroyTree(GameObject tree) {
		if (!tree.CompareTag("Tree")) {
			print("Tried to destroy tree that wasn't a tree.");
			return;
		}

		tree.SetActive(false);
		StartCoroutine( RespawnTree(tree) );
	}

	private IEnumerator RespawnTree(GameObject tree) {
		yield return new WaitForSeconds(treeRespawnTime);
		tree.SetActive(true);
	}

}
