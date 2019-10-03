using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*TODO
 * health regen 4% per second
 * invis fade delay 2.0s
 * less night vision
 * */

[CreateAssetMenu(menuName = "Status Effects/Hound/Sleep")]
public class SleepBuff : StatusEffect {

	[Header("Sleep")]
	public float healPercentEverySecond;
	public float tickEvery;
	public StatusEffect invisStatus;
	private float tickTimer;
	private float healValueEveryTick; // percentage

	// default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Sleep buff";
		type = StatusEffectTypes.BUFF_NOTDISPELLABLE;

		healPercentEverySecond = 0.04f;
		tickEvery = 0.1f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply() {
		base.Apply();
		tickTimer = tickEvery;
		healValueEveryTick = (unit.unitInfo.maxHealth * healPercentEverySecond) / ( 1 / tickEvery);

		unit.ForceStop();
		unit.ApplyStatusEffect(invisStatus);
		unit.networkHelper.PlayAnimation(Animations.LAYDOWN);
	}

	public override void Update() {
		base.Update(); // tracks duration

		tickTimer -= Time.deltaTime;
		if (tickTimer < 0) {
			networkHelper.HealDamageOn(unit, healValueEveryTick); // TODO: Is this efficient?
			tickTimer = tickEvery;
		}

	}

	public override void End() {
		unit.networkHelper.PlayAnimation(Animations.WAKEUP);
		unit.RemoveStatusEffect(invisStatus);
		base.End();
	}

}
