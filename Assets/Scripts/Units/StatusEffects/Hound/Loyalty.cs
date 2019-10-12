using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Hound/Loyalty")]
public class Loyalty : StatusEffect {

	[Header("Loyalty")]
	private UnitController loyalTo;

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset() {
		statusName = "Loyal";
		type = StatusEffectTypes.BUFF_NOTDISPELLABLE;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply() {
		base.Apply();
		//unit.SetCurrentTarget(ability.caster);
		loyalTo = ability.caster;
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		base.Stack(status);
		loyalTo = status.unit; // TODO: this status effect still references the original caster's ability. Stacking needs rework.
	}

	public override void End() {
		base.End();
	}


	//////////////////////

	public UnitController GetLoyaltyTarget() {
		return loyalTo;
	}
}
