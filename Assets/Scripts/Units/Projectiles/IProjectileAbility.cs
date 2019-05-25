using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileAbility {
	bool OnHitEnemy(UnitController enemy);
	bool OnHitAlly(UnitController ally);
	bool OnHitTree(Tree tree);
}

