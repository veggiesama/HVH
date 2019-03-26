using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Airborn")]
public class Airborn : StatusEffect {

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Airborn";
		type = StatusEffectTypes.AIRBORN;
		duration = 3f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability, UnitController inflictor) {
		base.Initialize(obj, ability, inflictor);
	}

	public override void Apply()
	{
		base.Apply();
		unit.DetachFromNav();
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		//base.Stack(status);
	}

	public override void End() {
		unit.AttachToNav();
		base.End();
	}
}
