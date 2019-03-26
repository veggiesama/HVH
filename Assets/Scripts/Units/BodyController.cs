using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

	private Rigidbody rb;
	private UnitController unit;

	// Use this for initialization
	void Start () {
		unit = GetComponentInParent<UnitController>();
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public UnitController GetUnitController() {
		return transform.parent.GetComponent<UnitController>();
	}

	public Vector3 GetBodyPosition() {
		return transform.position;
	}

	// performances
	public void PerformDeath(Vector3 killedFromDirection) {
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.None;

		float upwardMagnitude = Random.Range(50f, 150f);
		float impactMagnitude = 400f;

		rb.AddForce(Vector3.up * upwardMagnitude);
		if (!Util.IsNullVector(killedFromDirection))
			rb.AddForce((GetBodyPosition() - killedFromDirection).normalized * impactMagnitude);
	}

	public void PerformLaunch(Vector3 velocityVector) {
		rb.AddForce(velocityVector, ForceMode.VelocityChange);
	}

	public void PerformAirborn(Vector3 velocityVector) {
		rb.isKinematic = false;
		rb.AddForce(velocityVector, ForceMode.VelocityChange);
		//SetNoclip(true);
	}

	public void ResetBody() {
		rb.constraints = RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY |
			RigidbodyConstraints.FreezeRotationZ;
		rb.isKinematic = true;
		SetDefaultClip();
	}

	public void SetDefaultClip() {
		gameObject.layer = LayerMask.NameToLayer("Default");
	}

	public void SetTreeClipOnly() {
		gameObject.layer = LayerMask.NameToLayer("PhysicsIgnoreTrees"); 
	}

	public void SetNoclip() {
		gameObject.layer = LayerMask.NameToLayer("PhysicsNoclip"); 
	}

	public void FixedUpdate_ForceTurn(Vector3 targetPosition) {
		float turnDampening = 1f;
		Quaternion wantedRotation = Quaternion.LookRotation(targetPosition - GetBodyPosition());
		float step = unit.unitInfo.turnRate * Time.fixedDeltaTime * turnDampening;
		unit.body.transform.rotation = Quaternion.RotateTowards(unit.body.transform.rotation, wantedRotation, step);
									// Quaternion.Lerp(unit.body.transform.rotation, wantedRotation, percentageComplete);
	}



	// THIS IS ALL SHIT


	public delegate void OnCollisionDelegate(Collision col);
	public event OnCollisionDelegate OnCollisionEventHandler;
	
	public void OnCollisionEnter(Collision collision)
	{
		if (OnCollisionEventHandler != null) {
			OnCollisionEventHandler(collision);	
		}
	}

}
