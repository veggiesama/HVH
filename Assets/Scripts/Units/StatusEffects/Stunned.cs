﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Stunned")]
public class Stunned : StatusEffect {

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Stunned";
		type = StatusEffectTypes.STUNNED;
		duration = 2f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	// 
	public override void Apply()
	{
		base.Apply();
		unit.ForceStop(); // TODO: fix
		unit.CancelAllOrders();
		unit.SetOrderRestricted(true);
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		base.Stack(status);
	}

	public override void End() {
		unit.SetOrderRestricted(false);
		base.End();
	}
}
