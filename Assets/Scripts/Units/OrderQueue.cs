using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderQueue : MonoBehaviour {
	[SerializeField] private List<Order> queue = new List<Order>();
	private bool doingOrder = false;

    void Start()
    {
    }

    void Update()
    {
        if (queue.Count == 0) return;
		
		if (doingOrder)
			queue[0].Update();
		else 
			ExecuteNextOrder();
    }

	void FixedUpdate()
	{
	    if (queue.Count == 0) return;
		if (doingOrder)
			queue[0].FixedUpdate();
	}

	public void Add(Order order, bool doNotCancelOrderQueue = false) {

		// new order is shift+queued to the back of queue
	   	bool shiftQueueing = Input.GetButton("Queue (Hold)");
		if (shiftQueueing) {
			QueueNewOrder(order);
		}

		// new order preserves current order queue
		else if (doNotCancelOrderQueue) {
			SuspendCurrentOrder(order.orderType);
			BeginNewOrder(order);
		}

		// new order interrupts existing orders
		else if (queue.Count > 0) {
			CancelAllOrders();
			BeginNewOrder(order);
		}

		// new order is only order
		else {
			BeginNewOrder(order);
		}

	}

	private void ExecuteNextOrder() {
		queue[0].Execute();
		doingOrder = true;
	}

	private void BeginNewOrder(Order order) {
		queue.Insert(0, order);
		doingOrder = false;
		//ExecuteNextOrder();
	}

	private void QueueNewOrder(Order order) {
		queue.Add(order);
	}

	private void SuspendCurrentOrder(OrderTypes suspendedBy) {
		if (queue.Count > 0)
			queue[0].Suspend(suspendedBy);
	}

	//public void CompleteOrder() {
	//	queue.RemoveAt(0);
	//	doingOrder = false;
	//}

	public void CompleteOrder(Order order) {
		int index = queue.IndexOf(order);
		queue.RemoveAt(index);
		if (index == 0)
			doingOrder = false;
	}

	public void CancelAllOrders() {
		SuspendCurrentOrder(OrderTypes.NONE);
		queue.Clear();
		doingOrder = false;
	}
}
