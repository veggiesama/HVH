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
		if (doingOrder) { 
			queue[0].Update();
			return;
		}

		//Debug.Log("Executing next order.");
		ExecuteNextOrder();
    }

	void FixedUpdate()
	{
	    if (queue.Count == 0) return;
		if (doingOrder) { 
			queue[0].FixedUpdate();
			return;
		}
	}

	// public functions
	public void Add(Order order, bool bypassQueue = false) {
		//Debug.Log("# " + queue.Count + ", Interrupts: " + order.interrupts);

		bool shiftQueuing = Input.GetButton("Queue (Hold)");

		if (bypassQueue && !shiftQueuing) {
			if (queue.Count > 0)
				queue[0].Suspend();

			queue.Insert(0, order);
			ExecuteNextOrder();
			return;
		}

		if (queue.Count > 0 && !shiftQueuing) {
			CancelAllOrders();
		}

		queue.Add(order);	
	}

	private void ExecuteNextOrder() {
		queue[0].Execute();
		doingOrder = true;
	}

	public void CompleteOrder() {
		queue.RemoveAt(0);
		doingOrder = false;
	}

	public void CancelOrder() {
		queue.RemoveAt(0);
		doingOrder = false;
	}

	public void CancelAllOrders() {
		queue.Clear();
		doingOrder = false;
	}
}
