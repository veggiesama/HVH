using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Revealed")]
public class Revealed : StatusEffect {

	[Header("Revealed")]
	public GameObject particlePrefab;

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Revealed";
		type = StatusEffectTypes.REVEALED;
		duration = 2f;
		overrideAbilityDuration = true;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply() {
		base.Apply();
		InstantiateParticleOnUnit(particlePrefab, unit, BodyLocations.WEAPON, duration);
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		base.Stack(status);
		InstantiateParticleOnUnit(particlePrefab, unit, BodyLocations.WEAPON, duration);
	}

	public override void End() {
		base.End();
	}
}
