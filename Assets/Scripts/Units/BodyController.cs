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

	private void FixedUpdate() {
		if (unit.IsMouseLooking()) {
			Vector3 mousePosition = unit.GetOwnerController().GetMouseLocationToGround();
			unit.body.FixedUpdate_ForceTurn(mousePosition);
		}
	}

	public UnitController GetUnitController() {
		return transform.parent.GetComponent<UnitController>();
	}

	// performances
	public void PerformDeath(Vector3 killedFromDirection) {
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.None;

		float upwardMagnitude = Random.Range(50f, 150f);
		float impactMagnitude = 400f;

		rb.AddForce(Vector3.up * upwardMagnitude);
		if (!Util.IsNullVector(killedFromDirection))
			rb.AddForce((transform.position - killedFromDirection).normalized * impactMagnitude);
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
		float turnDampening = 0.5f;

		targetPosition.y = transform.position.y; // adjust to same plane
		Quaternion wantedRotation = Quaternion.LookRotation(targetPosition - transform.position);
		float step = unit.unitInfo.turnRate * Time.fixedDeltaTime * turnDampening;
		unit.body.transform.rotation = Quaternion.RotateTowards(unit.body.transform.rotation, wantedRotation, step);
									// Quaternion.Lerp(unit.body.transform.rotation, wantedRotation, percentageComplete);
	}

	public bool IsFacing(Vector3 targetPosition) {
		if (targetPosition.Equals(default)) return true;

		targetPosition.y = transform.position.y; // adjust to same plane
		Quaternion wantedRotation = Quaternion.LookRotation(targetPosition - transform.position);
		return Quaternion.Angle(transform.rotation, wantedRotation) < Constants.FrontAngle;
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
