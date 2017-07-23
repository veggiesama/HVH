using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {
	private UnitController target, attacker;
	private GameObject targetObject;

	private Rigidbody rb;
	public float speed;
	private AttackInfo attackInfo;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		rb.transform.LookAt(target.transform);
		rb.velocity = transform.forward * speed;
	}

	public void Initialize(UnitController attacker, UnitController target) {
		this.attacker = attacker;
		this.target = target;
		this.targetObject = target.body.gameObject;

		// if enemy is deleted mid-shot, this might save us? who knows
		//this.attackInfo = Instantiate<AttackInfo>(attacker.attackInfo);
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("OnTriggerEnter!");
		attacker.DealsDamageTo(target, attacker.attackInfo.damage);

		//attackInfo.damage 
		if (other.gameObject.Equals(targetObject))
			Destroy(this.gameObject);
	}
}
