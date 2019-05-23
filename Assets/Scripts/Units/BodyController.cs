﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BodyController : MonoBehaviour {

	private Rigidbody rb;
	[HideInInspector] public UnitController unit;
	[HideInInspector] public Animator anim;

	[HideInInspector] public GameObject projectileSpawner;
	[HideInInspector] public GameObject head;
	[HideInInspector] public GameObject mouth;
	[HideInInspector] public GameObject feet;
	[HideInInspector] public Renderer[] bodyMeshes;
	[HideInInspector] public FieldOfView fov;

	private bool isVisible = true;
	private Vector3 lastPosition = Vector3.zero;
	private float updateAnimationSpeedFloatEvery = 0.1f;

	// Use this for initialization
	void Awake () {
		unit = GetComponentInParent<UnitController>();
		rb = GetComponent<Rigidbody>();
		fov = GetComponentInChildren<FieldOfView>();

		InvokeRepeating("UpdateAnimationSpeedFloat", 0f, updateAnimationSpeedFloatEvery);
	}

	public void ResetAnimator() {
		if (anim != null)
			Destroy(anim.gameObject);
				
		GameObject animGO = Instantiate(unit.unitInfo.animationPrefab, this.transform);
		anim = animGO.GetComponent<Animator>();
		
		BodyLocationFinder finder = anim.GetComponent<BodyLocationFinder>();
		projectileSpawner = finder.projectileSpawner;
		head = finder.head;
		mouth = finder.mouth;
		feet = finder.feet;
		bodyMeshes = finder.bodyMeshes;
	}

	private void FixedUpdate() {
		if (unit.IsMouseLooking()) {
			Vector3 mousePosition = unit.GetPlayer().GetMouseLocationToGround();
			unit.body.FixedUpdate_ForceTurn(mousePosition);
		}
	}

	private void UpdateAnimationSpeedFloat() {
		if (anim == null) return;

		float speed = ((transform.position - lastPosition).magnitude) / Time.deltaTime;
		lastPosition = transform.position;
		anim.SetFloat("speed", speed);
	}

	// performances
	public void PerformDeath(Vector3 killedFromDirection) {
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		float upwardMagnitude = Random.Range(50f, 150f);
		float impactMagnitude = 400f;

		rb.AddForce(Vector3.up * upwardMagnitude);
		if (!Util.IsNullVector(killedFromDirection))
			rb.AddForce((transform.position - killedFromDirection).normalized * impactMagnitude);
		anim.SetBool("isDead", true);
	}

	public void PerformLaunch(Vector3 velocityVector) {
		rb.AddForce(velocityVector, ForceMode.VelocityChange);
	}

	public void PerformAirborn(Vector3 velocityVector) {
		rb.isKinematic = false;
		rb.AddForce(velocityVector, ForceMode.VelocityChange);
	}

	//public void SetTreeClipOnly() {
	//	gameObject.layer = LayerMask.NameToLayer("PhysicsIgnoreTrees"); 
	//}

	public void SetNoclip() {
		gameObject.layer = LayerMask.NameToLayer("PhysicsNoclip"); 
	}

	public void SetDefaultClip() {
		gameObject.layer = LayerMask.NameToLayer("Body");
	}

	public void SetTriggerable(bool enable) {
		GetComponent<Collider>().isTrigger = enable;
	}

	public void ResetBody() {
		rb.constraints = RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY |
			RigidbodyConstraints.FreezeRotationZ;
		rb.isKinematic = true;
		SetDefaultClip();
		SetTriggerable(false);
		anim.SetBool("isDead", false);
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

	public delegate void OnCollidedTreeDelegate(Tree tree);
	public event OnCollidedTreeDelegate OnCollidedTreeEventHandler;

	public void OnCollidedTree(Tree tree) {
		if (OnCollidedTreeEventHandler != null) {
			OnCollidedTreeEventHandler(tree);	
		}
	}

	public delegate void OnCollisionDelegate(Collision col);
	public event OnCollisionDelegate OnCollisionEventHandler;
	
	public void OnCollisionEnter(Collision collision)
	{
		if (OnCollisionEventHandler != null) {
			OnCollisionEventHandler(collision);	
		}
	}
	 
	public void SetVisibility(bool enable) {
		if (anim != null) {
			foreach (Renderer bodyMesh in bodyMeshes) {
				bodyMesh.enabled = enable;
			}
			isVisible = enable;
			//anim.gameObject.SetActive(enable);
		}
	}

	public bool IsVisible() {
		if (anim != null) 
			return isVisible;
		else
			return false;
	}

}
