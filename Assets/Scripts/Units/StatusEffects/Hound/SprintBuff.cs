using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*TODO
 * 65% bonus speed for hound
 * +25 speed for ally
 * 1800 radius
 * 6s duration
 * */

[CreateAssetMenu(menuName = "Status Effects/Hound/Sprint")]
public class SprintBuff : StatusEffect {
	
	[Header("SprintBuff")]
	public float speedBonus;

	// default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset() {
		statusName = "Sprint buff";
		type = StatusEffectTypes.BUFF_DISPELLABLE;
		
		speedBonus = 5f;
		overrideAbilityDuration = true;

	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply() {
		base.Apply();
		unit.SetSpeed(unit.unitInfo.movementSpeed + speedBonus);
	}

	public override void End() {
		unit.SetSpeed(unit.unitInfo.movementSpeedOriginal);
		base.End();
	}
}
