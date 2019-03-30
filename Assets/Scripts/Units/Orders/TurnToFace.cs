using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToFace : Order {
	public void Initialize(GameObject obj, Vector3 targetLocation) {
		base.Initialize(obj, null, targetLocation); // TODO: bug? add targetLocation to arguments?
		this.orderType = OrderTypes.TURN_TO_FACE;
	}

	public override void Execute()
	{
	}

	public override void Update()
	{
	}

	public override void FixedUpdate() {
		if (!unit.IsMouseLooking())
			unit.body.FixedUpdate_ForceTurn(targetLocation);

		if (unit.IsFacing(targetLocation)) {
			End();
		}
	}

	public override void Suspend()
	{
		End();
	}

	public override void End() {
		base.End();
	}

}
