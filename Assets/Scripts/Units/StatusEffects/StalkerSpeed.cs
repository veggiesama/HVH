using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/StalkerSpeed")]
public class StalkerSpeed : StatusEffect {

	[Header("Stalker Speed")]
	public float swipeCooldownReduction;

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Stalker speed";
		type = StatusEffectTypes.BUFF_NOTDISPELLABLE;
		swipeCooldownReduction = 0.5f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply() {
		base.Apply();
		unit.SetSpeed(unit.unitInfo.movementSpeed + ((Stalker)ability).nightMoveSpeedBonus);
	}

	public override void End() {
		unit.SetSpeed(unit.unitInfo.movementSpeedOriginal);
		base.End();
	}
}
