using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastNoTarget : Order {
	public void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability, default);
		this.orderType = OrderTypes.CAST_NO_TARGET;
	}

	public override void Execute()
	{
		//CastResults.FAILURE_COOLDOWN_NOT_READY;
		if (!ability.IsCooldownReady()) {
			End();
			return;
		}

		CastResults results = ability.Cast(this);
		if (results == CastResults.SUCCESS)
			ability.StartCooldown();
	}

	public override void Update()
	{
		End();
	}

	public override void FixedUpdate() {}

	public override void Suspend()
	{
	}

	public override void End() {
		base.End();
	}
}
