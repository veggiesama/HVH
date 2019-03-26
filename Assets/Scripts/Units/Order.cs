using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Order : ScriptableObject {

	public OrderTypes orderType;

	protected OrderQueue queue;
	protected UnitController unit;
	protected UnitController target;
	public Vector3 targetLocation;
	protected Ability ability;

	public virtual void Initialize(GameObject obj, Ability ability = null, Vector3 targetLocation = default) {
		this.queue = obj.GetComponentInChildren<OrderQueue>();
		this.unit = obj.GetComponent<UnitController>();
		
		if (ability != null) this.ability = ability;
		if (targetLocation != default) this.targetLocation = targetLocation;
	}

	public abstract void Update();

	public abstract void FixedUpdate();

	public abstract void Execute();

	// occurs when the ability is forced out of its first-place position in the queue; do cleanup if needed
	public abstract void Suspend();

	public virtual void End() {
		queue.CompleteOrder();
	}
}
