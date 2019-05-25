using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastTree : Cast {
	public void Initialize(GameObject obj, Ability ability, Tree tree) {
		base.Initialize(obj, ability, tree.transform.position, null, null, tree);
		this.orderType = OrderTypes.CAST_TREE;
	}

	public override void Execute() {
		if (tree == null) {
			failCast = true;
			Debug.Log("Cast failure: Tree no longer exists.");
			return;
		}

		base.Execute();
	}
}
