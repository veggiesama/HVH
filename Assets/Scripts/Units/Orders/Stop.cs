using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : Order {
	public void Initialize(GameObject obj) {
		base.Initialize(obj);
		this.orderType = OrderTypes.STOP;
	}

	public override void Execute()
	{
		//Update();
	}

	public override void Update()
	{
		End(); // needed?
	}

	public override void FixedUpdate() {}

	public override void Suspend(OrderTypes suspendedBy)
	{
	}

	public override void End() {
		unit.ForceStop();
		base.End();
	}
}
