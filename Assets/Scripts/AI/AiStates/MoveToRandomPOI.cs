using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/MoveTo")]
public class MoveToRandomPOI : MoveTo {

	public override void Execute() {
		base.Execute();
		SetDestination(GameResources.Instance.GetRandomSpawnPoint().position);
	}

}
