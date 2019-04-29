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
	public Ability ability;
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

	// do NOT call End() from inside Excecute. At least one Update() should pass.
	public abstract void Execute();

	public abstract void Update();

	public abstract void FixedUpdate();

	// occurs when the ability is forced out of its first-place position in the queue; do cleanup if needed
	public abstract void Suspend(OrderTypes suspendedBy);

	public virtual void End() {
		//Debug.Log("Ending order " + orderType);
		queue.CompleteOrder(this);
	}
}
