using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*TODO
 * 1% health regen per sec
 * 20% magic resist
 * +7 armor
 * 1800 radius
 * */

[CreateAssetMenu(menuName = "Status Effects/Hound/Man's Best Friend")]
public class MansBestFriend : StatusEffect {

	// default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "";
		type = StatusEffectTypes.STUNNED;
		duration = 2f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply()
	{
		base.Apply();
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
