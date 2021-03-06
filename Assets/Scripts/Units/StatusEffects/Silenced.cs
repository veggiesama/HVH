﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Silenced")]
public class Silenced : StatusEffect {

	[Header("Silenced")]
	public GameObject particlePrefab;

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Silenced";
		type = StatusEffectTypes.SILENCED;
		//duration = 2f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply() {
		base.Apply();

		InstantiateParticleOnUnit(particlePrefab, unit, BodyLocations.FEET, duration);

	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		base.Stack(status);
	}

	public override void End() {
		base.End();
	}
}
