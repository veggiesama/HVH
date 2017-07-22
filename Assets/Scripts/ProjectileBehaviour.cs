using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {
	private GameObject target;
	private Rigidbody rb;
	public float speed;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		rb.transform.LookAt(target.transform);
		rb.velocity = transform.forward * speed;
	}

	public void SetTarget(GameObject target) {
		this.target = target; 
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.Equals(target))
			Destroy(this.gameObject);
	}
}
