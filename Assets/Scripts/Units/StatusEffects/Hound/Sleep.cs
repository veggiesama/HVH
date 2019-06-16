using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*TODO
 * health regen 4% per second
 * invis fade delay 2.0s
 * less night vision
 * */

[CreateAssetMenu(menuName = "Status Effects/Hound/Sleep")]
public class Sleep : StatusEffect {

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
