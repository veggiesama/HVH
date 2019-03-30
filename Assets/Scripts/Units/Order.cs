using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Order : ScriptableObject {

	public OrderTypes orderType;

	protected OrderQueue queue;
	protected UnitController unit;
	protected UnitController allyTarget;
	protected UnitController enemyTarget;
	public Vector3 targetLocation;
	protected Ability ability;
	public Tree tree;

	public virtual void Initialize(GameObject obj, Ability ability = null, Vector3 targetLocation = default,
	  UnitController allyTarget = null, UnitController enemyTarget = null, Tree tree = null) {
		this.queue = obj.GetComponentInChildren<OrderQueue>();
		this.unit = obj.GetComponent<UnitController>();
		this.ability = ability;
		this.targetLocation = targetLocation;
		this.allyTarget = allyTarget;
		this.enemyTarget = enemyTarget;
		this.tree = tree;
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
