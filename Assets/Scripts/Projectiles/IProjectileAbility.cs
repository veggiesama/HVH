using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileAbility {
	bool OnHitEnemy(UnitController unit);
	bool OnHitAlly(UnitController unit);
	bool OnHitTree(TreeHandler treeHandler, GameObject tree);
}

