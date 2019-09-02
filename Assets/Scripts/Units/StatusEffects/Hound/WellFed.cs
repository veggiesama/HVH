using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Hound/WellFed")]
public class WellFed : StatusEffect {

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset() {
		statusName = "Well fed";
		type = StatusEffectTypes.WELL_FED;
		duration = 4f;
		overrideAbilityDuration = true;
	}

}
