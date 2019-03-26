using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToFace : Order {

	Quaternion wantedRotation;
	//float elapsedTime;
	float turnDampening = 0.5f; // TODO: small lerp smoothing at beginning?

	public void Initialize(GameObject obj, Vector3 targetLocation) {
		base.Initialize(obj);

		this.orderType = OrderTypes.TURN_TO_FACE;
		this.targetLocation = targetLocation;
	}

	public override void Execute()
	{
	}

	public override void Update()
	{
	}

	public override void FixedUpdate() {
		unit.body.FixedUpdate_ForceTurn(targetLocation);
	/*
		wantedRotation = Quaternion.LookRotation(targetLocation - unit.GetBodyPosition());
		float step = unit.unitInfo.turnRate * Time.fixedDeltaTime * turnDampening;  // TODO: switch to Time.fixedDeltaTime?
		unit.body.transform.rotation = Quaternion.RotateTowards(unit.body.transform.rotation, wantedRotation, step);
									// Quaternion.Lerp(unit.body.transform.rotation, wantedRotation, percentageComplete);
	*/
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
