using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/_EmptyAbility")]
public class EmptyAbility : Ability {

	public override void Reset() {
		abilityName = "EmptyAbility";
	}

	public override void Initialize(GameObject obj) {
		isEmptyAbility = true;
	}
}