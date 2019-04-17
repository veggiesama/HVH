using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Dead")]
public class Dead : StatusEffect {

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Dead";
		type = StatusEffectTypes.DEAD;
		duration = 3f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply() {
		base.Apply();
		unit.CancelAllOrders();
		unit.SetOrderRestricted(true);

		Vector3 killFromDirection;
		if (ability != null)
			killFromDirection = ability.caster.GetBodyPosition();
		else
			killFromDirection = unit.GetBodyPosition() + Random.insideUnitSphere * 1.5f;
		
		networkHelper.Die(killFromDirection);
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		//base.Stack(status);
	}

	public override void End() {
		networkHelper.Respawn();
		unit.SetOrderRestricted(false);
		base.End();
	}
}
