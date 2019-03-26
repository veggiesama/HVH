using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Slowed")]
public class Slowed : StatusEffect {

	// fields unique to this StatusEffect
	[Header("Slowed")]
	public float slowByPercentage = 0.5f;

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Slowed";
		type = StatusEffectTypes.SLOWED;
		duration = 3f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability, UnitController inflictor) {
		base.Initialize(obj, ability, inflictor);
	}

	// 
	public override void Apply()
	{
		base.Apply();
		unit.SetSpeed( unit.unitInfo.movementSpeed * (1-slowByPercentage) );
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		base.Stack(status);
	}

	public override void End() {
		unit.SetSpeed ( unit.unitInfo.movementSpeedOriginal );
		base.End();
	}
}
